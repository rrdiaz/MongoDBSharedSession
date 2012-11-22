using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using IOL.SharedSession;
using System.Collections.Specialized;
using System.Web;
using System.Web.SessionState;
using System.Diagnostics;

namespace IOL.SharedSessionServer.COMWrapper
{
	/// <summary>
	/// SharedSession class to use from Classic ASP
	/// </summary>
	[ComVisible(true)]
    [Guid("4A1198BD-BC9F-4942-A77E-83D5F9A0A946")]
    [ProgId("IOL.SharedSessionServer.COMWrapper.MongoSharedSession")]
    [ClassInterface(ClassInterfaceType.AutoDispatch)]
    [Serializable]
	public class SharedSession : IDisposable
	{
		internal static SharedSessionIDManager IdManager;
		internal static SharedSessionStoreProvider SessionStoreProvider = new SharedSessionStoreProvider();

		private Dictionary<string, CurrentRequestSession> _requests;

		/// <summary>
		/// The name of the cookie that stores the shared session ID
		/// </summary>
		public string SessionCookieName
		{
			get
			{
				return SharedSessionIDManager.SessionIDCookie;
			}
		}

		/// <summary>
		/// Static constructor
		/// </summary>
		static SharedSession()
		{
			IdManager = new SharedSessionIDManager();
			SessionStoreProvider = new SharedSessionStoreProvider();
			SessionStoreProvider.Initialize(
				"SharedSessionStoreProvider",
				new NameValueCollection());
		}

		/// <summary>
		/// Default constructor
		/// </summary>
		public SharedSession()
		{
			_requests = new Dictionary<string, CurrentRequestSession>();
		}

		/// <summary>
		/// Initializes a Classic ASP request
		/// </summary>
		/// <param name="currentSessionCookieValue">The current value for the shared session cookie</param>
		public CurrentRequestSession Start(string currentSessionCookieValue)
		{
                var request = new CurrentRequestSession(currentSessionCookieValue);
                _requests.Add(request.Id, request);
                return request;
		}

		
		/// <summary>
		/// Finalize a Classic ASP request, storing the modified values 
		/// of the shared session
		/// </summary>
		public void Finish(string requestId)
		{
			var request = _requests[requestId];

			if (request.ValueChanged)
			{
				if (request.RemoveSession)
				{
					SessionStoreProvider.RemoveItem(
						request.CurrentContextReference.Current,
						request.GetSessionId(),
						new object(),
						request.CurrentSessionData);
				}
				else
				{
					SessionStoreProvider.SetAndReleaseItemExclusive(
						request.CurrentContextReference.Current,
						request.GetSessionId(),
						request.CurrentSessionData,
						new object(),
						true);
				}
			}

			_requests.Remove(requestId);
		}

		#region IDisposable Members

		/// <summary>
		/// Performs application-defined tasks associated 
		/// with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			if (_requests != null)
			{
				var keys = new List<string>(_requests.Keys);

				foreach (var key in keys)
				{
					try
					{
						var request = _requests[key];
						request.Dispose();
						_requests.Remove(key);
					}
					catch
					{
					}
				}

				_requests = null;
			}
		}

		#endregion
	}
}
