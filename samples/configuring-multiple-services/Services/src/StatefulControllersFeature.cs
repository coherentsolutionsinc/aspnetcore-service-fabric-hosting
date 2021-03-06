﻿using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Services
{
    public class StatefulControllersFeature : IApplicationFeatureProvider<ControllerFeature>
    {
        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            for (var i = feature.Controllers.Count - 1; i >= 0; --i)
            {
                var type = feature.Controllers[i];
                if (!type.Name.StartsWith("Stateful")) feature.Controllers.RemoveAt(i);
            }
        }
    }
}