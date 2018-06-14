using System;
using System.ComponentModel;
using System.Reflection;

using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Common;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting
{
    public static partial class Extensions
    {
        public static string GetDescription(
            this object @this)
        {
            if (@this == null)
            {
                throw new ArgumentNullException(nameof(@this));
            }

            var type = @this.GetType();
            var attribute = type.GetCustomAttribute<DescriptionAttribute>(false);
            return attribute?.Description;
        }

        public static Action<T> Chain<T>(
            this Action<T> left,
            Action<T> right)
        {
            if (left == null)
            {
                return right;
            }

            if (right == null)
            {
                return left;
            }

            return v =>
            {
                left(v);
                right(v);
            };
        }

        public static IWebHostBuilder ConfigureOnRun(
            this IWebHostBuilder @this,
            Action<IServiceProvider> onRunAction)
        {
            if (@this == null)
            {
                throw new ArgumentNullException(nameof(@this));
            }

            if (onRunAction == null)
            {
                throw new ArgumentNullException(nameof(onRunAction));
            }

            if (!(@this is ExtensibleWebHostBuilder))
            {
                throw new InvalidOperationException(
                    $"Unable to configure on-run action execution because this "
                  + $"method is invoked outside of {nameof(IHybridHostBuilder)} boundaries.");
            }

            @this.ConfigureServices(
                services =>
                {
                    services.AddSingleton<IExtensibleWebHostOnRunAction>(new ExtensibleWebHostOnRunAction(onRunAction));
                });

            return @this;
        }
    }
}