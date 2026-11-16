FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY . ./

RUN dotnet workload restore
RUN dotnet restore PorphyStruct.Web/PorphyStruct.Web.csproj
RUN dotnet publish PorphyStruct.Web/PorphyStruct.Web.csproj \
    -c Release \
    -o /app/publish

FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html

COPY --from=build /app/publish/wwwroot .
COPY nginx.conf /etc/nginx/nginx.conf
