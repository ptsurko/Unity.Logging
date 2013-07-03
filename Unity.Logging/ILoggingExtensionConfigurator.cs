using System;
using System.Linq.Expressions;
using Microsoft.Practices.Unity;

namespace Unity.LoggingExtension
{
	public interface ILoggingExtensionConfigurator : IUnityContainerExtensionConfigurator
	{
		ILoggingExtensionConfigurator FormatType<T>(Expression<Func<T, object>> mapper);
	}
}
