using System;
using System.Collections.Generic;

using System.Text;
using IOL.SharedSession.Providers;
using System.Configuration;

namespace IOL.SharedSession
{
	public static class SharedSessionServerManager
	{
		private static ISharedSessionServerProvider _provider;

		/// <summary>
		/// Static constructor to initialize the SharedSessionServerManager using the application configuration
		/// </summary>
		static SharedSessionServerManager()
		{
			Initialize();
		}

		/// <summary>
		/// Initializes the Shared Session Server Manager with the setting in the application
		/// configuration file
		/// </summary>
		public static void Initialize()
		{
            var config = ConfigurationManager.GetSection(ProviderConfiguration.ConfigurationKey) as ProviderConfiguration;


            //string cfg = "rrr";
            //if (config != null) cfg = "wwww";
            //System.IO.FileStream fs = new System.IO.FileStream("c:/log4.txt", System.IO.FileMode.CreateNew);
            //System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            //byte[] b = encoding.GetBytes(cfg);
            //fs.Write(b, 0, b.Length);
            //fs.Close();

                
			if (config != null)
			{
				//Create a provider with the type specified in the configuration




                Type providerType = Type.GetType(config.ProviderType, false, true);
                
                
				if (providerType == null)
				{
					throw new ConfigurationException(
						String.Format("Type '{0} was not found",
							config.ProviderType));
				}

				var provider = Activator.CreateInstance(providerType) as ISharedSessionServerProvider;

				if (provider == null)
				{
					throw new ConfigurationException(
						String.Format("Unable to create an ISharedSessionServerProvider from type '{0}'",
							config.ProviderType));
				}

				_provider = provider;
			}
			else
			{
				throw new ConfigurationException("SharedSessionServerManager configuration was not found.");
			}
		}

		/// <summary>
		/// The ISharedSessionServerProvider instance to use
		/// </summary>
		public static ISharedSessionServerProvider Provider
		{
			get
			{
				return _provider;
			}
		}

		/// <summary>
		/// The Available server IDs
		/// </summary>
		public static List<string> AvailableSessionServers
		{
			get
			{
				return _provider.AvailableSessionServers;
			}
		}

		/// <summary>
		/// Returns a server ID
		/// </summary>
		/// <returns></returns>
		public static string GetNextServerId()
		{
			return _provider.GetNextServerId();
		}

		/// <summary>
		/// Stores data in a shared session server
		/// </summary>
		/// <param name="serverId">The server ID</param>
		/// <param name="sessionId">The session ID</param>
		/// <param name="data">The data to store</param>
		/// <param name="timestamp">The data timestamp</param>
		public static void SetData(string serverId, string sessionId, string data, DateTime timestamp)
		{
			_provider.SetData(serverId, sessionId, data, timestamp);
		}

		/// <summary>
		/// Recover data from a shared session server
		/// </summary>
		/// <param name="serverId">The server ID</param>
		/// <param name="sessionId">The session ID</param>
		/// <param name="sessionId">The session timestamp</param>
		/// <returns>The stored data</returns>
		public static StoredData GetData(string serverId, string sessionId, DateTime timestamp)
		{
			return _provider.GetData(serverId, sessionId, timestamp);
		}

		/// <summary>
		/// Remove session data from a shared session server
		/// </summary>
		/// <param name="serverId">The server ID</param>
		/// <param name="sessionId">The session ID</param>
		public static void Remove(string serverId, string sessionId)
		{
			_provider.Remove(serverId, sessionId);
		}

		/// <summary>
		/// Validates a session ID. If the server that stores the session data
		/// was changed, it will return the new server ID. Otherwise, the validation result will
		/// contain the same server ID that was used as a parameter
		/// </summary>
		/// <param name="serverId">The server ID</param>
		/// <param name="sessionId">The session ID</param>
		/// <param name="timestamp">The session timestamp</param>
		/// <returns>A ValidationResult indicating if the ID is valid and the server that is storing 
		/// the shared session data</returns>
		public static ValidationResult IsValid(string serverId, string sessionId, DateTime timestamp)
		{
			return _provider.IsValid(serverId, sessionId, timestamp);
		}
	}
}
