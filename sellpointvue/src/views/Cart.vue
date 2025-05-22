<!-- src/components/Cart.vue -->
<template>
  <teleport to="body">
    <div v-if="isCustomer"
         :class="[
          'cart-box',
          isPurchasePage ? 'purchase-page-style' : 'default-cart-style'
        ]">
      <h2 class="text-xl font-bold mb-2">Sepet</h2>
      <div v-if="cart.length === 0">Sepetiniz boş.</div>
      <div v-else>
        <table class="w-full text-sm border">
          <thead>
            <tr class="bg-gray-100">
              <th class="border p-2 text-left">Ürün</th>
              <th class="border p-2">Adet</th>
              <th class="border p-2">Toplam</th>
              <th class="border p-2">İşlem</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="item in cart" :key="item.productId" class="border">
              <td class="border p-2 font-semibold">{{ item.name }}</td>
              <td class="border p-2 text-center">
                <div class="flex justify-center items-center space-x-2">
                  <button @click="$emit('decrease', item)" class="bg-gray-300 px-2 rounded">-</button>
                  <span>{{ item.quantity }}</span>
                  <button @click="$emit('increase', item)"
                          :disabled="item.quantity >= item.maxStock"
                          class="bg-gray-300 px-2 rounded">
                    +
                  </button>
                </div>
              </td>
              <td class="border p-2 text-right">₺{{ (item.price * item.quantity).toFixed(2) }}</td>
              <td class="border p-2 text-center">
                <button @click="$emit('remove', item)" class="text-red-600 text-sm">Sil</button>
              </td>
            </tr>
          </tbody>
        </table>

        <div class="mt-4 border-t pt-2 flex justify-between items-center">
          <button v-if="!isPurchasePage"
                  @click="$emit('buy')"
                  class="bg-green-500 text-white px-4 py-2 rounded">
            Satın Al
          </button>
          <div class="font-semibold text-lg">Toplam: ₺{{ cartTotal.toFixed(2) }}</div>
        </div>
      </div>
    </div>
  </teleport>
</template>

<script>
export default {
  props: {
    cart: Array,
    isCustomer: Boolean,
    isPurchasePage: Boolean
  },
  computed: {
    cartTotal() {
      return this.cart.reduce((sum, item) => sum + item.price * item.quantity, 0);
    }
  }
};
</script>

<style scoped>
  .cart-box {
    position: fixed;
    background-color: white;
    border-radius: 8px;
    box-shadow: 0 4px 15px rgba(0, 0, 0, 0.1);
    overflow-y: auto;
    box-sizing: border-box;
    z-index: 10000;
  }

  .purchase-page-style {
    top: 30%;
    left: 50%;
    transform: translate(-50%, -50%);
    width: 90%;
    max-width: 600px;
    max-height: 80vh;
    padding: 20px;
    border: 2px solid #000;
  }

  .default-cart-style {
    top: 16px;
    right: 16px;
    width: 250px;
    max-height: 90vh;
    padding: 16px;
    border: 2px solid black;
  }
</style>
