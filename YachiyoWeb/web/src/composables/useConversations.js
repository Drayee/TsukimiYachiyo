import { ref } from 'vue';
import { chatAPI } from '../services/api.js';

export function useConversations() {
  const conversations = ref([]);
  const currentConversationId = ref(null);
  const isCreating = ref(false);
  const isLoading = ref(false);

  const loadConversations = async () => {
    try {
      const result = await chatAPI.getConversationList();
      if (result.success) {
        conversations.value = result.data || [];
      }
    } catch (error) {
      console.error('加载会话列表失败:', error);
    }
  };

  const createNewConversation = async () => {
    if (isCreating.value) return null;
    isCreating.value = true;
    try {
      const result = await chatAPI.createConversation();
      if (result.success) {
        const newConversation = {
          id: result.data,
          title: '新对话'
        };
        conversations.value.unshift(newConversation);
        return newConversation;
      }
    } catch (error) {
      console.error('创建会话失败:', error);
    } finally {
      isCreating.value = false;
    }
    return null;
  };

  const selectConversation = (id) => {
    currentConversationId.value = id;
  };

  return {
    conversations,
    currentConversationId,
    isCreating,
    isLoading,
    loadConversations,
    createNewConversation,
    selectConversation
  };
}
