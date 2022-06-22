FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
        libgdiplus \
        libc6-dev \
        libfontconfig1 \
        ffmpeg \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /app
COPY . .
RUN dotnet build VrijenhoekPhotos.Api/VrijenhoekPhotos.Api.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish VrijenhoekPhotos.Api/VrijenhoekPhotos.Api.csproj -c Release -o /app/publish

FROM base as final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT [ "dotnet", "VrijenhoekPhotos.Api.dll" ]