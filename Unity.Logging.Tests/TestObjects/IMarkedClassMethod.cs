namespace Unity.LoggingExtension.Tests.TestObjects
{
    public interface IMarkedClassMethod
    {
        void Method();
    }

    public class MarkedClassMethodInstance : IMarkedClassMethod
    {
        [Log]
        public void Method()
        {
            
        }
    }
}
