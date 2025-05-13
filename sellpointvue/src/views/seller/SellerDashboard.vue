<template>
  <div class="p-6">
    <div class="flex justify-between items-center mb-6">
      <h1 class="text-2xl font-bold">Ürünlerim</h1>
      <div class="space-x-2">
        <button @click="$router.push('/seller/add-product')" class="bg-green-600 text-white px-4 py-2 rounded">Ürün Ekle</button>
        <button @click="$router.push('/seller/seller-orders')" class="bg-blue-600 text-white px-4 py-2 rounded">Siparişlerim</button>
      </div>
    </div>

    <!-- Kategori filtresi -->
    <div class="mb-4">
      <label for="category" class="font-semibold mr-2">Kategori:</label>
      <select v-model="selectedCategory" id="category" class="border p-1 rounded">
        <option value="">Tümü</option>
        <option>Giyim</option>
        <option>Gıda</option>
        <option>Elektronik</option>
        <option>Kırtasiye</option>
      </select>
    </div>

    <!-- Ürün listesi -->
    <div class="product-grid">
      <div v-for="product in filteredProducts" :key="product.id" class="product-card">
        <img :src="product.imageUrl" alt="Ürün Resmi" class="product-image" />
        <h2 class="font-bold text-lg mb-1">{{ product.name }}</h2>
        <p class="text-sm text-gray-700 mb-1">{{ product.description }}</p>
        <p class="text-sm mb-1"><strong>Kategori:</strong> {{ product.category }}</p>
        <p class="text-sm mb-1"><strong>Fiyat:</strong> ₺{{ product.price.toFixed(2) }}</p>
        <p class="text-sm mb-2"><strong>Stok:</strong> {{ product.stock }}</p>
        <div class="flex space-x-2">
          <button @click="openEditModal(product)" class="bg-yellow-500 text-white px-3 py-1 rounded">Düzenle</button>
          <button @click="deleteProduct(product.id)" class="bg-red-600 text-white px-3 py-1 rounded">Sil</button>
        </div>
      </div>
    </div>
  </div>
  <!-- Düzenleme Modalı -->
  <div v-if="showEditModal" class="modal-overlay">
    <div class="modal-content">
      <h2 class="text-xl font-semibold mb-4">Ürünü Düzenle</h2>

      <label>Ürün Adı:</label>
      <input v-model="editProduct.name" class="border p-1 w-full mb-2" />

      <label>Açıklama:</label>
      <textarea v-model="editProduct.description" class="border p-1 w-full mb-2" />

      <label>Kategori:</label>
      <select v-model="editProduct.category" class="border p-1 w-full mb-2">
        <option>Giyim</option>
        <option>Gıda</option>
        <option>Elektronik</option>
        <option>Kırtasiye</option>
      </select>

      <label>Fiyat:</label>
      <input type="number" v-model="editProduct.price" class="border p-1 w-full mb-2" />

      <label>Adet:</label>
      <input type="number" v-model="editProduct.stock" class="border p-1 w-full mb-2" />

      <label>Resim (URL):</label>
      <input v-model="editProduct.imageUrl" class="border p-1 w-full mb-2" />

      <div class="flex justify-end gap-2 mt-4">
        <button @click="updateProduct" class="bg-blue-500 text-white px-4 py-1 rounded">Kaydet</button>
        <button @click="closeEditModal" class="bg-gray-400 text-white px-4 py-1 rounded">İptal</button>
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
            headers: {
              Authorization: `Bearer ${token}`
            }
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
            headers: {
              Authorization: `Bearer ${token}`
            }
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
            headers: {
              Authorization: `Bearer ${token}`
            }
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
  .product-grid {
    display: flex;
    flex-wrap: wrap;
    justify-content: center;
    gap: 20px;
  }

  .product-card {
    width: 450px;
    border: 2px solid black;
    border-radius: 8px;
    padding: 16px;
    display: flex;
    flex-direction: column;
    align-items: center;
    text-align: center;
    background-color: white;
  }

  .product-image {
    width: 200px;
    height: 200px;
    object-fit: cover;
    border: 1px solid #ccc;
    margin-bottom: 12px;
  }


  /* Modal arka planı */
  .modal-overlay {
    position: fixed;
    top: 0;
    left: 0;
    width: 100vw;
    height: 100vh;
    background-color: rgba(0, 0, 0, 0.5); /* Siyah yarı saydam */
    display: flex;
    align-items: center;
    justify-content: center;
    z-index: 1000;
  }

  /* Modal içeriği */
  .modal-content {
    background-color: white;
    padding: 24px;
    border-radius: 8px;
    width: 400px;
    max-width: 90vw;
    box-shadow: 0 5px 15px rgba(0, 0, 0, 0.3);
    display: flex;
    flex-direction: column;
    gap: 16px; /* Elemanlar arasındaki mesafe */
  }

    /* Form elemanları (input, textarea, select) */
    .modal-content label {
      font-weight: bold;
      margin-bottom: 8px;
    }

    .modal-content input,
    .modal-content select,
    .modal-content textarea {
      padding: 10px;
      border: 1px solid #ccc;
      border-radius: 4px;
      width: 100%;
      box-sizing: border-box; /* Padding ve border dahil edilerek genişlik ayarlanır */
      font-size: 14px;
    }

    /* Modal alt kısmındaki butonlar */
    .modal-content .buttons {
      display: flex;
      justify-content: flex-end;
      gap: 10px;
    }

      .modal-content .buttons button {
        padding: 8px 16px;
        border-radius: 4px;
        font-size: 14px;
      }

</style>
