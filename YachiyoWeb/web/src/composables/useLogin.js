import { ref, onMounted } from 'vue';
import axios from 'axios';
import { useRouter } from 'vue-router';
import { useAuth } from './useAuth.js';

export function useLogin() {
  const router = useRouter();
  const { login } = useAuth();

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
    setTimeout(() => {
      if (introVideo.value) {
        introVideo.value.style.display = 'none';
      }
      if (cycleVideo.value) {
        cycleVideo.value.style.display = 'block';
      }
    }, 3000);
    
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
      const response = await axios.post('/api/v1/auth/login', {
        username: form.value.username,
        password: form.value.password
      });
      
      if (response.data && response.data.code === '200') {
        login(response.data.data, form.value.username);
        loginSuccess.value = true;
        setTimeout(() => {
          router.push('/home');
        }, 1000);
      } else {
        const errorCode = response.data.code;
        if (errorCode === '400.1') {
          try {
            const registerResponse = await axios.post('/api/v1/auth/register', {
              username: form.value.username,
              password: form.value.password
            });

            if (registerResponse.data && registerResponse.data.code === '200') {
              login(registerResponse.data.data, form.value.username);
              loginSuccess.value = true;
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
          error.value = '密码错误，请重新输入';
        } else {
          error.value = response.data?.message || '操作失败，请重试';
        }
      }
    } catch (err) {
      console.error('[Login] 登录失败:', err);
      
      if (err.response && err.response.data) {
        const errorCode = err.response.data.code;
        const errorMessage = err.response.data.message;
      } else {
        error.value = '网络错误，请检查网络连接';
      }
    } finally {
      isLoading.value = false;
    }
  };

  return {
    introVideo,
    cycleVideo,
    showForm,
    isLoading,
    error,
    loginSuccess,
    form,
    onIntroEnd,
    handleSubmit
  };
}
