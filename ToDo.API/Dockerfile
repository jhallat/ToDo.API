FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["ToDo.API/ToDo.API.csproj", "ToDo.API/"]
RUN dotnet restore "ToDo.API/ToDo.API.csproj"
COPY . .
WORKDIR "/src/ToDo.API"
RUN dotnet build "ToDo.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ToDo.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ToDo.API.dll"]
