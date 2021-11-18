FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["CdkTest.Web/CdkTest.Web.csproj", "CdkTest.Web/"]
RUN dotnet restore "CdkTest.Web/CdkTest.Web.csproj"
COPY . .
WORKDIR "/src/CdkTest.Web"
RUN dotnet build "CdkTest.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CdkTest.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CdkTest.Web.dll"]
