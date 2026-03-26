# =========================
# Build stage
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore for caching
COPY SkillBuddy.Blazor/SkillBuddy.Blazor.csproj SkillBuddy.Blazor/
RUN dotnet restore "SkillBuddy.Blazor/SkillBuddy.Blazor.csproj"

# Copy source and publish
COPY . .
WORKDIR "/src/SkillBuddy.Blazor"
RUN dotnet publish "SkillBuddy.Blazor.csproj" -c Release -o /app/publish /p:UseAppHost=false

# =========================
# Runtime stage
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Create non-root user
ARG APP_UID=1000
RUN adduser --disabled-password --gecos '' --uid ${APP_UID} appuser

# Copy HTTPS certificate
COPY https.pfx /https/https.pfx

# Expose ports
EXPOSE 80
EXPOSE 443

# Environment configuration
ENV ASPNETCORE_URLS="http://+:80;https://+:443"
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/https/https.pfx
ENV ASPNETCORE_Kestrel__Certificates__Default__Password=skillbuddy123

# Copy published output with proper ownership
COPY --chown=appuser:appuser --from=build /app/publish .

# Switch to non-root user
USER appuser

# Healthcheck removed as curl is not available in the base image

ENTRYPOINT ["dotnet", "SkillBuddy.Blazor.dll"]
