<template>
  <div class="p-6">
    <!-- Sepet Kutusu -->
    <div class="fixed top-4 right-4 w-96 max-h-96 overflow-y-auto border-2 border-black rounded p-4 bg-white shadow">
      <h2 class="text-xl font-bold mb-2">Sepet</h2>
      <div v-if="cart.length === 0">Sepetiniz boş.</div>
      <div v-else>
        <div v-for="item in cart" :key="item.productId" class="pb-2 mb-2 border-b">
          <!-- Ürün Bilgileri Satırda -->
          <div class="flex justify-between items-center mb-1">
            <span class="font-semibold">{{ item.name }}</span>
            <div class="flex items-center space-x-2">
              <button @click="decreaseCartQuantity(item)" class="bg-gray-300 px-2 rounded">-</button>
              <span>{{ item.quantity }}</span>
              <button @click="increaseCartQuantity(item)" :disabled="item.quantity >= item.maxStock" class="bg-gray-300 px-2 rounded">+</button>
              <button @click="removeFromCart(item)" class="text-red-600 text-sm ml-2">Sil</button>
            </div>
          </div>
          <!-- Ürün Fiyatı -->
          <div class="text-right text-sm text-gray-700">₺{{ (item.price * item.quantity).toFixed(2) }}</div>
        </div>

        <!-- Sepet Altı: Satın Al ve Toplam -->
        <div class="mt-4 border-t pt-2">
          <div class="flex justify-between items-center">
            <button @click="buyCart" class="bg-green-500 text-white px-4 py-2 rounded">Satın Al</button>
            <div class="font-semibold text-lg">Toplam: ₺{{ cartTotal.toFixed(2) }}</div>
          </div>
        </div>
      </div>
    </div>

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

        <div class="flex items-center space-x-2">
          <button @click="decreaseQuantity(product)" :disabled="product.selectedQuantity <= 0" class="bg-gray-300 p-2 rounded">-</button>
          <input v-model.number="product.selectedQuantity" type="number" min="0" :max="product.stock" @input="validateQuantity(product)" class="w-12 text-center border rounded" />
          <button @click="increaseQuantity(product)" :disabled="product.selectedQuantity >= product.stock" class="bg-gray-300 p-2 rounded">+</button>
        </div>

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
      },
      cartTotal() {
        return this.cart.reduce((sum, item) => sum + item.price * item.quantity, 0);
      }
    },
    methods: {
      async loadCart() {
        try {
          const customerId = localStorage.getItem('userId');
          const token = localStorage.getItem('token');

          const response = await axios.get(`http://localhost:5195/api/cart/${customerId}`, {
            headers: { Authorization: `Bearer ${token}` }
          });
          this.cart = response.data;
        } catch (error) {
          console.error('Sepet yüklenirken hata:', error);
        }
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
      async increaseCartQuantity(item) {
        if (item.quantity < item.stock) {
          item.quantity++;
          const prod = this.products.find(p => p.id === item.productId);
          if (prod) prod.stock--;

          const token = localStorage.getItem('token');
          await axios.put('http://localhost:5195/api/cart/update', {
            productId: item.productId,
            customerId: localStorage.getItem('userId'),
            quantity: item.quantity
          }, {
            headers: { Authorization: `Bearer ${token}` }
          });
        }
      },
      async decreaseCartQuantity(item) {
        if (item.quantity > 1) {
          item.quantity--;
          const prod = this.products.find(p => p.id === item.productId);
          if (prod) prod.stock++;

          const token = localStorage.getItem('token');
          await axios.put('http://localhost:5195/api/cart/update', {
            productId: item.productId,
            customerId: localStorage.getItem('userId'),
            quantity: item.quantity
          }, {
            headers: { Authorization: `Bearer ${token}` }
          });
        }
      },
      async removeFromCart(item) {
        const index = this.cart.findIndex(p => p.productId === item.productId);
        if (index !== -1) {
          const prod = this.products.find(p => p.id === item.productId);
          if (prod) prod.stock += this.cart[index].quantity;
          this.cart.splice(index, 1);

          const token = localStorage.getItem('token');
          await axios.delete(`http://localhost:5195/api/cart/${localStorage.getItem('userId')}/${item.productId}`, {
            headers: { Authorization: `Bearer ${token}` }
          });

        }
      },
      buyNow(product) {
        alert(`Satın alma işlemi başlatıldı: ${product.name}`);
        // Yönlendirme veya satın alma işlemleri burada yapılabilir
      },
      buyCart() {
        alert("Sepetteki tüm ürünler satın alındı.");
        // Sepetteki tüm ürünler için işlem yapılabilir
      }
    },
    mounted() {
      this.fetchProducts();
      this.loadCart();
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
    background-color: white;
    text-align: center;
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
