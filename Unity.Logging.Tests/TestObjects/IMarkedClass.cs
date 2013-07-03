namespace Unity.LoggingExtension.Tests.TestObjects
{
    public interface IMarkedClass
    {
        void Method();
    }

    [Log]
    public class MarkedClassInstance : IMarkedClass
    {
        public void Method()
        {
            
        }
    }
}
