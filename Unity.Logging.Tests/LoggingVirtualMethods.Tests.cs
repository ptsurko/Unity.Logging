using Microsoft.Practices.Unity;
using NSubstitute;
using NUnit.Framework;
using Unity.LoggingExtension.Tests.TestObjects;

namespace Unity.LoggingExtension.Tests
{
    [TestFixture]
    public class LoggingVirtualMethods
    {
        private IUnityContainer _unityContainer;
        private ILogger _logger;

        [SetUp]
        public void Setup()
        {
            _unityContainer = new UnityContainer();
            _unityContainer.AddNewExtension<LoggingExtension>();

            _logger = Substitute.For<ILogger>();

            _unityContainer.RegisterType<MarkedVirtualMethod>();
            _unityContainer.RegisterType<MarkedClassWithVirtualMethod>();
            _unityContainer.RegisterInstance(_logger);
        }

        [Test]
        public void Should_log_for_marked_virtual_method()
        {
            _unityContainer.Resolve<MarkedVirtualMethod>().VirtualMethod();

            _logger.Received(2).Log(Arg.Any<string>());
        }

        [Test]
        public void Should_not_log_for_any_non_virtual_marked_methods()
        {
            _unityContainer.Resolve<MarkedVirtualMethod>().NotVirtualMethod();

            _logger.DidNotReceive().Log(Arg.Any<string>());
        }

        [Test]
        public void Should_log_for_virtual_method_in_marked_class()
        {
            _unityContainer.Resolve<MarkedClassWithVirtualMethod>().VirtualMethod();

            _logger.Received(2).Log(Arg.Any<string>());
        }

        [Test]
        public void Should_not_log_for_non_virtual_methods_in_marked_class()
        {
            _unityContainer.Resolve<MarkedClassWithVirtualMethod>().NotVirtualMethod();

            _logger.DidNotReceive().Log(Arg.Any<string>());
        }
    }
}
