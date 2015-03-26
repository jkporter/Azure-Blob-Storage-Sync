using System.Configuration;

namespace AzureBlobStorageSync
{
	public class JobsConfigurationSection: ConfigurationSection
	{
		#region Constructors
		static JobsConfigurationSection()
		{
			s_propMaxAge = new ConfigurationProperty(
				"maxAge",
				typeof(int),
				-1,
				ConfigurationPropertyOptions.IsRequired
				);

			s_propJobs = new ConfigurationProperty(
				"",
				typeof(JobConfigurationElementCollection),
				null,
				ConfigurationPropertyOptions.IsRequired | ConfigurationPropertyOptions.IsDefaultCollection
				);

            s_properties = new ConfigurationPropertyCollection { s_propMaxAge, s_propJobs };
		}
		#endregion

		#region Fields
		private static ConfigurationPropertyCollection s_properties;
        private static ConfigurationProperty s_propMaxAge;
		private static ConfigurationProperty s_propJobs;
		#endregion

		#region Properties
		public int MaxAge
		{
            get { return (int)base[s_propMaxAge]; }
            set { base[s_propMaxAge] = value; }
		}

		public JobConfigurationElementCollection Jobs
		{
			get { return (JobConfigurationElementCollection)base[s_propJobs]; }
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