using System;
using System.Linq;
using System.Reflection;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace Unity.LoggingExtension
{
    internal class LoggingInterfaceMethodBehavior : LoggingInterceptionBehavior
    {
        private readonly Type _actualInterceptedType;

        public LoggingInterfaceMethodBehavior(ILogger logger, PropertyMappingDictionary propertyMappingDictionary, Type actualInterceptedType)
            : base(logger, propertyMappingDictionary)
        {
            _actualInterceptedType = actualInterceptedType;
        }

        protected override bool IsMethodRequireLogging(IMethodInvocation input)
        {
            if (input.MethodBase.DeclaringType == null)
                return false;

            var map = _actualInterceptedType.GetInterfaceMap(input.MethodBase.DeclaringType);
            var interceptedInstanceMethod = map.TargetMethods[map.InterfaceMethods.Cast<MethodBase>().ToList().IndexOf(input.MethodBase)];

            return interceptedInstanceMethod.GetMemberCustomAttributes<LogAttribute>().Any();
        }
    }
}