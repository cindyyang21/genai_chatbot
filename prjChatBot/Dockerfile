# 使用 ASP.NET Core 執行環境
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# 使用 .NET SDK 作為構建環境
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# 複製 .csproj 檔案並執行還原
COPY prjChatBot.csproj ./
RUN dotnet restore "./prjChatBot.csproj"

# 複製剩下的所有專案文件
COPY . .

# 構建專案
WORKDIR /src
RUN dotnet build "prjChatBot.csproj" -c $BUILD_CONFIGURATION -o /app/build

# 執行發佈
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "prjChatBot.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# 使用已發布的應用作為最終映像
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "prjChatBot.dll"]
