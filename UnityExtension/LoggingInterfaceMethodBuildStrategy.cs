using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace UnityExtension
{
    internal class LoggingInterfaceMethodBuildStrategy : BuilderStrategy
    {
        private readonly InterfaceInterceptor _interfaceInterceptor = new InterfaceInterceptor();

        public override void PostBuildUp(IBuilderContext context)
        {
            if (context.Existing is IInterceptingProxy || !_interfaceInterceptor.CanIntercept(context.OriginalBuildKey.Type))
                return;

            if (!context.Existing.GetType().HasCustomerAttributes<LogAttribute>())
                return;

            //var methods = _interfaceInterceptor.GetInterceptableMethods(context.OriginalBuildKey.Type, context.BuildKey.Type);

            var loggingInterceptionBehavior = new LoggingInterfaceMethodBehavior(context.NewBuildUp<ILogger>(), context.NewBuildUp<PropertyMappingDictionary>(), context.Existing.GetType());

            context.Existing = Intercept.ThroughProxy(context.OriginalBuildKey.Type, context.Existing, _interfaceInterceptor, new[] { loggingInterceptionBehavior });
        }
    }
}