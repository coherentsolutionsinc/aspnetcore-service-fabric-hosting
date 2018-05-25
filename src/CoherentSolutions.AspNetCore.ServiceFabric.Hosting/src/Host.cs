using System;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting
{
    public class Host : IHost
    {
        private readonly IHostRunner runner;

        public Host(
            IHostRunner runner)
        {
            this.runner = runner
             ?? throw new ArgumentNullException(nameof(runner));
        }

        public void Run()
        {
            this.runner.Run();
        }
    }
}