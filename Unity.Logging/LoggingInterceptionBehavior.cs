using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace Unity.LoggingExtension
{
    internal abstract class LoggingInterceptionBehavior : IInterceptionBehavior
    {
        private readonly ILogMethodInvocation _logMethodInvocation;
        private readonly PropertyMappingDictionary _propertyMappingDictionary;

        protected LoggingInterceptionBehavior(ILogMethodInvocation logMethodInvocation, PropertyMappingDictionary propertyMappingDictionary)
        {
            _logMethodInvocation = logMethodInvocation;
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

                _logMethodInvocation.LogMethodEntering(input.MethodBase, parameters);
            }

            var methodReturn = methodInfo.Invoke(input, getNext);

            if (isLoggingRequired)
            {
                var result = GetResultValue(methodReturn);

                _logMethodInvocation.LogMethodLeaving(input.MethodBase, result);
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
                result.Add(GetFormatedParameterValue(parameters[i], input.Arguments[i]));
            }
            return result;
        }

        private KeyValuePair<ParameterInfo, object> GetFormatedParameterValue(ParameterInfo parameter, object parameterValue)
        {
            LambdaExpression parameterTypeMap;
            if (parameter.ParameterType.IsArray || typeof(IEnumerable).IsAssignableFrom(parameter.ParameterType))
            {
                var result = new List<object>();
                //TODO: consider nested lists
                foreach (var item in (IEnumerable)parameterValue)
                {
                    result.Add(_propertyMappingDictionary.TryGetValue(item.GetType(), out parameterTypeMap)
                        ? parameterTypeMap.Compile().DynamicInvoke(item)
                        : item);
                }

                return new KeyValuePair<ParameterInfo, object>(parameter, result);
            }
            else if (_propertyMappingDictionary.TryGetValue(parameter.ParameterType, out parameterTypeMap))
            {
                return new KeyValuePair<ParameterInfo, object>(parameter, parameterTypeMap.Compile().DynamicInvoke(parameterValue));
            }
            else
            {
                return new KeyValuePair<ParameterInfo, object>(parameter, parameterValue);
            }
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