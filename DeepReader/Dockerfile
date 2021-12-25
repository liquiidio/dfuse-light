FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build

# Install required packages
RUN apt-get update \
        && apt-get install -y \ 
            libusb-1.0-0 \
            libcurl3-gnutls \
            libpq5 \
            git \
            build-essential

#RUN git clone https://github.com/dfuse-io/eos.git && cd eos && git checkout tags/v2.0.5wax01-dm-12.0 && git submodule update --init --recursive && ./scripts/eosio_build.sh -y
#RUN git clone https://github.com/EOSIO/eos.git && cd eos && git checkout tags/v2.2.0-rc1 && git submodule update --init --recursive && ./scripts/eosio_build.sh -y
RUN git clone https://github.com/Duncan-Ultra/eos.git && cd eos && git checkout update_fc_dmlog_fix_2.2.x && git submodule update --init --recursive && ./scripts/eosio_build.sh -y /usr/bin && ./scripts/eosio_install.sh
RUN ln -s /root/eosio/2.2/bin/nodeos /bin/nodeos
RUN ln -s /root/eosio/2.2/bin/cleos /bin/cleos
RUN ln -s /root/eosio/2.2/bin/keosd /bin/keosd
RUN ln -s /root/eosio/2.2/bin/eosio-launcher /bin/eosio-launcher
RUN ln -s /root/eosio/2.2/bin/trace_api_util /bin/trace_api_util
#RUN ln -s /usr/share/dotnet/dotnet /usr/bin/dotnet todo nodeos

#RUN wget http://security.ubuntu.com/ubuntu/pool/main/i/icu/libicu60_60.2-3ubuntu3.2_amd64.deb
#RUN apt install -y ./libicu60_60.2-3ubuntu3.2_amd64.deb

#RUN wget https://github.com/eosio/eos/releases/download/v2.1.0/eosio_2.1.0-1-ubuntu-18.04_amd64.deb
#RUN wget https://github.com/EOSIO/eos/releases/download/v2.2.0-rc1/eosio_2.2.0-rc1-ubuntu-18.04_amd64.deb
#RUN wget https://github.com/EOSIO/eos/releases/download/v2.0.13/eosio_2.0.13-1-ubuntu-18.04_amd64.deb
#RUN apt install -y ./eosio_2.1.0-1-ubuntu-18.04_amd64.deb
#RUN apt install -y ./eosio_2.2.0-rc1-ubuntu-18.04_amd64.deb
#RUN apt install -y ./eosio_2.0.13-1-ubuntu-18.04_amd64.deb

WORKDIR /app

WORKDIR /src
COPY ["DeepReader/DeepReader.csproj", "DeepReader/"]
RUN dotnet restore "DeepReader/DeepReader.csproj"
COPY . .
WORKDIR "/src/DeepReader"
RUN dotnet build "DeepReader.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DeepReader.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DeepReader.dll"]

EXPOSE 9876