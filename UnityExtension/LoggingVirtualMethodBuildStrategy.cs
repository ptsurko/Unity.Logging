using System;
using System.Linq;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace UnityExtension
{
	internal class LoggingVirtualMethodBuildStrategy : BuilderStrategy
	{
		private readonly VirtualMethodInterceptor _virtualMethodInterceptor = new VirtualMethodInterceptor();

		public override void PreBuildUp(IBuilderContext context)
		{
			if (context.Existing != null
                || !_virtualMethodInterceptor.CanIntercept(context.OriginalBuildKey.Type)
                || !context.BuildKey.Type.HasCustomerAttributes<LogAttribute>())
				return;

			var interceptingType = _virtualMethodInterceptor.CreateProxyType(context.BuildKey.Type, Type.EmptyTypes);

			IPolicyList selectorPolicyDestination;
			var originalSelectorPolicy = context.Policies.Get<IConstructorSelectorPolicy>(context.BuildKey, out selectorPolicyDestination);

			selectorPolicyDestination.Set<IConstructorSelectorPolicy>(new DerivedTypeConstructorSelectorPolicy(interceptingType, originalSelectorPolicy), context.BuildKey);

			context.Policies.Set(new InterceptVirtualMethodPolicy(), context.BuildKey);
		}

		public override void PostBuildUp(IBuilderContext context)
		{
			var proxy = context.Existing as IInterceptingProxy;
			if (proxy == null || context.Policies.Get<InterceptVirtualMethodPolicy>(context.BuildKey) == null)
				return;

			//var methods = _virtualMethodInterceptor.GetInterceptableMethods(context.OriginalBuildKey.Type, context.BuildKey.Type);

			//proxy.AddInterceptionBehavior(new LoggingVirtualMethodBehavior(context.NewBuildUp<ILogger>(), context.NewBuildUp<PropertyMappingDictionary>()));
			proxy.AddInterceptionBehavior(context.NewBuildUp<LoggingVirtualMethodBehavior>());
		}

		private class DerivedTypeConstructorSelectorPolicy : IConstructorSelectorPolicy
		{
			private readonly Type _interceptingType;
			private readonly IConstructorSelectorPolicy _originalConstructorSelectorPolicy;

			public DerivedTypeConstructorSelectorPolicy(Type interceptingType, IConstructorSelectorPolicy originalConstructorSelectorPolicy)
			{
				_interceptingType = interceptingType;
				_originalConstructorSelectorPolicy = originalConstructorSelectorPolicy;
			}

			public SelectedConstructor SelectConstructor(IBuilderContext context, IPolicyList resolverPolicyDestination)
			{
				var originalConstructor = _originalConstructorSelectorPolicy.SelectConstructor(context, resolverPolicyDestination);

				return FindNewConstructor(originalConstructor, _interceptingType);
			}

			private static SelectedConstructor FindNewConstructor(SelectedConstructor originalConstructor, Type interceptingType)
			{
				var originalParams = originalConstructor.Constructor.GetParameters();

				var newConstructorInfo = interceptingType.GetConstructor(originalParams.Select(pi => pi.ParameterType).ToArray());

				var newConstructor = new SelectedConstructor(newConstructorInfo);

				foreach (var key in originalConstructor.GetParameterKeys())
				{
					newConstructor.AddParameterKey(key);
				}

				return newConstructor;
			}
		}

		private class InterceptVirtualMethodPolicy : IBuilderPolicy { }
	}
}