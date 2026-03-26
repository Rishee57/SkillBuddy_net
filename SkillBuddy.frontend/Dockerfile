# =========================================
# Base runtime image
# =========================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# expose the ports the app will listen on
EXPOSE 80
EXPOSE 443

# run as non-root user for security
ARG APP_UID=1000
RUN adduser --disabled-password --gecos '' --uid ${APP_UID} appuser \
    && chown -R appuser /app
USER appuser

# =========================================
# Build image
# =========================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# copy csproj and restore to leverage layer caching
COPY ["SkillBuddy.frontend/SkillBuddy.frontend.csproj", "SkillBuddy.frontend/"]
RUN dotnet restore "SkillBuddy.frontend/SkillBuddy.frontend.csproj"

# copy everything else and build
COPY . .
RUN dotnet build "SkillBuddy.frontend/SkillBuddy.frontend.csproj" -c Release -o /app/build

# =========================================
# Publish stage
# =========================================
FROM build AS publish
RUN dotnet publish "SkillBuddy.frontend/SkillBuddy.frontend.csproj" -c Release -o /app/publish /p:UseAppHost=false

# =========================================
# Final runtime image
# =========================================
FROM base AS final
WORKDIR /app

# copy published output from build stage
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "SkillBuddy.frontend.dll"]
