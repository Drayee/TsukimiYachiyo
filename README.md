# TsukimiYachiyo

一个基于《超时空辉夜姬！》中月见八千代角色设计的AI聊天应用，使用Ollama和Qwen模型提供智能对话功能。

作者：Drayee
版本：1.0.0 SNAPSHOT

---

## 📋 项目简介

TsukimiYachiyo是一个完整的AI聊天应用，包含客户端和服务端两个主要模块。客户端提供简洁的GUI界面，服务端负责与本地Ollama模型交互，实现智能对话功能。

---

## 🛠️ 技术栈

### 客户端 (YachiyoClient)
- Python 3.13
- Tkinter (GUI框架)
- Requests (HTTP请求)
- PyInstaller (打包工具)

### 服务端 (YachiyoService)
- Java 25
- Spring Boot 4.0.2
- Spring AI
- Maven

### AI模型
- Ollama
- Qwen 14B

---

## 📁 项目结构

```
TsukimiYachiyo/
├── YachiyoClient/              # Python客户端模块
│   ├── config.json            # 配置文件
│   ├── icon.ico               # 应用图标
│   └── yachiyo.py             # 主程序
├── YachiyoService/            # Spring Boot服务端
│   ├── Common/                # 公共模块
│   ├── Config/                # 配置模块
│   ├── Controller/            # 控制器模块
│   ├── Filter/                # 过滤器模块
│   ├── Service/               # 服务模块
│   └── pom.xml                # Maven配置
├── ollama/                    # Ollama模型文件
│   ├── v0.0.1/                # 版本1模型
│   └── v0.0.2/                # 版本2模型
└── README.md                  # 项目说明文档
```

---

## 🚀 快速开始

### 1. 准备工作

#### 安装Ollama
请先安装Ollama并下载Qwen模型：

```bash
ollama pull qwen:13b
```

### 2. 启动服务端

```bash
cd YachiyoService
mvn spring-boot:run
```

服务将在默认端口启动，等待客户端连接。

### 3. 运行客户端

#### 方式一：直接运行Python脚本

```bash
cd YachiyoClient
pip install requests
python yachiyo.py
```

#### 方式二：打包成可执行文件

```bash
cd YachiyoClient
pip install requests pyinstaller
pyinstaller -w -F -i icon.ico yachiyo.py
```

然后双击 `dist` 目录下的 `yachiyo.exe` 即可运行。

---

## ⚙️ 配置说明

### 客户端配置

客户端使用 `config.json` 文件进行配置，默认内容：

```json
{
  "url": "http://1bo19870064ac.vicp.fun/api/v1/ai/chat"
}
```

如果服务端在本地运行，可以修改为：

```json
{
  "url": "http://localhost:8080/api/v1/ai/chat"
}
```

### 服务端配置

服务端配置文件位于 `YachiyoService/Config/src/main/resources/application.yml`，用于配置AI模型连接等信息。

---

## 📡 API文档

### 聊天接口

- **URL**: `/api/v1/ai/chat`
- **方法**: `POST`
- **请求体**: `text/plain` - 用户消息内容
- **响应**: `text/plain` - AI回复内容

#### 示例请求

```bash
curl -X POST -d "你好，八千代" http://localhost:8080/api/v1/ai/chat
```

#### 示例响应

```
你好！我是月见八千代，很高兴为你服务！
```

---

## 🎨 客户端界面

客户端提供简洁的GUI界面：
- 输入框：用于输入消息
- 发送按钮：发送消息到服务端
- 显示区域：显示AI回复

---

## 🔧 开发说明

### 客户端开发

客户端基于Python Tkinter开发，主要文件：
- `yachiyo.py`: 主程序，包含GUI和网络请求逻辑
- `config.json`: 配置文件，存储服务端URL

### 服务端开发

服务端基于Spring Boot开发，主要模块：
- `Controller`: 处理HTTP请求
- `Config`: 配置AI客户端
- `Service`: 业务逻辑层

---

## 📄 许可证

本项目采用MIT许可证，详情请查看LICENSE文件。

---

## 📧 联系方式

如有问题或建议，请联系：Drayee

---

## 更新日志

### v1.0.0 SNAPSHOT
- 初始版本发布
- 实现基本的AI聊天功能
- 支持客户端和服务端分离架构
- 支持Ollama Qwen 14B模型