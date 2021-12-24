FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build

#RUN ln -s /usr/share/dotnet/dotnet /usr/bin/dotnet todo nodeos

# Install required packages
RUN apt-get update \
        && apt-get install -y \ 
            libusb-1.0-0 \
            libcurl3-gnutls \
            libpq5

RUN wget https://github.com/eosio/eos/releases/download/v2.1.0/eosio_2.1.0-1-ubuntu-18.04_amd64.deb
RUN apt install -y ./eosio_2.1.0-1-ubuntu-18.04_amd64.deb

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


