<template>
  <div class="p-6">
    <!-- Başlık ve kategori filtresi -->
    <div class="flex justify-between items-center mb-6">
      <h1 class="text-2xl font-bold">Ürünler</h1>
      <div class="space-x-2">
        <label for="category" class="font-semibold">Kategori:</label>
        <select v-model="selectedCategory" id="category" class="border p-1 rounded">
          <option value="">Tümü</option>
          <option>Elektronik</option>
          <option>Gıda</option>
          <option>Giyim</option>
          <option>Kırtasiye</option>
        </select>
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

        <!-- Adet Seçimi -->
        <div class="flex items-center space-x-2">
          <button @click="decreaseQuantity(product)"
                  :disabled="product.selectedQuantity <= 0"
                  class="bg-gray-300 p-2 rounded">
            -
          </button>
          <input v-model.number="product.selectedQuantity"
                 type="number"
                 min="0"
                 :max="product.stock"
                  @input="validateQuantity(product)"
                 class="w-12 text-center border rounded" />
          <button @click="increaseQuantity(product)"
                  :disabled="product.selectedQuantity >= product.stock"
                  class="bg-gray-300 p-2 rounded">
            +
          </button>
        </div>

        <!-- Butonlar -->
        <div class="flex space-x-2 mt-2">
          <button @click="addToCart(product)" class="bg-blue-500 text-white px-4 py-2 rounded">Sepete Ekle</button>
          <button @click="buyNow(product)" class="bg-green-500 text-white px-4 py-2 rounded">Satın Al</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
  import axios from 'axios';

  export default {
    name: 'CustomerDashboard',
    data() {
      return {
        products: [],
        selectedCategory: '',
        cart: []
      };
    },
    computed: {
      filteredProducts() {
        if (this.selectedCategory === '') return this.products;
        return this.products.filter(p => p.category === this.selectedCategory);
      }
    },
    methods: {
      async fetchProducts() {
        try {
          const token = localStorage.getItem('token');
          const res = await axios.get('http://localhost:5195/api/products', {
            headers: {
              Authorization: `Bearer ${token}`
            }
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
        if (product.selectedQuantity > product.stock) {
          product.selectedQuantity = product.stock;
        } else if (product.selectedQuantity < 0 || isNaN(product.selectedQuantity)) {
          product.selectedQuantity = 0;
        }
      },
      increaseQuantity(product) {
        if (product.selectedQuantity < product.stock) {
          product.selectedQuantity++;
        }
      },
      decreaseQuantity(product) {
        if (product.selectedQuantity > 0) {
          product.selectedQuantity--;
        }
      },
      addToCart(product) {
        if (product.selectedQuantity > 0) {
          const item = { ...product };
          item.selectedQuantity = product.selectedQuantity;
          this.cart.push(item);
          alert(`${product.name} sepete eklendi.`);
        } else {
          alert('Adet seçmelisiniz.');
        }
      },
      buyNow(product) {
        if (product.selectedQuantity > 0) {
          alert(`Satın alındı: ${product.name}, Adet: ${product.selectedQuantity}`);
          // Satın alma API çağrısı buraya eklenebilir
        } else {
          alert('Adet seçmelisiniz.');
        }
      }
    },
    mounted() {
      this.fetchProducts();
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

  .product-card {
    width: 450px;
    border: 2px solid black;
    border-radius: 8px;
    padding: 16px;
    display: flex;
    flex-direction: column;
    align-items: center;
    text-align: center;
    background-color: white;
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
