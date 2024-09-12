#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Services/AuthService/HomeManagement.AuthService.Api/HomeManagement.AuthService.Api.csproj", "src/Services/AuthService/HomeManagement.AuthService.Api/"]
COPY ["src/Services/AuthService/HomeManagement.AuthService.Application/HomeManagement.AuthService.Application.csproj", "src/Services/AuthService/HomeManagement.AuthService.Application/"]
COPY ["src/Services/AuthService/HomeManagement.AuthService.Domain/HomeManagement.AuthService.Domain.csproj", "src/Services/AuthService/HomeManagement.AuthService.Domain/"]
COPY ["src/Services/HomeManagement.Shared/HomeManagement.Shared.csproj", "src/Services/HomeManagement.Shared/"]
COPY ["src/Services/AuthService/HomeManagement.AuthService.Infrastructure/HomeManagement.AuthService.Infrastructure.csproj", "src/Services/AuthService/HomeManagement.AuthService.Infrastructure/"]
RUN dotnet restore "src/Services/AuthService/HomeManagement.AuthService.Api/HomeManagement.AuthService.Api.csproj"
COPY . .
WORKDIR "/src/src/Services/AuthService/HomeManagement.AuthService.Api"
RUN dotnet build "HomeManagement.AuthService.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HomeManagement.AuthService.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

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
ENTRYPOINT ["dotnet", "HomeManagement.AuthService.Api.dll"]
