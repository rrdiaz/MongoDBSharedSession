using System;
using System.Collections.Generic;

using System.Text;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace IOL.SharedSessionServer
{
	/// <summary>
	/// Service contract for the SessionServer
	/// </summary>
	[ServiceContract]
	public interface ISessionServer
	{
		/// <summary>
		/// Indicates if the server is alive
		/// </summary>
		/// <returns>True if the server is alive</returns>
		[OperationContract]
		bool IsAlive();

		/// <summary>
		/// Stores data in the server
		/// </summary>
		/// <param name="sessionId">The session ID</param>
		/// <param name="data">The data to store</param>
		/// <param name="timestamp">The data timestamp</param>
		[OperationContract]
		void SetData(string sessionId, string data, DateTime timestamp);

		/// <summary>
		/// Retrieves data from the server
		/// </summary>
		/// <param name="sessionId">The session ID</param>
		/// <param name="timestamp">The session timestamp</param>
		/// <returns>The retrieved data</returns>
		[OperationContract]
		StoredData GetData(string sessionId, DateTime timestamp);

		/// <summary>
		/// Remove the stored data from the server
		/// </summary>
		/// <param name="sessionId">The session ID</param>
		[OperationContract]
		void Remove(string sessionId);

		/// <summary>
		/// Checks if a session ID is valid
		/// </summary>
		/// <param name="sessionId">The session ID</param>
		/// <param name="timestamp">The session timestamp</param>
		/// <returns>True if the session ID is valid</returns>
		[OperationContract]
		bool IsValid(string sessionId, DateTime timestamp);
	}
}
