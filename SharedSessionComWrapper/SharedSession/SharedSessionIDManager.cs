using System;
using System.Collections.Generic;

using System.Text;
using System.Web.SessionState;
using System.Web;
using System.Diagnostics;

namespace IOL.SharedSession
{
	/// <summary>
	/// A ISessionIDManager implementation to use with a ISharedSessionServerProvider
	/// </summary>
	public class SharedSessionIDManager : ISessionIDManager
	{
		public const string SessionIDCookie = "SharedSessionID";

		#region ISessionIDManager Members

		/// <summary>
		/// Creates a valid Session ID. If there is a cookie present from this - or another -  shared-session
		/// application try to reuse the existing Session ID
		/// </summary>
		/// <param name="context">The HttpContext of the current request</param>
		/// <returns>A valid Session ID</returns>
		public string CreateSessionID(HttpContext context)
		{
			string id = String.Empty;

			// If there is a Shared Session cookie and it is valid, return
			// the stored ID
			if (HasSharedSessionCookie(context.Request.Cookies))
			{
				id = GetCookieValue(context);
				if (Validate(id))
				{
					return id;
				}
			}

			// If no valid ID was found, generate a new one, asociating the session
			// with an avaliable Shared Session server
			Guid guid = Guid.NewGuid();
            string serverId = SharedSessionServerManager.GetNextServerId();

			DateTime timestamp = DateTime.Now;
			SharedSessionServerManager.SetData(serverId, guid.ToString(), String.Empty, timestamp);

			id = GetFullSessionID(serverId, guid.ToString(), timestamp);

			return id;

		}

		/// <summary>
		/// Get the current session ID. If the asigned server is no longer available, the ID
		/// will be modified to use a new server
		/// </summary>
		/// <param name="context">The HttpContext of the current request</param>
		/// <returns>A valid Session ID</returns>
		public string GetSessionID(HttpContext context)
		{
			bool found = HasSharedSessionCookie(context.Request.Cookies) || HasSharedSessionCookie(context.Response.Cookies);

			// If the cookie was found, try to use the stored session ID
			if (found)
			{
				string id = GetCookieValue(context);
				
				var result = ValidateInternal(id);
				if (result.IsValid)
				{
					var parts = GetSessionIdAndServer(id);
					var timestamp = GetSessionTimestamp(id);
					var serverId = parts[1];
					var sessionId = parts[0];

					// If the ID is valid and it can keep using the same session server,
					// return the stored ID
					if (serverId == result.ServerId)
					{
						return id;
					}
					else
					{
						// Is the session server was changed, modify the cookie and return
						// the new ID

						context.Response.Cookies.Remove(SessionIDCookie);

						var cookieValue = GetFullSessionID(result.ServerId, sessionId, timestamp);
						//var cookie = new HttpCookie(SessionIDCookie, cookieValue);
						context.Response.Cookies[SessionIDCookie].Value = cookieValue;

						return cookieValue;
					}
				}
				else
				{
					return null;
				}
			}
			else
			{
				return null;
			}
		}

		private static bool HasSharedSessionCookie(HttpCookieCollection cookies)
		{
			bool found = false;
			foreach (string key in cookies.AllKeys)
			{
				if (key == SessionIDCookie)
				{
					found = true;
					break;
				}
			}
			return found;
		}

		private static string GetCookieValue(HttpContext context)
		{
			String id;

			if (HasSharedSessionCookie(context.Response.Cookies))
			{
				id = context.Response.Cookies[SessionIDCookie].Value;
			}
			else if (HasSharedSessionCookie(context.Request.Cookies))
			{
				id = context.Request.Cookies[SessionIDCookie].Value;
			}
			else
			{
				return null;
			}

			if (!String.IsNullOrEmpty(id) && id.Contains("%"))
			{
				id = context.Server.UrlDecode(id);
			}

			return id;
		}

		/// <summary>
		/// Initialize the SharedSessionIDManager
		/// </summary>
		public void Initialize()
		{
		}

		/// <summary>
		/// Initialize a request
		/// </summary>
		/// <param name="context">The current request context</param>
		/// <param name="suppressAutoDetectRedirect">true if the session-ID manager should redirect to determine cookie support;
		/// otherwise, false to suppress automatic redirection to determine cookie support.</param>
		/// <param name="supportSessionIDReissue">When this method returns, contains a Boolean that indicates whether the System.Web.SessionState.ISessionIDManager
		/// object supports issuing new session IDs when the original ID is out of date.</param>
		/// <returns>Always returns false, indicating that no redirect was performed.</returns>
		public bool InitializeRequest(HttpContext context, bool suppressAutoDetectRedirect, out bool supportSessionIDReissue)
		{
			supportSessionIDReissue = false;

			return false;
		}

		/// <summary>
		/// Deletes the session identifier from the cookie.
		/// </summary>
		/// <param name="context">The current request context</param>
		public void RemoveSessionID(HttpContext context)
		{
			context.Request.Cookies.Remove(SessionIDCookie);
			context.Response.Cookies.Remove(SessionIDCookie);
		}

		/// <summary>
		/// Saves a newly created session identifier to the HTTP response
		/// </summary>
		/// <param name="context">The current request context</param>
		/// <param name="id">The session id</param>
		/// <param name="redirected">Indicates if the request was redirected (always false)</param>
		/// <param name="cookieAdded">Indicates if a cookie was added (always true)</param>
		public void SaveSessionID(HttpContext context, string id, out bool redirected, out bool cookieAdded)
		{
			SetSessionIdInternal(context, id);

			redirected = false;
			cookieAdded = true;
		}

		/// <summary>
		/// Indicates if a session id is valid
		/// </summary>
		/// <param name="id">The session id to validate</param>
		/// <returns>True if the session id is valid</returns>
		public bool Validate(string id)
		{
			return ValidateInternal(id).IsValid;
		}

		#endregion

		private ValidationResult ValidateInternal(string id)
		{
			if (String.IsNullOrEmpty(id))
			{
				return new ValidationResult()
				{
					IsValid = false
				};
			}

			var ids = GetSessionIdAndServer(id);
			if (ids.Length != 3)
			{
				return new ValidationResult()
				{
					IsValid = false
				};
			}

			var serverId = ids[1];
			var sessionId = ids[0];
			var timestamp = GetSessionTimestamp(id);

			//If the session id is well-formed, use a shared session server to validate it
			var result = SharedSessionServerManager.IsValid(serverId, sessionId, timestamp);

			return result;
		}

		/// <summary>
		/// Returns the server ID and the session ID parts
		/// </summary>
		/// <param name="id">A generated shared session ID</param>
		/// <returns>An array with the session ID in the first possition and the server in the second possition</returns>
		public static string[] GetSessionIdAndServer(string id)
		{
			if (String.IsNullOrEmpty(id))
			{
				throw new ArgumentException("Invalid Id", "id");
			}
			
			return id.Split('/');
		}

		/// <summary>
		/// Get the timestamp part of a session id
		/// </summary>
		/// <param name="id">The session id</param>
		/// <returns>The timestamp asociated to a session id</returns>
		public static DateTime GetSessionTimestamp(string id)
		{
			var parts = GetSessionIdAndServer(id);
			return DateTime.FromBinary(long.Parse(parts[2]));
		}

		/// <summary>
		/// Generates a shared session ID
		/// </summary>
		/// <param name="serverId">The server ID</param>
		/// <param name="sessionId">The session ID</param>
		/// <param name="sessionTimestamp">The session data timestamp</param>
		/// <returns>A well-formated shared session ID</returns>
		public static string GetFullSessionID(string serverId, string sessionId, DateTime sessionTimestamp)
		{
			return String.Format("{0}/{1}/{2}",
				sessionId,
				serverId,
				sessionTimestamp.ToBinary());
		}

		/// <summary>
		/// Updates the timestamp asociated to a session id, modifying the stored cookie value
		/// </summary>
		/// <param name="context">The current request context</param>
		/// <param name="currentId">The current session id</param>
		/// <param name="timestamp">The new timestamp</param>
		public static void UpdateCurrentSessionIdTimestamp(HttpContext context, string currentId, DateTime timestamp)
		{
			var parts = GetSessionIdAndServer(currentId);
			var newId = GetFullSessionID(parts[1], parts[0], timestamp);

			SetSessionIdInternal(context, newId);
		}

		private static void SetSessionIdInternal(HttpContext context, string id)
		{
			context.Response.Cookies[SessionIDCookie].Value = id;
		}
	}
}
