using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Web.SessionState;
using IOL.SharedSession;

namespace IOL.SharedSessionServer.COMWrapper
{
	[ComVisible(true)]
    [Guid("425E0145-9F29-458D-9954-9304C5A60B35")]
    [ProgId("IOL.SharedSessionServer.COMWrapper.CurrentRequestSession")]
    [ClassInterface(ClassInterfaceType.AutoDispatch)]
    [Serializable]
	public class CurrentRequestSession : IDisposable
	{
		internal FakeCurrentHttpContext CurrentContextReference
		{
			get;
			set;
		}

		internal SessionStateStoreData CurrentSessionData
		{
			get;
			set;
		}

		bool _removeSession;
		public bool RemoveSession
		{
			get
			{
				return _removeSession;
			}
		}
		
		bool _valueChanged;
		public bool ValueChanged
		{
			get
			{
				return _valueChanged;
			}
		}

		private string _id = Guid.NewGuid().ToString();
		public string Id
		{
			get
			{
				return _id;
			}
		}

		public CurrentRequestSession(string currentSessionCookieValue)
		{
			CurrentContextReference = new FakeCurrentHttpContext();

			if (!String.IsNullOrEmpty(currentSessionCookieValue))
			{
                CurrentContextReference.Current.Response.Cookies[SessionCookieName].Value = currentSessionCookieValue;
			}

			CurrentSessionData = GetSessionData(GetSessionId());

			_removeSession = false;
			_valueChanged = false;
		}

		/// <summary>
		/// Returns current shared session ID
		/// </summary>
		/// <returns></returns>
		public string GetSessionId()
		{
			var existingSessionId = SharedSession.IdManager.GetSessionID(CurrentContextReference.Current);
			if (String.IsNullOrEmpty(existingSessionId))
			{
				var newSessionId = SharedSession.IdManager.CreateSessionID(CurrentContextReference.Current);

				bool redirected;
				bool cookieAdded;
				SharedSession.IdManager.SaveSessionID(
					CurrentContextReference.Current,
					newSessionId,
					out redirected,
					out cookieAdded);

				return newSessionId;
			}
			else
			{
				return existingSessionId;
			}
		}

		/// <summary>
		/// Gets a value stored in a shared session
		/// </summary>
		/// <param name="key">The key associated with the entry to get</param>
		/// <returns></returns>
		public string Get(string key)
		{
			return CurrentSessionData.Items[key] as string;
		}

	
        /// <summary>
        /// Stores a new entry in a shared session
        /// </summary>
        /// <param name="key">The key for the entry</param>
        /// <param name="value">The value for the entry</param>
        public void Set(string key, string value)
        {
            CurrentSessionData.Items[key] = value;
            _valueChanged = true;
        }

        public SessionValue CreateSessionValue()
        {
            return new SessionValue();
        }


		/// <summary>
		/// Delete the data stored in a shared session
		/// </summary>
		public void Delete()
		{
			_removeSession = true;
			_valueChanged = true;
		}

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

		#region Support Methods
		private SessionStateStoreData GetSessionData(string sessionId)
		{
			bool locked;
			TimeSpan timeSpan;
			object lockId;
			SessionStateActions actions;

			var sessionValues = SharedSession.SessionStoreProvider.GetItem(
				CurrentContextReference.Current,
				sessionId,
				out locked,
				out timeSpan,
				out lockId,
				out actions);

			return sessionValues;
		}
		#endregion

		#region IDisposable Members

		/// <summary>
		/// Performs application-defined tasks associated 
		/// with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			if (CurrentContextReference != null)
			{
				CurrentContextReference.Dispose();
				CurrentContextReference = null;
			}
		}

		#endregion
	}
}
