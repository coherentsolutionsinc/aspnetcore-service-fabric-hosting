using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions
{
    public class UseDelegateTheoryExtension : IUseDelegateTheoryExtension
    {
        public Delegate Delegate { get; private set; }

        public UseDelegateTheoryExtension()
        {
            this.Delegate = new Action(
                () =>
                {
                });
        }

        public UseDelegateTheoryExtension Setup(
            Delegate @delegate)
        {
            this.Delegate = @delegate;

            return this;
        }
    }
}