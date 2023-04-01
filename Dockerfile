# 将应用程序运行时设置为较小的运行时映像
FROM mcr.microsoft.com/dotnet/runtime:5.0 AS runtime

# 在容器中创建一个工作目录
WORKDIR /app

# 指定应用程序的启动命令
ENTRYPOINT ["dotnet", "/app/OpenAIAPI.dll"]