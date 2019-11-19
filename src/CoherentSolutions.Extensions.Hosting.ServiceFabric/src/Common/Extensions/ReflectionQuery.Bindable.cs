using System.Reflection;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Common.Extensions
{
    public static partial class ReflectionQuery
    {
        public abstract class Bindable : IBindable
        {
            protected BindingFlags BindingFlags { get; private set; }

            void IBindable.Public()
            {
                this.BindingFlags &= ~BindingFlags.NonPublic;
                this.BindingFlags |= BindingFlags.Public;
            }

            void IBindable.NonPublic()
            {
                this.BindingFlags &= ~BindingFlags.Public;
                this.BindingFlags |= BindingFlags.NonPublic;
            }
            void IBindable.Static()
            {
                this.BindingFlags &= ~BindingFlags.Instance;
                this.BindingFlags |= BindingFlags.Static;
            }

            void IBindable.Instance()
            {
                this.BindingFlags &= ~BindingFlags.Static;
                this.BindingFlags |= BindingFlags.Instance;
            }
        }
    }
}