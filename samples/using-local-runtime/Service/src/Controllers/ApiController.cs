using System;
using System.Collections;
using System.Fabric;
using System.IO;
using System.Text;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

using Remotion.Linq.Clauses.ResultOperators;

namespace Service.Controllers
{
    [Route("api")]
    public class ApiController : Controller
    {
        [HttpGet]
        [Route("print")]
        public string Print(
            [FromServices] ServiceContext context,
            [FromServices] IConfiguration configuration)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var sb = new StringBuilder().AppendLine("Output:");

            sb.AppendLine("Service Context:")
               .AppendFormat("- ServiceName: {0}", context.ServiceName).AppendLine()
               .AppendFormat("- ServiceTypeName: {0}", context.ServiceTypeName).AppendLine()
               .AppendFormat("- PartitionId: {0}", context.PartitionId).AppendLine()
               .AppendFormat("- ListenAddress: {0}", context.ListenAddress).AppendLine()
               .AppendFormat("- PublishAddress: {0}", context.PublishAddress).AppendLine()
               .AppendFormat("- ReplicaOrInstanceId: {0}", context.ReplicaOrInstanceId).AppendLine()
               .AppendFormat("- TraceId: {0}", context.TraceId).AppendLine()
               .AppendLine();

            sb.AppendLine("Node Context:")
               .AppendFormat("- NodeName: {0}", context.NodeContext.NodeName).AppendLine()
               .AppendFormat("- IPAddressOrFQDN: {0}", context.NodeContext.IPAddressOrFQDN).AppendLine()
               .AppendFormat("- NodeId: {0}", context.NodeContext.NodeId).AppendLine()
               .AppendFormat("- NodeInstanceId: {0}", context.NodeContext.NodeInstanceId).AppendLine()
               .AppendFormat("- NodeType: {0}", context.NodeContext.NodeType).AppendLine()
               .AppendLine();

            sb.AppendLine("Code Activation Context:")
               .AppendFormat("- ApplicationName: {0}", context.CodePackageActivationContext.ApplicationName).AppendLine()
               .AppendFormat("- ApplicationTypeName: {0}", context.CodePackageActivationContext.ApplicationTypeName).AppendLine()
               .AppendFormat("- CodePackageName: {0}", context.CodePackageActivationContext.CodePackageName).AppendLine()
               .AppendFormat("- CodePackageVersion: {0}", context.CodePackageActivationContext.CodePackageVersion).AppendLine()
               .AppendFormat("- ContextId: {0}", context.CodePackageActivationContext.ContextId).AppendLine()
               .AppendFormat("- LogDirectory: {0}", context.CodePackageActivationContext.LogDirectory).AppendLine()
               .AppendFormat("- TempDirectory: {0}", context.CodePackageActivationContext.TempDirectory).AppendLine()
               .AppendFormat("- WorkDirectory: {0}", context.CodePackageActivationContext.WorkDirectory).AppendLine()
               .AppendLine();

            sb.AppendLine("Code Packages:");

            var codePackageNames = context.CodePackageActivationContext.GetCodePackageNames();
            if (codePackageNames.Count == 0)
            {
                sb.AppendLine("- No code packages");
            }
            else
            {
                foreach (var name in codePackageNames)
                {
                    var obj = context.CodePackageActivationContext.GetCodePackageObject(name);
                    sb.AppendFormat("- Package: {0}", obj.Description.Name).AppendLine()
                       .AppendFormat("  * Path                   : {0}", obj.Path).AppendLine()
                       .AppendFormat("  * ServiceManifestName    : {0}", obj.Description.ServiceManifestName).AppendLine()
                       .AppendFormat("  * ServiceManifestVersion : {0}", obj.Description.ServiceManifestVersion).AppendLine();
                }
            }

            sb.AppendLine();

            sb.AppendLine("Data Packages:");

            var dataPackageNames = context.CodePackageActivationContext.GetDataPackageNames();
            if (dataPackageNames.Count == 0)
            {
                sb.AppendLine("- No data packages");
            }
            else
            {
                foreach (var name in dataPackageNames)
                {
                    var obj = context.CodePackageActivationContext.GetDataPackageObject(name);
                    sb.AppendFormat("- Package: {0}", obj.Description.Name).AppendLine()
                       .AppendFormat("  * Path                   : {0}", obj.Path).AppendLine()
                       .AppendFormat("  * ServiceManifestName    : {0}", obj.Description.ServiceManifestName).AppendLine()
                       .AppendFormat("  * ServiceManifestVersion : {0}", obj.Description.ServiceManifestVersion).AppendLine()
                       .AppendLine("  * Content:");

                    foreach (var f in Directory.EnumerateFileSystemEntries(obj.Path))
                    {
                        sb.AppendFormat("    + {0}", f).AppendLine();
                    }
                }
            }

            sb.AppendLine();

            sb.AppendLine("Configuration Packages:");

            var configurationPackageNames = context.CodePackageActivationContext.GetConfigurationPackageNames();
            if (configurationPackageNames.Count == 0)
            {
                sb.AppendLine("- No configuration packages");
            }
            else
            {
                foreach (var name in configurationPackageNames)
                {
                    var obj = context.CodePackageActivationContext.GetConfigurationPackageObject(name);
                    sb.AppendFormat("- Package: {0}", obj.Description.Name).AppendLine()
                       .AppendFormat("  * Path                   : {0}", obj.Path).AppendLine()
                       .AppendFormat("  * ServiceManifestName    : {0}", obj.Description.ServiceManifestName).AppendLine()
                       .AppendFormat("  * ServiceManifestVersion : {0}", obj.Description.ServiceManifestVersion).AppendLine()
                       .AppendLine("  * Settings:");

                    if (obj.Settings is null)
                    {
                        sb.AppendLine("      + No Settings.xml");
                    }
                    else
                    {
                        foreach (var section in obj.Settings.Sections)
                        {
                            foreach (var parameter in section.Parameters)
                            {
                                sb.AppendFormat("      + {0}/{1} = {2}", section.Name, parameter.Name, parameter.Value).AppendLine();
                            }
                        }
                    }
                }
            }

            sb.AppendLine();

            sb.AppendLine("Endpoints:");

            var endpoints = context.CodePackageActivationContext.GetEndpoints();
            if (endpoints.Count == 0)
            {
                sb.AppendLine("- No endpoints");
            }
            else
            {
                foreach (var endpoint in endpoints)
                {
                    sb.AppendFormat("- {0}", endpoint.Name).AppendLine();
                }
            }

            sb.AppendLine();

            sb.AppendLine("Service Types:");

            var serviceTypes = context.CodePackageActivationContext.GetServiceTypes();
            if (serviceTypes.Count == 0)
            {
                sb.AppendLine("- No service types");
            }
            else
            {
                foreach (var serviceType in serviceTypes)
                {
                    sb.AppendFormat("- {0}/{1}", serviceType.ServiceTypeName, serviceType.ServiceTypeKind.ToString()).AppendLine();
                }
            }

            sb.AppendLine();

            sb.AppendLine("Environment Variables from Environment:");
            foreach (DictionaryEntry ev in Environment.GetEnvironmentVariables())
            {
                if (ev.Key is string s && s.StartsWith("Fabric_"))
                    sb.AppendFormat("- {0} = {1}", ev.Key, ev.Value).AppendLine();
            }

            sb.AppendLine();

            sb.AppendLine("Environment Variables from IConfiguration:");
            foreach (var ev in configuration.AsEnumerable())
            {
                if (ev.Key is string s && s.StartsWith("Fabric_"))
                    sb.AppendFormat("- {0} = {1}", ev.Key, ev.Value).AppendLine();
            }
            
            return sb.ToString();
        }
    }
}