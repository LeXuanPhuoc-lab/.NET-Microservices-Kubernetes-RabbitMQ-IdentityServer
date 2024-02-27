FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env 
WORKDIR /app 
EXPOSE 80 

# copy all csproj files and solution file 
COPY Phuoclx.Udemy.Microservices.sln Phuoclx.Udemy.Microservices.sln
COPY src/AuctionService/AuctionService.csproj src/AuctionService/AuctionService.csproj
COPY src/SearchService/SearchService.csproj src/SearchService/SearchService.csproj
COPY src/IdentityService/IdentityService.csproj src/IdentityService/IdentityService.csproj
COPY src/GatewayService/GatewayService.csproj src/GatewayService/GatewayService.csproj
COPY src/Constracts/Constracts.csproj src/Constracts/Constracts.csproj

# restore package deps 
RUN dotnet restore Phuoclx.Udemy.Microservices.sln 

# copy app folders 
COPY src/IdentityService src/IdentityService 
WORKDIR /app/src/IdentityService 
RUN dotnet publish -c Release -o /app/src/out 

# runtime 
FROM mcr.microsoft.com/dotnet/aspnet:8.0 
WORKDIR /app 
COPY --from=build-env /app/src/out .
ENTRYPOINT [ "dotnet", "IdentityService.dll" ]