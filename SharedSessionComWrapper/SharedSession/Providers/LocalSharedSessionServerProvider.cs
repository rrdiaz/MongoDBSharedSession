using System;
using System.Collections.Generic;
using System.Text;
using IOL.SharedSessionServer;

namespace IOL.SharedSession.Providers
{
	public class LocalSharedSessionServerProvider : ISharedSessionServerProvider
	{
		private const string LocalServerId = "LocalServer";

		private SessionServer _localServer;


		/// <summary>
		/// Default constructor for the provider to initialize the Session Server with the application configuration
		/// </summary>		
		public LocalSharedSessionServerProvider()
		{
			_localServer = new SessionServer();
		}

		/// <summary>
		/// Constructor for the provider to initialize the Session Server with the specified parameters
		/// </summary>
		/// <param name="sessionTimeOut">The timeout to expire the session, in minutes</param>
		/// <param name="pollingSeconds">The seconds to poll the sessions to expire</param>
		/// <param name="connStringName">The connection string name</param>
		public LocalSharedSessionServerProvider(int sessionTimeOut, int pollingSeconds, string connStringName)
		{
			_localServer = new SessionServer(sessionTimeOut, pollingSeconds, connStringName);
		}

		#region ISharedSessionServerProvider Members

		/// <summary>
		/// The Available server IDs. Always returns "LocalServer"
		/// </summary>
		public List<string> AvailableSessionServers
		{
			get 
			{
				return new List<string>() { LocalServerId };
			}
		}

		/// <summary>
		/// Returns a server ID
		/// </summary>
		/// <returns>Always returns "LocalServer"</returns>
		public string GetNextServerId()
		{
			return LocalServerId;
		}

		/// <summary>
		/// Stores data in the local session server
		/// </summary>
		/// <param name="serverId">The server ID. This parameter is ignored</param>
		/// <param name="sessionId">The session ID</param>
		/// <param name="data">The data to store</param>
		/// <param name="timestamp">The data timestamp</param>
		public void SetData(string serverId, string sessionId, string data, DateTime timestamp)
		{
            //System.Diagnostics.Debugger.Launch();
			_localServer.SetData(sessionId, data, timestamp);
		}

		/// <summary>
		/// Recover data from the local session server
		/// </summary>
		/// <param name="serverId">The server ID. This parameter is ignored</param>
		/// <param name="sessionId">The session ID</param>
		/// <param name="timestamp">The session timestamp</param>
		/// <returns>The stored data</returns>
		public StoredData GetData(string serverId, string sessionId, DateTime timestamp)
		{
			var data = _localServer.GetData(sessionId, timestamp);
			return new StoredData()
			{
				Data = data.Data,
				TimeOut = data.TimeOut
			};
		}

		/// <summary>
		/// Remove session data from the local session server
		/// </summary>
		/// <param name="serverId">The server ID. This parameter is ignored</param>
		/// <param name="sessionId">The session ID</param>
		public void Remove(string serverId, string sessionId)
		{
			_localServer.Remove(sessionId);
		}

		/// <summary>
		/// Validates if a session ID is valid.
		/// </summary>
		/// <param name="serverId">The server ID. This parameter is ignored</param>
		/// <param name="sessionId">The session ID</param>
		/// <param name="timestamp">The session timestamp</param>
		/// <returns>A ValidationResult indicating if the ID is valid. The ServerId property will always
		/// be "LocalServer"</returns>
		public ValidationResult IsValid(string serverId, string sessionId, DateTime timestamp)
		{
			return new ValidationResult()
			{
				IsValid = _localServer.IsValid(sessionId, timestamp),
				ServerId = LocalServerId
			};
		}

		#endregion

		#region IDisposable Members

		/// <summary>
		/// Summary:
		/// Performs application-defined tasks associated with freeing, releasing, or
		/// resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			if (_localServer != null)
			{
				_localServer.Dispose();
				_localServer = null;
			}
		}

		#endregion
	}
}
