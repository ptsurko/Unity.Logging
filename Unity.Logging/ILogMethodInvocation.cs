using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Unity.LoggingExtension
{
	public interface ILogMethodInvocation
	{
		void LogMethodEntering(MethodBase methodBase, IEnumerable<KeyValuePair<ParameterInfo, object>> parameters);
		void LogMethodLeaving(MethodBase methodBase, object result);
	}

	public class LoggerMethodInvocation : ILogMethodInvocation
	{
		private readonly ILogger _logger;

		public LoggerMethodInvocation(ILogger logger)
		{
			_logger = logger;
		}

		public void LogMethodEntering(MethodBase methodBase, IEnumerable<KeyValuePair<ParameterInfo, object>> parameters)
		{
            _logger.Log(string.Format("Entering {0}: {1}", methodBase.Name, string.Join(", ", parameters.Select(p => string.Format("{0}: [{1}]", p.Key.Name, FormatValue(p.Value))))));
		}

		public void LogMethodLeaving(MethodBase methodBase, object result)
		{
            _logger.Log(string.Format("Leaving {0}: result - [{1}]", methodBase.Name, FormatValue(result)));
		}

	    private static object FormatValue(object val)
	    {
	        if (val is IEnumerable && !(val is string))
	        {
                return string.Join(",", ((IEnumerable<object>)val).Select(v => v.ToString()));
	        }
	        return val;
	    }
	}
}
