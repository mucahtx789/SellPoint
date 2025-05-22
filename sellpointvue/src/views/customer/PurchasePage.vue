<template>
  <div class="purchase-page">
    <!-- app vue den sepet -->
  </div>
  <div class="page">

    <!-- adres gösterme -->
    <div class="p-6 max-w-xl mx-auto bg-white rounded shadow">
      <h2 class="text-lg font-semibold mb-2">Adres Bilgisi</h2>

      <div v-if="address">
        <p class="mb-2">{{ address }}</p>
      </div>
      <div v-else>
        <p class="mb-2 text-red-600">Adres yok</p>
      </div>

      <button @click="showForm = !showForm"
              class="mb-4 bg-blue-500 text-white px-4 py-2 rounded">
        Adres Ekle / Değiştir
      </button>

      <!-- Gizli Adres Formu -->
      <div v-if="showForm" class="border p-4 rounded bg-gray-50">
        <div class="mb-2">
          <label class="block text-sm font-semibold">İl</label>
          <input v-model="city" class="border p-1 w-full rounded" />
        </div>
        <div class="mb-2">
          <label class="block text-sm font-semibold">İlçe</label>
          <input v-model="district" class="border p-1 w-full rounded" />
        </div>
        <div class="mb-2">
          <label class="block text-sm font-semibold">Adres</label>
          <textarea v-model="fullAddress" class="border p-1 w-full rounded"></textarea>
        </div>
        <div class="mb-2">
          <label class="block text-sm font-semibold">Telefon</label>
          <input v-model="phone"
                 class="border p-1 w-full rounded"
                 type="tel"
                 @input="phone = phone.replace(/\D/g, '')"
                 maxlength="10"
                 placeholder="5XXXXXXXXX" />
          <!-- Telefon hatası varsa göster -->
          <p v-if="phoneError" class="text-red-500 text-sm mt-1">Lütfen geçerli bir 10 haneli telefon numarası girin.</p>
        </div>

        <div class="flex justify-end space-x-2">
          <button @click="saveAddress"
                  class="bg-green-500 text-white px-4 py-2 rounded">
            Kaydet
          </button>
          <button @click="cancel"
                  class="bg-gray-400 text-white px-4 py-2 rounded">
            İptal Et
          </button>
        </div>
      </div>

      <p v-if="successMessage" class="mt-4 text-green-600 font-semibold">
        {{ successMessage }}
      </p>
    </div>
    <button @click="buy"
            :disabled="!address"
            class="bg-green-500 text-white px-4 py-2 rounded disabled:opacity-50 disabled:cursor-not-allowed">
      Satın Al
    </button>
    <router-link to="/customer/dashboard">← Geri Dön</router-link>
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
        // Telefon numarasını kontrol et
        if (!/^[0-9]{10}$/.test(this.phone)) {
          this.phoneError = true;  // Hata mesajını göstermek için
          return;  // Telefon hatalıysa işlemi durdur
        }

        // Hata yoksa, adresi kaydetmeye devam et
        this.phoneError = false;  // Hata mesajını gizle
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

          // Form verilerini temizle
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
          }, 2000); // 2 saniye sonra yönlendir

        } catch (error) {
          console.error("Satın alma işlemi sırasında hata oluştu:", error);
          this.successMessage = "Satın alma işlemi başarısız.";
        }
      }
    },
    mounted() {
      // Eğer sadece "PurchasePage" sayfasında sepeti farklı bir şekilde göstereceksek,
      // app.vue'deki stilin sadece burada uygulanabilmesi için bu sınıfı ekliyoruz.
      document.body.classList.add('purchase-page');
      this.fetchAddress();
    },
    beforeUnmount() {
      document.body.classList.remove('purchase-page');
    }
  }
</script>

<style scoped>
  .page {
    
    margin-top: 550px; /* app.vue'deki sepete göre ayarlanabilir */
    display: flex;
    flex-direction: column;
    align-items: center;
    text-align: center;
  }

  .back-button {
    text-decoration: underline;
    color: #3182ce;
    font-size: 16px;
    margin-bottom: 20px;
  }
</style>
