using System.Configuration;

namespace AzureBlobStorageSync
{
	public class JobConfigurationElement: ConfigurationElement
	{
		#region Constructors
		static JobConfigurationElement()
		{
			s_propName = new ConfigurationProperty(
				"name",
				typeof(string),
				null,
				ConfigurationPropertyOptions.IsRequired | ConfigurationPropertyOptions.IsKey
				);

			s_propContainer = new ConfigurationProperty(
				"container",
				typeof(string),
				null,
				ConfigurationPropertyOptions.None
				);

            s_propSourceContainer = new ConfigurationProperty(
                "sourcecContainer",
                typeof(string),
                null,
                ConfigurationPropertyOptions.None
                );

            s_propDestinationContainer = new ConfigurationProperty(
                "destinationContainer",
                typeof(string),
                null,
                ConfigurationPropertyOptions.None
                );

            s_propConnection = new ConfigurationProperty(
                "connection",
                typeof(string),
                null,
                ConfigurationPropertyOptions.None
				);

            s_propSourceConnection = new ConfigurationProperty(
                "sourceConnection",
                typeof(string),
                null,
                ConfigurationPropertyOptions.None
				);

            s_propDestinationConnection = new ConfigurationProperty(
                "destinationConnection",
                typeof(string),
                null,
                ConfigurationPropertyOptions.IsRequired
                );

            s_propRemoveExtraBlobs = new ConfigurationProperty(
                "removeExtraBlobs",
                typeof(bool),
                false,
                ConfigurationPropertyOptions.None
                );

			s_properties = new ConfigurationPropertyCollection
			{
			    s_propName,
			    s_propContainer,
                s_propSourceContainer,
                s_propDestinationContainer,
                s_propConnection,
			    s_propSourceConnection,
			    s_propDestinationConnection,
                s_propRemoveExtraBlobs
			};
		}
		#endregion

		#region Fields
		private static ConfigurationPropertyCollection s_properties;
		private static ConfigurationProperty s_propName;
		private static ConfigurationProperty s_propContainer;
        private static ConfigurationProperty s_propSourceContainer;
        private static ConfigurationProperty s_propDestinationContainer;
        private static ConfigurationProperty s_propConnection;
        private static ConfigurationProperty s_propSourceConnection;
        private static ConfigurationProperty s_propDestinationConnection;
        private static ConfigurationProperty s_propRemoveExtraBlobs;
		#endregion

		#region Properties
		public string Name
		{
			get { return (string)base[s_propName]; }
			set { base[s_propName] = value; }
		}

		public string Container
		{
			get { return (string)base[s_propContainer]; }
			set { base[s_propContainer] = value; }
		}

        public string SourceContainer
        {
            get { return (string)base[s_propSourceContainer]; }
            set { base[s_propSourceContainer] = value; }
        }

        public string DestinationContainer
        {
            get { return (string)base[s_propSourceContainer]; }
            set { base[s_propSourceContainer] = value; }
        }

        public string Connection
        {
            get { return (string)base[s_propConnection]; }
            set { base[s_propConnection] = value; }
        }

        public string SourceConnection
		{
            get { return (string)base[s_propSourceConnection]; }
            set { base[s_propSourceConnection] = value; }
		}

        public string DestinationConnection
		{
            get { return (string)base[s_propDestinationConnection]; }
            set { base[s_propDestinationConnection] = value; }
		}

        public bool RemoveExtraBlobs
        {
            get { return (bool)base[s_propRemoveExtraBlobs]; }
            set { base[s_propRemoveExtraBlobs] = value; }
        }

		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				return s_properties;
			}
		}
		#endregion
	}
}
