<template>
  <nav>
    <!-- Giriş yapılmışsa çıkış butonunu göster -->
    <button v-if="isLoggedIn" @click="logout" class="logout-btn">Çıkış Yap</button>
  </nav>
  <router-view />
</template>

<script>
  export default {
    data() {
      return {
        isLoggedIn: false
      };
    },
    created() {
      this.checkLoginStatus(); // Sayfa ilk yüklendiğinde kontrol
    },
    mounted() {
      // Sayfa geçişlerinden sonra login durumu kontrol edilir
      this.$router.afterEach(() => {
        this.checkLoginStatus();
      });
    },
    methods: {
      checkLoginStatus() {
        this.isLoggedIn = localStorage.getItem('token') !== null;
      },
      logout() {
        localStorage.removeItem('token');
        this.isLoggedIn = false;
        this.$router.push('/');
      }
    }
  };
</script>
