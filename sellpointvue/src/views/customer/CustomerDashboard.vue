<template>
  <div class="p-6">
    <!-- Başlık ve kategori filtresi -->
    <div class="mb-6">
      <h1 class="text-2xl font-bold mb-4">Ürünler</h1>
      <div class="flex space-x-2">
        <button v-for="category in categories"
                :key="category"
                @click="selectedCategory = category"
                :class="['px-4 py-2 rounded border', selectedCategory === category ? 'bg-blue-500 text-white' : 'bg-white text-black']">
          {{ category === '' ? 'Tümü' : category }}
        </button>
      </div>
    </div>

    <!-- Ürün listesi -->
    <div class="product-grid">
      <div v-for="product in filteredProducts" :key="product.id" class="product-card">
        <img :src="product.imageUrl" alt="Ürün Resmi" class="product-image" />
        <h2 class="font-bold text-lg mb-1">{{ product.name }}</h2>
        <p class="text-sm text-gray-700 mb-1">{{ product.description }}</p>
        <p class="text-sm mb-1"><strong>Kategori:</strong> {{ product.category }}</p>
        <p class="text-sm mb-1"><strong>Fiyat:</strong> ₺{{ product.price.toFixed(2) }}</p>
        <p class="text-sm mb-2"><strong>Stok:</strong> {{ product.stock }}</p>

        <div class="flex items-center space-x-2">
          <button @click="decreaseQuantity(product)" :disabled="product.selectedQuantity <= 0" class="bg-gray-300 p-2 rounded">-</button>
          <input v-model.number="product.selectedQuantity" type="number" min="0" :max="product.stock" @input="validateQuantity(product)" class="w-12 text-center border rounded" />
          <button @click="increaseQuantity(product)" :disabled="product.selectedQuantity >= product.stock" class="bg-gray-300 p-2 rounded">+</button>
        </div>

        <div class="flex space-x-2 mt-2">
          <button @click="addToCart(product)" class="bg-blue-500 text-white px-4 py-2 rounded">Sepete Ekle</button>
          <button @click="buyNow(product)" class="bg-green-500 text-white px-4 py-2 rounded">Satın Al</button>
          <router-link :to="`/urun/${encodeURIComponent(product.name)}/${product.id}`" class="bg-yellow-500 text-white px-4 py-2 rounded">
            İncele
          </router-link>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
  import axios from 'axios';
  import * as signalR from '@microsoft/signalr'; // signalr

  export default {
    name: 'CustomerDashboard',
    data() {
      return {
        products: [],
        selectedCategory: '',
        categories: ['', 'Elektronik', 'Gıda', 'Giyim', 'Kırtasiye'],
        cart: [],
        connection: null
      };
    },
    computed: {
      filteredProducts() {
        if (this.selectedCategory === '') return this.products;
        return this.products.filter(p => p.category === this.selectedCategory);
      },
    },
    methods: {
      async addToCart(product) {
        if (product.selectedQuantity === 0) return;

        const existing = this.cart.find(p => p.productId === product.id);
        if (existing) {
          const total = existing.quantity + product.selectedQuantity;
          if (total <= existing.maxStock) {
            existing.quantity += product.selectedQuantity;
            product.stock -= product.selectedQuantity;
          }
        } else {
          this.cart.push({
            productId: product.id,
            name: product.name,
            price: product.price,
            quantity: product.selectedQuantity,
            maxStock: product.stock
          });
          product.stock -= product.selectedQuantity;
        }

        const token = localStorage.getItem('token');
        await axios.post('http://localhost:5195/api/cart/add', {
          productId: product.id,
          quantity: product.selectedQuantity,
          customerId: localStorage.getItem('userId')
        }, {
          headers: { Authorization: `Bearer ${token}` }
        });

        product.selectedQuantity = 0;
      },

      async fetchProducts() {
        try {
          const token = localStorage.getItem('token');
          const res = await axios.get('http://localhost:5195/api/products', {
            headers: { Authorization: `Bearer ${token}` }
          });
          this.products = res.data.map(product => ({
            ...product,
            selectedQuantity: 0
          }));
        } catch (err) {
          console.error('Ürünler alınamadı:', err);
        }
      },
      validateQuantity(product) {
        if (product.selectedQuantity > product.stock) product.selectedQuantity = product.stock;
        else if (product.selectedQuantity < 0 || isNaN(product.selectedQuantity)) product.selectedQuantity = 0;
      },
      increaseQuantity(product) {
        if (product.selectedQuantity < product.stock) product.selectedQuantity++;
      },
      decreaseQuantity(product) {
        if (product.selectedQuantity > 0) product.selectedQuantity--;
      },
      async buyNow(product) {
        await this.addToCart(product); // önce sepete ekle
        this.$router.push("/customer/purchasepage");
      },
    },
    mounted() {
      this.fetchProducts();

      this.connection = new signalR.HubConnectionBuilder()
        .withUrl("http://localhost:5195/producthub")
        .withAutomaticReconnect()
        .build();

      this.connection.start().then(() => {
       

        this.connection.on("ProductUpdated", () => {
         
          this.fetchProducts();
        });
      }).catch(err => console.error("SignalR bağlantı hatası:", err));
    }
  };
</script>

<style scoped>
  .product-grid {
    display: flex;
    flex-wrap: wrap;
    justify-content: center;
    gap: 20px;
  }

  .product-image {
    width: 200px;
    height: 200px;
    object-fit: cover;
    border: 1px solid #ccc;
    margin-bottom: 12px;
  }

  button:disabled {
    opacity: 0.5;
  }

  button {
    font-size: 0.9rem;
  }

    button.bg-blue-500 {
      background-color: #3b82f6;
    }

    button.bg-green-500 {
      background-color: #22c55e;
    }

    button.bg-gray-300 {
      background-color: #d1d5db;
    }
</style>
