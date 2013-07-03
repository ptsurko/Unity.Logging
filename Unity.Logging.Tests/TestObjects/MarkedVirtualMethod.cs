namespace Unity.LoggingExtension.Tests.TestObjects
{
    public class MarkedVirtualMethod
    {
        [Log]
        public virtual void VirtualMethod()
        {
            
        }

        [Log]
        public void NotVirtualMethod()
        {
            
        }
    }
}
