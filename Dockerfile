FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ./ ./
RUN dotnet workload restore
RUN apt-get update && \
    apt-get install -y --no-install-recommends \
        python3 \
        python-is-python3 \
        curl \
        ca-certificates \
    && rm -rf /var/lib/apt/lists/*

RUN curl -fsSL https://deb.nodesource.com/setup_20.x | bash - && \
    apt-get install -y nodejs

RUN dotnet restore PorphyStruct.Web/PorphyStruct.Web.csproj
RUN dotnet build PorphyStruct.Web/PorphyStruct.Web.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish PorphyStruct.Web/PorphyStruct.Web.csproj -c Release -o /app/publish

FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html
COPY --from=publish /app/publish/wwwroot .
COPY nginx.conf /etc/nginx/nginx.conf
