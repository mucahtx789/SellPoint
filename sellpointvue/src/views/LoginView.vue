<template>
  <div class="login-page" v-if="!isLoggedIn">
    <div class="login-box">
      <h2>Giriş Yap</h2>
      <form @submit.prevent="login">
        <div class="form-group">
          <label>Kullanıcı Adı:</label>
          <input v-model="username" required autocomplete="username" />
        </div>
        <div class="form-group">
          <label>Şifre:</label>
          <input type="password" v-model="password" required autocomplete="current-password" />
        </div>
        <button type="submit" class="btn-primary">Giriş Yap</button>
        <button type="button" class="btn-secondary" @click="goToRegister">Kayıt Ol</button>
      </form>
      <p v-if="errorMessage" class="error-message">{{ errorMessage }}</p>
    </div>
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
    min-height: 100vh;
    display: flex;
    justify-content: center;
    align-items: center;
    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
    padding: 20px;
  }

  .login-box {
    background: #fff;
    padding: 30px 40px;
    border-radius: 12px;
    box-shadow: 0 10px 25px rgba(0, 0, 0, 0.15);
    width: 100%;
    max-width: 400px;
    text-align: center;
  }

    .login-box h2 {
      margin-bottom: 25px;
      color: #333;
      font-weight: 700;
    }

  .form-group {
    margin-bottom: 20px;
    text-align: left;
  }

    .form-group label {
      display: block;
      margin-bottom: 8px;
      color: #555;
      font-weight: 600;
      font-size: 0.9rem;
    }

    .form-group input {
      width: 100%;
      padding: 10px 12px;
      border: 1.5px solid #ccc;
      border-radius: 6px;
      font-size: 1rem;
      transition: border-color 0.3s ease;
    }

      .form-group input:focus {
        outline: none;
        border-color: #5a9bd4;
        box-shadow: 0 0 5px rgba(90, 155, 212, 0.5);
      }

  button {
    cursor: pointer;
    font-weight: 600;
    font-size: 1rem;
    border: none;
    border-radius: 6px;
    padding: 12px 20px;
    margin: 5px 8px 0 0;
    transition: background-color 0.3s ease;
    min-width: 110px;
  }

  .btn-primary {
    background-color: #4a90e2;
    color: white;
  }

    .btn-primary:hover {
      background-color: #357abd;
    }

  .btn-secondary {
    background-color: #e0e0e0;
    color: #555;
  }

    .btn-secondary:hover {
      background-color: #c6c6c6;
    }

  .error-message {
    margin-top: 15px;
    color: #d9534f;
    font-weight: 600;
    font-size: 0.9rem;
  }
</style>
