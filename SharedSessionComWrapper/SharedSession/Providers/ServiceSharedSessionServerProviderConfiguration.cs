using System;
using System.Collections.Generic;

using System.Text;
using System.Configuration;

namespace BancoEstado.SharedSession.Providers
{
	/// <summary>
	/// Represents the service shared session provider configuration
	/// </summary>
	public class ServiceSharedSessionServerProviderConfiguration : ConfigurationSection
	{
		public const string ConfigurationKey = "serviceSharedSession";

		/// <summary>
		/// The period to check if the servers are alive
		/// </summary>
		[ConfigurationProperty("checkAlivePeriod", DefaultValue=10)]
		public int CheckAlivePeriod
		{
			get
			{
				return (int)this["checkAlivePeriod"];
			}
		}

		/// <summary>
		/// A list with the configured session servers
		/// </summary>
		[ConfigurationProperty("serverList", IsDefaultCollection=true, IsRequired=true)]
		public ServerInfoList ServerList
		{
			get
			{
				return this["serverList"] as ServerInfoList;
			}
		}

		public class ServerInfoList : ConfigurationElementCollection
		{
			protected override ConfigurationElement CreateNewElement()
			{
				return new ServerInfo();
			}

			protected override object GetElementKey(ConfigurationElement element)
			{
				return ((ServerInfo)element).Name;
			}
		}

		/// <summary>
		/// Represents the server information element in the provider configuration
		/// </summary>
		public class ServerInfo : ConfigurationElement
		{
			/// <summary>
			/// The server name
			/// </summary>
			[ConfigurationProperty("name", IsRequired=true, IsKey=true)]
			public string Name
			{
				get
				{
					return this["name"] as string;
				}
			}

			/// <summary>
			/// The server address
			/// </summary>
			[ConfigurationProperty("address", IsRequired=true)]
			public string Address
			{
				get
				{
					return this["address"] as string;
				}
			}
		}
	}


}
