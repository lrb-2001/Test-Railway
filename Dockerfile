# 基于官方的 .NET 6 SDK 镜像构建
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

# 设置工作目录
WORKDIR /app

# 复制项目文件到工作目录中
COPY . .

# 使用 dotnet 命令还原依赖项并构建项目
RUN dotnet restore
RUN dotnet publish -c Release -o out

# 构建运行时镜像
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app/out .

# 暴露端口
# EXPOSE 80
# ENV ASPNETCORE_URLS=http://+:80

# 启动应用程序
ENTRYPOINT ["dotnet", "RailwayTest.dll"]