FROM mcr.microsoft.com/vscode/devcontainers/dotnetcore:0-3.1

ENV APT_KEY_DONT_WARN_ON_DANGEROUS_USAGE=DontWarn

RUN apt-get update && \
    apt-get install dirmngr

RUN apt-key adv --keyserver apt-mo.trafficmanager.net --recv-keys 417A0893 && \
    apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 417A0893

RUN apt-get update && \
    export DEBIAN_FRONTEND=noninteractive && \
    apt-get install --assume-yes apt-transport-https && \
    echo "deb [arch=amd64] http://apt-mo.trafficmanager.net/repos/servicefabric/ xenial main" > /etc/apt/sources.list.d/servicefabric.list && \
    apt-get update && \
    apt-get download servicefabric=${SERVICE_FABRIC_VERSION} && \
    dpkg -x servicefabric_${SERVICE_FABRIC_VERSION}_amd64.deb .

# deleting unnecessary files to reduce image size
RUN find "/opt/microsoft/servicefabric/bin/Fabric/Fabric.Code" -name "*.exe" -type f -delete && \
    find "/opt/microsoft/servicefabric/bin/Fabric/Fabric.Code" -name "*.pdb" -type f -delete && \
    find "/opt/microsoft/servicefabric/bin/Fabric/Fabric.Code" -name "SFBlockStoreService" -type f -delete && \
    find "/opt/microsoft/servicefabric/bin/Fabric/Fabric.Code" -name "__FabricSystem_App*" -type d -exec rm -rf {} +;

RUN mkdir /etc/servicefabric && \
    echo -n /home/sfuser/sfdevcluster/data > /etc/servicefabric/FabricDataRoot && \
    echo -n /home/sfuser/sfdevcluster/data/log > /etc/servicefabric/FabricLogRoot && \
    echo -n /opt/microsoft/servicefabric/bin > /etc/servicefabric/FabricBinRoot && \
    echo -n /opt/microsoft/servicefabric/bin/Fabric/Fabric.Code > /etc/servicefabric/FabricCodePath
