<template>
  <div class="chat-container">
    <!-- 左侧会话栏 -->
    <aside class="sidebar">
      <div class="sidebar-header">
        <h2>对话选择</h2>
        <button class="new-chat-btn" @click="createNewConversation" :disabled="isCreating">
          <span class="plus-icon">+</span>
          <span class="btn-text">{{ isCreating ? '创建中...' : '新建' }}</span>
        </button>
      </div>
      <div class="conversation-list" ref="conversationListRef">
        <transition-group name="conversation">
          <div v-for="id in conversations" :key="id" :class="['conversation-item', { active: id === currentConversationId }]"
            @click="selectConversation(id)">
            <span class="conversation-icon">💬</span>
            <span class="conversation-name">对话 {{ id }}</span>
          </div>
        </transition-group>
        <div v-if="conversations.length === 0" class="empty-conversations">
          暂无会话，点击上方新建按钮开始对话
        </div>
      </div>
      <div class="user-info">
        <div class="user-avatar">
          <span>{{ username.charAt(0).toUpperCase() }}</span>
        </div>
        <span class="username">{{ username }}</span>
        <button class="logout-btn" @click="logout">
          <span>退出</span>
        </button>
      </div>
    </aside>

    <!-- 右侧聊天区 -->
    <main class="chat-main">
      <!-- 欢迎界面 -->
      <div v-if="!currentConversationId" class="welcome-screen">
        <div class="welcome-content">
          <h1>欢迎回来，{{ username }}</h1>
          <p>月见八千代随时为您服务</p>
          <button class="start-chat-btn" @click="createNewConversation">
            开始新对话
          </button>
        </div>
      </div>

      <!-- 消息列表 -->
      <div v-else class="message-list" ref="messageListRef">
        <transition-group name="message">
          <div v-for="(msg, index) in messages" :key="index" :class="['message', msg.type]">
            <div class="message-avatar">
              <span v-if="msg.type === 'user'">{{ username.charAt(0).toUpperCase() }}</span>
              <span v-else>🌙</span>
            </div>
            <div class="message-content-wrapper">
              <div class="message-sender">{{ msg.type === 'user' ? username : '月见八千代' }}</div>
              <div class="message-content">{{ msg.content }}</div>
              <div v-if="msg.type === 'assistant'" class="message-actions">
                <button class="voice-btn" @click="playVoice(msg.content)" title="播放语音">
                  🔊
                </button>
              </div>
            </div>
          </div>
        </transition-group>

        <!-- AI 输入中动画 -->
        <div v-if="isTyping" class="typing-indicator">
          <div class="message-avatar">
            <span>🌙</span>
          </div>
          <div class="typing-bubbles">
            <span></span>
            <span></span>
            <span></span>
          </div>
        </div>
      </div>

      <!-- 输入区 -->
      <div v-if="currentConversationId" class="input-area">
        <div class="input-wrapper">
          <input v-model="inputMessage" @keyup.enter="sendMessage" placeholder="输入消息，按 Enter 发送..."
            :disabled="isLoading" ref="inputRef" />
          <button class="send-btn" @click="sendMessage" :disabled="isLoading || !inputMessage.trim()">
            <span v-if="isLoading" class="loading-spinner"></span>
            <span v-else>发送</span>
          </button>
        </div>
      </div>
    </main>
  </div>
</template>

<script setup>
import { ref, onMounted, nextTick, watch } from 'vue';
import { useRouter } from 'vue-router';
import { chatAPI } from '../services/api.js';

const router = useRouter();

const username = ref('');
const conversations = ref([]);
const currentConversationId = ref(null);
const messages = ref([]);
const inputMessage = ref('');
const isLoading = ref(false);
const isTyping = ref(false);
const isCreating = ref(false);
const messageListRef = ref(null);
const inputRef = ref(null);

onMounted(() => {
  const storedUsername = localStorage.getItem('username');
  const token = localStorage.getItem('token');

  if (!token) {
    router.push('/');
    return;
  }

  username.value = storedUsername || '用户';
  loadConversations();
});

watch(messages, () => {
  scrollToBottom();
}, { deep: true });

const scrollToBottom = () => {
  nextTick(() => {
    if (messageListRef.value) {
      messageListRef.value.scrollTop = messageListRef.value.scrollHeight;
    }
  });
};

const loadConversations = async () => {
  try {
    const result = await chatAPI.getConversationList();
    console.log('加载会话列表:', result);
    if (result.success) {
      conversations.value = result.data || [];
    } else {
      console.error('加载会话列表失败:', result.message, result.detail);
    }
  } catch (error) {
    console.error('加载会话列表失败:', error.message || error);
  }
};

const selectConversation = async (id) => {
  if (currentConversationId.value === id) return;

  currentConversationId.value = id;
  messages.value = [];

  try {
    const result = await chatAPI.getHistory(id);
    if (result.success && Array.isArray(result.data)) {
      messages.value = result.data.flatMap(item => [
        { type: 'user', content: item.user },
        { type: 'assistant', content: item.assistant }
      ]);
    } else {
      console.error('加载历史记录失败:', result.message, result.detail);
    }
  } catch (error) {
    console.error('加载历史记录失败:', error.message || error);
  }

  nextTick(() => {
    inputRef.value?.focus();
  });
};

const createNewConversation = async () => {
  if (isCreating.value) return;

  isCreating.value = true;
  try {
    const result = await chatAPI.createConversation();
    if (result.success) {
      conversations.value.unshift(result.data);
      await selectConversation(result.data);
    } else {
      console.error('创建会话失败:', result.message, result.detail);
      alert('创建会话失败: ' + result.message);
    }
  } catch (error) {
    console.error('创建会话失败:', error.message || error);
    alert('创建会话失败: ' + (error.message || '未知错误'));
  } finally {
    isCreating.value = false;
  }
};

const sendMessage = async () => {
  const message = inputMessage.value.trim();
  if (!message || isLoading.value || !currentConversationId.value) return;

  messages.value.push({ type: 'user', content: message });
  inputMessage.value = '';
  isLoading.value = true;
  isTyping.value = true;

  try {
    const result = await chatAPI.chat(message, currentConversationId.value);
    if (result.success) {
      messages.value.push({ type: 'assistant', content: result.data });
    } else {
      console.error('发送消息失败:', result.message, result.detail);
      messages.value.push({ type: 'assistant', content: '抱歉，发生了错误: ' + result.message });
    }
  } catch (error) {
    console.error('发送消息失败:', error.message || error);
    messages.value.push({ type: 'assistant', content: '抱歉，发生了错误: ' + (error.message || '未知错误') });
  } finally {
    isLoading.value = false;
    isTyping.value = false;
  }
};

const playVoice = async (text) => {
  try {
    const result = await chatAPI.speak(text);
    console.log(result);
    if (result.success && result.data) {
      // 将 Base64 字符串转换为音频 Blob
      const base64Data = result.data;
      const byteCharacters = atob(base64Data);
      const byteNumbers = new Array(byteCharacters.length);
      for (let i = 0; i < byteCharacters.length; i++) {
        byteNumbers[i] = byteCharacters.charCodeAt(i);
      }
      const byteArray = new Uint8Array(byteNumbers);
      const blob = new Blob([byteArray], { type: 'audio/mp3' });

      // 创建 Blob URL 播放音频
      const audioUrl = URL.createObjectURL(blob);
      const audio = new Audio(audioUrl);

      audio.onended = () => {
        URL.revokeObjectURL(audioUrl);
      };

      await audio.play();
    } else {
      console.error('播放语音失败:', result.message, result.detail);
      alert('播放语音失败: ' + result.message);
    }
  } catch (error) {
    console.error('播放语音失败:', error.message || error);
    alert('播放语音失败: ' + (error.message || '未知错误'));
  }
};

const logout = () => {
  localStorage.removeItem('token');
  localStorage.removeItem('username');
  router.push('/');
};
</script>

<style scoped>
.chat-container {
  display: flex;
  width: 100vw;
  height: 100vh;
  background: linear-gradient(135deg, #1a237e 0%, #0d1642 100%);
  overflow: hidden;
}

/* 左侧边栏 */
.sidebar {
  width: 280px;
  display: flex;
  flex-direction: column;
  background: rgba(255, 255, 255, 0.05);
  backdrop-filter: blur(10px);
  border-right: 1px solid rgba(255, 255, 255, 0.1);
}

.sidebar-header {
  padding: 20px;
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.sidebar-header h2 {
  color: #fff;
  font-size: 18px;
  font-weight: 500;
  margin: 0;
}

.new-chat-btn {
  display: flex;
  align-items: center;
  gap: 6px;
  padding: 8px 16px;
  background: linear-gradient(135deg, #2196F3 0%, #1976D2 100%);
  color: white;
  border: none;
  border-radius: 20px;
  cursor: pointer;
  font-size: 14px;
  transition: all 0.3s ease;
  box-shadow: 0 4px 15px rgba(33, 150, 243, 0.3);
}

.new-chat-btn:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 6px 20px rgba(33, 150, 243, 0.4);
}

.new-chat-btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.plus-icon {
  font-size: 18px;
  font-weight: bold;
}

.conversation-list {
  flex: 1;
  overflow-y: auto;
  padding: 10px;
}

.empty-conversations {
  padding: 20px;
  text-align: center;
  color: rgba(255, 255, 255, 0.5);
  font-size: 14px;
}

.conversation-item {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 12px 16px;
  margin-bottom: 8px;
  border-radius: 12px;
  cursor: pointer;
  transition: all 0.3s ease;
  background: rgba(255, 255, 255, 0.05);
  border: 1px solid transparent;
}

.conversation-item:hover {
  background: rgba(255, 255, 255, 0.1);
  border-color: rgba(255, 255, 255, 0.2);
  transform: translateX(5px);
}

.conversation-item.active {
  background: linear-gradient(135deg, rgba(33, 150, 243, 0.3) 0%, rgba(25, 118, 210, 0.3) 100%);
  border-color: rgba(33, 150, 243, 0.5);
  box-shadow: 0 4px 15px rgba(33, 150, 243, 0.2);
}

.conversation-icon {
  font-size: 18px;
}

.conversation-name {
  color: #fff;
  font-size: 14px;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

/* 会话列表动画 */
.conversation-enter-active,
.conversation-leave-active {
  transition: all 0.3s ease;
}

.conversation-enter-from {
  opacity: 0;
  transform: translateX(-20px);
}

.conversation-leave-to {
  opacity: 0;
  transform: translateX(20px);
}

/* 用户信息 */
.user-info {
  padding: 15px 20px;
  border-top: 1px solid rgba(255, 255, 255, 0.1);
  display: flex;
  align-items: center;
  gap: 12px;
}

.user-avatar {
  width: 40px;
  height: 40px;
  border-radius: 50%;
  background: linear-gradient(135deg, #2196F3 0%, #1976D2 100%);
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-weight: bold;
  font-size: 16px;
}

.username {
  flex: 1;
  color: #fff;
  font-size: 14px;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.logout-btn {
  padding: 6px 12px;
  background: rgba(244, 67, 54, 0.2);
  color: #f44336;
  border: 1px solid rgba(244, 67, 54, 0.3);
  border-radius: 6px;
  cursor: pointer;
  font-size: 12px;
  transition: all 0.3s ease;
}

.logout-btn:hover {
  background: rgba(244, 67, 54, 0.3);
  transform: scale(1.05);
}

/* 主聊天区 */
.chat-main {
  flex: 1;
  display: flex;
  flex-direction: column;
  position: relative;
}

/* 欢迎界面 */
.welcome-screen {
  flex: 1;
  display: flex;
  align-items: center;
  justify-content: center;
}

.welcome-content {
  text-align: center;
  animation: fadeInUp 0.6s ease;
}

.welcome-content h1 {
  color: #fff;
  font-size: 36px;
  margin-bottom: 10px;
  background: linear-gradient(135deg, #fff 0%, #64B5F6 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}

.welcome-content p {
  color: rgba(255, 255, 255, 0.7);
  font-size: 18px;
  margin-bottom: 30px;
}

.start-chat-btn {
  padding: 15px 40px;
  background: linear-gradient(135deg, #2196F3 0%, #1976D2 100%);
  color: white;
  border: none;
  border-radius: 30px;
  font-size: 16px;
  cursor: pointer;
  transition: all 0.3s ease;
  box-shadow: 0 4px 20px rgba(33, 150, 243, 0.4);
}

.start-chat-btn:hover {
  transform: translateY(-3px);
  box-shadow: 0 8px 30px rgba(33, 150, 243, 0.5);
}

@keyframes fadeInUp {
  from {
    opacity: 0;
    transform: translateY(30px);
  }

  to {
    opacity: 1;
    transform: translateY(0);
  }
}

/* 消息列表 */
.message-list {
  flex: 1;
  overflow-y: auto;
  padding: 20px;
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.message {
  display: flex;
  gap: 12px;
  animation: messageSlideIn 0.3s ease;
}

@keyframes messageSlideIn {
  from {
    opacity: 0;
    transform: translateY(20px);
  }

  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.message-avatar {
  width: 40px;
  height: 40px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 18px;
  flex-shrink: 0;
}

.message.user .message-avatar {
  background: linear-gradient(135deg, #2196F3 0%, #1976D2 100%);
  color: white;
  font-weight: bold;
}

.message.assistant .message-avatar {
  background: linear-gradient(135deg, #FF9800 0%, #F57C00 100%);
}

.message-content-wrapper {
  max-width: 70%;
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.message-sender {
  font-size: 12px;
  color: rgba(255, 255, 255, 0.6);
  margin-bottom: 2px;
}

.message-content {
  padding: 12px 16px;
  border-radius: 16px;
  font-size: 14px;
  line-height: 1.6;
  word-wrap: break-word;
}

.message.user .message-content {
  background: linear-gradient(135deg, #2196F3 0%, #1976D2 100%);
  color: white;
  border-bottom-right-radius: 4px;
}

.message.assistant .message-content {
  background: rgba(255, 255, 255, 0.1);
  color: #fff;
  border-bottom-left-radius: 4px;
  border: 1px solid rgba(255, 255, 255, 0.1);
}

.message-actions {
  display: flex;
  gap: 8px;
  margin-top: 4px;
}

.voice-btn {
  padding: 4px 8px;
  background: rgba(255, 255, 255, 0.1);
  border: none;
  border-radius: 6px;
  cursor: pointer;
  font-size: 14px;
  transition: all 0.3s ease;
}

.voice-btn:hover {
  background: rgba(255, 255, 255, 0.2);
  transform: scale(1.1);
}

/* 消息动画 */
.message-enter-active,
.message-leave-active {
  transition: all 0.3s ease;
}

.message-enter-from {
  opacity: 0;
  transform: translateY(20px);
}

.message-leave-to {
  opacity: 0;
  transform: translateY(-20px);
}

/* 输入中动画 */
.typing-indicator {
  display: flex;
  align-items: center;
  gap: 12px;
}

.typing-bubbles {
  display: flex;
  gap: 6px;
  padding: 12px 16px;
  background: rgba(255, 255, 255, 0.1);
  border-radius: 16px;
  border-bottom-left-radius: 4px;
}

.typing-bubbles span {
  width: 8px;
  height: 8px;
  background: rgba(255, 255, 255, 0.6);
  border-radius: 50%;
  animation: typingBounce 1.4s infinite ease-in-out both;
}

.typing-bubbles span:nth-child(1) {
  animation-delay: -0.32s;
}

.typing-bubbles span:nth-child(2) {
  animation-delay: -0.16s;
}

@keyframes typingBounce {

  0%,
  80%,
  100% {
    transform: scale(0);
    opacity: 0.5;
  }

  40% {
    transform: scale(1);
    opacity: 1;
  }
}

/* 输入区 */
.input-area {
  padding: 20px;
  border-top: 1px solid rgba(255, 255, 255, 0.1);
  background: rgba(0, 0, 0, 0.2);
}

.input-wrapper {
  display: flex;
  gap: 12px;
  max-width: 900px;
  margin: 0 auto;
}

.input-wrapper input {
  flex: 1;
  padding: 14px 20px;
  background: rgba(255, 255, 255, 0.1);
  border: 1px solid rgba(255, 255, 255, 0.2);
  border-radius: 25px;
  color: #fff;
  font-size: 14px;
  outline: none;
  transition: all 0.3s ease;
}

.input-wrapper input::placeholder {
  color: rgba(255, 255, 255, 0.4);
}

.input-wrapper input:focus {
  background: rgba(255, 255, 255, 0.15);
  border-color: rgba(33, 150, 243, 0.5);
  box-shadow: 0 0 20px rgba(33, 150, 243, 0.2);
}

.input-wrapper input:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.send-btn {
  padding: 14px 30px;
  background: linear-gradient(135deg, #2196F3 0%, #1976D2 100%);
  color: white;
  border: none;
  border-radius: 25px;
  font-size: 14px;
  cursor: pointer;
  transition: all 0.3s ease;
  display: flex;
  align-items: center;
  justify-content: center;
  min-width: 80px;
}

.send-btn:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 6px 20px rgba(33, 150, 243, 0.4);
}

.send-btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.loading-spinner {
  width: 16px;
  height: 16px;
  border: 2px solid rgba(255, 255, 255, 0.3);
  border-top-color: white;
  border-radius: 50%;
  animation: spin 0.8s linear infinite;
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}

/* 滚动条样式 */
::-webkit-scrollbar {
  width: 6px;
}

::-webkit-scrollbar-track {
  background: rgba(255, 255, 255, 0.05);
}

::-webkit-scrollbar-thumb {
  background: rgba(255, 255, 255, 0.2);
  border-radius: 3px;
}

::-webkit-scrollbar-thumb:hover {
  background: rgba(255, 255, 255, 0.3);
}

/* 响应式设计 */
@media (max-width: 768px) {
  .sidebar {
    width: 240px;
  }

  .message-content-wrapper {
    max-width: 80%;
  }

  .welcome-content h1 {
    font-size: 28px;
  }
}

@media (max-width: 480px) {
  .sidebar {
    position: absolute;
    left: -280px;
    z-index: 100;
    transition: left 0.3s ease;
  }

  .sidebar.open {
    left: 0;
  }
}
</style>
