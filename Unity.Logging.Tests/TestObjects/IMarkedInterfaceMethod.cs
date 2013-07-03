namespace Unity.LoggingExtension.Tests.TestObjects
{
    public interface IMarkedInterfaceMethod
    {
        [Log]
        void Method();
    }

    public class MarkedInterfaceMethodInstance : IMarkedInterfaceMethod
    {
        public void Method()
        {
            
        }
    }
}
