# ─────────────── Stage: build ────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

ARG CONFIG=Release
ARG DOTNET_FLAGS=""

WORKDIR /src
COPY *.sln ./
COPY WebApp/         WebApp/
COPY Application/    Application/
COPY Core/           Core/
COPY Infrastructure/ Infrastructure/
COPY Tests/ Tests/
COPY Utilities/      Utilities/

RUN dotnet restore
RUN dotnet publish WebApp/WebApp.csproj \
        -c ${CONFIG} ${DOTNET_FLAGS} \
        -o /app/publish

# ─────────────── Stage: runtime-release ──────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime-release
WORKDIR /app
COPY --from=build /app/publish/ .
ENTRYPOINT ["dotnet","WebApp.dll"]

# ─────────────── Stage: runtime-debug ────────────────────────
FROM runtime-release AS runtime-debug

#
# 1. docker-CLI, vsdbg
#
RUN apt-get update && \
    apt-get install -y --no-install-recommends docker.io curl && \
    curl -sSL https://aka.ms/getvsdbgsh \
        | bash /dev/stdin -v latest -l /vsdbg && \
    apt-get clean && rm -rf /var/lib/apt/lists/*

#
# 2. shell-scripts
#
COPY Utilities/ /app/Utilities/
RUN find /app/Utilities -type f -name "*.sh" -exec sed -i 's/\r$//' {} +
RUN chmod +x /app/Utilities/*.sh

ENV ASPNETCORE_ENVIRONMENT=Development
EXPOSE 9229
