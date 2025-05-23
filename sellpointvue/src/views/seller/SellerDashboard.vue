<template>
  <div class="p-6">
    <div class="flex justify-between items-center mb-6">
      <h1 class="text-2xl font-bold mb-4">Ürünlerim</h1>
      <div class="space-x-2">
        <button @click="$router.push('/seller/add-product')" class="product-button bg-green-500 text-white">Ürün Ekle</button>
        <button @click="$router.push('/seller/orders')" class="product-button bg-blue-500 text-white">Siparişlerim</button>
      </div>
    </div>

    <div class="mb-6">
      <label for="category" class="font-semibold mr-2">Kategori:</label>
      <div class="flex space-x-2">
        <button v-for="category in categories"
                :key="category"
                @click="selectedCategory = category"
                :class="['category-button', selectedCategory === category ? 'bg-blue-500 text-white' : 'bg-white text-black']">
          {{ category === '' ? 'Tümü' : category }}
        </button>
      </div>
    </div>

    <div class="product-grid">
      <div v-for="product in filteredProducts" :key="product.id" class="product-card">
        <img :src="product.imageUrl" alt="Ürün Resmi" class="product-image" />
        <h2 class="font-bold text-lg mb-1">{{ product.name }}</h2>
        <p class="text-sm text-gray-700 mb-1">{{ product.description }}</p>
        <p class="text-sm mb-1"><strong>Kategori:</strong> {{ product.category }}</p>
        <p class="text-sm mb-1"><strong>Fiyat:</strong> ₺{{ product.price.toFixed(2) }}</p>
        <p class="text-sm mb-2"><strong>Stok:</strong> {{ product.stock }}</p>
        <div class="flex space-x-2">
          <button @click="openEditModal(product)" class="product-button bg-yellow-500 text-white">Düzenle</button>
          <button @click="deleteProduct(product.id)" class="product-button bg-red-600 text-white">Sil</button>
        </div>
      </div>
    </div>
  </div>

  <!-- Düzenleme Modalı -->
  <div v-if="showEditModal" class="modal-overlay">
    <div class="modal-content">
      <h2 class="text-xl font-semibold mb-4">Ürünü Düzenle</h2>

      <label>Ürün Adı:</label>
      <input v-model="editProduct.name" class="input-field mb-2" />

      <label>Açıklama:</label>
      <textarea v-model="editProduct.description" class="input-field mb-2" />

      <label>Kategori:</label>
      <select v-model="editProduct.category" class="input-field mb-2">
        <option v-for="cat in categories" :key="cat" :value="cat" v-if="cat !== ''">{{ cat }}</option>
      </select>

      <label>Fiyat:</label>
      <input type="number" v-model="editProduct.price" class="input-field mb-2" />

      <label>Adet:</label>
      <input type="number" v-model="editProduct.stock" class="input-field mb-2" />

      <label>Resim (URL):</label>
      <input v-model="editProduct.imageUrl" class="input-field mb-2" />

      <div class="flex justify-end gap-2 mt-4">
        <button @click="updateProduct" class="product-button bg-blue-500 text-white">Kaydet</button>
        <button @click="closeEditModal" class="product-button bg-gray-400 text-white">İptal</button>
      </div>
    </div>
  </div>
</template>

<script>
  import axios from 'axios';

  export default {
    name: 'SellerDashboard',
    data() {
      return {
        products: [],
        selectedCategory: '',
        categories: ['', 'Elektronik', 'Gıda', 'Giyim', 'Kırtasiye'],
        showEditModal: false,
        editProduct: {
          id: null,
          name: '',
          description: '',
          category: '',
          price: 0,
          stock: 0,
          imageUrl: ''
        }
      };
    },
    computed: {
      filteredProducts() {
        if (this.selectedCategory === '') return this.products;
        return this.products.filter(p => p.category === this.selectedCategory);
      }
    },
    methods: {
      async fetchProducts() {
        try {
          const token = localStorage.getItem('token');
          const res = await axios.get('http://localhost:5195/api/products/my', {
            headers: { Authorization: `Bearer ${token}` }
          });
          this.products = res.data;
        } catch (err) {
          console.error('Ürünler alınamadı:', err);
        }
      },
      openEditModal(product) {
        this.editProduct = { ...product };
        this.showEditModal = true;
      },
      closeEditModal() {
        this.showEditModal = false;
      },
      async updateProduct() {
        try {
          const token = localStorage.getItem('token');
          await axios.put(`http://localhost:5195/api/products/${this.editProduct.id}`, this.editProduct, {
            headers: { Authorization: `Bearer ${token}` }
          });
          this.closeEditModal();
          this.fetchProducts();
        } catch (err) {
          console.error('Ürün güncellenemedi:', err);
        }
      },
      async deleteProduct(id) {
        if (!confirm('Bu ürünü silmek istediğinize emin misiniz?')) return;
        try {
          const token = localStorage.getItem('token');
          await axios.delete(`http://localhost:5195/api/products/${id}`, {
            headers: { Authorization: `Bearer ${token}` }
          });
          this.fetchProducts();
        } catch (err) {
          console.error('Ürün silinemedi:', err);
        }
      }
    },
    mounted() {
      this.fetchProducts();
    }
  };
</script>

<style scoped>
  body {
    background: linear-gradient(135deg, #74ebd5 0%, #ACB6E5 100%);
  }

  /* Başlık */
  h1 {
    font-size: 2rem;
    font-weight: bold;
    color: #1f2937;
  }

  /* Kategori Butonları */
  .category-button {
    background-color: #e5e7eb;
    border: 1px solid #000;
    color: #111827;
    font-weight: 600;
    padding: 10px 18px;
    font-size: 1rem;
    border-radius: 6px;
    transition: 0.2s;
  }

    .category-button:hover {
      background-color: #cbd5e1;
    }

    .category-button.bg-blue-500 {
      background-color: #2563eb !important;
      color: white !important;
    }

  /* Ürün Grid */
  .product-grid {
    display: grid;
    grid-template-columns: repeat(3, minmax(0, 1fr));
    gap: 24px;
  }

  /* Ürün Kartları */
  .product-card {
    background-color: #d6e4f0;
    border-radius: 12px;
    box-shadow: 0 2px 12px rgba(0, 0, 0, 0.1);
    padding: 16px;
    display: flex;
    flex-direction: column;
    align-items: center;
    transition: 0.2s;
  }

    .product-card:hover {
      transform: translateY(-4px);
      box-shadow: 0 4px 20px rgba(0, 0, 0, 0.15);
    }

  /* Ürün Resmi */
  .product-image {
    width: 100%;
    max-width: 180px;
    height: 180px;
    object-fit: cover;
    border-radius: 8px;
    border: 1px solid #94a3b8;
    margin-bottom: 12px;
  }

  /* Sayfa içi butonlar */
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

  /* Renkler */
  .bg-green-500 {
    background-color: #22c55e;
  }

  .bg-yellow-500 {
    background-color: #facc15;
    color: #1f2937;
  }

  .bg-gray-400 {
    background-color: #94a3b8;
    color: white;
  }

  .bg-blue-500 {
    background-color: #2563eb;
  }

  /* Modal */
  .modal-overlay {
    position: fixed;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    background-color: rgba(0, 0, 0, 0.5);
    display: flex;
    justify-content: center;
    align-items: center;
    z-index: 1000;
  }

  .modal-content {
    background-color: white;
    padding: 24px;
    border-radius: 12px;
    width: 400px;
    box-shadow: 0 4px 20px rgba(0, 0, 0, 0.1);
  }

  .input-field {
    width: 100%;
    padding: 10px;
    border-radius: 6px;
    border: 1px solid #d1d5db;
    margin-bottom: 12px;
  }

  button {
    cursor: pointer;
  }
</style>
