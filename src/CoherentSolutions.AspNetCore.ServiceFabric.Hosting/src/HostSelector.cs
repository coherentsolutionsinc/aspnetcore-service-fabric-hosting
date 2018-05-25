using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting
{
    public class HostSelector : IHostSelector
    {
        private class Match
        {
            public IHostDescriptor Descriptor { get; }

            public long Relativity { get; }

            public Match(
                IHostDescriptor descriptor,
                long relativity)
            {
                this.Descriptor = descriptor;
                this.Relativity = relativity;
            }
        }

        private class RelativityComparer : IComparer<Match>
        {
            public static readonly IComparer<Match> Default;

            static RelativityComparer()
            {
                Default = new RelativityComparer();
            }

            public int Compare(
                Match x,
                Match y)
            {
                return -1 * x.Relativity.CompareTo(y.Relativity);
            }
        }

        public IHostDescriptor Select(
            IEnumerable<IHostKeywordsProvider> keywordsProviders,
            IEnumerable<IHostDescriptor> hostDescriptors)
        {
            if (keywordsProviders == null)
            {
                throw new ArgumentNullException(nameof(keywordsProviders));
            }

            if (hostDescriptors == null)
            {
                throw new ArgumentNullException(nameof(hostDescriptors));
            }

            var matches = new List<Match>();

            var evaluated = new HashSet<string>(
                keywordsProviders.SelectMany(provider => provider.GetKeywords()),
                StringComparer.OrdinalIgnoreCase);

            foreach (var hostDescriptor in hostDescriptors)
            {
                var matched = true;

                var relativity = 0;
                foreach (var keyword in hostDescriptor.Keywords.GetKeywords())
                {
                    if (!evaluated.Contains(keyword))
                    {
                        matched = false;
                        break;
                    }

                    relativity++;
                }

                if (matched)
                {
                    matches.Add(new Match(hostDescriptor, relativity));
                }
            }

            if (matches.Count == 0)
            {
                var builder = new StringBuilder()
                   .AppendLine($"Cannot find {nameof(IHost)} because non of registered hosts match environment.")
                   .AppendLine($"The environment has the following keywords defined: {string.Join(",", evaluated)}");

                throw new InvalidOperationException(builder.ToString());
            }

            matches.Sort(RelativityComparer.Default);

            if (matches.Count > 1 && matches[0].Relativity == matches[1].Relativity)
            {
                var builder = new StringBuilder()
                   .AppendLine($"Cannot select {nameof(IHost)} because multiple instances matches environment.")
                   .AppendLine($"The environment has the following keywords defined: {string.Join(",", evaluated)}")
                   .AppendLine($"Hosts:");

                var relativity = matches[0].Relativity;
                foreach (var match in matches.Where(match => match.Relativity == relativity))
                {
                    builder.AppendLine($"  Host: {match.Descriptor.GetDescription() ?? "<unknown>"}");
                    builder.AppendLine($"  - keywords: {string.Join(",", match.Descriptor.Keywords.GetKeywords())}");
                }

                throw new InvalidOperationException(builder.ToString());
            }

            return matches[0].Descriptor;
        }
    }
}