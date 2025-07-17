FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env 
WORKDIR /app 
EXPOSE 80 

# copy all csproj file and restore as distinct layers
# copy each project file and solution to take advantage of docker caching 
COPY src/Phuoclx.Udemy.Microservices.sln src/Phuoclx.Udemy.Microservices.sln 
COPY src/AuctionService/AuctionService.csproj src/AuctionService/AuctionService.csproj
COPY src/BindingService/BindingService.csproj src/BindingService/BindingService.csproj
COPY src/IdentityService/IdentityService.csproj src/IdentityService/IdentityService.csproj
COPY src/SearchService/SearchService.csproj src/SearchService/SearchService.csproj
COPY src/GatewayService/GatewayService.csproj src/GatewayService/GatewayService.csproj
COPY src/NotificationService/NotificationService.csproj src/NotificationService/NotificationService.csproj
COPY src/Constracts/Constracts.csproj src/Constracts/Constracts.csproj
COPY tests/AuctionService.UnitTests/AuctionService.UnitTests.csproj tests/AuctionService.UnitTests/AuctionService.UnitTests.csproj
COPY tests/AuctionService.IntergrationTests/AuctionService.IntergrationTests.csproj tests/AuctionService.IntergrationTests/AuctionService.IntergrationTests.csproj

# Restore package deps 
RUN dotnet restore src/Phuoclx.Udemy.Microservices.sln 

# copy app folders over 
COPY src/NotificationService src/NotificationService
COPY src/Constracts src/Constracts
WORKDIR /app/src/NotificationService
RUN dotnet publish -c Release -o /app/src/out 

# runtime 
FROM mcr.microsoft.com/dotnet/aspnet:8.0 
WORKDIR /app 
COPY --from=build-env /app/src/out .
ENTRYPOINT [ "dotnet", "NotificationService.dll" ]



