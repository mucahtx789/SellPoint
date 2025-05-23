<template>
  <div class="p-6 max-w-4xl mx-auto">
    <h1 class="text-2xl font-bold mb-4">Siparişlerim</h1>

    <div v-if="orders.length === 0" class="text-center text-gray-500">
      Henüz siparişiniz yok.
    </div>

    <div v-for="order in orders" :key="order.id" class="order-card mb-6 p-4 rounded shadow-lg bg-white">
      <h2 class="font-semibold text-lg mb-2">Sipariş No: {{ order.id }}</h2>
      <p class="text-sm text-gray-600 mb-4">Tarih: {{ new Date(order.orderDate).toLocaleString() }}</p>

      <div class="overflow-x-auto">
        <table class="w-full text-left table-auto">
          <thead>
            <tr class="bg-gray-100">
              <th class="p-2 text-sm font-semibold">Ürün</th>
              <th class="p-2 text-sm font-semibold">Adet</th>
              <th class="p-2 text-sm font-semibold">Birim Fiyat (₺)</th>
              <th class="p-2 text-sm font-semibold">Toplam (₺)</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="item in order.items" :key="item.productId" class="border-b hover:bg-gray-50">
              <td class="p-2 text-sm">{{ item.productName }}</td>
              <td class="p-2 text-sm">{{ item.quantity }}</td>
              <td class="p-2 text-sm">{{ item.unitPrice.toFixed(2) }}</td>
              <td class="p-2 text-sm">{{ (item.quantity * item.unitPrice).toFixed(2) }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Dashboard yönlendirme butonu -->
    <div class="text-center mt-6">
      <button @click="goToDashboard" class="dashboard-btn">
        Dashboard'a Git
      </button>
    </div>
  </div>
</template>

<script>
  import axios from 'axios';

  export default {
    name: 'SellerOrders',
    data() {
      return {
        orders: []
      };
    },
    methods: {
      async fetchOrders() {
        try {
          const token = localStorage.getItem('token');
          const res = await axios.get('http://localhost:5195/api/purchase/my-orders', {
            headers: { Authorization: `Bearer ${token}` }
          });
          this.orders = res.data;
        } catch (err) {
          console.error('Siparişler alınamadı:', err);
        }
      },
      goToDashboard() {
        this.$router.push('/seller/dashboard');  // /seller/dashboard sayfasına yönlendir
      }
    },
    mounted() {
      this.fetchOrders();
    }
  };
</script>

<style scoped>
  
  .order-card {
    background-color: white;
    border-radius: 8px;
    box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
  }

  
  table {
    width: 100%;
    border-collapse: collapse;
    margin-top: 16px;
  }

  th, td {
    padding: 12px;
    text-align: left;
    border-bottom: 1px solid #e5e7eb;
    font-size: 0.875rem;
  }

  th {
    background-color: #f3f4f6;
    font-weight: 600;
    color: #4b5563;
  }

  tbody tr:hover {
    background-color: #f9fafb;
  }

  
  thead {
    background-color: #f3f4f6;
  }

  tbody tr:hover {
    background-color: #f9fafb;
  }

  h1 {
    font-size: 1.875rem;
    font-weight: bold;
    color: #1f2937;
  }

  h2 {
    font-size: 1.125rem;
    font-weight: 600;
    color: #111827;
  }

  
  .p-6 {
    padding: 24px;
  }

  .mb-4 {
    margin-bottom: 16px;
  }

  .mb-6 {
    margin-bottom: 24px;
  }

  .text-gray-500 {
    color: #6b7280;
  }

  .text-gray-600 {
    color: #4b5563;
  }

  
  .border-b {
    border-bottom: 1px solid #e5e7eb;
  }

  
  .dashboard-btn {
    background-color: #4CAF50; 
    color: white; 
    font-size: 16px;
    font-weight: 600; 
    padding: 12px 24px; 
    border-radius: 8px; 
    border: none; 
    cursor: pointer; 
    transition: all 0.3s ease;
  }

    .dashboard-btn:hover {
      background-color: #45a049; 
    }

    .dashboard-btn:focus {
      outline: none; 
      box-shadow: 0 0 0 4px rgba(72, 153, 85, 0.4); 
    }

    .dashboard-btn:active {
      background-color: #3e8e41; 
    }
</style>
