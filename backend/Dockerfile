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
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "LessonRunner.Api.dll"]
