<template>
  <div class="login-page" v-if="!isLoggedIn">
    <h2>Giriş Yap</h2>
    <form @submit.prevent="login">
      <div>
        <label>Kullanıcı Adı:</label>
        <input v-model="username" required />
      </div>
      <div>
        <label>Şifre:</label>
        <input type="password" v-model="password" required />
      </div>
      <button type="submit">Giriş Yap</button>
      <button @click="goToRegister"> Kayıt Ol </button>
    </form>

    <p v-if="errorMessage" style="color:red">{{ errorMessage }}</p>
  </div>
</template>

<script>
  import axios from 'axios';
  import router from '../router';

  export default {
    name: 'LoginView',
    data() {
      return {
        username: '',
        password: '',
        errorMessage: '',
        isLoggedIn: false
      };
    },
    mounted() {
      this.checkIfLoggedIn();
    },
    methods: {
      async login() {
        try {
          const response = await axios.post('http://localhost:5195/api/auth/login', {
            username: this.username,
            password: this.password
          });

          const { token, role, userId, username } = response.data;

          localStorage.setItem('token', token);
          localStorage.setItem('role', role);
          localStorage.setItem('userId', userId);
          localStorage.setItem('username', username);

          // role'e göre yönlendirme
          if (role === 'Seller') {
            router.push('/seller/dashboard');
          } else if (role === 'Customer') {
            router.push('/customer/dashboard');
          } else {
            router.push('/');
          }
        } catch (err) {
          this.errorMessage = 'Username or password is incorrect.';
        }
      },
      goToRegister() {
        this.$router.push('/register');
      },
      checkIfLoggedIn() {
        const token = localStorage.getItem('token');
        const role = localStorage.getItem('role');

        if (token) {
          // Kullanıcı oturumu açık, otomatik yönlendir
          if (role === 'Seller') {
            this.$router.push('/seller/dashboard');
          } else if (role === 'Customer') {
            this.$router.push('/customer/dashboard');
          }
          this.isLoggedIn = true;
        }
      }
    }
  };
</script>

<style scoped>
  .login-page {
    max-width: 400px;
    margin: 40px auto;
    padding: 20px;
    border: 1px solid #ddd;
    border-radius: 8px;
  }

    .login-page form div {
      margin-bottom: 10px;
    }
</style>
