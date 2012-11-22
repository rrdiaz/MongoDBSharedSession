using System;
using System.Collections.Generic;

using System.Text;
using System.Web;
using System.IO;
using System.Runtime.InteropServices;

namespace IOL.SharedSessionServer.COMWrapper
{
	/// <summary>
	/// Simulates an HttpContext for Classic ASP
	/// </summary>
    [ComVisible(false)]
	public class FakeCurrentHttpContext : IDisposable
	{
		Stream _stream;
		StreamWriter _writer;
		//string _tempFileName;

		/// <summary>
		/// The simulated HttpContext
		/// </summary>
		public HttpContext Current
		{
			get;
			set;
		}

		/// <summary>
		/// Default constructor
		/// </summary>
		public FakeCurrentHttpContext()
		{
			_stream = new MemoryStream();
			_writer = new StreamWriter(_stream);

			string fileName = Guid.NewGuid().ToString() + ".aspx";
			Current = new HttpContext(new HttpRequest(fileName, "http://localhost/" + fileName, ""), new HttpResponse(_writer));

			HttpContext.Current = Current;
		}

		#region IDisposable Members

		/// <summary>
		/// Performs application-defined tasks associated 
		/// with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			if (_writer != null)
			{
				_writer.Dispose();
				_writer = null;

				_stream.Dispose();
				_stream = null;
			}

			Current = null;
			HttpContext.Current = null;
		}

		#endregion
	}
}
