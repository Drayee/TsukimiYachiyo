<template>
  <div class="chat-container">
    <div class="chat-header">
      <h1>月见八千代</h1>
      <button @click="logout" class="logout-btn">退出登录</button>
    </div>

    <div class="chat-messages">
      <div v-for="(message, index) in messages" :key="index" :class="['message', message.type]">
        <div class="message-content">{{ message.content }}</div>
      </div>
    </div>

    <div class="chat-input">
      <input
        type="text"
        v-model="inputMessage"
        placeholder="输入消息..."
        @keyup.enter="sendMessage"
      >
      <button @click="sendMessage" class="send-btn">发送</button>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { chatAPI } from '../services/api.js';

const router = useRouter();
const messages = ref([]);
const inputMessage = ref('');
const conservationId = ref(null);

onMounted(async () => {
  const token = localStorage.getItem('token');
  if (!token) {
    router.push('/');
    return;
  }

  await createConversation();
});

const createConversation = async () => {
  try {
    const result = await chatAPI.createConversation();
    if (result.success) {
      conservationId.value = result.data;
    } else {
      console.error('创建会话失败:', result.message, result.detail);
    }
  } catch (error) {
    console.error('创建会话失败:', error.message || error);
  }
};

const sendMessage = async () => {
  const message = inputMessage.value.trim();
  if (!message || !conservationId.value) return;

  messages.value.push({ type: 'user', content: message });
  inputMessage.value = '';

  try {
    const result = await chatAPI.chat(message, conservationId.value);
    if (result.success) {
      messages.value.push({ type: 'assistant', content: result.data });
    } else {
      console.error('发送消息失败:', result.message, result.detail);
      messages.value.push({ type: 'assistant', content: '抱歉，发生了错误: ' + result.message });
    }
  } catch (error) {
    console.error('发送消息失败:', error.message || error);
    messages.value.push({ type: 'assistant', content: '抱歉，发生了错误: ' + (error.message || '未知错误') });
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
  width: 100vw;
  height: 100vh;
  display: flex;
  flex-direction: column;
  background: linear-gradient(135deg, #1a237e 0%, #0d1642 100%);
}

.chat-header {
  padding: 20px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);
}

.chat-header h1 {
  color: #fff;
  margin: 0;
  font-size: 24px;
}

.logout-btn {
  padding: 8px 16px;
  background: rgba(244, 67, 54, 0.2);
  color: #f44336;
  border: 1px solid rgba(244, 67, 54, 0.3);
  border-radius: 6px;
  cursor: pointer;
  transition: all 0.3s ease;
}

.logout-btn:hover {
  background: rgba(244, 67, 54, 0.3);
}

.chat-messages {
  flex: 1;
  overflow-y: auto;
  padding: 20px;
  display: flex;
  flex-direction: column;
  gap: 15px;
}

.message {
  max-width: 70%;
  padding: 12px 16px;
  border-radius: 12px;
  animation: messageSlideIn 0.3s ease;
}

@keyframes messageSlideIn {
  from {
    opacity: 0;
    transform: translateY(10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.message.user {
  align-self: flex-end;
  background: linear-gradient(135deg, #2196F3 0%, #1976D2 100%);
  color: white;
  border-bottom-right-radius: 4px;
}

.message.assistant {
  align-self: flex-start;
  background: rgba(255, 255, 255, 0.1);
  color: #fff;
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-bottom-left-radius: 4px;
}

.chat-input {
  padding: 20px;
  display: flex;
  gap: 10px;
  border-top: 1px solid rgba(255, 255, 255, 0.1);
  background: rgba(0, 0, 0, 0.2);
}

.chat-input input {
  flex: 1;
  padding: 12px 20px;
  background: rgba(255, 255, 255, 0.1);
  border: 1px solid rgba(255, 255, 255, 0.2);
  border-radius: 25px;
  color: #fff;
  font-size: 14px;
  outline: none;
}

.chat-input input::placeholder {
  color: rgba(255, 255, 255, 0.4);
}

.send-btn {
  padding: 12px 24px;
  background: linear-gradient(135deg, #2196F3 0%, #1976D2 100%);
  color: white;
  border: none;
  border-radius: 25px;
  cursor: pointer;
  transition: all 0.3s ease;
}

.send-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 15px rgba(33, 150, 243, 0.4);
}
</style>
