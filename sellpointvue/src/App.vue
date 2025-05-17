<template>
  <nav>
    <!-- Giriş yapılmışsa çıkış butonunu göster -->
    <button v-if="isLoggedIn" @click="logout" class="logout-btn">Çıkış Yap</button>

    <!-- Sepet Kutusu -->
    <div v-if="isCustomer" class="fixed top-4 right-4 w-96 max-h-96 overflow-y-auto border-2 border-black rounded p-4 bg-white shadow">
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

  </nav>
  <router-view />
</template>

<script>
  import axios from 'axios';
  export default {
    data() {
      return {
        cart: [],
        products: [],
        isLoggedIn: false,
        isCustomer: false
        
      };
    },
    computed: {
      cartTotal() {
        return this.cart.reduce((sum, item) => sum + item.price * item.quantity, 0);
      }
    },
    created() {
      this.checkLoginStatus(); // Sayfa ilk yüklendiğinde kontrol
    },
    mounted() {
      this.$router.afterEach(() => {
        this.checkLoginStatus();
        if (this.isCustomer) {
          this.loadCart();
        }
      });
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
      buyCart() {
        alert("Sepetteki tüm ürünler satın alındı.");
        // Sepetteki tüm ürünler için işlem yapılabilir
      },
      checkLoginStatus() {
        this.isLoggedIn = localStorage.getItem('token') !== null;
        this.isCustomer = localStorage.getItem('role')=='Customer';
      },
      logout() {
        localStorage.removeItem('token');
        localStorage.removeItem('role');
        localStorage.removeItem('userId');
        this.isLoggedIn = false;
        this.isCustomer = false;
        this.$router.push('/');
      }
    }
  };
</script>

<style>
  .product-card {
    width: 450px;
    border: 2px solid black;
    border-radius: 8px;
    padding: 16px;
    background-color: white;
    text-align: center;
  }

  </style>
