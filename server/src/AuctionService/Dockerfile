# as build: set up with base image 
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app 
EXPOSE 80 

# copy all .csproj files and restore as distinct layers. Use of same COPY command
# for every dockerfile in the project to take advantage of docker caching
COPY server/src/Phuoclx.Udemy.Microservices.sln src/Phuoclx.Udemy.Microservices.sln 
COPY server/src/AuctionService/AuctionService.csproj src/AuctionService/AuctionService.csproj
COPY server/src/BindingService/BindingService.csproj src/BindingService/BindingService.csproj
COPY server/src/IdentityService/IdentityService.csproj src/IdentityService/IdentityService.csproj
COPY server/src/SearchService/SearchService.csproj src/SearchService/SearchService.csproj
COPY server/src/GatewayService/GatewayService.csproj src/GatewayService/GatewayService.csproj
COPY server/src/NotificationService/NotificationService.csproj src/NotificationService/NotificationService.csproj
COPY server/src/Constracts/Constracts.csproj src/Constracts/Constracts.csproj
COPY server/tests/AuctionService.UnitTests/AuctionService.UnitTests.csproj tests/AuctionService.UnitTests/AuctionService.UnitTests.csproj
COPY server/tests/AuctionService.IntergrationTests/AuctionService.IntergrationTests.csproj tests/AuctionService.IntergrationTests/AuctionService.IntergrationTests.csproj

# Restore package deps 
RUN dotnet restore server/src/Phuoclx.Udemy.Microservices.sln

# Copy the app folders over
COPY server/src/AuctionService src/AuctionService
COPY server/src/Constracts src/Constracts
WORKDIR /app/src/AuctionService
RUN dotnet publish -c Release -o /app/src/out

# Build runtime image 
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app 
COPY --from=build-env /app/src/out .
ENTRYPOINT [ "dotnet", "AuctionService.dll" ]