# =========================================
# Base runtime image
# =========================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
# RUN adduser --disabled-password --gecos '' --uid ${APP_UID} appuser \
#     && chown -R appuser /app
# USER appuser

# Install curl for VSDBG
RUN apt-get update && apt-get install -y --no-install-recommends curl unzip \
    && rm -rf /var/lib/apt/lists/* \
    && curl -sSL https://aka.ms/getvsdbgsh | bash /dev/stdin -v latest -l /root/.vsdbg

# Document the Expose ports used by ASP.NET Core
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
ARG BUILD_CONFIGURATION=Release

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
RUN dotnet build "SkillBuddy.ui/SkillBuddy.ui.csproj" -c $BUILD_CONFIGURATION -o /app/build


# =========================================
# Publish stage
# =========================================
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "SkillBuddy.ui/SkillBuddy.ui.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false


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