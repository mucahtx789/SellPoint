import { createRouter, createWebHistory } from 'vue-router';

// Giriş ve kayıt
import LoginView from './views/LoginView.vue';
import RegisterView from './views/RegisterView.vue';
import ProductDetail from './views/ProductDetail.vue';

// Satıcı sayfaları
import SellerDashboard from './views/seller/SellerDashboard.vue';
import SellerOrders from './views/seller/SellerOrders.vue';
import Addproduct from './views/seller/AddProduct.vue';

// Müşteri sayfaları
import CustomerDashboard from './views/customer/CustomerDashboard.vue';
import CartView from './views/customer/CartView.vue';
import CheckoutView from './views/customer/CheckoutView.vue';

const routes = [
  { path: '/', redirect: '/login' },
  { path: '/login', component: LoginView },
  { path: '/register', component: RegisterView },
  { path: '/urun/:name/:id', component: ProductDetail },

  // Satıcı
  {
    path: '/seller/dashboard',
    component: SellerDashboard,
    meta: { requiresAuth: true, role: 'Seller' },
  },
  {
    path: '/seller/orders',
    component: SellerOrders,
    meta: { requiresAuth: true, role: 'Seller' },
  },
  {
    path: '/seller/add-product',
    component: Addproduct,
    meta: { requiresAuth: true, role: 'Seller' }
  },
  // Müşteri
  {
    path: '/customer/dashboard',
    component: CustomerDashboard,
    meta: { requiresAuth: true, role: 'Customer' },
  },
  {
    path: '/cart',
    component: CartView,
    meta: { requiresAuth: true, role: 'Customer' },
  },
  {
    path: '/checkout',
    component: CheckoutView,
    meta: { requiresAuth: true, role: 'Customer' },
  },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

// Route Guard
router.beforeEach((to, from, next) => {
  const token = localStorage.getItem('token');
  const role = localStorage.getItem('role');

  if (to.meta.requiresAuth) {
    if (!token) {
      return next('/login');
    }

    if (to.meta.role && to.meta.role !== role) {
      // Yanlış rol, yönlendir
      return next('/login');
    }
  }

  next();
});

export default router;
