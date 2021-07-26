FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["./ToDo.API.csproj", "./"]
RUN dotnet restore "./ToDo.API.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "ToDo.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ToDo.API.csproj" -c Release -o /app/publish

FROM base AS final
ENV ASPNETCORE_URLS=http://*:5000
ENV ASPNETCORE_ENVIRONMENT=Production
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ToDo.API.dll"]
