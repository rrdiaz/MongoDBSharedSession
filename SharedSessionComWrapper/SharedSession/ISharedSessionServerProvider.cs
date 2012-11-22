using System;
using System.Collections.Generic;

using System.Text;

namespace IOL.SharedSession
{
	/// <summary>
	/// Provider for a shared session server connector
	/// </summary>
	public interface ISharedSessionServerProvider : IDisposable
	{
		/// <summary>
		/// The Available server IDs
		/// </summary>
		List<string> AvailableSessionServers
		{
			get;
		}

		/// <summary>
		/// Returns a server ID
		/// </summary>
		/// <returns>The server ID</returns>
		string GetNextServerId();

		/// <summary>
		/// Stores data in a shared session server
		/// </summary>
		/// <param name="serverId">The server ID</param>
		/// <param name="sessionId">The session ID</param>
		/// <param name="data">The data to store</param>
		/// <param name="timestamp">The data timestamp</param>
		void SetData(string serverId, string sessionId, string data, DateTime timestamp);

		/// <summary>
		/// Recover data from a shared session server
		/// </summary>
		/// <param name="serverId">The server ID</param>
		/// <param name="sessionId">The session ID</param>
		/// <param name="sessionId">The session timestamp</param>
		/// <returns>The stored data</returns>
		StoredData GetData(string serverId, string sessionId, DateTime timestamp);

		/// <summary>
		/// Remove session data from a shared session server
		/// </summary>
		/// <param name="serverId">The server ID</param>
		/// <param name="sessionId">The session ID</param>
		void Remove(string serverId, string sessionId);

		/// <summary>
		/// Validates if a session ID. If the server that stores the session data
		/// was changed, it will return the new server ID. Otherwise, the validation result will
		/// contain the same server ID that was used as a parameter
		/// </summary>
		/// <param name="serverId">The server ID</param>
		/// <param name="sessionId">The session ID</param>
		/// <param name="timestamp">The session timestamp</param>
		/// <returns>A ValidationResult indicating if the ID is valid and the server that is storing 
		/// the shared session data</returns>
		ValidationResult IsValid(string serverId, string sessionId, DateTime timestamp);
	}
}
