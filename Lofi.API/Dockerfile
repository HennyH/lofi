FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 8010

ENV ASPNETCORE_URLS=http://+:8010
RUN apt-get update && apt-get install -y libgdiplus

# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY Lofi.API.csproj .
RUN dotnet restore "Lofi.API.csproj"
COPY . .
RUN dotnet build "Lofi.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Lofi.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
ENV MUSIC_DIRECTORY "/music"
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Lofi.API.dll"]
