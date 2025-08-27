# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
# For more information, please see https://aka.ms/containercompat

# This stage is used when running from VS in fast mode (Default for Debug configuration)


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /BE
COPY ["BE/BE/BE.csproj", "BE/"]
COPY ["BE/Data/Data.csproj", "Data/"]
COPY ["BE/Domain/Domain.csproj", "Domain/"]
COPY ["BE/Common/Common.csproj", "Common/"]
COPY ["BE/Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["BE/Service/Service.csproj", "Service/"]
RUN dotnet restore "./BE/BE.csproj"

COPY . .
WORKDIR /BE
RUN dotnet build "BE/BE.csproj" -c Release -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
RUN dotnet publish "BE/BE.csproj" -c Release -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0
ENV ASPNETCORE_HTTP_PORTS=5001
EXPOSE 5001
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BE.dll"]