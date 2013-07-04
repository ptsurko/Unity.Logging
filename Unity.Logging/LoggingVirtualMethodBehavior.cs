using System.Linq;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace Unity.LoggingExtension
{
	internal class LoggingVirtualMethodBehavior : LoggingInterceptionBehavior
	{
        public LoggingVirtualMethodBehavior(ILogMethodInvocation logMethodInvocation, PropertyMappingDictionary propertyMappingDictionary)
            : base(logMethodInvocation, propertyMappingDictionary)
		{
		}

		protected override bool IsMethodRequireLogging(IMethodInvocation input)
		{
			return input.MethodBase.GetMemberCustomAttributes<LogAttribute>().Any();
		}
	}
}