<script setup>
import { useLogin } from '../../composables/useLogin.js';

const {
  introVideo,
  cycleVideo,
  showForm,
  isLoading,
  error,
  loginSuccess,
  form,
  onIntroEnd,
  handleSubmit
} = useLogin();
</script>

<template src="./templates/Login.html"></template>

<style scoped src="./styles/Login.css"></style>
