# Yachiyo 项目 API 文档

## 项目概述

Yachiyo 是一个集成了 AI 聊天和语音合成功能的项目，包含以下组件：

- **EasyYachiyoClient**：基于 WPF 开发的前端应用
- **EasyYachiyoClient-old**：基于 Python 开发的前端应用
- **YachiyoClient**：基于 Python 开发的云端客户端
- **YachiyoService**：基于 Java Spring Boot 开发的后端服务
- **ollama**：模型训练文件

## 后端服务 API

### 基础信息

- **服务地址**：`http://127.0.0.1:8080`
- **API 版本**：`v1`
- **请求格式**：JSON (聊天接口) / 纯文本 (语音接口)

### 接口详情

#### 1. 聊天接口

**接口地址**：`/api/v1/ai/chat`

**请求方法**：`POST`

**请求体**：
```json
{
  "prompt": "聊天内容"
}
```

**响应**：
- 成功：返回 AI 回复的文本内容
- 失败：返回错误信息

**示例**：
```bash
# 请求
curl -X POST http://127.0.0.1:8080/api/v1/ai/chat \
  -H "Content-Type: application/json" \
  -d '{"prompt": "你好，我是用户"}'

# 响应
"你好！我是 Yachiyo，很高兴为你服务。"
```

#### 2. 语音合成接口

**接口地址**：`/api/v1/ai/speak`

**请求方法**：`POST`

**请求体**：纯文本内容

**响应**：
- 成功：返回音频数据（byte[]）
- 失败：返回错误信息

**示例**：
```bash
# 请求
curl -X POST http://127.0.0.1:8080/api/v1/ai/speak \
  -H "Content-Type: text/plain" \
  -d "你好，我是 Yachiyo"

# 响应
[音频二进制数据]
```

## 后端服务实现细节

### 语音合成流程

1. 接收前端发送的文本
2. 将文本翻译成日语
3. 构造 TTS 请求对象
4. 调用语音合成服务（地址：`http://0.0.0.0:9882`）
5. 接收并返回音频数据

### 核心类

- **AIChatController**：处理 API 请求
- **SpeakService**：语音合成服务接口
- **SpeakServiceImpl**：语音合成服务实现
- **TTSRequest**：语音合成请求数据结构

## 前端集成

### WPF 前端（EasyYachiyoClient）

#### 聊天功能集成

```csharp
// 发送消息到后端
using HttpClient client = new HttpClient();
client.Timeout = System.TimeSpan.FromSeconds(30);

// 构建请求体
var requestBody = new { prompt = messageContent };
string jsonBody = JsonSerializer.Serialize(requestBody);
var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

// 发送请求
HttpResponseMessage response = await client.PostAsync("http://127.0.0.1:8080/api/v1/ai/chat", content);
response.EnsureSuccessStatusCode();

// 读取响应
string responseContent = await response.Content.ReadAsStringAsync();
```

#### 语音合成功能集成

```csharp
// 调用语音合成服务
using HttpClient client = new HttpClient();
client.Timeout = TimeSpan.FromSeconds(30);

// 直接发送文本内容
var content = new StringContent(text);
HttpResponseMessage response = await client.PostAsync("http://127.0.0.1:8080/api/v1/ai/speak", content, cancellationToken);
response.EnsureSuccessStatusCode();

// 读取音频数据
byte[] audioData = await response.Content.ReadAsByteArrayAsync(cancellationToken);
```

### Python 前端（EasyYachiyoClient-old）

类似的集成方式，使用 Python 的 `requests` 库发送 HTTP 请求。

## 项目结构

```
TsukimiYachiyo/
├── EasyYachiyoClient/         # WPF 前端
├── EasyYachiyoClient-old/     # Python 前端
├── ollama/                    # 模型训练文件
│   ├── v0.0.1/                # 模型版本 1
│   └── v0.0.2/                # 模型版本 2
├── YachiyoClient/             # Python 云端客户端
└── YachiyoService/            # Java 后端服务
    ├── Common/                # 公共模块
    ├── Config/                # 配置模块
    ├── Controller/            # 控制器模块
    ├── Service/               # 服务模块
    └── dto/                   # 数据传输对象
```

## 环境要求

### 后端服务

- Java 8+
- Spring Boot 3.0+
- Maven

### 前端应用

- WPF：.NET 8.0+
- Python 前端：Python 3.7+

## 运行说明

1. 启动后端服务：在 YachiyoService 目录下执行 `mvn spring-boot:run`
2. 启动前端应用：运行 EasyYachiyoClient.exe 或 Python 脚本
3. 确保语音合成服务（地址：`http://0.0.0.0:9882`）已启动

## 常见问题

1. **API 调用失败**：检查后端服务是否运行，端口是否正确
2. **语音合成失败**：检查语音合成服务是否运行，地址是否正确
3. **翻译失败**：检查翻译服务配置是否正确

## 版本历史

- v0.0.1：初始版本
- v0.0.2：优化语音合成功能

## 联系方式

如有问题，请联系项目维护者。