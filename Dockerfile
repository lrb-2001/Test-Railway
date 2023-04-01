RUN apt-get update \
    && apt-get install -y --no-install-recommends \
        curl \
        && curl -SL --output dotnet.tar.gz https://dotnetcli.azureedge.net/dotnet/Sdk/5.0.400/dotnet-sdk-5.0.400-linux-x64.tar.gz \
        && dotnet_sha512='a5a6b21d6a5a18ba32542afab28549e8ec1ce42517675ccda99cfbb4cf4e5424c4d4a4ef9c9cd4b58d11c3f3a45e2a618edaf102f8e97dc83562d03b9d39a23a *dotnet.tar.gz' \
        && echo "$dotnet_sha512" | sha512sum -c - \
        && mkdir -p /usr/share/dotnet \
        && tar -zxf dotnet.tar.gz -C /usr/share/dotnet \
        && rm dotnet.tar.gz \
        && ln -s /usr/share/dotnet/dotnet /usr/bin/dotnet

# 将应用程序运行时设置为较小的运行时映像
FROM mcr.microsoft.com/dotnet/runtime:5.0 AS runtime

# 在容器中创建一个工作目录
WORKDIR /app

COPY . .

# 指定应用程序的启动命令
ENTRYPOINT ["dotnet", "/app/OpenAIAPI.dll"]