FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 8001

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["MatrixNotifierApi.csproj", "./"]
RUN dotnet restore "MatrixNotifierApi.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "MatrixNotifierApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MatrixNotifierApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

USER 65534
EXPOSE 8001
ENV ASPNETCORE_URLS=http://*:8001
ENTRYPOINT ["dotnet", "MatrixNotifierApi.dll"]
