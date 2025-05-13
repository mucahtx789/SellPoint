<template>
  <div class="max-w-xl mx-auto p-4">
    <h2 class="text-2xl font-bold mb-4">Ürün Ekle</h2>
    <form @submit.prevent="submitProduct">
      <label class="label">Ürün Adı</label>
      <input v-model="product.name" placeholder="Ürün Adı" class="input" required />

      <label class="label">Açıklama</label>
      <textarea v-model="product.description" placeholder="Açıklama" class="input" required></textarea>

      <label class="label">Kategori</label>
      <select v-model="product.category" class="input" required>
        <option value="Elektronik">Elektronik</option>
        <option value="Giyim">Giyim</option>
        <option value="Gıda">Gıda</option>
        <option value="Kırtasiye">Kırtasiye</option>
      </select>

      <label class="label">Fiyat</label>
      <input v-model.number="product.price" type="number" step="0.01" placeholder="Fiyat" class="input" required />

      <label class="label">Adet</label>
      <input v-model.number="product.stock" type="number" placeholder="Adet" class="input" required />

      <label class="label">İnternetten Resim URL’si</label>
      <input v-model="product.imageUrl" placeholder="https://example.com/resim.jpg" class="input" required />

      <div class="button-group">
        <button type="submit" class="btn">Ürün Ekle</button>
        <button type="button" class="btn secondary" @click="$router.push('/seller/dashboard')">Dashboard’a Git</button>
      </div>
    </form>
  </div>
</template>

<script>
  import axios from 'axios';

  export default {
    data() {
      return {
        product: {
          name: '',
          description: '',
          category: 'Elektronik',
          price: 0,
          stock: 0, // 
          imageUrl: ''
        }
      };
    },
    methods: {
      async submitProduct() {
        try {
          const token = localStorage.getItem('token');
          const userId = localStorage.getItem('userId'); // Oturum açan kullanıcının ID'sini alıyoruz
          const productData = {
            ...this.product,
            sellerId: userId // sellerId'yi ekliyoruz
          };

          const response = await axios.post('http://localhost:5195/api/products', productData, {
            headers: { Authorization: `Bearer ${token}` }
          });
          alert('Ürün başarıyla eklendi!');
          this.$router.push('/seller/dashboard');
        } catch (error) {
          alert('Ürün eklenirken hata oluştu.');
          console.error(error);
        }
      }
    }
  };
</script>

<style scoped>
  .input {
    display: block;
    width: 100%;
    padding: 8px;
    margin-bottom: 16px;
    border: 1px solid #ccc;
    border-radius: 6px;
  }

  .label {
    font-weight: 600;
    margin-bottom: 4px;
    display: block;
  }

  .button-group {
    display: flex;
    gap: 12px;
  }

  .btn {
    background-color: #4caf50;
    color: white;
    padding: 10px 20px;
    border: none;
    border-radius: 6px;
    cursor: pointer;
  }

    .btn.secondary {
      background-color: #2196f3;
    }
</style>
