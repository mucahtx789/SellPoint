import { createApp } from 'vue';
import App from './App.vue';
import router from './router'; // router.js'den import ediyoruz
import axios from 'axios';

axios.interceptors.request.use(config => {
  const token = localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

const app = createApp(App);


// Vue Router'ı uygulamaya dahil et
app.use(router);


app.mount('#app');
