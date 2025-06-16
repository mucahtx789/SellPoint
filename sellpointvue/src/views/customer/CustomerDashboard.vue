<template>
  <div class="p-6">
    <!-- Başlık ve kategori filtresi -->
    <div class="mb-6">
      <h1 class="text-2xl font-bold mb-4">Ürünler</h1>
      <div class="flex space-x-2">
        <button v-for="category in categories"
                :key="category"
                @click="selectedCategory = category"
                :class="['category-button', selectedCategory === category ? 'bg-blue-500 text-white' : 'bg-white text-black']">
          {{ category === '' ? 'Tümü' : category }}
        </button>
        <input v-model="searchTerm"
               type="text"
               placeholder="Ürün adı ara..."
               class="search-input border rounded px-3 py-2 w-60" />
        <button @click="sortByPrice('asc')"
                :class="['category-button', sortDirection === 'asc' ? 'active' : '']">
          Fiyat Artan
        </button>

        <button @click="sortByPrice('desc')"
                :class="['category-button', sortDirection === 'desc' ? 'active' : '']">
          Fiyat Azalan
        </button>
      </div> 
       
    </div>

    <!-- Ürün liste -->
    <div class="product-grid">
      <div v-for="product in filteredProducts" :key="product.id" class="product-card">
        <img :src="product.imageUrl" alt="Ürün Resmi" class="product-image" />
        <h2 class="font-bold text-lg mb-1">{{ product.name }}</h2>
        <p class="text-sm text-gray-700 mb-1">{{ product.description }}</p>
        <p class="text-sm mb-1"><strong>Kategori:</strong> {{ product.category }}</p>
        <p class="text-sm mb-1"><strong>Fiyat:</strong> ₺{{ product.price.toFixed(2) }}</p>
        <p class="text-sm mb-2"><strong>Stok:</strong> {{ product.stock }}</p>

        <div class="flex items-center space-x-2">
          <button @click="decreaseQuantity(product)" :disabled="product.selectedQuantity <= 0" class="product-button bg-gray-300">-</button>
          <input v-model.number="product.selectedQuantity" type="number" min="0" :max="product.stock" @input="validateQuantity(product)" class="w-12 text-center border rounded" />
          <button @click="increaseQuantity(product)" :disabled="product.selectedQuantity >= product.stock" class="product-button bg-gray-300">+</button>
        </div>

        <div class="flex space-x-2 mt-2">
          <button @click="addToCart(product)" class="product-button bg-blue-500 text-white">Sepete Ekle</button>
          <button @click="buyNow(product)" class="product-button bg-green-500 text-white">Satın Al</button>
          <router-link :to="`/urun/${encodeURIComponent(product.name)}/${product.id}`" class="product-button bg-yellow-500 text-white">
            İncele
          </router-link>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
  import axios from 'axios';
  import * as signalR from '@microsoft/signalr';

  export default {
    name: 'CustomerDashboard',
    data() {
      return {
        searchTerm: '',
        sortDirection: '', // fiyat sıralama yönü
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
      }
    },
    methods: {
      sortByPrice(direction) {
        this.sortDirection = direction;
        if (direction === 'asc') {
          this.products.sort((a, b) => a.price - b.price);
        } else if (direction === 'desc') {
          this.products.sort((a, b) => b.price - a.price);
        }
      },
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
            selectedQuantity: 1
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
        await this.addToCart(product);
        this.$router.push("/customer/purchasepage");
      }
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
    },
    computed: {
      filteredProducts() {
        return this.products.filter(product => {
          const matchesCategory = this.selectedCategory === '' || product.category === this.selectedCategory;
          const matchesSearch = product.name.toLowerCase().includes(this.searchTerm.toLowerCase());
          return matchesCategory && matchesSearch;
        });
      }
    }

  };
</script>

<style>
  body {
    background: linear-gradient(135deg, #74ebd5 0%, #ACB6E5 100%);
  }

  /* Başlık */
  h1 {
    font-size: 2rem;
    font-weight: bold;
    color: #1f2937;
    margin-bottom: 16px;
    padding-bottom: 8px;
  }
  .search-input {
    border: 1px solid #64748b;
    border-radius: 6px;
    font-size: 1rem;
    outline: none;
  }

    .search-input:focus {
      border-color: #2563eb;
      box-shadow: 0 0 0 2px rgba(37, 99, 235, 0.5);
    }


  /* Kategori Butonları */
  .category-button {
    background-color: #e5e7eb;
    border: 1px solid #000;
    color: #111827;
    font-weight: 600;
    padding: 10px 18px;
    font-size: 1rem;
    border-radius: 6px;
    transition: 0.2s;
  }

    .category-button:hover {
      background-color: #cbd5e1;
    }

    .category-button.bg-blue-500 {
      background-color: #2563eb !important;
      color: white !important;
    }

  /* Filtre butonlarının alt çizgisi */
  .mb-6::after {
    content: '';
    display: block;
    margin-top: 20px;
    border-bottom: 2px solid #1e293b;
  }

  /* Ürün Grid */
  .product-grid {
    width: 100%;
    max-width: 100%;
    box-sizing: border-box;
    display: grid;
    grid-template-columns: repeat(3, minmax(0, 1fr));
    gap: 24px;
    padding-right: 320px;

  }

  /* Ürün Kartları */
  .product-card {
    background-color: #d6e4f0;
    border-radius: 12px;
    box-shadow: 0 2px 12px rgba(0, 0, 0, 0.1);
    padding: 16px;
    display: flex;
    flex-direction: column;
    align-items: center;
    transition: 0.2s;
  }

    .product-card:hover {
      transform: translateY(-4px);
      box-shadow: 0 4px 20px rgba(0, 0, 0, 0.15);
    }

  /* Ürün Resmi */
  .product-image {
    width: 100%;
    max-width: 180px;
    height: 180px;
    object-fit: cover;
    border-radius: 8px;
    border: 1px solid #94a3b8;
    margin-bottom: 12px;
  }

  /* Ürün Metinleri */
  .product-card h2 {
    font-size: 1.1rem;
    font-weight: 700;
    color: #0f172a;
  }

  .product-card p {
    font-size: 0.9rem;
    color: #334155;
    margin: 2px 0;
  }

  /* Sayı input */
  input[type="number"] {
    width: 48px;
    text-align: center;
    border: 1px solid #64748b;
    border-radius: 4px;
    padding: 4px;
    font-size: 0.9rem;
  }

  /* Ürün içi butonlar */
  .product-button {
    padding: 6px 10px;
    border-radius: 5px;
    font-size: 0.85rem;
    font-weight: 500;
    border: 1px solid #000;
    cursor: pointer;
    transition: background-color 0.2s ease;
  }

    .product-button:disabled {
      opacity: 0.5;
      cursor: not-allowed;
    }


  .bg-green-500 {
    background-color: #22c55e;
    color: white;
  }

  .bg-yellow-500 {
    background-color: #facc15;
    color: #1f2937;
  }

  .bg-gray-300 {
    background-color: #e2e8f0;
    color: #111827;
  }

  /* Responsive */
  @media (max-width: 1024px) {
    .product-grid {
      grid-template-columns: repeat(2, 1fr);
      max-width: 100%;
    }
  }

  @media (max-width: 640px) {
    .product-grid {
      grid-template-columns: 1fr;
      max-width: 100%;
    }
  }
</style>
