FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /challenge
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["NOS.Engineering.Challenge.API/NOS.Engineering.Challenge.API.csproj", "NOS.Engineering.Challenge.API/"]
COPY ["NOS.Engineering.Challenge/NOS.Engineering.Challenge.csproj", "NOS.Engineering.Challenge/"]
RUN dotnet restore "NOS.Engineering.Challenge.API/NOS.Engineering.Challenge.API.csproj"
COPY . .
WORKDIR "/src/NOS.Engineering.Challenge.API"
RUN dotnet build "NOS.Engineering.Challenge.API.csproj" -c Release -o /challenge/build

FROM build AS publish
RUN dotnet publish "NOS.Engineering.Challenge.API.csproj" -c Release -o /challenge/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /challenge
COPY --from=publish /challenge/publish .
ENTRYPOINT ["dotnet", "NOS.Engineering.Challenge.API.dll"]
