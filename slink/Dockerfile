FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["slink/slink.csproj", "slink/"]
RUN dotnet restore "slink/slink.csproj"
COPY . .
WORKDIR "/src/slink"
RUN dotnet build "slink.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "slink.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "slink.dll"]