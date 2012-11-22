using System;
using System.Collections.Generic;

using System.Text;
using System.Configuration;

namespace IOL.SharedSessionServer
{
	/// <summary>
	/// Represents the SessionServer configuration section
	/// </summary>
	public class SessionServerConfiguration : ConfigurationSection
	{
		public const string ConfigurationKey = "sessionServer";

		[ConfigurationProperty("sessionTimeOut", DefaultValue=30)]
		public int SessionTimeOut
		{
			get
			{
				return (int)this["sessionTimeOut"];
			}
		}

		[ConfigurationProperty("connectionStringName", DefaultValue = "SessionDB")]
		public string ConnectionStringName
		{
			get
			{
				return this["connectionStringName"] as string;
			}
		}

		[ConfigurationProperty("expirationPollingPeriod", DefaultValue = 10)]
		public int ExpirationPollingPeriod
		{
			get
			{
				return (int)this["expirationPollingPeriod"];
			}
		}
	}
}
