# .NET Core Web API service for LIMS ENV#


### .NET Core ###
use ASP.NET Core 3.1

### Setup Ubuntu 18.04 LTS with Nginx for hosting .NET Core Web API###
1. 参考文档（微软官方文档，非常全面）:
    - 整体：[Host ASP.NET Core on Linux with Nginx][1]
    - 安装：[在 Ubuntu 上安装 .NET Core SDK 或 .NET Core 运行时][2]

2. 通讯架构：
.NET Web API <-- localhost:5000 --> Nginx <-- limsapi.eachcloud.co --> End User


3. Nginx反向代理核心配置：
```
server {
    listen        80;
    server_name   limsapi.eachcloud.co;
    location / {
        proxy_pass         http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header   Upgrade $http_upgrade;
        proxy_set_header   Connection keep-alive;
        proxy_set_header   Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header   X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header   X-Forwarded-Proto $scheme;
    }
}
```

3. 使用Ubuntu的Systemd将Rest service服务化
```shell
$sudo vim /etc/systemd/system/limsapi.service
```

```shell
[Unit]
Description=LIMS API .NET Web API running on Ubuntu

[Service]
WorkingDirectory=/var/www/limsapi
ExecStart=/usr/bin/dotnet /var/www/limsapi/limsapi.dll
Restart=always
# Restart service after 10 seconds if the dotnet service crashes:
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=dotnet-example
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target
```

启用limsapi.service:
```
$sudo systemctl enable limsapi.service
```

启动limsapi.service:
```
$sudo systemctl start limsapi
```

查看状态：
```
$sudo systemctl status limsapi
```

重启limsapi.service:
```
$sudo systemctl restart limsapi
```

### 将应用部署到ubuntu服务器 ### sudo -i 切换到root用户
1. 应用程序发布和打包：将应用程序发布到文件夹后，将所有发布出来的文件整个打入一个zip包中
2. 转移应用程序到服务器：
    -清空`/var/www/limsapi/`文件夹下所有文件`$sudo rm -rf /var/www/limsapi/*`
    - 在zip打包的文件夹下，打开终端，运行`scp publish.zip niels@106.15.41.99:/var/www/limsapi`
    - `ssh`到服务器，`ssh niels@106.15.41.99`, pwd: Cat4Eat.2020
    -并解压，`$sudo unzip publish.zip`
    -ResponseFile文件夹权限赋值 `chmod -R 777 ResponseFile`
    - 重启limsapi.service：`$sudo systemctl restart limsapi`

3. 应用程序外部网址：https://limsapi.eachcloud.co

### Docker ###
1. 构建镜像，在Dockerfile所在文件夹执行： `docker build -t limsapi .`

### 疑难解决 ###
1. 应用程序报错，api无法访问
    - 先关闭limsapi.service: `$sudo systemctl stop limsapi`
    - `cd`到`/var/www/limsapi/`文件夹下，直接运行`$dotnet limsapi.dll` 看输出

### 参考资料 ###
1. [Docker —— 从入门到实践][1]
2. [Docker docs][2]

 [1]: https://yeasy.gitbook.io/docker_practice/
 [2]: https://docs.docker.com/













[1]: https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/linux-nginx?view=aspnetcore-3.1
[2]: https://docs.microsoft.com/zh-cn/dotnet/core/install/linux-ubuntu#1804-