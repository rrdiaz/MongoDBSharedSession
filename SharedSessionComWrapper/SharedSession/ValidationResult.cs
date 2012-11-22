using System;
using System.Collections.Generic;

using System.Text;

namespace IOL.SharedSession
{
	/// <summary>
	/// The result of a validation of a session id validation request
	/// </summary>
	public class ValidationResult
	{
		/// <summary>
		/// Indicates if the session id is valid
		/// </summary>
		public bool IsValid
		{
			get;
			set;
		}

		/// <summary>
		/// Indicates the server ID where the session is stored
		/// </summary>
		public string ServerId
		{
			get;
			set;
		}
	}
}
