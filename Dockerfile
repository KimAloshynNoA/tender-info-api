FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .
RUN dotnet restore "TenderInfoAPI.sln"

COPY . .
RUN dotnet build "TenderInfoAPI/TenderInfoAPI.csproj" -c Release -o /app/build

RUN dotnet publish "TenderInfoAPI/TenderInfoAPI.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080

ENTRYPOINT ["dotnet", "TenderInfoAPI.dll"]
