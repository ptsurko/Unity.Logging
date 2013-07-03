using Microsoft.Practices.Unity;
using NSubstitute;
using NUnit.Framework;
using Unity.LoggingExtension.Tests.TestObjects;

namespace Unity.LoggingExtension.Tests
{
    [TestFixture]
    public class LoggingInterfaceTests
    {
        private IUnityContainer _unityContainer;
        private ILogger _logger;

        [SetUp]
        public void Setup()
        {
            _unityContainer = new UnityContainer();
            _unityContainer.AddNewExtension<LoggingExtension>();

            _logger = Substitute.For<ILogger>();

            _unityContainer.RegisterType<IMarkedClass, MarkedClassInstance>();
            _unityContainer.RegisterType<IMarkedClassMethod, MarkedClassMethodInstance>();
            _unityContainer.RegisterType<IMarkedInterface, MarkedInterfaceInstance>();
            _unityContainer.RegisterType<IMarkedInterfaceMethod, MarkedInterfaceMethodInstance>();
            _unityContainer.RegisterInstance(_logger);
        }

        [Test]
        public void Should_log_for_marked_interface()
        {
            _unityContainer.Resolve<IMarkedInterface>().Method();

            _logger.Received(2).Log(Arg.Any<string>());
        }

        [Test]
        public void Should_log_for_marked_interface_method()
        {
            _unityContainer.Resolve<IMarkedInterfaceMethod>().Method();

            _logger.Received(2).Log(Arg.Any<string>());
        }

        [Test]
        public void Should_log_for_marked_class()
        {
            _unityContainer.Resolve<IMarkedClass>().Method();

            _logger.Received(2).Log(Arg.Any<string>());
        }

        [Test]
        public void Should_log_for_marked_class_method()
        {
            _unityContainer.Resolve<IMarkedClassMethod>().Method();

            _logger.Received(2).Log(Arg.Any<string>());
        }
    }
}
