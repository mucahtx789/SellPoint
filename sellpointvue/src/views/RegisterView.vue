<template>
  <div class="register-page">
    <div class="register-box">
      <h2>Kayıt Ol</h2>
      <form @submit.prevent="register">
        <div class="form-group">
          <label>Kullanıcı Adı:</label>
          <input v-model="username" type="text" required autocomplete="username" />
        </div>
        <div class="form-group">
          <label>Şifre:</label>
          <input v-model="password" type="password" required autocomplete="new-password" />
        </div>
        <div class="form-group">
          <label>Rol:</label>
          <select v-model="role" required>
            <option disabled value="">Rol seçin</option>
            <option value="Customer">Müşteri</option>
            <option value="Seller">Satıcı</option>
          </select>
        </div>
        <button type="submit" class="btn-primary">Kayıt Ol</button>
        <button type="button" class="btn-secondary" @click="$router.push('/login')">Oturum Aç</button>
      </form>

      <p v-if="errorMessage" class="error-message">{{ errorMessage }}</p>
      <p v-if="successMessage" class="success-message">{{ successMessage }}</p>
    </div>
  </div>
</template>

<script>
  import axios from 'axios';

  export default {
    name: 'RegisterView',
    data() {
      return {
        username: '',
        password: '',
        role: '',
        errorMessage: '',
        successMessage: ''
      };
    },
    methods: {
      async register() {
        this.errorMessage = '';
        this.successMessage = '';

        try {
          const response = await axios.post('http://localhost:5195/api/auth/register', {
            username: this.username,
            password: this.password,
            role: this.role
          });

          this.successMessage = 'Kayıt başarılı. Giriş yapabilirsiniz.';
          this.username = '';
          this.password = '';
          this.role = '';
          this.$router.push('/login');
        } catch (err) {
          if (err.response && err.response.data) {
            this.errorMessage = err.response.data.message || 'Kayıt başarısız.';
          } else {
            this.errorMessage = 'Bir hata oluştu.';
          }
        }
      }
    }
  };
</script>

<style scoped>
  .register-page {
    min-height: 100vh;
    display: flex;
    justify-content: center;
    align-items: center;
    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
    padding: 20px;
  }

  .register-box {
    background: #fff;
    padding: 30px 40px;
    border-radius: 12px;
    box-shadow: 0 10px 25px rgba(0, 0, 0, 0.15);
    width: 100%;
    max-width: 400px;
    text-align: center;
  }

    .register-box h2 {
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

    .form-group input,
    .form-group select {
      width: 100%;
      padding: 10px 12px;
      border: 1.5px solid #ccc;
      border-radius: 6px;
      font-size: 1rem;
      transition: border-color 0.3s ease;
    }

      .form-group input:focus,
      .form-group select:focus {
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

  .success-message {
    margin-top: 15px;
    color: #28a745;
    font-weight: 600;
    font-size: 0.9rem;
  }
</style>
