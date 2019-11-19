using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime
{
    public class GhostServiceTrace
    {
        public void Write(
            string msg,
            Exception exception = null)
        {
            Console.WriteLine(msg);
            if (!object.ReferenceEquals(exception, null))
            {
                Console.WriteLine(exception.ToString());
            }
        }
    }
}