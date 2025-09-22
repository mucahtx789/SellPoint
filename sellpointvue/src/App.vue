<template>
  <div id="app" class="flex flex-col min-h-screen">
    <!-- Header -->
    <header class="header">
      <div class="container">
        <h1 class="logo" @click="$router.push('/')">🛍️ Sell Point</h1>

        <nav class="nav">
          <router-link to="/" class="nav-link">Ana Sayfa</router-link>
          <router-link to="/products" v-if="isLoggedIn" class="nav-link">Ürünler</router-link>
          <router-link v-if="isCustomer" to="/customer/dashboard" class="nav-link">Hesabım</router-link>
          <router-link v-if="!isLoggedIn" to="/login" class="nav-link">Giriş Yap</router-link>
        </nav>

        <!-- Çıkış butonu -->
        <button v-if="isLoggedIn"
                @click="logout"
                class="logout-btn">
          Çıkış
        </button>
      </div>
    </header>

    <!-- Sepet bileşeni -->
    <Cart v-if="isCustomer"
          :cart="cart"
          :isCustomer="isCustomer"
          :isPurchasePage="isPurchasePage"
          @buy="buyCart"
          @increase="increaseCartQuantity"
          @decrease="decreaseCartQuantity"
          @remove="removeFromCart" />

    <!-- Sayfa İçeriği -->
    <main class="content">
      <div class="container">
        <transition name="fade-slide" mode="out-in">
          <router-view />
        </transition>
      </div>
    </main>

    <!-- Footer -->
    <footer class="footer">
      <div class="container footer-inner">
        <p>&copy; 2025 Sell Point. Tüm hakları saklıdır.</p>
        <div class="footer-links">
          <a href="#" class="footer-link">Hakkımızda</a>
          <a href="#" class="footer-link">İletişim</a>
          <a href="#" class="footer-link">Destek</a>
        </div>
      </div>
    </footer>
  </div>
</template>

<script>
  import axios from "axios";
  import * as signalR from "@microsoft/signalr";
  import Cart from "./views/Cart.vue";

  export default {
    components: { Cart },
    data() {
      return {
        cart: [],
        products: [],
        isLoggedIn: false,
        isCustomer: false,
        connection: null,
      };
    },
    computed: {
      cartTotal() {
        return this.cart.reduce((sum, item) => sum + item.price * item.quantity, 0);
      },
      isPurchasePage() {
        return this.$route.path === "/customer/purchasepage";
      },
    },
    created() {
      this.checkLoginStatus();
    },
    mounted() {
      this.$router.afterEach(() => {
        this.checkLoginStatus();
        if (this.isCustomer) {
          this.loadCart();
        }
      });

      this.connection = new signalR.HubConnectionBuilder()
        .withUrl("http://localhost:5195/carthub")
        .withAutomaticReconnect()
        .build();

      this.connection
        .start()
        .then(() => {
          this.connection.on("CartUpdated", (customerId) => {
            if (customerId == localStorage.getItem("userId")) {
              this.loadCart();
            }
          });
        })
        .catch((err) => console.error("SignalR hata:", err));
    },
    methods: {
      async loadCart() {
        try {
          const customerId = localStorage.getItem("userId");
          const token = localStorage.getItem("token");
          const response = await axios.get(
            `http://localhost:5195/api/cart/${customerId}`,
            { headers: { Authorization: `Bearer ${token}` } }
          );
          this.cart = response.data;
        } catch (error) {
          console.error("Sepet yüklenirken hata:", error);
        }
      },
      async increaseCartQuantity(item) {
        if (item.quantity < item.stock) {
          item.quantity++;
          const prod = this.products.find((p) => p.id === item.productId);
          if (prod) prod.stock--;

          const token = localStorage.getItem("token");
          await axios.put("http://localhost:5195/api/cart/update", {
            productId: item.productId,
            customerId: localStorage.getItem("userId"),
            quantity: item.quantity,
          }, {
            headers: { Authorization: `Bearer ${token}` }
          });
        }
      },
      async decreaseCartQuantity(item) {
        if (item.quantity > 1) {
          item.quantity--;
          const prod = this.products.find((p) => p.id === item.productId);
          if (prod) prod.stock++;

          const token = localStorage.getItem("token");
          await axios.put("http://localhost:5195/api/cart/update", {
            productId: item.productId,
            customerId: localStorage.getItem("userId"),
            quantity: item.quantity,
          }, {
            headers: { Authorization: `Bearer ${token}` }
          });
        }
      },
      async removeFromCart(item) {
        const index = this.cart.findIndex((p) => p.productId === item.productId);
        if (index !== -1) {
          const prod = this.products.find((p) => p.id === item.productId);
          if (prod) prod.stock += this.cart[index].quantity;
          this.cart.splice(index, 1);

          const token = localStorage.getItem("token");
          await axios.delete(
            `http://localhost:5195/api/cart/${localStorage.getItem("userId")}/${item.productId}`,
            { headers: { Authorization: `Bearer ${token}` } }
          );
        }
      },
      buyCart() {
        this.$router.push("/customer/purchasepage");
      },
      checkLoginStatus() {
        this.isLoggedIn = localStorage.getItem("token") !== null;
        this.isCustomer = localStorage.getItem("role") === "Customer";
      },
      logout() {
        localStorage.removeItem("token");
        localStorage.removeItem("role");
        localStorage.removeItem("userId");
        this.isLoggedIn = false;
        this.isCustomer = false;
        this.$router.push("/");
      },
    },
  };
</script>

<style>
  /* ==== GENEL STİL ==== */
  body, #app {
    font-family: "Segoe UI", Roboto, Arial, sans-serif;
    background-color: #f9fafb;
    color: #333;
    margin: 0;
    padding: 0;
  }

  /* ==== CONTAINER ==== */
  .container {
    max-width: 1200px;
    margin: 0 auto;
    padding: 0 16px;
  }

  /* ==== HEADER ==== */
  .header {
    background: linear-gradient(to right, #4f46e5, #9333ea, #ec4899);
    padding: 14px 0;
    color: white;
    box-shadow: 0 4px 12px rgba(0,0,0,0.15);
    position: sticky;
    top: 0;
    z-index: 1000;
  }

  .logo {
    font-size: 1.6rem;
    font-weight: bold;
    cursor: pointer;
  }

  .header .container {
    display: flex;
    align-items: center;
    justify-content: space-between;
  }

  .nav {
    display: flex;
    gap: 20px;
  }

  .nav-link {
    position: relative;
    color: white;
    font-weight: 500;
    transition: color 0.3s ease;
  }

    .nav-link:hover {
      color: #ffd700;
    }

    .nav-link::after {
      content: "";
      position: absolute;
      width: 0%;
      height: 2px;
      bottom: -4px;
      left: 0;
      background-color: #ffd700;
      transition: width 0.3s ease;
    }

    .nav-link:hover::after {
      width: 100%;
    }

  /* ==== BUTONLAR ==== */
  button {
    transition: all 0.3s ease;
    cursor: pointer;
  }

    button:hover {
      transform: translateY(-2px);
      box-shadow: 0 6px 16px rgba(0,0,0,0.15);
    }

  .logout-btn {
    background: linear-gradient(135deg, #f87171, #ef4444);
    border: none;
    color: white;
    border-radius: 8px;
    font-weight: bold;
    padding: 8px 16px;
  }

    .logout-btn:hover {
      background: linear-gradient(135deg, #dc2626, #b91c1c);
    }

  /* ==== MAIN ==== */
  .content {
    flex: 1;
    padding: 30px 0;
  }

  /* ==== FOOTER ==== */
  .footer {
    background: linear-gradient(to right, #111827, #1f2937);
    color: #9ca3af;
    padding: 20px 0;
    margin-top: auto;
  }

  .footer-inner {
    display: flex;
    flex-direction: column;
    gap: 12px;
    text-align: center;
  }

  @media (min-width: 768px) {
    .footer-inner {
      flex-direction: row;
      justify-content: space-between;
      text-align: left;
    }
  }

  .footer-link {
    color: #9ca3af;
    margin-left: 16px;
    transition: color 0.3s ease;
  }

    .footer-link:hover {
      color: white;
    }

  /* ==== SAYFA GEÇİŞ ANİMASYONU ==== */
  .fade-slide-enter-active {
    animation: fadeSlideIn 0.5s ease forwards;
  }

  .fade-slide-leave-active {
    animation: fadeSlideOut 0.4s ease forwards;
  }

  @keyframes fadeSlideIn {
    0% {
      opacity: 0;
      transform: translateY(20px);
    }

    100% {
      opacity: 1;
      transform: translateY(0);
    }
  }

  @keyframes fadeSlideOut {
    0% {
      opacity: 1;
      transform: translateY(0);
    }

    100% {
      opacity: 0;
      transform: translateY(-20px);
    }
  }
</style>
