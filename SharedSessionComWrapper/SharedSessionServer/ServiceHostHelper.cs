using System;
using System.Collections.Generic;

using System.Text;
using System.Threading;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace IOL.SharedSessionServer
{
	/// <summary>
	/// Helper to host WCF services
	/// </summary>
	public static class ServiceHostHelper
	{
		private static Dictionary<string, ServiceHost> _hostedServices;

		/// <summary>
		/// Static constructor
		/// </summary>
		static ServiceHostHelper ()
		{
			_hostedServices = new Dictionary<string, ServiceHost> ();
		}

		/// <summary>
		/// Host a service of the specified type
		/// </summary>
		/// <param name="serviceType">The service type</param>
		[System.Diagnostics.DebuggerHidden()] // TODO: Sacar luego de code coverage
		public static void HostService (Type serviceType)
		{
			lock (_hostedServices) 
			{
				var host = new ServiceHost (serviceType);
				host.Open ();
				_hostedServices.Add (serviceType.FullName, host);
			}
		}

		/// <summary>
		/// Host a service of the specified type, contract and address
		/// </summary>
		/// <param name="serviceType">The service type</param>
		/// <param name="contractType">The service contract</param>
		/// <param name="address">The service address</param>
		public static void HostService (Type serviceType, Type contractType, Uri address)
		{
			lock (_hostedServices) 
			{
				var baseAddress = new Uri (String.Format ("{0}://{1}/", address.Scheme, address.Authority));
				
				
				var host = new ServiceHost(serviceType, baseAddress);

				var binding = new BasicHttpBinding();
				host.AddServiceEndpoint (contractType.FullName, binding, address);
				
				var behavior = host.Description.Behaviors.Find<ServiceBehaviorAttribute>();
				behavior.IncludeExceptionDetailInFaults = true;
				
				ServiceMetadataBehavior metadataBehavior = host.Description.Behaviors.Find<ServiceMetadataBehavior> ();
				if (metadataBehavior == null) 
				{
					metadataBehavior = new ServiceMetadataBehavior ();
					host.Description.Behaviors.Add (metadataBehavior);
				}
				host.AddServiceEndpoint (typeof(IMetadataExchange), new CustomBinding (new HttpTransportBindingElement ()), "MEX");
				
				host.Open ();
				_hostedServices.Add (address.ToString(), host);
			}
		}

		/// <summary>
		/// Stop a hosted service
		/// </summary>
		/// <param name="serviceType">The service type</param>
		[System.Diagnostics.DebuggerHidden()] // TODO: Sacar luego de code coverage
		public static void StopService (Type serviceType)
		{
			lock (_hostedServices) 
			{
				if (_hostedServices.ContainsKey (serviceType.FullName)) 
				{
					_hostedServices[serviceType.FullName].Close ();
					_hostedServices.Remove (serviceType.FullName);
				}
			}
		}

		/// <summary>
		/// Stop a hosted service
		/// </summary>
		/// <param name="address">The service address</param>
		public static void StopService(Uri address)
		{
			lock (_hostedServices)
			{
				if (_hostedServices.ContainsKey(address.ToString()))
				{
					_hostedServices[address.ToString()].Close();
					_hostedServices.Remove(address.ToString());
				}
			}
		}
	}
}