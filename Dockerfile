FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["TodoAPI/TodoAPI.csproj", "TodoAPI/"]
RUN dotnet restore "TodoAPI/TodoAPI.csproj"
COPY . .
WORKDIR "/src/TodoAPI"
RUN dotnet build "TodoAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TodoAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
RUN mkdir -p /app/db
COPY TodoAPI/db /app/db
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TodoAPI.dll"]
