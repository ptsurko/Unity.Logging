using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity.InterceptionExtension;
using NUnit.Framework;

namespace Unity.LoggingExtension.Tests
{
    [TestFixture]
    public class LoggingInterceptionBehaviorTests
    {
        [SetUp]
        public void Setup()
        {
            
        }

        class FakeLoggingInterceptionBehavior : LoggingInterceptionBehavior
        {
            public FakeLoggingInterceptionBehavior(ILogMethodInvocation logMethodInvocation, PropertyMappingDictionary propertyMappingDictionary)
                : base(logMethodInvocation, propertyMappingDictionary)
            {
            }

            protected override bool IsMethodRequireLogging(IMethodInvocation input)
            {
                return true;
            }
        }
    }
}
