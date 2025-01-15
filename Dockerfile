# Use the official ASP.NET Core runtime as a parent image
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["ProductsApi.csproj", "./"]
RUN dotnet restore "./ProductsApi.csproj"
COPY . . 
WORKDIR "/src/"
RUN dotnet publish "ProductsApi.csproj" -c Release -o /app/publish

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish . 
ENTRYPOINT ["dotnet", "ProductsApi.dll"]
