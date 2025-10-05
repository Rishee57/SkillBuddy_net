# =========================================
# Base runtime image
# =========================================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
# Run as non-root for security
# Define an app user (UID 1000 by default, can be overridden at build/run)
ARG APP_UID=1000
RUN adduser --disabled-password --gecos '' --uid ${APP_UID} appuser \
    && chown -R appuser /app
USER appuser

# Document the Expose ports used by ASP.NET Core
EXPOSE 8080
EXPOSE 8081

# =========================================
# Build image
# =========================================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj and restore (fast layer caching)
#COPY ["SkillBuddy.ui.csproj", "."]
COPY ["SkillBuddy.ui/SkillBuddy.ui.csproj", "SkillBuddy.ui/"]
COPY ["SkillBuddy.data/SkillBuddy.data.csproj", "SkillBuddy.data/"]
COPY ["SkillBuddy.Core/SkillBuddy.Core.csproj", "SkillBuddy.Core/"]
COPY ["SkillBuddy.BLL/SkillBuddy.BLL.csproj", "SkillBuddy.BLL/"]
COPY ["SkillBuddy.Entity/SkillBuddy.Entity.csproj", "SkillBuddy.Entity/"]

#RUN dotnet restore "SkillBuddy.ui.csproj"(includes Npgsql, JwtBearer, etc.)
RUN dotnet restore "SkillBuddy.ui/SkillBuddy.ui.csproj"
# Copy everything else and build
COPY . .
RUN dotnet build "SkillBuddy.ui/SkillBuddy.ui.csproj" -c Release -o /app/build


# =========================================
# Publish stage
# =========================================
FROM build AS publish
RUN dotnet publish "SkillBuddy.ui/SkillBuddy.ui.csproj" -c Release -o /app/publish /p:UseAppHost=false


# =========================================
# Final runtime image
# =========================================
FROM base AS final
WORKDIR /app

# Copy published app from build stage
COPY --from=publish /app/publish .

# Environment variables for PostgreSQL & JWT
# (overridden at runtime via docker-compose or k8s secrets)
# ENV ConnectionStrings__Default="Host=postgres;Port=5432;Database=skillbuddy;Username=skilluser;Password=skillpass"
# ENV Jwt__Issuer="SkillBuddy"
# ENV Jwt__Audience="SkillBuddyUsers"
# ENV Jwt__Key="super-secret-key-change-me"

# Entry point
ENTRYPOINT ["dotnet", "SkillBuddy.ui.dll"]
# docker build -t skill_buddy:1.0 -f SkillBuddy.ui/Dockerfile .