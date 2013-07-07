using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.ObjectBuilder;

namespace Unity.LoggingExtension
{
    public class LoggingExtension : UnityContainerExtension, ILoggingExtensionConfigurator
    {
        protected override void Initialize()
        {
            Context.Container.RegisterInstance(new PropertyMappingDictionary(), new ContainerControlledLifetimeManager());
            Context.Container.RegisterType<ILogMethodInvocation, LoggerMethodInvocation>(new ContainerControlledLifetimeManager());

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
            if (_mappingDictionary.ContainsKey(type))
                throw new Exception(string.Format("Mapper for type '{0}' has already registered.", type.Name));

            _mappingDictionary.Add(type, expression);
        }

        public bool TryGetValue(Type type, out LambdaExpression expression)
        {
            if (!_mappingDictionary.TryGetValue(type, out expression))
            {
                var possibleTypes = _mappingDictionary.Keys.Where(k => k.IsAssignableFrom(type)).ToList();
                if (!possibleTypes.Any())
                {
                    return false;
                }
                if (possibleTypes.Count() == 1)
                {
                    return true;
                }

                var baseType = GetInheritancHierarchy(type).First(possibleTypes.Contains);
                expression = _mappingDictionary[baseType];
                return true;
            }
            return true;
        }

        private static IEnumerable<Type> GetInheritancHierarchy(Type type)
        {
            for (var current = type; current != null; current = current.BaseType)
                yield return current;
        }
    }
}