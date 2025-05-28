<template>
  <teleport to="body">
    <div v-if="isCustomer"
         :class="[
           'cart-box',
           isPurchasePage ? 'purchase-page-style' : 'default-cart-style'
         ]">
      <h2 class="text-xl font-bold mb-4">Sepet</h2>
      <div v-if="cart.length === 0">Sepetiniz boş.</div>
      <div v-else>
        <div v-for="item in cart" :key="item.productId" class="cart-product-card mb-4">
          <div class="font-semibold text-base mb-1">{{ item.name }}</div>
          <div class="text-sm mb-1">
            Adet:
            <span class="inline-flex items-center space-x-2">
              <button @click="$emit('decrease', item)" class="product-button bg-gray-300">-</button>
              <span>{{ item.quantity }}</span>
              <button @click="$emit('increase', item)" :disabled="item.quantity >= item.maxStock" class="product-button bg-gray-300">+</button>
            </span>
          </div>
          <div class="text-sm mb-1">Toplam: ₺{{ (item.price * item.quantity).toFixed(2) }}</div>
          <button @click="$emit('remove', item)" class="product-button bg-red-500 text-white mt-2">Sil</button>
        </div>

        <div class="mt-4 border-t pt-2 flex justify-between items-center">
          <button v-if="!isPurchasePage"
                  @click="$emit('buy')"
                  class="product-button bg-green-500 text-white">
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
    emits: ['buy', 'increase', 'decrease', 'remove'], // <-- Bunu ekledik
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
    position: fixed;
    top: 32%;
    left: 50%;
    transform: translate(-50%, -50%);
    width: 90%;
    max-width: 1000px;
    height: 500px;
    padding: 20px;
    border: 2px solid #000;
    background-color: white;
    z-index: 10000;
    display: flex;
    flex-direction: column;
  }

  .cart-box {
    overflow: hidden;
  }

    .cart-box > div:last-child {
      overflow-y: auto;
      flex-grow: 1;
      margin-top: 1rem;
    }

  .default-cart-style {
    top: 16px;
    right: 16px;
    width: 320px;
    max-height: 90vh;
    padding: 16px;
    border: 2px solid black;
    height: 500px;
    max-height: 90vh;
    overflow-y: auto;
  }

  .cart-product-card {
    background-color: #d6e4f0;
    border-radius: 12px;
    padding: 12px;
    box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
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

  .bg-yellow-500 {
    background-color: #facc15;
    color: #1f2937;
  }

  .bg-gray-300 {
    background-color: #e2e8f0;
    color: #111827;
  }

  .bg-red-500 {
    background-color: #ef4444;
    color: white;
  }
</style>
