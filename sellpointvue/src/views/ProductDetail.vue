<template>
  <div class="p-6 max-w-2xl mx-auto border rounded bg-white shadow">
    <img :src="product.imageUrl" alt="Ürün Resmi" class="product-image" />
    <h1 class="text-2xl font-bold mb-2">{{ product.name }}</h1>
    <p class="mb-2 text-gray-700">{{ product.description }}</p>
    <p><strong>Kategori:</strong> {{ product.category }}</p>
    <p><strong>Fiyat:</strong> ₺{{ product.price }}</p>
    <p><strong>Stok:</strong> {{ product.stock }}</p>

    <div class="flex items-center space-x-2">
      <button @click="decreaseQuantity(product)" :disabled="product.selectedQuantity <= 0" class="bg-gray-300 p-2 rounded">-</button>
      <input v-model.number="product.selectedQuantity" type="number" min="0" :max="product.stock" @input="validateQuantity(product)" class="w-12 text-center border rounded" />
      <button @click="increaseQuantity(product)" :disabled="product.selectedQuantity >= product.stock" class="bg-gray-300 p-2 rounded">+</button>
    </div>

    <div class="flex space-x-2 mt-2">
      <button @click="addToCart(product)" class="bg-blue-500 text-white px-4 py-2 rounded">Sepete Ekle</button>
      <button @click="buyNow(product)" class="bg-green-500 text-white px-4 py-2 rounded">Satın Al</button>
      <router-link to="/customer/dashboard" class="inline-block mt-4 text-blue-600 underline">← Geri Dön</router-link>
    </div>

  </div>
</template>

<script>
  import axios from 'axios';

  export default {
    name: 'ProductDetail',
    data() {
      return {
        product: {},
        cart: []
      };
    },
    mounted() {
      this.loadProduct();
    },
    methods: {
      async loadProduct() {
        const productId = this.$route.params.id;
        const token = localStorage.getItem('token');
        try {
          const res = await axios.get(`http://localhost:5195/api/products/${productId}`, {
            headers: { Authorization: `Bearer ${token}` }
          });
          this.product = res.data;
          // selectedQuantity alanını elle ekle
          this.product.selectedQuantity = 0; 
        } catch (err) {
          console.error('Ürün detay alınamadı:', err);
        }
      },

      validateQuantity() {
        if (this.product.selectedQuantity > this.product.stock) {
          this.product.selectedQuantity = this.product.stock;
        } else if (this.product.selectedQuantity < 0 || isNaN(this.product.selectedQuantity)) {
          this.product.selectedQuantity = 0;
        }
      },

      increaseQuantity() {
        if (this.product.selectedQuantity < this.product.stock) {
          this.product.selectedQuantity++;
        }
      },

      decreaseQuantity() {
        if (this.product.selectedQuantity > 0) {
          this.product.selectedQuantity--;
        }
      },

      async addToCart() {
        if (this.product.selectedQuantity === 0) return;

        const token = localStorage.getItem('token');
        try {
          await axios.post('http://localhost:5195/api/cart/add', {
            productId: this.product.id,
            quantity: this.product.selectedQuantity,
            customerId: localStorage.getItem('userId')
          }, {
            headers: { Authorization: `Bearer ${token}` }
          });

          this.product.stock -= this.product.selectedQuantity;
          this.product.selectedQuantity = 0;
          
        } catch (err) {
          console.error("Sepete ekleme hatası:", err);
          alert("Sepete eklenirken bir hata oluştu.");
        }
      },

      buyNow() {
        alert(`Satın alma işlemi başlatıldı: ${this.product.name}`);
      }
    }
  };
</script>


<style scoped>
  .product-image {
    width: 500px;
    height: 500px;
    object-fit: cover;
    border: 1px solid #ccc;
    margin-bottom: 12px;
  }
</style>
