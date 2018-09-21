FROM microsoft/dotnet:2.1.4-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:sdk AS build
WORKDIR /src
COPY ["src/ITHiring.Api/ITHiring.Api.csproj", "src/Hiring.Api/"]
RUN dotnet restore "src/ITHiring.Api/ITHiring.Api.csproj"
COPY . .
WORKDIR "/src/src/ITHiring.Api"
RUN dotnet build "ITHiring.Api.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "ITHiring.Api.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ITHiring.Api.dll"]