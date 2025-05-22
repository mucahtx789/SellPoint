<template>
  <div class="p-6 max-w-4xl mx-auto">
    <h1 class="text-2xl font-bold mb-4">Siparişlerim</h1>

    <div v-if="orders.length === 0" class="text-center text-gray-500">
      Henüz siparişiniz yok.
    </div>

    <div v-for="order in orders" :key="order.id" class="border p-4 rounded mb-4 shadow">
      <h2 class="font-semibold mb-2">Sipariş No: {{ order.id }}</h2>
      <p class="text-sm text-gray-600 mb-4">Tarih: {{ new Date(order.orderDate).toLocaleString() }}</p>

      <table class="w-full text-left border-collapse">
        <thead>
          <tr>
            <th class="border-b p-2">Ürün</th>
            <th class="border-b p-2">Adet</th>
            <th class="border-b p-2">Birim Fiyat (₺)</th>
            <th class="border-b p-2">Toplam (₺)</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="item in order.items" :key="item.productId">
            <td class="border-b p-2">{{ item.productName }}</td>
            <td class="border-b p-2">{{ item.quantity }}</td>
            <td class="border-b p-2">{{ item.unitPrice.toFixed(2) }}</td>
            <td class="border-b p-2">{{ (item.quantity * item.unitPrice).toFixed(2) }}</td>
          </tr>
        </tbody>
      </table>
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
    }
  },
  mounted() {
    this.fetchOrders();
  }
};
</script>

<style scoped>
  table {
    border: 1px solid #ccc;
    border-radius: 6px;
  }

  th, td {
    border: 1px solid #ddd;
  }
</style>
