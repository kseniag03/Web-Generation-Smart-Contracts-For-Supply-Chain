# ─────────────── Stage: build ────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

ARG CONFIGURATION=Release
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
        -c ${CONFIGURATION} ${DOTNET_FLAGS} \
        -o /app/publish

# ─────────────── Stage: runtime-release ──────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime-release
WORKDIR /app

COPY --from=build /app/publish/ .

COPY ./https/dev-certificate.pfx /https/dev-certificate.pfx
COPY ./https/root.crt /usr/local/share/ca-certificates/custom.crt

RUN apt-get update && \
    apt-get install -y --no-install-recommends ca-certificates && \
    update-ca-certificates && \
    rm -rf /var/lib/apt/lists/*

RUN apt-get update && \
    apt-get install -y --no-install-recommends \
        ca-certificates \
        curl \
        gnupg \
        lsb-release && \
    install -m 0755 -d /etc/apt/keyrings && \
    curl -fsSL https://download.docker.com/linux/debian/gpg | \
        gpg --dearmor -o /etc/apt/keyrings/docker.gpg && \
    echo "deb [arch=$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/docker.gpg] https://download.docker.com/linux/debian \
        $(lsb_release -cs) stable" > /etc/apt/sources.list.d/docker.list && \
    apt-get update && \
    apt-get install -y --no-install-recommends docker-ce-cli && \
    curl -sSL https://aka.ms/getvsdbgsh \
        | bash /dev/stdin -v latest -l /vsdbg && \
    apt-get clean && rm -rf /var/lib/apt/lists/*

COPY Utilities/ /app/Utilities/
RUN find /app/Utilities -type f -name "*.sh" -exec sed -i 's/\r$//' {} +
RUN chmod +x /app/Utilities/*.sh

EXPOSE 8080
EXPOSE 443

ENTRYPOINT ["dotnet", "WebApp.dll"]

# ─────────────── Stage: runtime-debug ────────────────────────
FROM runtime-release AS runtime-debug

ENV ASPNETCORE_ENVIRONMENT=Development
EXPOSE 9229
EXPOSE 8080
EXPOSE 443

ENTRYPOINT ["dotnet", "WebApp.dll"]
