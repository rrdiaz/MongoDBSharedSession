using System;
using System.Collections.Generic;

using System.Text;
using System.ServiceModel;
using System.Timers;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Transactions;

namespace IOL.SharedSessionServer
{
	/// <summary>
	/// Session server implementation
	/// </summary>
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
	public class SessionServer : ISessionServer, IDisposable
	{
		private int _sessionTimeOut = 30;
		private Timer _expireTimer;
		private string _connString;
        MongoDataAccess conn = null;
        

		/// <summary>
		/// Default constructor for the SessionServer to initialize the server with the application configuration
		/// </summary>
		public SessionServer()
		{
			var config = ConfigurationManager.GetSection(SessionServerConfiguration.ConfigurationKey)
				as SessionServerConfiguration;

			if (config != null)
			{
				Init(config.SessionTimeOut, config.ExpirationPollingPeriod, config.ConnectionStringName);
			}

            conn = new MongoDataAccess(_connString);

		}

		/// <summary>
		/// Constructor for the SessionServer
		/// </summary>
		/// <param name="sessionTimeOut">The timeout to expire the session, in minutes</param>
		/// <param name="pollingSeconds">The seconds to poll the sessions to expire</param>
		/// <param name="connStringName">The connection string name</param>
		public SessionServer(int sessionTimeOut, int pollingSeconds, string connStringName)
		{
			Init(sessionTimeOut, pollingSeconds, connStringName);
		}

		private void Init(int sessionTimeOut, int pollingSeconds, string connStringName)
		{
			_connString = ConfigurationManager.ConnectionStrings[connStringName].ConnectionString;
			_sessionTimeOut = sessionTimeOut;

			_expireTimer = new Timer(pollingSeconds * 1000);
			_expireTimer.Elapsed += new ElapsedEventHandler(ExpireSession);
			_expireTimer.Start();
			ExpireSessionInternal();
		}

		void ExpireSession(object sender, ElapsedEventArgs e)
		{
			ExpireSessionInternal();
		}

		private void ExpireSessionInternal()
		{
			// Iterate over the stored sessions and remove the expired items
			var server = this;

		}

		#region ISessionServer Members

		/// <summary>
		/// Stores the data in the server cache and the DB
		/// </summary>
		/// <param name="sessionId">The session ID</param>
		/// <param name="data">The data to store</param>
		public void SetData(string sessionId, string data, DateTime timestamp)
		{
            //System.Diagnostics.Debugger.Launch();

			// Store the data locally

			// Store the data in the Session DB
            //using (var conn = MongoDataAccess(_connString)) 
            //{
            //    conn.SetData(sessionId, data, DateTime.Now);
            //}

            conn.SetData(sessionId, data, DateTime.Now);

		}

		/// <summary>
		/// Gets the stored data and updates the LastAccess field in the DB
		/// </summary>
		/// <param name="sessionId">The session ID</param>
		/// <returns>The stored data</returns>
		public StoredData GetData(string sessionId, DateTime timestamp)
		{

            return new StoredData()
            {
                Data = conn.GetSessionData(sessionId),
                TimeOut = _sessionTimeOut
            };
			
            //    // If the data wasn't cached, recover it from the Session DB
            //    using (var conn = new SqlConnection(_connString))
            //    {
            //        conn.Open();
            //        using (var command = conn.CreateCommand())
            //        {
            //            command.CommandType = CommandType.StoredProcedure;
            //            command.CommandText = "GetSessionData";
            //            command.Parameters.AddWithValue("@sessionId", sessionId);
            //            command.Parameters.AddWithValue("@minDate", DateTime.Now.AddMinutes(_sessionTimeOut * -1));

            //            var data = command.ExecuteScalar() as String;
            //            if (data != null)
            //            {
            //                //Store the data in the local cache

            //                return new StoredData()
            //                {
            //                    Data = data,
            //                    TimeOut = _sessionTimeOut
            //                };
            //            }
            //        }

            //        conn.Close();

            //    // If it wasn't able to recover the session data, return an 
            //    // a result indicating that its data is not valid
            //    return new StoredData()
            //    {
            //        Valid = false
            //    };
            //}

             
		}

		/// <summary>
		/// Removes the session from the server and the DB
		/// </summary>
		/// <param name="sessionId">The session ID to remove</param>
		public void Remove(string sessionId)
		{
			// If the data is in the local cache, remove it
			// Remove the data from the Session DB
			using (var conn = new SqlConnection(_connString))
			{
				conn.Open();
				using (var command = conn.CreateCommand())
				{
					command.CommandType = CommandType.StoredProcedure;
					command.CommandText = "DeleteSessionData";
					command.Parameters.AddWithValue("@sessionId", sessionId);

					command.ExecuteNonQuery();
				}

				conn.Close();
			}
		}

		/// <summary>
		/// Checks if a session ID is valid
		/// </summary>
		/// <param name="sessionId">The session ID</param>
		/// <param name="timestamp">The session timestamp</param>
		/// <returns></returns>
		public bool IsValid(string sessionId, DateTime timestamp)
		{
				// If the session is not in the local cache, try recovering it from the DB
				return GetData(sessionId, timestamp).Valid;
		}

		/// <summary>
		/// Returns if the server is alive
		/// </summary>
		/// <returns>Always true</returns>
		public bool IsAlive()
		{
			return true;
		}

		#endregion

		private class SessionData
		{
			public SessionData(string data, DateTime timestamp)
			{
				Data = data;
				LastAccess = DateTime.Now;
				Timestamp = timestamp;
			}

			public string Data
			{
				get;
				set;
			}

			public DateTime LastAccess
			{
				get;
				set;
			}

			public DateTime Timestamp
			{
				get;
				set;
			}
		}

		#region IDisposable Members

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or
		/// resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			try
			{
				if (_expireTimer != null)
				{
					_expireTimer.Dispose();
					_expireTimer = null;
				}
			}
			catch
			{
			}
		}

		#endregion
	}
}
