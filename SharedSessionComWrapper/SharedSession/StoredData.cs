using System;
using System.Collections.Generic;

using System.Text;

namespace IOL.SharedSession
{
	/// <summary>
	/// Represents the data stored in a shared session server
	/// </summary>
	public class StoredData
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		public StoredData()
		{
			Data = String.Empty;
		}

		/// <summary>
		/// The stored data
		/// </summary>
		public string Data
		{
			get;
			set;
		}

		/// <summary>
		/// Timeout in minutes until the data is erased from the server
		/// </summary>
		public int TimeOut
		{
			get;
			set;
		}

		[System.Diagnostics.DebuggerHidden()] // TODO: Sacar luego de code coverage
		public override bool Equals(object obj)
		{
			StoredData d = obj as StoredData;
			if (d != null)
			{
				return d.Data == this.Data && d.TimeOut == this.TimeOut;
			}
			else
			{
				return false;
			}
		}

		[System.Diagnostics.DebuggerHidden()] // TODO: Sacar luego de code coverage
		public override int GetHashCode()
		{
			return this.Data.GetHashCode();
		}
	}
}
