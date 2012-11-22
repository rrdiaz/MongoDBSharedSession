using System;
using System.Collections.Generic;

using System.Text;
using System.ServiceModel;
using System.Globalization;

namespace IOL.SharedSessionServer
{
	/// <summary>
	/// A helper to consume WCF Services
	/// </summary>
	/// <typeparam name="TChannel"></typeparam>
	public class ServiceClient<TChannel> : ClientBase<TChannel> where TChannel : class
	{
		#region Constructors

		/// <summary>
		/// Default constructor
		/// </summary>
		[System.Diagnostics.DebuggerHidden()] // TODO: Sacar luego de code coverage
		public ServiceClient()
			: base()
		{
		}

		/// <summary>
		/// Constructor used to specify the endpoint to use by name
		/// </summary>
		/// <param name="endpointConfigurationName">The endpoint name</param>
		[System.Diagnostics.DebuggerHidden()] // TODO: Sacar luego de code coverage
		public ServiceClient(string endpointConfigurationName)
			: base(endpointConfigurationName)
		{
		}

		/// <summary>
		/// Constructor used to specify the endpoint to use by name and the address
		/// </summary>
		/// <param name="endpointConfigurationName">The endpoint name</param>
		/// <param name="remoteAddress">The service address</param>
		[System.Diagnostics.DebuggerHidden()] // TODO: Sacar luego de code coverage
		public ServiceClient(string endpointConfigurationName, string remoteAddress)
			: base(endpointConfigurationName, new EndpointAddress(remoteAddress))
		{
		}

		#endregion

		#region Properties

		/// <summary>
		/// The channel to use with the server
		/// </summary>
		public TChannel InvokerChannel
		{
			[System.Diagnostics.DebuggerHidden()] // TODO: Sacar luego de code coverage
			get { return base.Channel; }
		}

		#endregion

		#region Public Methods
		/// <summary>
		/// Creates a channel to consume a WCF service
		/// </summary>
		/// <param name="endpointConfigurationName">The endpoint to use</param>
		/// <returns></returns>
		[System.Diagnostics.DebuggerHidden()] // TODO: Sacar luego de code coverage
		public static TChannel CreateServiceChannel(string endpointConfigurationName)
		{
			if (endpointConfigurationName == null)
				throw new Exception(String.Format(CultureInfo.InvariantCulture, "El endpoint no existe", typeof(TChannel).FullName));

			ChannelFactory<TChannel> factory = new ChannelFactory<TChannel>(endpointConfigurationName);

			return factory.CreateChannel();
		}

		/// <summary>
		/// Creates a channel to consume a WCF service
		/// </summary>
		/// <param name="endpointConfigurationName">The endpoint to use</param>
		/// <param name="remoteAddress">The service address</param>
		/// <returns></returns>
		[System.Diagnostics.DebuggerHidden()] // TODO: Sacar luego de code coverage
		public static TChannel CreateServiceChannel(string endpointConfigurationName, string remoteAddress)
		{
			ChannelFactory<TChannel> factory = new ChannelFactory<TChannel>(endpointConfigurationName, new EndpointAddress(remoteAddress));
			return factory.CreateChannel();
		}

		/// <summary>
		/// Creates a channel to consume a WCF service
		/// </summary>
		/// <param name="binding">The binding to use</param>
		/// <param name="remoteAddress">The service address</param>
		/// <returns></returns>
		public static TChannel CreateServiceChannel(System.ServiceModel.Channels.Binding binding, string remoteAddress)
		{
			ChannelFactory<TChannel> factory = new ChannelFactory<TChannel>(
				binding,
				new EndpointAddress(remoteAddress));

			return factory.CreateChannel();
		}

		/// <summary>
		/// Release the resouces used by the generated proxy
		/// </summary>
		/// <param name="proxy">The generated proxy</param>
		[System.Diagnostics.DebuggerHidden()] // TODO: Sacar luego de code coverage
		public static void DisposeProxy(TChannel proxy)
		{
			ICommunicationObject wcfProxy = proxy as ICommunicationObject;
			if (wcfProxy != null && wcfProxy.State == CommunicationState.Faulted)
			{
				wcfProxy.Abort();
			}
			else if (wcfProxy != null && wcfProxy.State != CommunicationState.Closed)
			{
				wcfProxy.Close();
			}

			IDisposable dispProxy = proxy as IDisposable;
			if (dispProxy != null)
				dispProxy.Dispose();
		}

		#endregion
	}
}
