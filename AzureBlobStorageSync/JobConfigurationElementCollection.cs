using System.Configuration;

namespace AzureBlobStorageSync
{
	public class JobConfigurationElementCollection: ConfigurationElementCollection
	{
		#region Constructor

	    #endregion

		#region Properties
		public override ConfigurationElementCollectionType CollectionType
		{
			get
			{
				return ConfigurationElementCollectionType.BasicMap;
			}
		}
		protected override string ElementName
		{
			get
			{
				return "job";
			}
		}

		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				return new ConfigurationPropertyCollection();
			}
		}
		#endregion

		#region Indexers
        public JobConfigurationElement this[int index]
		{
			get
			{
                return (JobConfigurationElement)BaseGet(index);
			}
			set
			{
				if (BaseGet(index) != null)
				{
					BaseRemoveAt(index);
				}
				base.BaseAdd(index, value);
			}
		}

        public new JobConfigurationElement this[string name]
		{
			get
			{
                return (JobConfigurationElement)BaseGet(name);
			}
		}
		#endregion

		#region Methods
        public void Add(JobConfigurationElement item)
		{
			base.BaseAdd(item);
		}

        public void Remove(JobConfigurationElement item)
		{
			BaseRemove(item);
		}

		public void RemoveAt(int index)
		{
			BaseRemoveAt(index);
		}
		#endregion

		#region Overrides
		protected override ConfigurationElement CreateNewElement()
		{
            return new JobConfigurationElement();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
            return (element as JobConfigurationElement).Name;
		}
		#endregion
	}
}
