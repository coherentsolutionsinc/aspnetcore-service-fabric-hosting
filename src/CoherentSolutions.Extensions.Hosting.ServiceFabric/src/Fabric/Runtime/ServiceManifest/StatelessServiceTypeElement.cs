﻿namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ServiceManifest
{
    public class StatelessServiceTypeElement : ServiceTypeElement
    {
        public override ServiceTypeElementKind Kind
        {
            get
            {
                return ServiceTypeElementKind.Stateless;
            }
        }
    }
}