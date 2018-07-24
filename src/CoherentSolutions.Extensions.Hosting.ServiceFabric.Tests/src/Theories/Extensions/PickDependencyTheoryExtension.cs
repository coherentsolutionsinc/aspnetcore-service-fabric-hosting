using System;
using System.Collections.Generic;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions
{
    public class PickDependencyTheoryExtension : IPickDependencyTheoryExtension
    {
        private readonly LinkedList<Action<IServiceProvider>> delegates;

        public IEnumerable<Action<IServiceProvider>> PickActions => this.delegates;

        public PickDependencyTheoryExtension()
        {
            this.delegates = new LinkedList<Action<IServiceProvider>>();
        }

        public PickDependencyTheoryExtension Setup(
            Type type,
            Action<object> action)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            this.delegates.AddLast(
                provider =>
                {
                    var value = provider.GetService(type);
                    action(value);
                });

            return this;
        }
    }
}