<template>
  <div class="purchase-page">
    <!-- app.vue'den gelen sepet burada gösterilecek -->
  </div>

  <div class="page">
    <!-- Adres gösterme kutusu -->
    <div class="purchase-card">
      <h2 class="text-lg font-semibold mb-2">Adres Bilgisi</h2>

      <div v-if="address">
        <p class="mb-2">{{ address }}</p>
      </div>
      <div v-else>
        <p class="mb-2 text-red-600">Adres yok</p>
      </div>

      <button @click="showForm = !showForm"
              class="product-button bg-blue-500 text-white mb-4">
        Adres Ekle / Değiştir
      </button>

      <!-- Adres Formu -->
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
          <input v-model="phone"
                 class="input-field"
                 type="tel"
                 @input="phone = phone.replace(/\D/g, '')"
                 maxlength="10"
                 placeholder="5XXXXXXXXX" />
          <p v-if="phoneError" class="text-red-500 text-sm mt-1">Lütfen geçerli bir 10 haneli telefon numarası girin.</p>
        </div>

        <div class="flex justify-end space-x-2 mt-2">
          <button @click="saveAddress" class="product-button bg-green-500 text-white">Kaydet</button>
          <button @click="cancel" class="product-button bg-gray-300">İptal Et</button>
        </div>
      </div>

      <p v-if="successMessage" class="mt-4 text-green-600 font-semibold">
        {{ successMessage }}
      </p>
    </div>

    <!-- Satın alma butonu -->
    <button @click="buy"
            :disabled="!address"
            class="product-button bg-green-500 text-white mt-4 disabled:opacity-50 disabled:cursor-not-allowed">
      Satın Al
    </button>

    <router-link to="/customer/dashboard" class="back-button mt-4">← Geri Dön</router-link>
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
    methods: {
      async fetchAddress() {
        try {
          const token = localStorage.getItem('token');
          const response = await axios.get('http://localhost:5195/api/purchase/GetAddress', {
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
          const token = localStorage.getItem('token');
          await axios.put('http://localhost:5195/api/purchase/UpdateAddress', {
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
      async buy() {
        try {
          const token = localStorage.getItem('token');
          const response = await axios.put('http://localhost:5195/api/purchase/createOrder', {}, {
            headers: { Authorization: `Bearer ${token}` }
          });

          this.successMessage = response.data.message || "Satın alma işlemi başarıyla tamamlandı.";
          setTimeout(() => {
            this.$router.push("/customer/dashboard");
          }, 2000);
        } catch (error) {
          console.error("Satın alma işlemi sırasında hata oluştu:", error);
          this.successMessage = "Satın alma işlemi başarısız.";
        }
      }
    },
    mounted() {
      document.body.classList.add('purchase-page');
      this.fetchAddress();
    },
    beforeUnmount() {
      document.body.classList.remove('purchase-page');
    }
  };
</script>

<style scoped>
  .page {
    margin-top: 550px;
    display: flex;
    flex-direction: column;
    align-items: center;
    
  }

  .purchase-card {
    background-color: #d6e4f0;
    padding: 24px;
    border-radius: 12px;
    box-shadow: 0 2px 12px rgba(0, 0, 0, 0.1);
    width: 100%;
    max-width: 500px;
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
    height: 42px;
    border: 1px solid #64748b;
    border-radius: 6px;
    font-size: 1rem;
    box-sizing: border-box;
  }
  textarea.input-field {
    min-height: 80px;
    resize: vertical;
  }

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

  .bg-blue-500 {
    background-color: #2563eb;
    color: white;
  }

  .bg-gray-300 {
    background-color: #e2e8f0;
    color: #111827;
  }

  .back-button {
    text-decoration: underline;
    color: #2563eb;
    font-size: 16px;
  }
</style>
