namespace Unity.LoggingExtension.Tests.TestObjects
{
    [Log]
    public interface IMarkedInterface
    {
        void Method();
    }

    public class MarkedInterfaceInstance : IMarkedInterface
    {
        public void Method()
        {
            
        }
    }
}
