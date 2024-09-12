#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
# EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Services/UserService/HomeManagement.UserService.Api/HomeManagement.UserService.Api.csproj", "src/Services/UserService/HomeManagement.UserService.Api/"]
COPY ["src/Services/UserService/HomeManagement.UserService.Application/HomeManagement.UserService.Application.csproj", "src/Services/UserService/HomeManagement.UserService.Application/"]
COPY ["src/Services/UserService/HomeManagement.UserService.Domain/HomeManagement.UserService.Domain.csproj", "src/Services/UserService/HomeManagement.UserService.Domain/"]
COPY ["src/Services/HomeManagement.Shared/HomeManagement.Shared.csproj", "src/Services/HomeManagement.Shared/"]
COPY ["src/Services/UserService/HomeManagement.UserService.Infrastructure/HomeManagement.UserService.Infrastructure.csproj", "src/Services/UserService/HomeManagement.UserService.Infrastructure/"]
RUN dotnet restore "src/Services/UserService/HomeManagement.UserService.Api/HomeManagement.UserService.Api.csproj"
COPY . .
WORKDIR "/src/src/Services/UserService/HomeManagement.UserService.Api"
RUN dotnet build "HomeManagement.UserService.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HomeManagement.UserService.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS final
WORKDIR /app

# Install EF Core tools
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

COPY --from=publish /app/publish .

# Make sure the app user can access the necessary directories
RUN mkdir -p /home/app/.dotnet/tools && chown -R app:app /home/app

# Switch to non-root user
USER app

# Update PATH for the app user
ENV PATH="$PATH:/home/app/.dotnet/tools"
ENTRYPOINT ["dotnet", "HomeManagement.UserService.Api.dll"]
