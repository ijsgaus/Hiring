FROM microsoft/dotnet:2.1.4-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:sdk AS build
WORKDIR /src
COPY ["src/Hiring.Api/Hiring.Api.csproj", "src/Hiring.Api/"]
RUN dotnet restore "src/Hiring.Api/Hiring.Api.csproj"
COPY . .
WORKDIR "/src/src/Hiring.Api"
RUN dotnet build "Hiring.Api.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Hiring.Api.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Hiring.Api.dll"]