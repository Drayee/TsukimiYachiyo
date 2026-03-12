import axios from 'axios';

// 使用相对路径，让 Vite 代理处理请求
const apiClient = axios.create({
  baseURL: '',
  headers: {
    'Content-Type': 'application/json'
  }
});

apiClient.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

apiClient.interceptors.response.use(
  (response) => {
    const responseData = response.data;
    if (responseData && responseData.code === "200") {
      return {
        success: true,
        code: responseData.code,
        message: responseData.message,
        data: responseData.data,
        detail: responseData.detail
      };
    }
    return Promise.reject({
      success: false,
      code: responseData?.code || 400,
      message: responseData?.message || '请求失败',
      data: responseData?.data || null,
      detail: responseData?.detail || null
    });
  },
  (error) => {
    if (error.response) {
      const responseData = error.response.data;
      return Promise.reject({
        success: false,
        code: responseData?.code || error.response.status,
        message: responseData?.message || '请求失败',
        data: responseData?.data || null,
        detail: responseData?.detail || error.message
      });
    }
    return Promise.reject({
      success: false,
      code: 500,
      message: '网络错误',
      data: null,
      detail: '请检查网络连接'
    });
  }
);

export const chatAPI = {
  chat(message, conservationId) {
    return apiClient.post('/api/v2/ai/chat', {
      message,
      conservationId: String(conservationId)
    });
  },

  createConversation() {
    return apiClient.post('/api/v2/ai/create');
  },

  getHistory(conversationId) {
    return apiClient.get(`/api/v2/history/${conversationId}`);
  },

  getConversationList() {
    return apiClient.get('/api/v2/history/list');
  },

  speak(text) {
    return apiClient.post('/api/v2/ai/speak', { text: text }, {

    });
  }
};

export default apiClient;
