using System;
using System.Collections.Generic;

using System.Text;
using BancoEstado.SharedSessionServer;
using System.ServiceModel;
using System.Configuration;
using System.Timers;

namespace BancoEstado.SharedSession.Providers
{
	/// <summary>
	/// A ISharedSessionServer implementation over WCF communication with the servers
	/// </summary>
	public class ServiceSharedSessionServerProvider : ISharedSessionServerProvider
	{
		private static Random _random = new Random();
		List<Server> _servers = new List<Server>();
		Timer _checkAliveTimer;
		private int _checkAlivePeriod;

		/// <summary>
		/// Gets the time interval to check if a server is connected
		/// </summary>
		public int CheckAlivePeriod
		{
			get
			{
				return _checkAlivePeriod;
			}
		}

		/// <summary>
		/// Creates a new instance of ServiceSharedSessionServerProvider
		/// </summary>
		public ServiceSharedSessionServerProvider()
		{
			Init();
		}

		/// <summary>
		/// Creates a new instance of ServiceSharedSessionServerProvider using the specified parameters
		/// </summary>
		/// <param name="servers">A dictionary that represents a server lists. 
		/// The keys indicate the names and the values indicate the address</param>
		/// <param name="checkAlivePeriod">The CheckAlivePeriod to use, in seconds</param>
		public ServiceSharedSessionServerProvider(Dictionary<string, string> servers, int checkAlivePeriod)
		{
			Init(servers, checkAlivePeriod);
		}

		/// <summary>
		/// Initializes the provider
		/// </summary>
		/// <param name="servers">A dictionary that represents a server lists. 
		/// The keys indicate the names and the values indicate the address</param>
		/// <param name="checkAlivePeriod">The CheckAlivePeriod to use, in seconds</param>
		private void Init(Dictionary<string, string> servers, int checkAlivePeriod)
		{
			foreach (var k in servers.Keys)
			{
				string address = servers[k];

				Server s = new Server()
				{
					Name = k,
					Address = address,
					Running = false,
					Proxy = ServiceClient<ISessionServer>.CreateServiceChannel(
						new BasicHttpBinding(),
						address)
				};

				_servers.Add(s);
			}

			_checkAlivePeriod = checkAlivePeriod;

			EnableTimer();
		}

		/// <summary>
		/// Initializes the provider using the application configuration.
		/// </summary>
		public void Init()
		{
			//Get the configuration from the application configuration file
			var config = ConfigurationManager.GetSection(ServiceSharedSessionServerProviderConfiguration.ConfigurationKey)
				as ServiceSharedSessionServerProviderConfiguration;

			if (config != null)
			{
				//Add the configured servers
				foreach (var elem in config.ServerList)
				{
					var serverInfo = (ServiceSharedSessionServerProviderConfiguration.ServerInfo)elem;

					Server s = new Server()
					{
						Name = serverInfo.Name,
						Address = serverInfo.Address,
						Running = false,
						Proxy = ServiceClient<ISessionServer>.CreateServiceChannel(
							new BasicHttpBinding(),
							serverInfo.Address)
					};

					_servers.Add(s);
				}

				_checkAlivePeriod = config.CheckAlivePeriod;

				EnableTimer();
			}
		}

		private void EnableTimer()
		{
			_checkAliveTimer = new Timer(CheckAlivePeriod * 1000);
			_checkAliveTimer.Elapsed += new ElapsedEventHandler(CheckAlive);
			_checkAliveTimer.Start();
			CheckAliveInternal();
		}

		void CheckAlive(object sender, ElapsedEventArgs e)
		{
			CheckAliveInternal();
		}

		private void CheckAliveInternal()
		{
			//Check if any of the non-running servers woke up
			foreach (var server in _servers)
			{
				if (!server.Running)
				{
					try
					{
						if (server.Proxy.IsAlive())
						{
							server.Running = true;
						}
					}
					catch (CommunicationException)
					{
					}
				}
			}
		}

		#region ISharedSessionServerProvider Members

		/// <summary>
		/// The Available server IDs
		/// </summary>
		public List<string> AvailableSessionServers
		{
			get 
			{
				List<string> ret = new List<string>();

				foreach (var server in _servers)
				{
					if (server.Running)
					{
						ret.Add(server.Name);
					}
				}

				return ret;
			}
		}

		/// <summary>
		/// Returns a server ID
		/// </summary>
		/// <returns>The server ID</returns>
		public string GetNextServerId()
		{
			List<string> servers = new List<string>(AvailableSessionServers);

			// Get an available server, selected by random
			var serverIndex = _random.Next(0, servers.Count);

			if (serverIndex < servers.Count)
			{
				return servers[serverIndex];
			}
			else
			{
				return String.Empty;
			}
			
		}

		/// <summary>
		/// Stores data in a shared session server
		/// </summary>
		/// <param name="serverId">The server ID</param>
		/// <param name="sessionId">The session ID</param>
		/// <param name="data">The data to store</param>
		/// <param name="timestamp">The data timestamp</param>
		public void SetData(string serverId, string sessionId, string data, DateTime timestamp)
		{
			var server = GetServer(serverId);
			if (server != null)
			{
				try
				{
					//If the server is valid and working, use it to store the session data
					server.Proxy.SetData(sessionId, data, timestamp);
				}
				catch (CommunicationException)
				{
					//Otherwise, disable it and store the data in a new different available server
					DisableServer(server);

					var newServerId = GetNextServerId();
					if (!String.IsNullOrEmpty(newServerId))
					{
						SetData(newServerId, sessionId, data, timestamp);
					}
				}
			}
		}

		/// <summary>
		/// Recover data from a shared session server
		/// </summary>
		/// <param name="serverId">The server ID</param>
		/// <param name="sessionId">The session ID</param>
		/// <param name="timestamp">The session timestamp</param>
		/// <returns>The stored data</returns>
		public StoredData GetData(string serverId, string sessionId, DateTime timestamp)
		{
			try
			{
				// Recover the session data from the selected server
				var server = GetServer(serverId);
				try
				{
					var storedData = server.Proxy.GetData(sessionId, timestamp);
					return new StoredData()
					{
						Data = storedData.Data,
						TimeOut = storedData.TimeOut
					};
				}
				catch (CommunicationException)
				{
					// If the server is down, disable it and try recovering the
					// session data from another available server
					DisableServer(server);

					var newServerId = GetNextServerId();
					if (!String.IsNullOrEmpty(newServerId))
					{
						return GetData(newServerId, sessionId, timestamp);
					}
					else
					{
						return new StoredData();
					}
				}
			}
			catch (Exception ex)
			{
				return new StoredData() { };
			}
		}

		/// <summary>
		/// Remove session data from a shared session server
		/// </summary>
		/// <param name="serverId">The server ID</param>
		/// <param name="sessionId">The session ID</param>
		public void Remove(string serverId, string sessionId)
		{
			var server = GetServer(serverId);
			if (server != null)
			{
				try
				{
					// Try removing the session data from the server
					server.Proxy.Remove(sessionId);
				}
				catch (CommunicationException)
				{
					// If the server is down, disable it and try removing the
					// data from another server
					DisableServer(server);
					
					var newServerId = GetNextServerId();
					if (!String.IsNullOrEmpty(newServerId))
					{
						Remove(newServerId, sessionId);
					}
				}
			}
		}

		/// <summary>
		/// Validates if a session ID is valid. If the server that stores the session data
		/// was changed, it will return the new server ID. Otherwise, the validation result will
		/// contain the same server ID that was used as a parameter
		/// </summary>
		/// <param name="serverId">The server ID</param>
		/// <param name="sessionId">The session ID</param>
		/// <param name="timestamp">The session timestamp</param>
		/// <returns>A ValidationResult indicating if the ID is valid and the server that is storing 
		/// the shared session data</returns>
		public ValidationResult IsValid(string serverId, string sessionId, DateTime timestamp)
		{
			var server = GetServer(serverId);
			try
			{
				// Check if a session id is valid
				return new ValidationResult()
				{
					ServerId = serverId,
					IsValid = server != null && server.Proxy.IsValid(sessionId, timestamp)
				};
			}
			catch (CommunicationException)
			{
				// If the selected server is down, disable it and try checking
				// the session id with another server
				DisableServer(server);

				var newServerId = GetNextServerId();
				if (!String.IsNullOrEmpty(newServerId))
				{
					return IsValid(newServerId, sessionId, timestamp);
				}

				return new ValidationResult()
				{
					ServerId = serverId,
					IsValid = false
				};
			}
		}

		#endregion

		#region Private Members

		private void DisableServer(Server server)
		{
			server.Running = false;
		}

		private Server GetServer(string id)
		{
			foreach (var server in _servers)
			{
				if (server.Name == id)
				{
					return server;
				}
			}

			return null;
		}

		#endregion

		#region Support Classes
		private class Server
		{
			public bool Running
			{
				get;
				set;
			}

			public string Address
			{
				get;
				set;
			}

			public string Name
			{
				get;
				set;
			}

			public ISessionServer Proxy
			{
				get;
				set;
			}
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			try
			{
				if (_checkAliveTimer != null)
				{
					_checkAliveTimer.Dispose();
					_checkAliveTimer = null;
				}
			}
			catch
			{
			}
		}

		#endregion
	}
}
