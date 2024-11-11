# 使用官方的 .NET 8.0 SDK 映像作為構建階段
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# 設置工作目錄
WORKDIR /app

# 複製所有文件到容器中
COPY . .

# 還原依賴項
RUN dotnet restore

# 構建應用程式
RUN dotnet publish -c Release -o out

# 使用官方的 .NET 8.0 運行時映像作為運行階段
FROM mcr.microsoft.com/dotnet/aspnet:8.0

# 設置工作目錄
WORKDIR /app

# 從構建階段複製構建輸出
COPY --from=build /app/out .

# 暴露應用程式運行的端口
EXPOSE 7260

# 設置應用程式的入口點
ENTRYPOINT ["dotnet", "Eywa.Vehicle.Defender.dll"]
