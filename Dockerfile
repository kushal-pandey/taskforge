# ---- Build stage ----
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY TaskForge.slnx .
COPY src/TaskForge.Api/TaskForge.Api.csproj src/TaskForge.Api/
COPY src/TaskForge.Application/TaskForge.Application.csproj src/TaskForge.Application/
COPY src/TaskForge.Domain/TaskForge.Domain.csproj src/TaskForge.Domain/
COPY src/TaskForge.Infrastructure/TaskForge.Infrastructure.csproj src/TaskForge.Infrastructure/

RUN dotnet restore src/TaskForge.Api/TaskForge.Api.csproj

COPY src/ src/
RUN dotnet publish src/TaskForge.Api/TaskForge.Api.csproj -c Release -o /app/publish --no-restore

# ---- Runtime stage ----
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "TaskForge.Api.dll"]