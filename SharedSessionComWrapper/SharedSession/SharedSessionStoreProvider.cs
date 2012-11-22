using System;
using System.Collections.Generic;

using System.Text;
using System.Web.SessionState;
using System.Web;
using System.IO;
using System.Collections.Specialized;
using System.Runtime.Serialization;

namespace IOL.SharedSession
{
	/// <summary>
	/// A SessionStateStoreProviderBase implementation that uses the 
	/// SharedSessionServerManager and the SharedSessionIDManager
	/// </summary>
	public class SharedSessionStoreProvider : SessionStateStoreProviderBase
	{
		/// <summary>
		/// The Provider's name
		/// </summary>
		public override string Name
		{
			get
			{
				return this.GetType().Name;
			}
		}

		/// <summary>
		/// Initializes the provider.
		/// </summary>
		/// <param name="name">The name to use with the provider</param>
		/// <param name="config">A collection of the name/value pairs representing the provider-specific attributes
		/// specified in the configuration for this provider.</param>
		public override void Initialize(string name, NameValueCollection config)
		{
			base.Initialize(name, config);
		}
		
		/// <summary>
		/// Creates a new System.Web.SessionState.SessionStateStoreData object to be
		/// used for the current request.
		/// </summary>
		/// <param name="context">The current request context</param>
		/// <param name="timeout">The session-state System.Web.SessionState.HttpSessionState.Timeout value
		/// for the new System.Web.SessionState.SessionStateStoreData.</param>
		/// <returns>A new System.Web.SessionState.SessionStateStoreData for the current request.</returns>
		public override SessionStateStoreData CreateNewStoreData(HttpContext context, int timeout)
		{
			SessionStateStoreData data = new SessionStateStoreData(
				new SessionStateItemCollection(),
				SessionStateUtility.GetSessionStaticObjects(context),
				timeout);

			return data;
		}

		/// <summary>
		/// Adds a new session-state item to the data store.
		/// </summary>
		/// <param name="context">The current request context</param>
		/// <param name="id">The System.Web.SessionState.HttpSessionState.SessionID for the current request.</param>
		/// <param name="timeout">The timeout for the current request</param>
		public override void CreateUninitializedItem(HttpContext context, string id, int timeout)
		{
			var ids = SharedSessionIDManager.GetSessionIdAndServer(id);
			if (ids == null || ids.Length != 3)
			{
				throw new ArgumentException("Invalid ID", "id");
			}
			var timestamp = SharedSessionIDManager.GetSessionTimestamp(id);
			SharedSessionServerManager.SetData(ids[1], ids[0], string.Empty, timestamp);
		}

		/// <summary>
		/// Summary:
		/// Performs application-defined tasks associated with freeing, releasing, or
		/// resetting unmanaged resources.
		/// </summary>
		public override void Dispose()
		{
		}

		/// <summary>
		/// Called by the System.Web.SessionState.SessionStateModule object for per-request
		/// initialization.
		/// </summary>
		/// <param name="context">The current request context</param>
		public override void EndRequest(HttpContext context)
		{

		}

		/// <summary>
		/// Returns read-only session-state data from the session data store.
		/// </summary>
		/// <param name="context">The current request context</param>
		/// <param name="id">The session id</param>
		/// <param name="locked">Indicates if the item is locked in the data store (always false)</param>
		/// <param name="lockAge">Indicates the age of the lock (always TimeSpan.MaxValue)</param>
		/// <param name="lockId">An object that represents the lock ID</param>
		/// <param name="actions">Actions to perform after the request (always SessionStateActions.None)</param>
		/// <returns></returns>
		public override SessionStateStoreData GetItem(
			HttpContext context, 
			string id, 
			out bool locked, 
			out TimeSpan lockAge, 
			out object lockId, 
			out SessionStateActions actions)
		{
			var ids = SharedSessionIDManager.GetSessionIdAndServer(id);
			var timestamp = SharedSessionIDManager.GetSessionTimestamp(id);
			var data = SharedSessionServerManager.GetData(ids[1], ids[0], timestamp);

			locked = false;
			lockAge = TimeSpan.MaxValue;
			lockId = new Object();
			actions = SessionStateActions.None;

			return Deserialize(context, data.Data, data.TimeOut);
		}

		/// <summary>
		/// Returns read-only session-state data from the session data store.
		/// </summary>
		/// <param name="context">The current request context</param>
		/// <param name="id">The session id</param>
		/// <param name="locked">Indicates if the item is locked in the data store (always false)</param>
		/// <param name="lockAge">Indicates the age of the lock (always TimeSpan.MaxValue)</param>
		/// <param name="lockId">An object that represents the lock ID</param>
		/// <param name="actions">Actions to perform after the request (always SessionStateActions.None)</param>
		/// <returns></returns>
		public override SessionStateStoreData GetItemExclusive(
			HttpContext context, 
			string id, 
			out bool locked, 
			out TimeSpan lockAge, 
			out object lockId, 
			out SessionStateActions actions)
		{
			return GetItem(
				context,
				id,
				out locked,
				out lockAge,
				out lockId,
				out actions);
		}

		/// <summary>
		/// Initializes the request
		/// </summary>
		/// <param name="context">The current request context</param>
		public override void InitializeRequest(HttpContext context)
		{
		}

		/// <summary>
		/// Releases an exclusive lock
		/// </summary>
		/// <param name="context">The current request context</param>
		/// <param name="id">The session id</param>
		/// <param name="lockId">The lock id</param>
		public override void ReleaseItemExclusive(HttpContext context, string id, object lockId)
		{
		}

		/// <summary>
		/// Removes an item from the session store
		/// </summary>
		/// <param name="context">The current request context</param>
		/// <param name="id">The session id</param>
		/// <param name="lockId">The lock id</param>
		/// <param name="item">The item to remove</param>
		public override void RemoveItem(
			HttpContext context, 
			string id, 
			object lockId, 
			SessionStateStoreData item)
		{
			var ids = SharedSessionIDManager.GetSessionIdAndServer(id);
			SharedSessionServerManager.Remove(ids[1], ids[0]);
		}

		/// <summary>
		/// Resets a session timeout
		/// </summary>
		/// <param name="context">The current request context</param>
		/// <param name="id">The session id</param>
		public override void ResetItemTimeout(HttpContext context, string id)
		{
			// Force a session get to update its timeout
			var ids = SharedSessionIDManager.GetSessionIdAndServer(id);
			var timestamp = SharedSessionIDManager.GetSessionTimestamp(id);
			var data = SharedSessionServerManager.GetData(ids[1], ids[0], timestamp);
		}

		/// <summary>Updates the session-item information in the session-state data store 
		/// with values from the current request, and clears the lock on the data.
		/// </summary>
		/// <param name="context">The current request context</param>
		/// <param name="id">The session id</param>
		/// <param name="item">The data to store</param>
		/// <param name="lockId">The lock id</param>
		/// <param name="newItem">Indicates if it is a new item</param>
		public override void SetAndReleaseItemExclusive(
			HttpContext context, 
			string id, 
			SessionStateStoreData item, 
			object lockId, 
			bool newItem)
		{
			//Stores the data in a shared session server
			var ids = SharedSessionIDManager.GetSessionIdAndServer(id);
			var timestamp = DateTime.Now; // new timestamp
			SharedSessionServerManager.SetData(ids[1], ids[0], Serialize((SessionStateItemCollection)item.Items), timestamp);

			// Update the session id information with the new timestamp
			SharedSessionIDManager.UpdateCurrentSessionIdTimestamp(context, id, timestamp);
		}

		/// <summary>
		/// Sets a reference to the System.Web.SessionState.SessionStateItemExpireCallback 
		/// delegate for the Session_OnEnd event defined in the Global.asax file.
		/// </summary>
		/// <param name="expireCallback">This parameter is ignored</param>
		/// <returns>False, indicating that the provider does not support expiration callbacks</returns>
		public override bool SetItemExpireCallback(SessionStateItemExpireCallback expireCallback)
		{
			return false;
		}

		#region Private Methods
		private string Serialize(SessionStateItemCollection items)
		{
			MemoryStream ms = new MemoryStream();
			BinaryWriter writer = new BinaryWriter(ms);

            List<string> itemsABorrar = new List<string>();
            foreach (string item in items.Keys)
            {
                if (!(items[item].GetType().IsSerializable))
                    itemsABorrar.Add(item);
            }

            foreach(string item in itemsABorrar)
                items.Remove(item);

			if (items != null)
			{
                items.Serialize(writer);
			}

			writer.Close();

			return Convert.ToBase64String(ms.ToArray());
		}

		private SessionStateStoreData Deserialize(HttpContext context, string serializedItems, int timeout)
		{
            SessionStateItemCollection sessionItems =
              new SessionStateItemCollection();
            if (serializedItems != null) 
            {
			    MemoryStream ms =
			      new MemoryStream(Convert.FromBase64String(serializedItems));


			    if (ms.Length > 0)
			    {
				    BinaryReader reader = new BinaryReader(ms);
				    sessionItems = SessionStateItemCollection.Deserialize(reader);
			    }
            }
			return new SessionStateStoreData(sessionItems,
			  SessionStateUtility.GetSessionStaticObjects(context),
			  timeout);
		}

		#endregion
	}
}
