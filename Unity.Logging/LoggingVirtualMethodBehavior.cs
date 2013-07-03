using System.Linq;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace Unity.LoggingExtension
{
	internal class LoggingVirtualMethodBehavior : LoggingInterceptionBehavior
	{
		public LoggingVirtualMethodBehavior(ILogger logger, PropertyMappingDictionary propertyMappingDictionary)
			: base(logger, propertyMappingDictionary)
		{
		}

		protected override bool IsMethodRequireLogging(IMethodInvocation input)
		{
			return input.MethodBase.GetMemberCustomAttributes<LogAttribute>().Any();
		}
	}
}