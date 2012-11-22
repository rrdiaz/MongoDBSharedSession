using System;
using System.Collections.Generic;

using System.Text;
using System.Configuration;

namespace IOL.SharedSession
{
	/// <summary>
	/// Represents the shered session provider configuration section
	/// </summary>
	public class ProviderConfiguration : ConfigurationSection
	{
		public const string ConfigurationKey = "sharedSessionStore";

		[ConfigurationProperty("providerType", DefaultValue = "IOL.SharedSession.Providers.ServiceSharedSessionServerProvider")]
		public string ProviderType
		{
			get
			{
				return this["providerType"] as string;
			}
		}
	}
}
