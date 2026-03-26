# =========================================
# Base runtime image
# =========================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# RUN adduser --disabled-password --gecos '' --uid ${APP_UID} appuser \
#     && chown -R appuser /app
# USER appuser

# Install curl for VSDBG (needed for debugging in VS Code)
RUN apt-get update && apt-get install -y --no-install-recommends curl unzip \
    && rm -rf /var/lib/apt/lists/* \
    && curl -sSL https://aka.ms/getvsdbgsh | bash /dev/stdin -v latest -l /root/.vsdbg

# =========================================
# Build image
# =========================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

ARG BUILD_CONFIGURATION=Release
RUN dotnet restore "SkillBuddy.Worker/SkillBuddy.Worker.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/SkillBuddy.Worker"
RUN dotnet build "SkillBuddy.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/build

# =========================================
# Publish stage
# =========================================
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "SkillBuddy.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# =========================================
# Final runtime image
# =========================================
FROM base AS final
WORKDIR /app

# Copy published app from build stage
COPY --from=publish /app/publish .

# Entry point
ENTRYPOINT ["dotnet", "SkillBuddy.Worker.dll"]
