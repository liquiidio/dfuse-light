FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build

# Install required packages
RUN apt-get update \
        && apt-get install -y \ 
            libcurl3-gnutls \
            libpq5 \
            git  \
            binaryen                    \
            build-essential             \
            ccache                      \
            cmake                       \
            curl                        \
            git                         \
            jq                          \
            libboost-all-dev            \
            libcurl4-openssl-dev        \
            libgmp-dev                  \
            libssl-dev                  \
            libtinfo5                   \
            libusb-1.0-0-dev            \
            libzstd-dev                 \
            llvm-11-dev                 \
            ninja-build                 \
            npm                         \
            pkg-config                  \
            time

RUN git clone https://github.com/eosnetworkfoundation/mandel.git --branch dfuse \
        && cd mandel                \
        && git submodule update     \
            --init                  \
            --recursive             \
        && mkdir build              \
        && cd build                 \
        && cmake -DCMAKE_BUILD_TYPE=Release             \
            -DDISABLE_WASM_SPEC_TESTS=yes               \
            -DCMAKE_INSTALL_PREFIX=/root/mandel/3.0 ..  \
        && make -j $(nproc)                             \
        && make install

RUN ln -s /root/mandel/3.0/bin/nodeos /bin/nodeos
RUN ln -s /root/mandel/3.0/bin/cleos /bin/cleos
RUN ln -s /root/mandel/3.0/bin/keosd /bin/keosd
RUN ln -s /root/mandel/3.0/bin/eosio-launcher /bin/eosio-launcher
RUN ln -s /root/mandel/3.0/bin/trace_api_util /bin/trace_api_util
#RUN ln -s /usr/share/dotnet/dotnet /usr/bin/dotnet

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