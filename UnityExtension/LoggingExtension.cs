using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.ObjectBuilder;

namespace UnityExtension
{
	public class LoggingExtension : UnityContainerExtension, ILoggingExtensionConfigurator
	{
		protected override void Initialize()
		{
			Context.Container.RegisterInstance(new PropertyMappingDictionary(), new ContainerControlledLifetimeManager());

			Context.Strategies.AddNew<LoggingInterfaceMethodBuildStrategy>(UnityBuildStage.PostInitialization);
			Context.Strategies.AddNew<LoggingVirtualMethodBuildStrategy>(UnityBuildStage.PreCreation);
		}

		public ILoggingExtensionConfigurator FormatType<T>(Expression<Func<T, object>> mapper)
		{
			var propertyMapping = Context.Container.Resolve<PropertyMappingDictionary>();
			propertyMapping.Add(typeof(T), mapper);

			return this;
		}
	}

	internal class PropertyMappingDictionary
	{
		private readonly IDictionary<Type, LambdaExpression> _mappingDictionary = new Dictionary<Type, LambdaExpression>();

		public void Add(Type type, LambdaExpression expression)
		{
			if(_mappingDictionary.ContainsKey(type))
				throw new Exception(string.Format("Mapper for type '{0}' has already registered.", type.Name));

			_mappingDictionary.Add(type, expression);
		}

		public bool TryGetValue(Type type, out LambdaExpression expression)
		{
			if (!_mappingDictionary.TryGetValue(type, out expression))
			{
				foreach (var mappingKey in _mappingDictionary.Keys.Where(k => k.IsAssignableFrom(type)))
				{
					return TryGetValue(mappingKey, out expression);
				}
				return false;
			}
			return true;
		}
	}
}