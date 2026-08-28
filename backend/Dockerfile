FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY backend/LessonRunner.Domain/LessonRunner.Domain.csproj backend/LessonRunner.Domain/
COPY backend/LessonRunner.Application/LessonRunner.Application.csproj backend/LessonRunner.Application/
COPY backend/LessonRunner.Infrastructure/LessonRunner.Infrastructure.csproj backend/LessonRunner.Infrastructure/
COPY backend/LessonRunner.Api/LessonRunner.Api.csproj backend/LessonRunner.Api/
RUN dotnet restore backend/LessonRunner.Api/LessonRunner.Api.csproj

COPY backend/ backend/
RUN dotnet publish backend/LessonRunner.Api/LessonRunner.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# `curl` wyłącznie do healthchecku kontenera. Obraz `aspnet` jest oparty na Debianie slim
# i nie zawiera ani curla, ani wgeta, a healthcheck bez narzędzia HTTP nie ma jak zapytać
# o `/health`. Bez healthchecku `depends_on` nie potrafi wstrzymać startu nginxa do czasu,
# aż API faktycznie odpowiada.
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

# Katalogi na dane trwałe muszą istnieć **w obrazie** i należeć do użytkownika `app`.
# Docker przenosi właściciela katalogu z obrazu na świeżo tworzony wolumen nazwany, więc
# bez tego `api-data`, `api-uploads` i `api-backups` powstałyby jako root i aplikacja bez
# uprawnień roota nie miałaby gdzie zapisać bazy, uploadów ani kopii zapasowej.
#
# UWAGA przy wdrożeniu na istniejącą instalację: wolumen utworzony wcześniej zachowuje
# starego właściciela. Trzeba go raz poprawić:
#   docker compose run --rm --user root api chown -R app:app /app/data /app/backups /app/wwwroot/uploads
RUN mkdir -p /app/data /app/backups /app/wwwroot/uploads \
    && chown -R app:app /app/data /app/backups /app/wwwroot

# Aplikacja nie potrzebuje roota do niczego: nasłuchuje na porcie 8080 (powyżej 1024),
# a wszystko, co zapisuje, leży w katalogach powyżej. Obraz `aspnet` ma gotowe konto `app`.
USER app

EXPOSE 8080

# `/health` sprawdza połączenie z bazą i zwraca 503, gdy jej nie ma - `--fail` przekłada
# to na niezerowy kod wyjścia, czyli kontener „unhealthy”.
HEALTHCHECK --interval=30s --timeout=5s --start-period=40s --retries=3 \
    CMD curl --fail --silent --output /dev/null http://localhost:8080/health

ENTRYPOINT ["dotnet", "LessonRunner.Api.dll"]
