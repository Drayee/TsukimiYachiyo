import { ref } from 'vue';
import { userAPI } from '../services/api.js';

export function useUserProfile() {
  const username = ref('');
  const userAvatar = ref('');

  const loadUserDetail = async () => {
    try {
      const [detailResult, avatarResult] = await Promise.all([
        userAPI.getUserDetail(),
        userAPI.getAvatar()
      ]);

      if (detailResult.success && detailResult.data.userName) {
        username.value = detailResult.data.userName;
      }

      if (avatarResult.success && avatarResult.data) {
        const avatarData = avatarResult.data;
        if (avatarData.startsWith('data:')) {
          userAvatar.value = avatarData;
        } else {
          userAvatar.value = `data:image/png;base64,${avatarData}`;
        }
      }
    } catch (error) {
      console.error('加载用户详情失败:', error);
    }
  };

  return {
    username,
    userAvatar,
    loadUserDetail
  };
}
