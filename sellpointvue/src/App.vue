<template>
  <div id="app" class="app-bg">
    <!-- Çıkış butonu -->
    <button v-if="isLoggedIn" @click="logout" class="logout-btn">Çıkış Yap</button>

    <!-- Sepet bileşeni -->
    <Cart v-if="isCustomer"
          :cart="cart"
          :isCustomer="isCustomer"
          :isPurchasePage="isPurchasePage"
          @buy="buyCart"
          @increase="increaseCartQuantity"
          @decrease="decreaseCartQuantity"
          @remove="removeFromCart"
    />

    <!-- Sayfa içeriği -->
    <router-view />
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
  html, body, #app {
    margin: 0;
    padding: 0;
    height: 100%;
    overflow-x: hidden;
    font-family: Arial, sans-serif;
    background: linear-gradient(135deg, #74ebd5 0%, #ACB6E5 100%);
  }

  .app-bg {
    min-height: 100vh;
    background: linear-gradient(135deg, #74ebd5 0%, #ACB6E5 100%);
  }

  .logout-btn {
    position: fixed;
    top: 16px;
    right: 335px;
    background-color: #dc2626;
    color: white;
    padding: 10px 20px;
    border: none;
    border-radius: 6px;
    font-weight: bold;
    cursor: pointer;
    z-index: 10001;
    white-space: nowrap;
  }

    .logout-btn:hover {
      background-color: #b91c1c;
    }
</style>
