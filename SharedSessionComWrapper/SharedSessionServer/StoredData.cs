using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace IOL.SharedSessionServer
{
	/// <summary>
	/// The data contract to use with the SessionServer service
	/// </summary>
	[DataContract]
	public class StoredData
	{
		public StoredData()
		{
			Data = String.Empty;
			Valid = true;
		}

		[DataMember]
		public string Data
		{
			get;
			set;
		}

		[DataMember]
		public int TimeOut
		{
			get;
			set;
		}

		public bool Valid
		{
			get;
			set;
		}
	}
}
