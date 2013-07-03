using System;

namespace Unity.LoggingExtension
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Method | AttributeTargets.Property, Inherited = true)]
    public class LogAttribute : Attribute
    {

    }
}