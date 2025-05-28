<template>
  <div class="product-detail-container">
    <div class="product-detail-card">
      <img :src="product.imageUrl" alt="Ürün Resmi" class="product-image" />
      <h1 class="product-title">{{ product.name }}</h1>
      <p class="product-description">{{ product.description }}</p>
      <p><strong>Kategori:</strong> {{ product.category }}</p>
      <p><strong>Fiyat:</strong> ₺{{ product.price }}</p>
      <p><strong>Stok:</strong> {{ product.stock }}</p>

      <div class="quantity-control">
        <button @click="decreaseQuantity(product)" :disabled="product.selectedQuantity <= 0" class="product-button bg-gray-300">-</button>
        <input v-model.number="product.selectedQuantity"
               type="number"
               min="0"
               :max="product.stock"
               @input="validateQuantity(product)"
               class="quantity-input" />
        <button @click="increaseQuantity(product)" :disabled="product.selectedQuantity >= product.stock" class="product-button bg-gray-300">+</button>
      </div>

      <div class="action-buttons">
        <button @click="addToCart(product)" class="product-button bg-blue-500 text-white">Sepete Ekle</button>
        <button @click="buyNow(product)" class="product-button bg-green-500 text-white">Satın Al</button>
      </div>

      <router-link to="/customer/dashboard" class="back-link">← Geri Dön</router-link>
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
          this.product.selectedQuantity = 1;
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

      async buyNow(product) {
        await this.addToCart(product);
        this.$router.push("/customer/purchasepage");
      }
    }
  };
</script>

<style scoped>
  .product-detail-container {
    display: flex;
    justify-content: center;
    padding: 3rem 1rem;
    background: linear-gradient(135deg, #74ebd5 0%, #ACB6E5 100%);
    min-height: 100vh;
    text-align: center;
  }

  .product-detail-card {
    background-color: #d6e4f0;
    border-radius: 12px;
    box-shadow: 0 2px 16px rgba(0, 0, 0, 0.15);
    padding: 24px;
    width: 100%;
    max-width: 700px;
  }

  .product-image {
    width: 100%;
    height: 400px;
    object-fit: cover;
    border-radius: 8px;
    border: 1px solid #94a3b8;
    margin-bottom: 16px;
  }

  .product-title {
    font-size: 1.5rem;
    font-weight: 700;
    color: #0f172a;
    margin-bottom: 8px;
  }

  .product-description {
    font-size: 1rem;
    color: #334155;
    margin-bottom: 16px;
  }

  .quantity-control {
    display: flex;
    align-items: center;
    gap: 8px;
    margin-top: 12px;
    justify-content: center;
  }

  .quantity-input {
    width: 56px;
    text-align: center;
    border: 1px solid #64748b;
    border-radius: 6px;
    padding: 6px;
    font-size: 0.95rem;
  }

  .product-button {
    padding: 8px 14px;
    border-radius: 6px;
    font-size: 0.9rem;
    font-weight: 500;
    border: 1px solid #000;
    cursor: pointer;
    transition: background-color 0.2s ease;

  }

    .product-button:disabled {
      opacity: 0.5;
      cursor: not-allowed;
    }

  .bg-blue-500 {
    background-color: #2563eb;
  }

  .bg-green-500 {
    background-color: #22c55e;
  }

  .bg-gray-300 {
    background-color: #e2e8f0;
    color: #111827;
  }

  .action-buttons {
    margin-top: 16px;
    display: flex;
    gap: 10px;
    flex-wrap: wrap;
    justify-content: center;
  }

  .back-link {
    display: inline-block;
    margin-top: 16px;
    color: #2563eb;
    font-weight: 500;
    text-decoration: underline;
    font-size: 0.95rem;
  }
</style>
