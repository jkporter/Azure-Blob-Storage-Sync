using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace AzureBlobStorageSync
{
    class Program
    {
        private static void Main(string[] args)
        {
            var section = ConfigurationManager.GetSection("jobs") as JobsConfigurationSection;

            var minDate = DateTime.UtcNow.AddDays(-section.MaxAge);
            foreach (JobConfigurationElement jobConfigurationElement in section.Jobs)
            {
                var sourceConnectionStringName= jobConfigurationElement.SourceConnection ?? jobConfigurationElement.Connection;
                var sourceContainerName = jobConfigurationElement.SourceContainer ?? jobConfigurationElement.Container;

                var destinationConnectionStringName = jobConfigurationElement.DestinationConnection ?? jobConfigurationElement.Connection;
                var destinationContainerName = jobConfigurationElement.DestinationContainer ?? jobConfigurationElement.Container;

                CloudBlobContainer destinationContainer;
                var destinationBlobs = GetBlobsEnumerator(destinationConnectionStringName,
                    destinationContainerName, true, true, out destinationContainer);

                CloudBlobContainer sourceContainer = null;
                var sourceBlobs = sourceConnectionStringName != null
                    ? GetBlobsEnumerator(sourceConnectionStringName, sourceContainerName,
                        false, false, out sourceContainer)
                    : null;

                var sourceSharedAccessSignature = sourceContainer != null
                    ? sourceContainer.GetSharedAccessSignature(new SharedAccessBlobPolicy()
                    {
                        Permissions = SharedAccessBlobPermissions.Read,
                        SharedAccessExpiryTime = DateTimeOffset.UtcNow.AddHours(1)
                    })
                    : null;

                var source = sourceBlobs != null && sourceBlobs.MoveNext() ? sourceBlobs.Current : null;
                var destination = destinationBlobs.MoveNext() ? destinationBlobs.Current : null;
                var deleteAccessCondtion = new AccessCondition() { IfNotModifiedSinceTime = minDate };
                while (source != null || destination != null)
                {
                    if (destination != null && destination.IsSnapshot)
                    {
                        destination.DeleteIfExists(accessCondition: deleteAccessCondtion);
                        destination = destinationBlobs.MoveNext() ? destinationBlobs.Current : null;
                        continue;
                    }

                    var nameCompare = source == null || destination == null
                        ? (int?)null
                        : string.Compare(source.Name, destination.Name, StringComparison.Ordinal);

                    if (nameCompare < 0 || destination == null)
                    {
                        destination = destinationContainer.GetBlockBlobReference(source.Name);
                        destination.StartCopyFromBlob(new Uri(source.Uri.AbsoluteUri + sourceSharedAccessSignature));
                        Console.WriteLine("{0}: {1}", source.Name, "Copying");
                    }
                    else if (nameCompare == 0 && !EqualCloubBlobs(source, destination))
                    {
                        CreateSnapshot(destination);
                        destination.StartCopyFromBlob(new Uri(source.Uri.AbsoluteUri + sourceSharedAccessSignature));
                        Console.WriteLine("{0}: {1}", destination.Name, "Update");
                    }
                    else if (jobConfigurationElement.RemoveExtraBlobs && (nameCompare > 0 || source == null))
                    {
                        destination.Delete(DeleteSnapshotsOption.IncludeSnapshots, deleteAccessCondtion);
                        Console.WriteLine("{0}: {1}", destination.Name, "Delete");
                    }

                    source = sourceBlobs != null && (nameCompare > 0 || sourceBlobs.MoveNext())
                        ? sourceBlobs.Current
                        : null;
                    destination = nameCompare < 0 || destinationBlobs.MoveNext()
                        ? destinationBlobs.Current
                        : null;
                }
            }
        }

        private static IEnumerator<ICloudBlob> GetBlobsEnumerator(string connection, string containerName, bool createIfNotExists, bool includeSnapshots, out CloudBlobContainer container)
        {
            var account = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings[connection].ConnectionString);
            var client = account.CreateCloudBlobClient();
            container = client.GetContainerReference(containerName);

            if (createIfNotExists)
                container.CreateIfNotExists();
            return
                container.ListBlobs(useFlatBlobListing: true,
                    blobListingDetails: BlobListingDetails.Copy | (includeSnapshots ? BlobListingDetails.Snapshots : 0))
                    .Cast<ICloudBlob>()
                    .GetEnumerator();
        }

        private static bool EqualCloubBlobs(ICloudBlob source, ICloudBlob destination)
        {
            return source.BlobType == destination.BlobType &&
                   source.Properties.ContentType == destination.Properties.ContentType &&
                   source.Properties.ContentLanguage == destination.Properties.ContentLanguage &&
                   source.Properties.Length == destination.Properties.Length &&
                   source.Properties.CacheControl == destination.Properties.CacheControl &&
                   source.Properties.ContentMD5 == destination.Properties.ContentMD5;
        }

        private static Task CreateSnapshotAsync(ICloudBlob cloubBlob)
        {
            switch (cloubBlob.BlobType)
            {
                case BlobType.BlockBlob:
                    return ((CloudBlockBlob)cloubBlob).CreateSnapshotAsync();
                case BlobType.PageBlob:
                    return ((CloudPageBlob)cloubBlob).CreateSnapshotAsync();
            }

            return null;
        }

        private static ICloudBlob CreateSnapshot(ICloudBlob cloubBlob)
        {
            switch (cloubBlob.BlobType)
            {
                case BlobType.BlockBlob:
                    return ((CloudBlockBlob)cloubBlob).CreateSnapshot();
                case BlobType.PageBlob:
                    return ((CloudPageBlob)cloubBlob).CreateSnapshot();
            }

            return null;
        }
    }
}
