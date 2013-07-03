using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace Unity.LoggingExtension
{
	internal abstract class LoggingInterceptionBehavior : IInterceptionBehavior
	{
		private readonly ILogger _logger;
		private readonly PropertyMappingDictionary _propertyMappingDictionary;

		protected LoggingInterceptionBehavior(ILogger logger, PropertyMappingDictionary propertyMappingDictionary)
		{
			_logger = logger;
			_propertyMappingDictionary = propertyMappingDictionary;
		}

		protected abstract bool IsMethodRequireLogging(IMethodInvocation input);

		public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
		{
			var methodInfo = getNext();

			var isLoggingRequired = IsMethodRequireLogging(input);
			
			if (isLoggingRequired)
			{
				var parameters = GetParametersWithValues(input);

				_logger.Log(string.Format("Entering {0} : {1}", input.MethodBase.Name, string.Join(", ", parameters.Select(p => string.Format("{0}: {1}", p.Key.Name, p.Value)))));
			}

			var methodReturn = methodInfo.Invoke(input, getNext);
			
			if (isLoggingRequired)
			{
				var result = GetResultValue(methodReturn);

				_logger.Log(string.Format("Leaving {0} : result - {1}", input.MethodBase.Name, result));
			}

			return methodReturn;
		}

		public IEnumerable<Type> GetRequiredInterfaces()
		{
			return Type.EmptyTypes;
		}

		public bool WillExecute
		{
			get { return true; }
		}

		private IEnumerable<KeyValuePair<ParameterInfo, object>> GetParametersWithValues(IMethodInvocation input)
		{
			var result = new List<KeyValuePair<ParameterInfo, object>>();
			var parameters = input.MethodBase.GetParameters();
			for (var i = 0; i < parameters.Count(); i++)
			{
				LambdaExpression parameterTypeMap;
				if (_propertyMappingDictionary.TryGetValue(parameters[i].ParameterType, out parameterTypeMap))
				{
					result.Add(new KeyValuePair<ParameterInfo, object>(parameters[i], parameterTypeMap.Compile().DynamicInvoke(input.Arguments[i])));
				}
				else
				{
					result.Add(new KeyValuePair<ParameterInfo, object>(parameters[i], input.Arguments[i]));
				}
			}
			return result;
		}

		private object GetResultValue(IMethodReturn methodReturn)
		{
			LambdaExpression parameterTypeMap;

			if (methodReturn.ReturnValue != null && _propertyMappingDictionary.TryGetValue(methodReturn.ReturnValue.GetType(), out parameterTypeMap))
			{
				return parameterTypeMap.Compile().DynamicInvoke(methodReturn.ReturnValue);
			}

			return methodReturn.ReturnValue;
		}
	}
}