
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["LiveAuction.API/LiveAuction.API.csproj", "LiveAuction.API/"]
COPY ["LiveAuction.Application/LiveAuction.Application.csproj", "LiveAuction.Application/"]
COPY ["LiveAuction.Core/LiveAuction.Core.csproj", "LiveAuction.Core/"]
COPY ["LiveAuction.Infrastructure/LiveAuction.Infrastructure.csproj", "LiveAuction.Infrastructure/"]

RUN dotnet restore "LiveAuction.API/LiveAuction.API.csproj"

COPY . .
WORKDIR "/src/LiveAuction.API"
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0-bookworm-slim AS final

RUN apt-get update && apt-get install -y libgssapi-krb5-2

WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "LiveAuction.API.dll"]