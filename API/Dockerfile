FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG TARGETARCH
WORKDIR /source

# Copy project files and the packages.lock.json file
COPY API/API.csproj API/
COPY API/packages.lock.json API/

# Restore using the lock file
RUN dotnet restore API/API.csproj /p:RestorePackagesWithLockFile=true /p:PublishReadyToRun=true -a $TARGETARCH 

# Copy the source code
COPY API/ API/

# Publish the application
RUN dotnet publish API/API.csproj /p:PublishReadyToRun=true -a $TARGETARCH --self-contained --no-restore -c Release -o /app

FROM mcr.microsoft.com/dotnet/runtime-deps:10.0 AS final
WORKDIR /app
COPY --from=build /app .
RUN chmod +x API
ENTRYPOINT ["./API"]