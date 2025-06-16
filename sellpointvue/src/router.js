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
import PurchasePage from './views/customer/PurchasePage.vue';

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
    meta: { requiresAuth: true, role: 'Seller' },
  },

  // Müşteri
  {
    path: '/customer/dashboard',
    component: CustomerDashboard,
    meta: { requiresAuth: true, role: 'Customer' },
  },
  {
    path: '/customer/purchasepage',
    component: PurchasePage,
    meta: { requiresAuth: true, role: 'Customer' },
  }
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

// Route Guard
router.beforeEach((to, from, next) => {
  const token = localStorage.getItem('token');
  const role = localStorage.getItem('role');

  const publicPages = ['/login', '/register'];
  const isPublicPage = publicPages.includes(to.path);

  // 1. Giriş yapmış kullanıcı login veya register sayfasına gidemez
  if (isPublicPage && token) {
    if (role === 'Seller') return next('/seller/dashboard');
    if (role === 'Customer') return next('/customer/dashboard');
    return next(); // Rol bilinmiyorsa yine de devam
  }

  // 2. Giriş yapılmamışsa ve sayfa giriş gerektiriyorsa login'e yönlendir
  if (to.meta.requiresAuth && !token) {
    return next('/login');
  }

  // 3. Giriş yapılmış ama yetkisiz rol ile erişim varsa rolüne uygun sayfaya yönlendir
  if (to.meta.requiresAuth && to.meta.role !== role) {
    if (role === 'Seller') return next('/seller/dashboard');
    if (role === 'Customer') return next('/customer/dashboard');
    return next('/login'); // rol yoksa veya tanımsızsa
  }

  next();
});

export default router;
