FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /DockerSource

LABEL org.opencontainers.image.source = "https://github.com/dpeter99/Aper_bot"

# Copy csproj and restore as distinct layers
COPY *.sln .
COPY NuGet.Config .
COPY Aper_bot/*.csproj ./Aper_bot/
COPY Aper_bot.Test/*.csproj ./Aper_bot.Test/
RUN dotnet restore Aper_bot

# Copy everything else and build website
COPY Aper_bot/. ./Aper_bot/

WORKDIR /DockerSource/Aper_bot

#RUN dotnet publish -c release -o /DockerOutput/Website --no-restore
RUN dotnet publish -c release -o /DockerOutput/Website

# Final stage / image
FROM mcr.microsoft.com/dotnet/aspnet:5.0

#HEALTHCHECK CMD curl --fail http://localhost:80/health || exit

WORKDIR /Aper_bot
COPY --from=build /DockerOutput/Website ./
ENTRYPOINT ["dotnet", "Aper_bot.dll"]


