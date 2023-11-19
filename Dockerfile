FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

ARG mongodb_connection_string
ENV MONGODB_CONNECTION_STRING=$mongodb_connection_string

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["LoLModule/LoLModule.csproj", "LoLModule/"]
RUN dotnet restore "LoLModule/LoLModule.csproj"
COPY . .
WORKDIR "/src/LoLModule"
RUN dotnet build "LoLModule.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LoLModule.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LoLModule.dll"]
