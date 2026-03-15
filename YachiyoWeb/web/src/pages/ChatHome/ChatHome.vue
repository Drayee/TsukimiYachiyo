<script setup>
import { useChatHome } from '../../composables/useChatHome.js';
import UserProfilePopover from '../../components/UserProfilePopover/UserProfilePopover.vue';
import Live2DModel from '../../components/Live2DModel/Live2DModel.vue';

const {
  username,
  userAvatar,
  conversations,
  currentConversationId,
  messages,
  inputMessage,
  isLoading,
  isTyping,
  isCreating,
  messageListRef,
  inputRef,
  isSidebarOpen,
  sidebarRef,
  isVoiceClickable,
  getVoiceStatus,
  toggleSidebar,
  closeSidebar,
  onTouchStart,
  onTouchEnd,
  selectConversation,
  createNewConversation,
  sendMessage,
  playVoice,
  logout
} = useChatHome();
</script>

<template src="./templates/ChatHome.html"></template>

<style scoped src="./styles/ChatHome.css"></style>
