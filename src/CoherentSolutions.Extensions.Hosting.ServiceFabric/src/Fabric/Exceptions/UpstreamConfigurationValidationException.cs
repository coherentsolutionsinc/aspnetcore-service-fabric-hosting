using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Exceptions
{
    public class UpstreamConfigurationValidationException : Exception
    {
        public UpstreamConfigurationValidationException(
            Type source,
            IEnumerable<ValidationResult> validationResults)
            : this(source, validationResults, null)
        {
        }

        public UpstreamConfigurationValidationException(
            Type source,
            IEnumerable<ValidationResult> validationResults,
            Exception innerException)
            : base(CreateMessage(source, validationResults), innerException)
        {

        }

        protected UpstreamConfigurationValidationException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }

        private static string CreateMessage(
            Type source,
            IEnumerable<ValidationResult> validationResults)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (validationResults is null)
            {
                throw new ArgumentNullException(nameof(validationResults));
            }

            var sb = new StringBuilder($"The instance of {source.Name} wasn't configured as required:");
            foreach (var validationResult in validationResults)
            {
                sb.AppendLine().Append($"- {string.Join(", ", validationResult.MemberNames)} : {validationResult.ErrorMessage}");
            }

            return sb.ToString();
        }
    }
}