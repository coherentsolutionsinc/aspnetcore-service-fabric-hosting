using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Exceptions;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools
{
    public abstract class ValidateableConfigurableObject<TParameters, TConfigurator> : ConfigurableObject<TConfigurator>
    {
        protected void ValidateUpstreamConfiguration(
            TParameters parameters)
        {
            if (parameters is null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            var results = new List<ValidationResult>();
            if (!Validator.TryValidateObject(parameters, new ValidationContext(parameters), results, false))
            {
                throw new UpstreamConfigurationValidationException(this.GetType(), results);
            }
        }
    }
}