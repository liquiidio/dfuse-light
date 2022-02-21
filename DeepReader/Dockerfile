FROM cmadh/mandel-dfuse-base AS build

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