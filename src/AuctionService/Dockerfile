# as build: set up with base image 
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app 
EXPOSE 80 

# copy all .csproj files and restore as distinct layers. Use of same COPY command
# for every dockerfile in the project to take advantage of docker caching
COPY Phuoclx.Udemy.Microservices.sln Phuoclx.Udemy.Microservices.sln 
COPY src/AuctionService/AuctionService.csproj src/AuctionService/AuctionService.csproj
COPY src/SearchService/SearchService.csproj src/SearchService/SearchService.csproj
COPY src/GatewayService/GatewayService.csproj src/GatewayService/GatewayService.csproj
COPY src/IdentityService/IdentityService.csproj src/IdentityService/IdentityService.csproj
COPY src/Constracts/Constracts.csproj src/Constracts/Constracts.csproj

# Restore package deps 
RUN dotnet restore Phuoclx.Udemy.Microservices.sln 

# Copy the app folders over
COPY src/AuctionService src/AuctionService
COPY src/Constracts src/Constracts
WORKDIR /app/src/AuctionService
RUN dotnet publish -c Release -o /app/src/out 

# Build runtime image 
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app 
COPY --from=build-env /app/src/out .
ENTRYPOINT [ "dotnet", "AuctionService.dll" ]