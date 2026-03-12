<template>
  <div class="login-container">
    <!-- 初始动画 -->
    <video 
      ref="introVideo" 
      class="intro-video" 
      autoplay 
      muted 
      @ended="onIntroEnd"
    >
      <source src="/resource/login_show.mp4" type="video/mp4">
      您的浏览器不支持视频播放。
    </video>
    
    <!-- 循环背景动画 -->
    <video 
      ref="cycleVideo" 
      class="cycle-video" 
      autoplay 
      muted 
      loop 
      style="display: none;"
    >
      <source src="/resource/login_show_cycle.mp4" type="video/mp4">
      您的浏览器不支持视频播放。
    </video>
    
    <!-- 登录表单 -->
    <div class="login-form" :class="{ 'fade-in': showForm }">
      
      <form @submit.prevent="handleSubmit">
        <div class="form-group">
          <label for="username">用户名</label>
          <input 
            type="text" 
            id="username" 
            v-model="form.username" 
            placeholder="请输入用户名"
            required
          >
        </div>
        
        <div class="form-group">
          <label for="password">密码</label>
          <input 
            type="password" 
            id="password" 
            v-model="form.password" 
            placeholder="请输入密码"
            required
          >
        </div>
        
        <div class="form-actions">
          <button type="submit" class="login-btn" :disabled="isLoading">
            {{ isLoading ? '登录中...' : '登录' }}
          </button>
        </div>
        
        <div v-if="error" class="error-message">
          {{ error }}
        </div>
      </form>
    </div>
    
    <!-- 登录成功背景过渡 -->
    <div class="success-overlay" :class="{ 'show': loginSuccess }"></div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import axios from 'axios';
import { useRouter } from 'vue-router';

const router = useRouter();
const introVideo = ref(null);
const cycleVideo = ref(null);
const showForm = ref(false);
const isLoading = ref(false);
const error = ref('');
const loginSuccess = ref(false);

const form = ref({
  username: '',
  password: ''
});

onMounted(() => {
  // 3秒后隐藏初始动画，显示循环动画
  setTimeout(() => {
    if (introVideo.value) {
      introVideo.value.style.display = 'none';
    }
    if (cycleVideo.value) {
      cycleVideo.value.style.display = 'block';
    }
  }, 3000);
  
  // 2秒后显示登录表单
  setTimeout(() => {
    showForm.value = true;
  }, 2000);
});

const onIntroEnd = () => {
  if (introVideo.value) {
    introVideo.value.style.display = 'none';
  }
  if (cycleVideo.value) {
    cycleVideo.value.style.display = 'block';
  }
};

const handleSubmit = async () => {
  error.value = '';
  isLoading.value = true;
  
  try {
    console.log('[Login] 开始登录流程');
    console.log('[Login] 发送请求到: /api/v1/auth/login');
    console.log('[Login] 请求数据:', { username: form.value.username });
    
    const response = await axios.post('/api/v1/auth/login', {
      username: form.value.username,
      password: form.value.password
    });
    
    console.log('[Login] 收到响应:', response.data);
    
    if (response.data && response.data.code === '200') {
      console.log('[Login] 登录成功');
      // 存储JWT令牌
      localStorage.setItem('token', response.data.data);
      localStorage.setItem('username', form.value.username);
      
      // 登录成功，触发背景过渡效果
      loginSuccess.value = true;
      
      // 1秒后跳转到主页
      setTimeout(() => {
        router.push('/home');
      }, 1000);
    } else {
      const errorCode = response.data.code;
      // 根据错误码处理不同情况
      if (errorCode === '400.1') {
        // 用户名不存在，自动创建用户
        console.log('[Login] 用户名不存在，准备创建用户');
        try {
          const registerResponse = await axios.post('/api/v1/auth/register', {
            username: form.value.username,
            password: form.value.password
          });

          console.log('[Login] 注册响应:', registerResponse.data);

          if (registerResponse.data && registerResponse.data.code === '200') {
            console.log('[Login] 用户创建成功，自动登录');
            // 存储JWT令牌
            localStorage.setItem('token', registerResponse.data.data);
            localStorage.setItem('username', form.value.username);

            // 登录成功，触发背景过渡效果
            loginSuccess.value = true;

            // 1秒后跳转到主页
            setTimeout(() => {
              router.push('/home');
            }, 1000);
          } else {
            error.value = registerResponse.data?.message || '创建用户失败';
          }
        } catch (registerErr) {
          console.error('[Login] 创建用户失败:', registerErr);
          error.value = '创建用户失败，请重试';
        }
      } else if (errorCode === '400.2') {
        // 密码错误
        console.log('[Login] 密码错误');
        error.value = '密码错误，请重新输入';
        error.value = response.data?.message || '操作失败，请重试';
      } else {
        // 其他错误
        error.value = errorMessage || err.response.data.detail || '操作失败，请重试';
      }
    }
  } catch (err) {
    console.error('[Login] 登录失败:', err);
    
    if (err.response && err.response.data) {
      const errorCode = err.response.data.code;
      const errorMessage = err.response.data.message;
      
      console.log('[Login] 错误码:', errorCode, '错误信息:', errorMessage);

    } else {
      error.value = '网络错误，请检查网络连接';
    }
  } finally {
    isLoading.value = false;
    console.log('[Login] 登录流程结束');
  }
};
</script>

<style scoped>
.login-container {
  position: relative;
  width: 100vw;
  height: 100vh;
  overflow: hidden;
  display: flex;
  justify-content: center;
  align-items: flex-start;
  padding-top: 10vh;
}

.intro-video,
.cycle-video {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  object-fit: cover;
  z-index: 0;
}

.login-form {
  position: relative;
  z-index: 1;
  background: rgba(255, 255, 255, 0.7);
  padding: 40px;
  border-radius: 10px;
  box-shadow: 0 0 20px rgba(0, 0, 0, 0.2);
  width: 90%;
  max-width: 400px;
  opacity: 0;
  transition: opacity 0.5s ease-in-out;
}

.login-form.fade-in {
  opacity: 1;
}

.login-form h2 {
  margin-top: 0;
  text-align: center;
  color: #333;
  font-size: 24px;
  margin-bottom: 10px;
}

.subtitle {
  text-align: center;
  color: #666;
  margin-bottom: 30px;
}

.form-group {
  margin-bottom: 20px;
}

.form-group label {
  display: block;
  margin-bottom: 5px;
  color: #333;
  font-weight: 500;
}

.form-group input {
  width: 100%;
  padding: 10px;
  border: 1px solid #ddd;
  border-radius: 5px;
  font-size: 16px;
}

.login-btn {
  padding: 10px 20px;
  border: none;
  border-radius: 5px;
  font-size: 16px;
  cursor: pointer;
  transition: all 0.3s ease;
  background-color: #64B5F6;
  color: white;
  width: 100%;
}

.login-btn:hover {
  background-color: #42A5F5;
}

.login-btn:disabled {
  background-color: #cccccc;
  cursor: not-allowed;
}

.error-message {
  margin-top: 20px;
  padding: 10px;
  background-color: #ffebee;
  color: #c62828;
  border-radius: 5px;
  text-align: center;
}

.success-overlay {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: black;
  opacity: 0;
  transition: opacity 1s ease-in-out;
  z-index: 2;
  pointer-events: none;
}

.success-overlay.show {
  opacity: 1;
  pointer-events: auto;
}

@media (max-width: 768px) {
  .login-form {
    padding: 30px;
  }
}
</style>
