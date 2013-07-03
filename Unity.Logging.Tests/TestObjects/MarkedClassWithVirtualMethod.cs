namespace Unity.LoggingExtension.Tests.TestObjects
{
    [Log]
    public class MarkedClassWithVirtualMethod
    {
        public virtual void VirtualMethod()
        {

        }

        public void NotVirtualMethod()
        {
            
        }
    }
}
