FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /DockerSource

# Copy csproj and restore as distinct layers
COPY *.sln .
COPY NuGet.Config .
COPY Aper_bot/*.csproj ./Aper_bot/
COPY Brigadier.NET/Brigadier.NET/*.csproj ./Brigadier.NET/Brigadier.NET/
COPY DSharpPlus.SlashCommands/DSharpPlus.SlashCommands/*.csproj ./DSharpPlus.SlashCommands/DSharpPlus.SlashCommands/
COPY Planning/*.csproj ./Planning/
RUN dotnet restore

# Copy everything else and build website
COPY Aper_bot/. ./Aper_bot/
COPY Brigadier.NET/. ./Brigadier.NET/
COPY DSharpPlus.SlashCommands/. ./DSharpPlus.SlashCommands/

WORKDIR /DockerSource/Aper_bot

#RUN dotnet publish -c release -o /DockerOutput/Website --no-restore
RUN dotnet publish -c release -o /DockerOutput/Website

# Final stage / image
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /Aper_bot
COPY --from=build /DockerOutput/Website ./
ENTRYPOINT ["dotnet", "Aper_bot.dll"]