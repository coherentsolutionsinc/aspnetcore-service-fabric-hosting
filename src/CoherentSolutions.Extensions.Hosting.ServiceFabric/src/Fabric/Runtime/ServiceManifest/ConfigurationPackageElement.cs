﻿using System.Xml.Serialization;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ServiceManifest
{
    public class ConfigurationPackageElement : PackageElement
    {
        [XmlIgnore]
        public ConfigurationSettingsElement Settings
        {
            get; 
            set;
        }
    }
}