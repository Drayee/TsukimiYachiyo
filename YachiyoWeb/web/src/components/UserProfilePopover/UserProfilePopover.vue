<script setup>
import { useUserProfilePopover } from '../../composables/useUserProfilePopover.js';

const props = defineProps({
  username: {
    type: String,
    default: '用户'
  },
  userAvatar: {
    type: String,
    default: ''
  }
});

const emit = defineEmits(['go-settings']);

const {
  isVisible,
  popoverRef,
  showPopover,
  hidePopover,
  keepVisible,
  goToSettings
} = useUserProfilePopover(props, emit);
</script>

<template src="./templates/UserProfilePopover.html"></template>

<style scoped src="./styles/UserProfilePopover.css"></style>
