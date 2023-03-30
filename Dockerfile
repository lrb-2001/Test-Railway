# 设置要使用的基础镜像
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env

# 在容器中创建一个工作目录
WORKDIR /app

# 将本地文件复制到容器中的工作目录
COPY . .

# 执行 dotnet restore 命令还原依赖项
RUN dotnet restore

# 执行 dotnet publish 命令将应用程序发布到一个目录中
RUN dotnet publish -c Release -o out

# 将应用程序运行时设置为较小的运行时映像
FROM mcr.microsoft.com/dotnet/runtime:5.0 AS runtime

# 在容器中创建一个工作目录
WORKDIR /app

# 从之前的构建阶段中复制应用程序
COPY --from=build-env /app/out .

# 指定应用程序的启动命令
ENTRYPOINT ["dotnet", "FrameCoreAPI.dll"]