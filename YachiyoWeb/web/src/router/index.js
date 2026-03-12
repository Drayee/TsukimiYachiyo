import { createRouter, createWebHistory } from 'vue-router';
import Login from '../components/Login.vue';

const routes = [
  {
    path: '/',
    name: 'Login',
    component: Login
  },
  {
    path: '/home',
    name: 'Home',
    // 懒加载聊天主页组件
    component: () => import('../views/ChatHome.vue'),
    // 路由守卫，需要登录才能访问
    meta: {
      requiresAuth: true
    }
  }
];

const router = createRouter({
  history: createWebHistory(),
  routes
});

// 路由守卫
router.beforeEach((to, from, next) => {
  // 检查路由是否需要认证
  if (to.matched.some(record => record.meta.requiresAuth)) {
    // 检查是否存在token
    const token = localStorage.getItem('token');
    if (!token) {
      // 没有token，重定向到登录页
      next({ name: 'Login' });
    } else {
      // 有token，继续访问
      next();
    }
  } else {
    // 不需要认证的路由，直接访问
    next();
  }
});

export default router;
