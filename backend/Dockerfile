FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /app

COPY *.csproj ./

RUN dotnet restore FitnessTracker.csproj

COPY . .

# Target the .csproj explicitly - once FitnessTracker.slnx is copied in
# below, "dotnet publish" with no project argument picks up the solution
# file instead, which also tries to build the sibling
# FitnessTracker.Tests project. That project lives outside this Dockerfile's
# build context (context: ./backend), so the build fails.
RUN dotnet publish FitnessTracker.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

WORKDIR /app

COPY --from=build /app/publish .

# Run as a non-root user - the base image's own "app" user (UID 64198)
# already exists for exactly this purpose.
USER app

EXPOSE 8080

ENTRYPOINT ["dotnet", "FitnessTracker.dll"]
