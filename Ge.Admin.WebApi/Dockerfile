# 请参阅 https://aka.ms/customizecontainer 以了解如何自定义调试容器，以及 Visual Studio 如何使用此 Dockerfile 生成映像以更快地进行调试。

# 此阶段用于在快速模式(默认为调试配置)下从 VS 运行时

#使用运行时镜像
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
#设置工作目录
WORKDIR /app
# 把目录下的内容都复制到当前目录下
COPY . .

EXPOSE 5255

# 运行镜像入口命令和可执行文件名称
ENTRYPOINT ["dotnet", "Ge.Admin.WebApi.dll"]