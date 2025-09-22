<template>
  <div class="purchase-page">
    <!-- Sepet bölümü -->
    <div class="cart-section">
      <h2 class="text-xl font-bold mb-4">Sepetiniz</h2>
      <div v-if="cart.length === 0">Sepetiniz boş.</div>
      <div v-else>
        <div v-for="item in cart" :key="item.productId" class="cart-product-card mb-4">
          <div class="font-semibold text-base mb-1">{{ item.name }}</div>
          <div class="text-sm mb-1">
            Adet:
            <span class="inline-flex items-center space-x-2">
              <button @click="$root.decreaseCartQuantity(item)" class="product-button bg-gray-300">-</button>
              <span>{{ item.quantity }}</span>
              <button @click="$root.increaseCartQuantity(item)"
                      :disabled="item.quantity >= item.maxStock"
                      class="product-button bg-gray-300">
                +
              </button>
            </span>
          </div>
          <div class="text-sm mb-1">Toplam: ₺{{ (item.price * item.quantity).toFixed(2) }}</div>
          <button @click="$root.removeFromCart(item)" class="product-button2  bg-red-500 text-white mt-2">
            Sil
          </button>
        </div>
        <div class="mt-4 border-t pt-2 flex justify-between items-center">

          <div class="font-semibold text-lg">Toplam: ₺{{ cartTotal.toFixed(2) }}</div>
        </div>
      </div>
    </div>

    <!-- Adres bölümü -->
    <div class="purchase-card">
      <h2 class="text-lg font-semibold mb-2">Adres Bilgisi</h2>
      <div v-if="address">
        <p class="mb-2">{{ address }}</p>
      </div>
      <div v-else>
        <p class="mb-2 text-red-600">Adres yok</p>
      </div>
      <button @click="showForm = !showForm" class="product-button2 bg-blue-500 text-white mb-4">
        Adres Ekle / Değiştir
      </button>
      <div v-if="showForm" class="address-form">
        <div class="mb-2">
          <label class="block text-sm font-semibold">İl</label>
          <input v-model="city" class="input-field" />
        </div>
        <div class="mb-2">
          <label class="block text-sm font-semibold">İlçe</label>
          <input v-model="district" class="input-field" />
        </div>
        <div class="mb-2">
          <label class="block text-sm font-semibold">Adres</label>
          <textarea v-model="fullAddress" class="input-field"></textarea>
        </div>
        <div class="mb-2">
          <label class="block text-sm font-semibold">Telefon</label>
          <input v-model="phone" type="tel" maxlength="10" placeholder="5XXXXXXXXX"
                 @input="phone = phone.replace(/\D/g,'')" class="input-field" />
          <p v-if="phoneError" class="text-red-500 text-sm mt-1">
            Lütfen geçerli bir 10 haneli telefon numarası girin.
          </p>
        </div>
        <div class="flex justify-end space-x-2 mt-2">
          <button @click="saveAddress" class="product-button2 bg-green-500 text-white">Kaydet</button>
          <button @click="cancel" class="product-button2 bg-gray-300">İptal Et</button>
        </div>
      </div>
      <p v-if="successMessage" class="mt-4 text-green-600 font-semibold">{{ successMessage }}</p>
    </div>
    <button @click="buy" class="product-button2 bg-green-500 text-white">
      Satın Al
    </button>
  </div>
</template>

<script>
  import axios from 'axios';

  export default {
    data() {
      return {
        address: '',
        city: '',
        district: '',
        fullAddress: '',
        phone: '',
        phoneError: false,
        showForm: false,
        successMessage: ''
      };
    },
    computed: {
      cart() {
        return this.$root.cart;
      },
      cartTotal() {
        return this.cart.reduce((sum, item) => sum + item.price * item.quantity, 0);
      }
    },
    methods: {
      async fetchAddress() {
        try {
          const token = localStorage.getItem("token");
          const response = await axios.get("http://localhost:5195/api/purchase/GetAddress", {
            headers: { Authorization: `Bearer ${token}` }
          });
          this.address = response.data.address || '';
        } catch (err) {
          console.error("Adres alınamadı:", err);
        }
      },
      async saveAddress() {
        if (!/^[0-9]{10}$/.test(this.phone)) {
          this.phoneError = true;
          return;
        }
        this.phoneError = false;
        const combinedAddress = `${this.city}, ${this.district}, ${this.fullAddress}, Tel: ${this.phone}`;
        try {
          const token = localStorage.getItem("token");
          await axios.put("http://localhost:5195/api/purchase/UpdateAddress", {
            address: combinedAddress
          }, {
            headers: { Authorization: `Bearer ${token}` }
          });
          this.address = combinedAddress;
          this.showForm = false;
          this.successMessage = "Adres başarıyla güncellendi.";
          this.city = '';
          this.district = '';
          this.fullAddress = '';
          this.phone = '';
        } catch (err) {
          console.error("Adres güncellenirken hata:", err);
        }
      },
      cancel() {
        this.showForm = false;
        this.successMessage = '';
      },
      buy() {
        if (!this.address) {
          alert("Lütfen önce adres bilgisi girin.");
          return;
        }
        alert("Satın alma işlemi tamamlandı!");
      }
    },
    mounted() {
      this.fetchAddress();
    }
  };
</script>

<style scoped>
  .purchase-page {
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 50px;
    padding: 20px;
    width: 100%;
  }

  .cart-section, .purchase-card {
    width: 90%;
    max-width: 955px;
    background-color: white;
    padding: 20px;
    border-radius: 8px;
    box-shadow: 0 4px 15px rgba(0,0,0,0.1);
  }

  .cart-product-card {
    background-color: #d6e4f0;
    border-radius: 12px;
    padding: 12px;
    margin-bottom: 8px;
  }

  .product-button {
    padding: 6px 10px;
    border-radius: 5px;
    border: 1px solid #000;
    cursor: pointer;
    width: 30px;
    height: 30px;
  }
  .product-button2 {
    padding: 6px 10px;
    border-radius: 5px;
    border: 1px solid #000;
    cursor: pointer;
    width:150px;
    height:30px;
  }
    .product-button.bg-red-500 {
      background-color: #ef4444;
      color: white;
    }
  .product-button2.bg-red-500 {
    background-color: #ef4444;
    color: white;
  }
  .address-form {
    border: 1px solid #cbd5e1;
    background-color: #f1f5f9;
    padding: 16px;
    border-radius: 8px;
    margin-top: 10px;
  }

  .input-field {
    width: 100%;
    padding: 10px 14px;
    border: 1px solid #64748b;
    border-radius: 6px;
    font-size: 1rem;
  }
</style>
