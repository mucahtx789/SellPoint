<template>
  <div class="register-page">
    <h2>Kayıt Ol</h2>
    <form @submit.prevent="register">
      <div>
        <label>Kullanıcı Adı:</label>
        <input v-model="username" type="text" required />
      </div>
      <div>
        <label>Şifre:</label>
        <input v-model="password" type="password" required />
      </div>
      <div>
        <label>Rol:</label>
        <select v-model="role" required>
          <option disabled value="">Rol seçin</option>
          <option value="Customer">Müşteri</option>
          <option value="Seller">Satıcı</option>
        </select>
      </div>
      <button type="submit">Kayıt Ol</button>
      <router-link to="/login"><button>Oturum Aç</button></router-link>
    </form>

    <p v-if="errorMessage" style="color:red">{{ errorMessage }}</p>
    <p v-if="successMessage" style="color:green">{{ successMessage }}</p>
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
    max-width: 400px;
    margin: 40px auto;
    padding: 20px;
    border: 1px solid #ddd;
    border-radius: 8px;
  }

    .register-page form div {
      margin-bottom: 10px;
    }

    .register-page input,
    .register-page select {
      width: 100%;
      padding: 8px;
    }
</style>
