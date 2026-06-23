# Tawreed Platform — Full API Specification

> **Base URL**: `/api/v1`  
> **Auth**: JWT Bearer token in `Authorization` header  
> **Localization**: `Accept-Language: en` or `ar` header; all `LocalizedString` fields return the matching language value  
> **Pagination**: `?page=1&limit=20` (default page=1, limit=20)  
> **Errors**: `{ "error": { "code": "string", "message": "string", "details?: any } }`

---

## Table of Contents

1. [Authentication](#1-authentication)
2. [Buyer APIs](#2-buyer-apis)
3. [Supplier APIs](#3-supplier-apis)
4. [Admin APIs](#4-admin-apis)
5. [Shared / Common APIs](#5-shared--common-apis)
6. [Real-time / WebSocket Events](#6-real-time--websocket-events)

---

## 1. Authentication

### 1.1 Login

```
POST /auth/login
```

**Request Body:**
```json
{
  "email": "string",
  "password": "string",
  "role": "buyer | supplier | admin"
}
```

**Response `200`:**
```json
{
  "token": "jwt_string",
  "user": {
    "id": "string",
    "name": "string",
    "email": "string",
    "role": "buyer | supplier | admin",
    "avatar": "string (url)"
  }
}
```

**Error `401`:** Invalid credentials

---

### 1.2 Register (Buyer)

```
POST /auth/register/buyer
```

**Request Body:**
```json
{
  "name": "string",
  "email": "string",
  "phone": "string",
  "password": "string",
  "companyName": { "en": "string", "ar": "string" },
  "region": "string",
  "address": { "en": "string", "ar": "string" }
}
```

**Response `201`:**
```json
{
  "token": "jwt_string",
  "user": { "id": "string", "name": "string", "email": "string", "role": "buyer" }
}
```

---

### 1.3 Register (Supplier)

```
POST /auth/register/supplier
```

**Request Body:**
```json
{
  "contactName": "string",
  "email": "string",
  "phone": "string",
  "password": "string",
  "companyName": { "en": "string", "ar": "string" },
  "category": "string",
  "region": "string",
  "address": { "en": "string", "ar": "string" },
  "businessLicense": "string (url)",
  "submittedDocuments": ["string (url)"]
}
```

**Response `201`:**
```json
{
  "token": "jwt_string",
  "user": { "id": "string", "name": "string", "email": "string", "role": "supplier" },
  "status": "pending"
}
```

> Supplier accounts start as `pending` until admin approval.

---

### 1.4 Logout

```
POST /auth/logout
```

**Headers:** `Authorization: Bearer <token>`  
**Response `200`:** `{ "message": "Logged out" }`

---

### 1.5 Refresh Token

```
POST /auth/refresh
```

**Request Body:**
```json
{ "refreshToken": "string" }
```

**Response `200`:**
```json
{ "token": "new_jwt_string" }
```

---

### 1.6 Change Password

```
PUT /auth/password
```

**Request Body:**
```json
{
  "currentPassword": "string",
  "newPassword": "string"
}
```

**Response `200`:** `{ "message": "Password updated" }`

---

### 1.7 Get Current User

```
GET /auth/me
```

**Response `200`:**
```json
{
  "id": "string",
  "name": "string",
  "email": "string",
  "phone": "string",
  "role": "buyer | supplier | admin",
  "avatar": "string",
  "company": { "en": "string", "ar": "string" },
  "joinedDate": "2024-01-15",
  "address": { "en": "string", "ar": "string" }
}
```

---

## 2. Buyer APIs

### 2.1 Buyer Dashboard

```
GET /buyer/dashboard
```

**Query Params:** `?region=string` (optional, filters nearby orders by region)

**Response `200`:**
```json
{
  "activeOrders": [
    {
      "id": "string",
      "orderNumber": "string",
      "title": { "en": "string", "ar": "string" },
      "status": "open | closing_soon | supplier_confirmed | in_transit | delivered",
      "deadline": "2024-12-20",
      "participantCount": 8,
      "totalValue": 22000,
      "currentDiscount": 3500,
      "projectedFinalCost": 18500,
      "productCount": 2,
      "host": "string",
      "region": "string",
      "isCreator": true
    }
  ],
  "nearbyOrders": [
    {
      "id": "string",
      "host": { "en": "string", "ar": "string" },
      "product": { "en": "string", "ar": "string" },
      "quantity": 50,
      "unitPrice": 72.25,
      "minJoin": 5,
      "currentParticipants": 8,
      "maxParticipants": 15,
      "deadline": "2024-12-20",
      "distance": "1.2 km",
      "savings": 15
    }
  ],
  "notifications": [
    {
      "id": "string",
      "title": { "en": "string", "ar": "string" },
      "message": { "en": "string", "ar": "string" },
      "type": "order | system | message",
      "read": false,
      "createdAt": "2024-12-01T10:30:00"
    }
  ],
  "trendingProducts": [
    {
      "id": "string",
      "name": { "en": "string", "ar": "string" },
      "price": 180,
      "imageUrl": "string",
      "category": { "en": "string", "ar": "string" },
      "orderCount": 42
    }
  ],
  "totalSavings": 12450,
  "unreadNotificationCount": 3
}
```

---

### 2.2 Buyer Orders — List

```
GET /buyer/orders
```

**Query Params:** `?status=open|closing_soon|supplier_confirmed|in_transit|delivered&page=1&limit=20`

**Response `200`:**
```json
{
  "orders": [
    {
      "id": "string",
      "orderNumber": "string",
      "title": { "en": "string", "ar": "string" },
      "status": "open | closing_soon | supplier_confirmed | in_transit | delivered",
      "createdAt": "2024-06-10",
      "deadline": "2024-06-17",
      "totalOrderValue": 22000,
      "currentDiscount": 3500,
      "projectedFinalCost": 18500,
      "participantCount": 12,
      "productCount": 2,
      "region": "Mansoura",
      "host": "string",
      "targetQuantity": 60,
      "currentQuantity": 45,
      "supplierName": "string",
      "deliveryRegion": "string",
      "expectedDelivery": "string"
    }
  ],
  "pagination": {
    "page": 1,
    "limit": 20,
    "total": 15,
    "totalPages": 1
  }
}
```

---

### 2.3 Buyer Order — Detail (Group Order)

```
GET /buyer/orders/:id
```

**Response `200`:**
```json
{
  "id": "string",
  "orderNumber": "string",
  "title": { "en": "string", "ar": "string" },
  "createdBy": "string",
  "region": "string",
  "createdAt": "2024-06-10",
  "deadline": "2024-06-17",
  "status": "open | closing_soon | supplier_confirmed | in_transit | delivered",
  "totalOrderValue": 22000,
  "currentDiscount": 3500,
  "projectedFinalCost": 18500,
  "products": [
    {
      "productId": "string",
      "productName": { "en": "string", "ar": "string" },
      "currentQuantity": 45,
      "targetQuantity": 60,
      "unit": "KG",
      "currentPrice": 155,
      "originalPrice": 180,
      "discountAchieved": true,
      "discountPercent": 14
    }
  ],
  "participants": [
    {
      "id": "string",
      "name": "string",
      "avatar": "string (url, optional)",
      "joinedAt": "2024-06-10",
      "items": [{ "productId": "string", "quantity": 5 }]
    }
  ],
  "activities": [
    {
      "id": "string",
      "type": "participant_joined | quantity_updated | discount_reached | supplier_update",
      "message": { "en": "string", "ar": "string" },
      "timestamp": "2024-06-11T09:00:00"
    }
  ],
  "supplier": {
    "id": "string",
    "name": "string",
    "deliveryRegion": "string",
    "expectedDelivery": "June 20 - June 22"
  }
}
```

**Error `404`:** Order not found

---

### 2.4 Join Group Order

```
POST /buyer/orders/:id/join
```

**Request Body:**
```json
{
  "items": [
    { "productId": "string", "quantity": 5 }
  ]
}
```

**Response `200`:**
```json
{
  "message": "Joined order successfully",
  "participant": {
    "id": "string",
    "name": "string",
    "items": [{ "productId": "string", "quantity": 5 }]
  },
  "updatedProducts": [
    {
      "productId": "string",
      "currentQuantity": 50,
      "discountAchieved": false
    }
  ]
}
```

**Error `400`:** Deadline passed / Order full / Invalid quantity

---

### 2.5 Leave Group Order

```
POST /buyer/orders/:id/leave
```

**Response `200`:**
```json
{
  "message": "Left order successfully",
  "updatedProducts": [
    {
      "productId": "string",
      "currentQuantity": 40
    }
  ]
}
```

---

### 2.6 Update Participation Quantities

```
PUT /buyer/orders/:id/participants/:participantId/items
```

**Request Body:**
```json
{
  "items": [
    { "productId": "string", "quantity": 8 }
  ]
}
```

**Response `200`:**
```json
{
  "message": "Quantities updated",
  "updatedProducts": [
    { "productId": "string", "currentQuantity": 48, "discountAchieved": true }
  ]
}
```

---

### 2.7 Create Group Order

```
POST /buyer/orders
```

**Request Body:**
```json
{
  "title": { "en": "string", "ar": "string" },
  "description": { "en": "string", "ar": "string" },
  "region": "string",
  "deadlineDate": "2026-06-20",
  "deadlineTime": "18:00",
  "visibility": "public | private",
  "notes": "string",
  "items": [
    {
      "productId": "string",
      "quantity": 20,
      "unit": "KG"
    }
  ]
}
```

**Response `201`:**
```json
{
  "id": "string",
  "orderNumber": "string",
  "status": "open",
  "totalOrderValue": 3600,
  "message": "Order created successfully"
}
```

---

### 2.8 Save Order as Draft

```
POST /buyer/orders/draft
```

**Request Body:** (same as Create Group Order)

**Response `201`:**
```json
{
  "id": "string",
  "type": "draft",
  "savedAt": "2026-06-15T10:30:00",
  "message": "Draft saved successfully"
}
```

---

### 2.9 List Drafts & Templates

```
GET /buyer/saved
```

**Query Params:** `?type=draft|template`

**Response `200`:**
```json
{
  "drafts": [
    {
      "id": "string",
      "name": "string",
      "description": "string",
      "region": "string",
      "deadlineDate": "2026-06-20",
      "deadlineTime": "18:00",
      "visibility": "public | private",
      "notes": "string",
      "items": [
        {
          "productId": "string",
          "name": "string",
          "category": "string",
          "quantity": 60,
          "price": 7.5,
          "unit": "L",
          "stock": 1500
        }
      ],
      "totalCost": 3600,
      "totalQuantity": 90,
      "savedAt": "2026-06-15T10:30:00",
      "type": "draft | template"
    }
  ],
  "templates": [ /* same structure */ ]
}
```

---

### 2.10 Delete Draft/Template

```
DELETE /buyer/saved/:id
```

**Response `200`:** `{ "message": "Deleted successfully" }`

---

### 2.11 Convert Draft to Template

```
POST /buyer/saved/:id/save-as-template
```

**Response `200`:**
```json
{
  "id": "string",
  "type": "template",
  "message": "Saved as template"
}
```

---

### 2.12 Resume Draft (Convert to Order)

```
POST /buyer/saved/:id/resume
```

**Response `200`:**
```json
{
  "order": { /* full GroupOrderDetail */ },
  "message": "Draft resumed"
}
```

---

### 2.13 Product Catalog (Buyer)

```
GET /buyer/products
```

**Query Params:** `?search=string&category=string&inStock=true&page=1&limit=20`

**Response `200`:**
```json
{
  "products": [
    {
      "id": "string",
      "name": { "en": "string", "ar": "string" },
      "description": { "en": "string", "ar": "string" },
      "price": 180,
      "imageUrl": "string",
      "category": { "en": "string", "ar": "string" },
      "stock": 200
    }
  ],
  "pagination": { "page": 1, "limit": 20, "total": 8, "totalPages": 1 }
}
```

---

### 2.14 Categories List (Buyer filter)

```
GET /buyer/categories
```

**Response `200`:**
```json
{
  "categories": [
    { "key": "dairy", "name": { "en": "Dairy", "ar": "ألبان" }, "productCount": 85 }
  ]
}
```

---

### 2.15 Notifications — List

```
GET /buyer/notifications
```

**Query Params:** `?unreadOnly=true&page=1&limit=20`

**Response `200`:**
```json
{
  "notifications": [
    {
      "id": "string",
      "title": { "en": "string", "ar": "string" },
      "message": { "en": "string", "ar": "string" },
      "type": "order | system | message",
      "read": false,
      "createdAt": "2024-12-01T10:30:00"
    }
  ],
  "unreadCount": 3,
  "pagination": { "page": 1, "limit": 20, "total": 6, "totalPages": 1 }
}
```

---

### 2.16 Mark Notification as Read

```
PUT /buyer/notifications/:id/read
```

**Response `200`:** `{ "message": "Marked as read" }`

---

### 2.17 Mark All Notifications as Read

```
PUT /buyer/notifications/read-all
```

**Response `200`:** `{ "message": "All marked as read" }`

---

### 2.18 Buyer Profile — Get

```
GET /buyer/profile
```

**Response `200`:**
```json
{
  "name": "Ahmed Al-Rashid",
  "email": "buyer@example.com",
  "phone": "+966 55 123 4567",
  "company": { "en": "string", "ar": "string" },
  "role": { "en": "Procurement Manager", "ar": "مدير المشتريات" },
  "avatar": "string",
  "joinedDate": "2024-01-15",
  "address": { "en": "string", "ar": "string" }
}
```

---

### 2.19 Buyer Profile — Update

```
PUT /buyer/profile
```

**Request Body:**
```json
{
  "name": "string",
  "phone": "string",
  "company": { "en": "string", "ar": "string" },
  "address": { "en": "string", "ar": "string" },
  "avatar": "string (url)"
}
```

**Response `200`:** `{ "message": "Profile updated", "profile": { /* updated profile */ } }`

---

### 2.20 Notification Preferences

```
GET /buyer/profile/notification-preferences
```

**Response `200`:**
```json
{
  "emailNotifications": true,
  "pushNotifications": true,
  "orderUpdates": true,
  "promotions": false,
  "systemAnnouncements": true
}
```

```
PUT /buyer/profile/notification-preferences
```

**Request Body:**
```json
{
  "emailNotifications": true,
  "pushNotifications": false,
  "orderUpdates": true,
  "promotions": false,
  "systemAnnouncements": true
}
```

**Response `200`:** `{ "message": "Preferences updated" }`

---

### 2.21 Product — Get Single

```
GET /buyer/products/:id
```

**Response `200`:** (single Product object)

---

## 3. Supplier APIs

### 3.1 Supplier Dashboard

```
GET /supplier/dashboard
```

**Response `200`:**
```json
{
  "kpi": {
    "totalRevenue": 450000,
    "totalOrders": 85,
    "activeOrders": 18,
    "pendingDeliveries": 5,
    "totalProducts": 45,
    "growthRate": 12.5,
    "completionRate": 96,
    "acceptanceRate": 97,
    "deliverySuccessRate": 95,
    "avgFulfillmentDays": 2.3
  },
  "ordersRequiringAction": [
    {
      "id": "string",
      "orderNumber": "string",
      "title": { "en": "string", "ar": "string" },
      "buyerName": "string",
      "totalAmount": 5900,
      "items": [{ "productName": { "en": "string", "ar": "string" }, "quantity": 50 }],
      "deadline": "2024-12-20",
      "region": "string",
      "receivedAt": "2024-12-01"
    }
  ],
  "activeGroupOrders": [
    {
      "id": "string",
      "title": { "en": "string", "ar": "string" },
      "participants": 12,
      "value": 22000,
      "deadline": "2024-06-17",
      "status": "open | closing_soon | supplier_confirmed"
    }
  ],
  "inventoryAlerts": [
    {
      "productId": "string",
      "productName": { "en": "string", "ar": "string" },
      "stock": 5,
      "threshold": 20,
      "status": "low | critical"
    }
  ],
  "demandInsights": [
    {
      "productName": { "en": "string", "ar": "string" },
      "orderCount": 15,
      "trend": "up | stable | down"
    }
  ],
  "deliveryOverview": {
    "preparing": 3,
    "inTransit": 2,
    "delivered": 15,
    "delayed": 1
  },
  "recentActivity": [
    {
      "action": { "en": "string", "ar": "string" },
      "time": "1 day ago"
    }
  ]
}
```

---

### 3.2 Supplier Products — List

```
GET /supplier/products
```

**Query Params:** `?search=string&category=string&status=active|inactive&page=1&limit=20`

**Response `200`:**
```json
{
  "products": [
    {
      "id": "string",
      "name": { "en": "string", "ar": "string" },
      "price": 180,
      "stock": 200,
      "category": "string",
      "status": "active | inactive",
      "image": "string (url)"
    }
  ],
  "pagination": { "page": 1, "limit": 20, "total": 6, "totalPages": 1 }
}
```

---

### 3.3 Supplier Product — Create

```
POST /supplier/products
```

**Request Body:**
```json
{
  "name": { "en": "string", "ar": "string" },
  "description": { "en": "string", "ar": "string" },
  "price": 180,
  "stock": 200,
  "category": "string",
  "image": "string (url)"
}
```

**Response `201`:**
```json
{
  "id": "string",
  "name": { "en": "string", "ar": "string" },
  "status": "active",
  "message": "Product created"
}
```

---

### 3.4 Supplier Product — Update

```
PUT /supplier/products/:id
```

**Request Body:** (same fields as create, all optional)

**Response `200`:**
```json
{
  "message": "Product updated",
  "product": { /* updated SupplierProduct */ }
}
```

---

### 3.5 Supplier Product — Delete

```
DELETE /supplier/products/:id
```

**Response `200`:** `{ "message": "Product deleted" }`

---

### 3.6 Supplier Product — Toggle Status

```
PUT /supplier/products/:id/toggle-status
```

**Response `200`:**
```json
{
  "message": "Product status changed",
  "status": "active | inactive"
}
```

---

### 3.7 Supplier Categories (for filter)

```
GET /supplier/categories
```

**Response `200`:**
```json
{
  "categories": [
    { "key": "Dairy", "name": { "en": "Dairy", "ar": "ألبان" } }
  ]
}
```

---

### 3.8 Supplier Inventory — List

```
GET /supplier/inventory
```

**Query Params:** `?search=string&filter=all|low|critical&page=1&limit=20`

**Response `200`:**
```json
{
  "products": [
    {
      "id": "string",
      "name": { "en": "string", "ar": "string" },
      "category": "string",
      "stock": 5,
      "threshold": 20,
      "status": "in_stock | low | critical",
      "price": 180,
      "lastRestocked": "2024-11-15"
    }
  ],
  "summary": {
    "totalProducts": 45,
    "lowStock": 3,
    "criticalStock": 1,
    "inStock": 41
  },
  "alerts": [
    {
      "productId": "string",
      "productName": { "en": "string", "ar": "string" },
      "stock": 5,
      "status": "low | critical"
    }
  ],
  "pagination": { "page": 1, "limit": 20, "total": 45, "totalPages": 3 }
}
```

---

### 3.9 Supplier Inventory — Update Stock

```
PUT /supplier/inventory/:productId/stock
```

**Request Body:**
```json
{
  "stock": 200
}
```

**Response `200`:**
```json
{
  "message": "Stock updated",
  "product": { /* updated product with new stock */ }
}
```

---

### 3.10 Supplier Incoming Orders — List

```
GET /supplier/orders
```

**Query Params:** `?status=pending|accepted|declined&page=1&limit=20`

**Response `200`:**
```json
{
  "orders": [
    {
      "id": "string",
      "orderNumber": "string",
      "title": { "en": "string", "ar": "string" },
      "buyerName": "string",
      "buyerCompany": "string",
      "totalAmount": 5900,
      "status": "pending | accepted | declined",
      "deadline": "2024-12-20",
      "region": "string",
      "receivedAt": "2024-12-01",
      "shippingAddress": { "en": "string", "ar": "string" },
      "paymentMethod": "string",
      "items": [
        {
          "productId": "string",
          "productName": { "en": "string", "ar": "string" },
          "quantity": 50,
          "unitPrice": 85,
          "totalPrice": 4250
        }
      ]
    }
  ],
  "pagination": { "page": 1, "limit": 20, "total": 5, "totalPages": 1 }
}
```

---

### 3.11 Supplier Order — Accept

```
POST /supplier/orders/:id/accept
```

**Request Body (optional):**
```json
{
  "estimatedDelivery": "2024-12-20",
  "notes": "string"
}
```

**Response `200`:**
```json
{
  "message": "Order accepted",
  "orderStatus": "confirmed"
}
```

---

### 3.12 Supplier Order — Decline

```
POST /supplier/orders/:id/decline
```

**Request Body:**
```json
{
  "reason": "string"
}
```

**Response `200`:**
```json
{
  "message": "Order declined",
  "orderStatus": "cancelled"
}
```

---

### 3.13 Supplier Deliveries — List

```
GET /supplier/deliveries
```

**Query Params:** `?status=preparing|in-transit|delivered|delayed&page=1&limit=20`

**Response `200`:**
```json
{
  "deliveries": [
    {
      "id": "string",
      "orderId": "string",
      "orderNumber": "string",
      "address": { "en": "string", "ar": "string" },
      "status": "preparing | in-transit | delivered | delayed",
      "estimatedDate": "2024-12-05",
      "actualDeliveryDate": "string (nullable)",
      "carrier": "string",
      "trackingNumber": "string",
      "buyerName": "string",
      "buyerPhone": "string",
      "items": [{ "productName": "string", "quantity": 20 }]
    }
  ],
  "pagination": { "page": 1, "limit": 20, "total": 4, "totalPages": 1 }
}
```

---

### 3.14 Supplier Delivery — Update Status

```
PUT /supplier/deliveries/:id/status
```

**Request Body:**
```json
{
  "status": "preparing | in-transit | delivered | delayed",
  "trackingNumber": "string (optional)",
  "carrier": "string (optional)",
  "estimatedDate": "2024-12-10 (optional)"
}
```

**Response `200`:**
```json
{
  "message": "Delivery status updated",
  "delivery": { /* updated Delivery */ }
}
```

---

### 3.15 Supplier Reports — Overview

```
GET /supplier/reports
```

**Response `200`:**
```json
{
  "kpi": {
    "totalRevenue": 450000,
    "totalOrders": 85,
    "totalProducts": 45,
    "completionRate": 96
  },
  "topProducts": [
    {
      "productName": { "en": "string", "ar": "string" },
      "totalOrders": 42,
      "totalRevenue": 75600,
      "revenueShare": 16.8
    }
  ],
  "deliveryStats": {
    "onTime": 40,
    "delayed": 2,
    "onTimeRate": 95.2,
    "avgDeliveryDays": 2.3
  },
  "monthlyRevenue": [
    { "month": "2024-01", "revenue": 35000, "orders": 7 },
    { "month": "2024-02", "revenue": 42000, "orders": 9 }
  ],
  "categoryBreakdown": [
    { "category": "Dairy", "revenue": 180000, "orderCount": 35 }
  ]
}
```

---

### 3.16 Supplier Reports — Download

```
GET /supplier/reports/:reportId/download
```

**Response `200`:** Binary file (PDF/CSV/Excel)

---

### 3.17 Supplier Notifications — List

```
GET /supplier/notifications
```

(Same structure as Buyer Notifications — 2.15)

---

### 3.18 Supplier Notification — Mark Read

```
PUT /supplier/notifications/:id/read
```

```
PUT /supplier/notifications/read-all
```

(Same as Buyer — 2.16, 2.17)

---

### 3.19 Supplier Profile — Get

```
GET /supplier/profile
```

**Response `200`:**
```json
{
  "name": "Mohammed Al-Qahtani",
  "email": "supplier@example.com",
  "phone": "+966 50 987 6543",
  "company": { "en": "string", "ar": "string" },
  "role": { "en": "Sales Director", "ar": "مدير المبيعات" },
  "avatar": "string (url)",
  "joinedDate": "2023-09-01",
  "address": { "en": "string", "ar": "string" },
  "rating": 4.8,
  "totalProducts": 45,
  "ordersFulfilled": 420,
  "completionRate": 96,
  "acceptanceRate": 97,
  "deliverySuccessRate": 95,
  "avgFulfillmentDays": 2.3
}
```

---

### 3.20 Supplier Profile — Update

```
PUT /supplier/profile
```

**Request Body:**
```json
{
  "name": "string",
  "phone": "string",
  "company": { "en": "string", "ar": "string" },
  "address": { "en": "string", "ar": "string" },
  "avatar": "string (url)"
}
```

**Response `200`:** `{ "message": "Profile updated", "profile": { /* updated profile */ } }`

---

### 3.21 Supplier Registration Status

```
GET /supplier/registration-status
```

**Response `200`:**
```json
{
  "status": "pending | approved | rejected | suspended",
  "rejectionReason": "string (nullable)",
  "documentsStatus": "pending_review | approved | rejected",
  "reviewedBy": "string (nullable)",
  "reviewedAt": "string (nullable)"
}
```

---

## 4. Admin APIs

### 4.1 Admin Dashboard

```
GET /admin/dashboard
```

**Response `200`:**
```json
{
  "kpi": {
    "totalUsers": 1280,
    "totalSuppliers": 62,
    "totalOrders": 340,
    "totalRevenue": 2850000,
    "pendingSuppliers": 8,
    "activeCategories": 7,
    "monthlyGrowth": 15.5,
    "activeBuyers": 980,
    "newThisMonth": 124
  },
  "pendingSupplierApplications": [
    {
      "id": "string",
      "companyName": { "en": "string", "ar": "string" },
      "contactName": "string",
      "email": "string",
      "category": "string",
      "region": "string",
      "submittedAt": "2024-11-20",
      "daysPending": 5
    }
  ],
  "recentOrders": [
    {
      "id": "string",
      "orderNumber": "string",
      "buyerName": "string",
      "totalAmount": 5900,
      "status": "confirmed",
      "createdAt": "2024-12-01",
      "riskLevel": "low | medium | high"
    }
  ],
  "systemAlerts": [
    {
      "id": "string",
      "type": "warning | info | error",
      "message": { "en": "string", "ar": "string" },
      "time": "2 hours ago"
    }
  ],
  "topRegions": [
    {
      "region": { "en": "Riyadh", "ar": "الرياض" },
      "orderCount": 120,
      "revenue": 950000,
      "supplierCount": 45,
      "buyerCount": 320
    }
  ],
  "topCategories": [
    {
      "category": { "en": "Dairy", "ar": "ألبان" },
      "orderCount": 1240,
      "revenue": 320000,
      "growth": 18
    }
  ]
}
```

---

### 4.2 Admin Users (Buyers) — List

```
GET /admin/users
```

**Query Params:** `?search=string&status=active|suspended&region=string&page=1&limit=20`

**Response `200`:**
```json
{
  "kpi": {
    "totalBuyers": 980,
    "activeBuyers": 920,
    "suspendedBuyers": 12,
    "newThisMonth": 124
  },
  "users": [
    {
      "id": "string",
      "name": "string",
      "email": "string",
      "phone": "string (nullable)",
      "role": { "en": "Buyer", "ar": "مشتري" },
      "status": "active | suspended",
      "businessName": { "en": "string", "ar": "string" } (nullable),
      "region": "string (nullable)",
      "joinedDate": "2024-01-15",
      "lastActive": "2024-12-10",
      "ordersCreated": 8,
      "ordersJoined": 42,
      "completedOrders": 38,
      "cancelledOrders": 2,
      "totalSavings": 12500
    }
  ],
  "pagination": { "page": 1, "limit": 20, "total": 980, "totalPages": 49 }
}
```

---

### 4.3 Admin User — Detail

```
GET /admin/users/:id
```

**Response `200`:**
```json
{
  "id": "string",
  "name": "string",
  "email": "string",
  "phone": "string",
  "role": { "en": "Buyer", "ar": "مشتري" },
  "status": "active | suspended",
  "businessName": { "en": "string", "ar": "string" },
  "region": "string",
  "joinedDate": "2024-01-15",
  "lastActive": "2024-12-10",
  "ordersCreated": 8,
  "ordersJoined": 42,
  "completedOrders": 38,
  "cancelledOrders": 2,
  "totalSavings": 12500,
  "suspensionReason": "string (nullable)",
  "activityHistory": [
    {
      "action": { "en": "string", "ar": "string" },
      "time": "2 days ago"
    }
  ]
}
```

**Error `404`:** User not found

---

### 4.4 Admin User — Suspend

```
POST /admin/users/:id/suspend
```

**Request Body:**
```json
{
  "reason": "string"
}
```

**Response `200`:**
```json
{
  "message": "User suspended",
  "status": "suspended"
}
```

---

### 4.5 Admin User — Reactivate

```
POST /admin/users/:id/reactivate
```

**Response `200`:**
```json
{
  "message": "User reactivated",
  "status": "active"
}
```

---

### 4.6 Admin User — Reset Password

```
POST /admin/users/:id/reset-password
```

**Response `200`:**
```json
{
  "message": "Password reset email sent",
  "temporaryPassword": "string (optional, if email not configured)"
}
```

---

### 4.7 Admin Suppliers — List

```
GET /admin/suppliers
```

**Query Params:** `?search=string&status=pending|approved|rejected|suspended&category=string&region=string&page=1&limit=20`

**Response `200`:**
```json
{
  "kpi": {
    "totalSuppliers": 62,
    "pendingApproval": 8,
    "approved": 48,
    "suspended": 4,
    "rejected": 2
  },
  "suppliers": [
    {
      "id": "string",
      "companyName": { "en": "string", "ar": "string" },
      "contactName": "string",
      "email": "string",
      "phone": "string",
      "category": "string",
      "status": "pending | approved | rejected | suspended",
      "region": "string",
      "joinedDate": "2023-09-01",
      "rating": 4.8,
      "totalProducts": 45,
      "ordersFulfilled": 420,
      "activeOrders": 18,
      "completionRate": 96,
      "acceptanceRate": 97,
      "deliverySuccessRate": 95,
      "avgFulfillmentDays": 2.3
    }
  ],
  "pagination": { "page": 1, "limit": 20, "total": 62, "totalPages": 4 }
}
```

---

### 4.8 Admin Supplier — Detail

```
GET /admin/suppliers/:id
```

**Response `200`:**
```json
{
  "id": "string",
  "companyName": { "en": "string", "ar": "string" },
  "contactName": "string",
  "email": "string",
  "phone": "string",
  "category": "string",
  "status": "pending | approved | rejected | suspended",
  "region": "string",
  "joinedDate": "2023-09-01",
  "rating": 4.8,
  "totalProducts": 45,
  "ordersFulfilled": 420,
  "activeOrders": 18,
  "completionRate": 96,
  "acceptanceRate": 97,
  "deliverySuccessRate": 95,
  "avgFulfillmentDays": 2.3,
  "address": { "en": "string", "ar": "string" },
  "businessLicense": "string (url)",
  "submittedDocuments": ["string (url)"],
  "activityTimeline": [
    { "action": { "en": "string", "ar": "string" }, "time": "1 day ago" }
  ],
  "products": [
    {
      "name": { "en": "string", "ar": "string" },
      "category": "string",
      "stock": 500,
      "unit": "L"
    }
  ]
}
```

---

### 4.9 Admin Supplier — Approve

```
POST /admin/suppliers/:id/approve
```

**Response `200`:**
```json
{
  "message": "Supplier approved",
  "status": "approved"
}
```

---

### 4.10 Admin Supplier — Reject

```
POST /admin/suppliers/:id/reject
```

**Request Body:**
```json
{
  "reason": "string"
}
```

**Response `200`:**
```json
{
  "message": "Supplier rejected",
  "status": "rejected"
}
```

---

### 4.11 Admin Supplier — Suspend

```
POST /admin/suppliers/:id/suspend
```

**Request Body:**
```json
{
  "reason": "string"
}
```

**Response `200`:**
```json
{
  "message": "Supplier suspended",
  "status": "suspended"
}
```

---

### 4.12 Admin Supplier — Reactivate

```
POST /admin/suppliers/:id/reactivate
```

**Response `200`:**
```json
{
  "message": "Supplier reactivated",
  "status": "approved"
}
```

---

### 4.13 Admin Orders — List

```
GET /admin/orders
```

**Query Params:** `?status=pending|confirmed|shipped|delivered|cancelled&region=string&risk=low|medium|high&page=1&limit=20`

**Response `200`:**
```json
{
  "kpi": {
    "totalOrders": 340,
    "pending": 15,
    "confirmed": 48,
    "shipped": 32,
    "delivered": 220,
    "cancelled": 25,
    "atRisk": 7,
    "totalRevenue": 2850000
  },
  "orders": [
    {
      "id": "string",
      "orderNumber": "string",
      "title": { "en": "string", "ar": "string" },
      "buyerName": "string",
      "buyerCompany": "string",
      "supplierName": "string",
      "totalAmount": 5900,
      "status": "pending | confirmed | shipped | delivered | cancelled",
      "region": "string",
      "createdAt": "2024-12-01",
      "deadline": "2024-12-20",
      "participants": 8,
      "risk": {
        "level": "low | medium | high",
        "reasons": ["string"]
      }
    }
  ],
  "pagination": { "page": 1, "limit": 20, "total": 340, "totalPages": 17 }
}
```

---

### 4.14 Admin Order — Detail

```
GET /admin/orders/:id
```

**Response `200`:**
```json
{
  "id": "string",
  "orderNumber": "string",
  "title": { "en": "string", "ar": "string" },
  "buyer": {
    "id": "string",
    "name": "string",
    "company": "string",
    "email": "string",
    "phone": "string"
  },
  "supplier": {
    "id": "string",
    "name": "string",
    "company": "string"
  },
  "region": "string",
  "createdAt": "2024-12-01",
  "deadline": "2024-12-20",
  "status": "string",
  "totalOrderValue": 22000,
  "currentDiscount": 3500,
  "projectedFinalCost": 18500,
  "paymentMethod": "string",
  "shippingAddress": { "en": "string", "ar": "string" },
  "items": [
    {
      "productId": "string",
      "productName": { "en": "string", "ar": "string" },
      "quantity": 45,
      "targetQuantity": 60,
      "unitPrice": 180,
      "totalPrice": 3600,
      "currentPrice": 155,
      "discountPercent": 14
    }
  ],
  "participants": [
    {
      "id": "string",
      "name": "string",
      "joinedAt": "2024-06-10",
      "items": [{ "productId": "string", "quantity": 5 }]
    }
  ],
  "timeline": [
    { "event": "Order created", "timestamp": "2024-12-01", "actor": "string" },
    { "event": "Supplier confirmed", "timestamp": "2024-12-03", "actor": "string" },
    { "event": "Payment received", "timestamp": "2024-12-04", "actor": "string" }
  ],
  "risk": {
    "level": "low | medium | high",
    "alerts": [
      {
        "type": "warning | critical",
        "message": "string",
        "resolved": false
      }
    ]
  }
}
```

---

### 4.15 Admin Order — Force Close

```
POST /admin/orders/:id/force-close
```

**Request Body:**
```json
{
  "reason": "string"
}
```

**Response `200`:**
```json
{
  "message": "Order force closed",
  "status": "cancelled"
}
```

---

### 4.16 Admin Order — Resolve Risk Alert

```
PUT /admin/orders/:orderId/risk/:alertId/resolve
```

**Response `200`:**
```json
{
  "message": "Risk alert resolved",
  "riskLevel": "low"
}
```

---

### 4.17 Admin Categories — List

```
GET /admin/categories
```

**Query Params:** `?search=string&page=1&limit=20`

**Response `200`:**
```json
{
  "kpi": {
    "totalCategories": 7,
    "hotCategories": 3,
    "activeCategories": 7,
    "inactiveCategories": 0
  },
  "categories": [
    {
      "id": "string",
      "name": { "en": "string", "ar": "string" },
      "productCount": 85,
      "supplierCount": 12,
      "active": true,
      "monthlyOrders": 1240,
      "growth": 18,
      "revenue": 320000,
      "trend": "hot | growing | stable | declining",
      "supplierBreakdown": { "large": 3, "medium": 5, "small": 4 },
      "topProducts": [
        { "name": "Milk", "orders": 420 }
      ]
    }
  ],
  "pagination": { "page": 1, "limit": 20, "total": 7, "totalPages": 1 }
}
```

---

### 4.18 Admin Category — Detail

```
GET /admin/categories/:id
```

**Response `200`:**
```json
{
  "id": "string",
  "name": { "en": "string", "ar": "string" },
  "productCount": 85,
  "supplierCount": 12,
  "active": true,
  "monthlyOrders": 1240,
  "growth": 18,
  "revenue": 320000,
  "trend": "hot | growing | stable | declining",
  "supplierBreakdown": { "large": 3, "medium": 5, "small": 4 },
  "topProducts": [{ "name": "Milk", "orders": 420 }],
  "monthlyTrend": [
    { "month": "2024-06", "orders": 1100, "revenue": 290000 },
    { "month": "2024-07", "orders": 1240, "revenue": 320000 }
  ]
}
```

---

### 4.19 Admin Category — Create

```
POST /admin/categories
```

**Request Body:**
```json
{
  "name": { "en": "string", "ar": "string" }
}
```

**Response `201`:**
```json
{
  "id": "string",
  "name": { "en": "string", "ar": "string" },
  "active": true,
  "message": "Category created"
}
```

---

### 4.20 Admin Category — Update

```
PUT /admin/categories/:id
```

**Request Body:**
```json
{
  "name": { "en": "string", "ar": "string" },
  "active": true
}
```

**Response `200`:** `{ "message": "Category updated" }`

---

### 4.21 Admin Category — Deactivate

```
PUT /admin/categories/:id/deactivate
```

**Request Body (optional):**
```json
{
  "reassignProductsTo": "categoryId (string, optional)"
}
```

**Response `200`:** `{ "message": "Category deactivated" }`

---

### 4.22 Admin Category — Merge

```
POST /admin/categories/:id/merge
```

**Request Body:**
```json
{
  "targetCategoryId": "string"
}
```

**Response `200`:** `{ "message": "Categories merged" }`

---

### 4.23 Admin Regions — List

```
GET /admin/regions
```

**Query Params:** `?search=string`

**Response `200`:**
```json
{
  "regions": [
    {
      "id": "string",
      "name": { "en": "string", "ar": "string" },
      "supplierCount": 45,
      "buyerCount": 320,
      "active": true
    }
  ]
}
```

---

### 4.24 Admin Region — Create

```
POST /admin/regions
```

**Request Body:**
```json
{
  "name": { "en": "string", "ar": "string" }
}
```

**Response `201`:**
```json
{
  "id": "string",
  "name": { "en": "string", "ar": "string" },
  "active": true,
  "message": "Region created"
}
```

---

### 4.25 Admin Region — Toggle Active

```
PUT /admin/regions/:id/toggle
```

**Response `200`:**
```json
{
  "message": "Region status changed",
  "active": true
}
```

---

### 4.26 Admin Reports — List

```
GET /admin/reports
```

**Response `200`:**
```json
{
  "reports": [
    {
      "id": "string",
      "title": { "en": "Monthly Revenue Report", "ar": "تقرير الإيرادات الشهري" },
      "type": { "en": "Financial", "ar": "مالي" },
      "period": "November 2024",
      "generatedAt": "2024-12-01",
      "total": 450000,
      "growth": 12.3,
      "availableFormats": ["pdf", "csv", "xlsx"]
    }
  ]
}
```

---

### 4.27 Admin Report — Generate

```
POST /admin/reports/generate
```

**Request Body:**
```json
{
  "type": "revenue | orders | users | suppliers | categories | regions",
  "period": "this_month | last_month | quarter | year",
  "format": "pdf | csv | xlsx"
}
```

**Response `201`:**
```json
{
  "id": "string",
  "status": "generating | ready",
  "message": "Report generation started"
}
```

---

### 4.28 Admin Report — Download

```
GET /admin/reports/:id/download
```

**Query Params:** `?format=pdf|csv|xlsx`

**Response `200`:** Binary file stream

---

### 4.29 Admin Settings — Get

```
GET /admin/settings
```

**Response `200`:**
```json
{
  "general": {
    "platformName": { "en": "Tawreed", "ar": "توريد" },
    "supportEmail": "support@tawreed.com",
    "supportPhone": "+966 800 123 4567",
    "currency": "SAR",
    "language": "en | ar"
  },
  "security": {
    "minPasswordLength": 8,
    "twoFactorRequired": false,
    "sessionTimeout": 60
  },
  "payment": {
    "gateway": "stripe | paytabs | moyasar",
    "commissionRate": 2.5,
    "minimumPayout": 500
  },
  "registration": {
    "enableUserRegistration": true,
    "requireSupplierApproval": true,
    "allowedDomains": ["string"]
  }
}
```

---

### 4.30 Admin Settings — Update

```
PUT /admin/settings
```

**Request Body:**
```json
{
  "general": { /* partial */ },
  "security": { /* partial */ },
  "payment": { /* partial */ },
  "registration": { /* partial */ }
}
```

**Response `200`:** `{ "message": "Settings updated" }`
    

### 4.31 Admin Settings — Toggle Registration

```
PUT /admin/settings/registration/toggle
```

**Request Body:**
```json
{
  "enableUserRegistration": true,
  "requireSupplierApproval": false
}
```

**Response `200`:** `{ "message": "Registration settings updated" }`

---

### 4.32 Admin — Add User (manually)

```
POST /admin/users
```

**Request Body:**
```json
{
  "name": "string",
  "email": "string",
  "phone": "string",
  "password": "string",
  "role": "buyer | supplier",
  "businessName": { "en": "string", "ar": "string" },
  "region": "string"
}
```

**Response `201`:**
```json
{
  "id": "string",
  "message": "User created"
}
```

---

## 5. Shared / Common APIs

### 5.1 Regions — List (all roles)

```
GET /regions
```

**Response `200`:**
```json
{
  "regions": [
    {
      "id": "string",
      "name": { "en": "Riyadh", "ar": "الرياض" },
      "active": true
    }
  ]
}
```

---

### 5.2 Categories — List (all roles)

```
GET /categories
```

**Response `200`:**
```json
{
  "categories": [
    {
      "id": "string",
      "name": { "en": "Dairy", "ar": "ألبان" },
      "active": true
    }
  ]
}
```

---

### 5.3 Notifications — Unread Count (all roles)

```
GET /notifications/unread-count
```

**Response `200`:**
```json
{
  "count": 3
}
```

---

### 5.4 Upload File

```
POST /upload
```

**Request:** `multipart/form-data` with field `file`

**Response `201`:**
```json
{
  "url": "https://cdn.tawreed.com/uploads/filename.jpg",
  "filename": "filename.jpg",
  "size": 102400,
  "mimeType": "image/jpeg"
}
```

---

### 5.5 Upload Multiple Files

```
POST /upload/multiple
```

**Request:** `multipart/form-data` with field `files[]`

**Response `201`:**
```json
{
  "files": [
    { "url": "string", "filename": "string", "size": 123, "mimeType": "string" }
  ]
}
```

---

### 5.6 Search — Global

```
GET /search
```

**Query Params:** `?q=string&type=products|orders|suppliers&page=1&limit=20`

**Response `200`:**
```json
{
  "results": {
    "products": [ /* Product[] */ ],
    "orders": [ /* Order[] */ ],
    "suppliers": [ /* SupplierEntry[] */ ]
  }
}
```

---

## 6. Real-time / WebSocket Events

**Connection:** `wss://api.tawreed.com/ws?token=jwt_string`

### 6.1 Events the Client Receives

| Event | Payload | Description |
|-------|---------|-------------|
| `notification:new` | `{ notification: Notification }` | New notification for the user |
| `order:updated` | `{ orderId, status, changes }` | Order status / quantity / discount changed |
| `order:participant_joined` | `{ orderId, participant }` | Someone joined a group order |
| `order:discount_reached` | `{ orderId, productId, discountPercent }` | A product hit its discount threshold |
| `order:deadline_approaching` | `{ orderId, deadline }` | Order deadline approaching (24h/6h/1h) |
| `inventory:alert` | `{ productId, stock, status }` | Supplier — stock hit low/critical threshold |
| `supplier:status_changed` | `{ status, reason? }` | Supplier account approved/rejected/suspended |

### 6.2 Events the Client Sends

| Event | Payload | Description |
|-------|---------|-------------|
| `subscribe:order` | `{ orderId }` | Subscribe to real-time updates for a specific order |
| `unsubscribe:order` | `{ orderId }` | Unsubscribe from an order's updates |
| `typing` | `{ orderId, userId }` | User is typing (for chat) |

### 6.3 Chat / Messaging

```
POST /messages
```

**Request Body:**
```json
{
  "orderId": "string",
  "receiverId": "string",
  "message": { "en": "string", "ar": "string" }
}
```

**Response `201`:**
```json
{
  "id": "string",
  "senderId": "string",
  "receiverId": "string",
  "message": { "en": "string", "ar": "string" },
  "timestamp": "2024-12-01T10:30:00",
  "read": false
}
```

```
GET /messages/:orderId
```

**Query Params:** `?page=1&limit=50`

**Response `200`:**
```json
{
  "messages": [ /* Message[] */ ],
  "pagination": { "page": 1, "limit": 50, "total": 120 }
}
```

---

## Data Models Summary

| Model | Key Fields |
|-------|-----------|
| **User** | id, name, email, phone, role, avatar, company, joinedDate, address |
| **Product** | id, name(LocalizedString), description(LS), price, imageUrl, category(LS), stock |
| **Order** | id, orderNumber, items(OrderItem[]), totalAmount, status, createdAt, estimatedDelivery, paymentMethod, shippingAddress(LocalizedString), trackingNumber |
| **GroupOrderDetail** | id, orderNumber, title(LS), createdBy, region, createdAt, deadline, status(GroupOrderStatus), participants[], products[], activities[], totalOrderValue, currentDiscount, projectedFinalCost, supplier{name,deliveryRegion,expectedDelivery} |
| **GroupOrderParticipant** | id, name, avatar?, joinedAt, items[{productId,quantity}] |
| **GroupOrderProductDetail** | productId, productName(LS), currentQuantity, targetQuantity, unit, currentPrice, originalPrice, discountAchieved, discountPercent |
| **GroupOrderActivity** | id, type, message(LS), timestamp |
| **Notification** | id, title(LS), message(LS), type(order/system/message), read, createdAt |
| **SupplierProduct** | id, name(LS), price, stock, category, status(active/inactive), image |
| **Delivery** | id, orderId, orderNumber, address(LS), status(preparing/in-transit/delivered/delayed), estimatedDate, carrier, trackingNumber |
| **SupplierEntry** | id, companyName(LS), contactName, email, phone, category, status, region, joinedDate, rating, totalProducts, ordersFulfilled, completionRate, acceptanceRate, deliverySuccessRate, avgFulfillmentDays, address, businessLicense, submittedDocuments[], activityTimeline[], products[] |
| **UserEntry** | id, name, email, phone, role(LS), status(active/suspended), businessName(LS), region, joinedDate, lastActive, ordersCreated, ordersJoined, completedOrders, cancelledOrders, totalSavings, suspensionReason?, activityHistory[] |
| **CategoryEntry** | id, name(LS), productCount, supplierCount, active, monthlyOrders, growth, revenue, trend, supplierBreakdown, topProducts[] |
| **RegionEntry** | id, name(LS), supplierCount, buyerCount, active |
| **SavedOrderDraft** | id, name, description, region, deadlineDate, deadlineTime, visibility(public/private), notes, items(DraftCartItem[]), totalCost, totalQuantity, savedAt, type(draft/template) |
| **NearbyOrder** | id, host(LS), product(LS), quantity, unitPrice, minJoin, currentParticipants, maxParticipants, deadline, distance, savings |
| **ReportEntry** | id, title(LS), type(LS), period, generatedAt, total, growth |
| **AdminStats** | totalUsers, totalSuppliers, totalOrders, totalRevenue, pendingSuppliers, activeCategories, monthlyGrowth |
| **Message** | id, senderId, receiverId, orderId, message(LS), timestamp, read |

---

## API Grouped by Feature (Quick Reference)

| Feature | Endpoints |
|---------|-----------|
| **Auth** | `POST /auth/login`, `POST /auth/register/buyer`, `POST /auth/register/supplier`, `POST /auth/logout`, `POST /auth/refresh`, `PUT /auth/password`, `GET /auth/me` |
| **Buyer Dashboard** | `GET /buyer/dashboard` |
| **Buyer Orders** | `GET /buyer/orders`, `GET /buyer/orders/:id`, `POST /buyer/orders`, `POST /buyer/orders/:id/join`, `POST /buyer/orders/:id/leave`, `PUT /buyer/orders/:id/participants/:pid/items` |
| **Buyer Drafts** | `POST /buyer/orders/draft`, `GET /buyer/saved`, `DELETE /buyer/saved/:id`, `POST /buyer/saved/:id/save-as-template`, `POST /buyer/saved/:id/resume` |
| **Buyer Products** | `GET /buyer/products`, `GET /buyer/products/:id`, `GET /buyer/categories` |
| **Buyer Notifications** | `GET /buyer/notifications`, `PUT /buyer/notifications/:id/read`, `PUT /buyer/notifications/read-all` |
| **Buyer Profile** | `GET /buyer/profile`, `PUT /buyer/profile`, `GET /buyer/profile/notification-preferences`, `PUT /buyer/profile/notification-preferences` |
| **Supplier Dashboard** | `GET /supplier/dashboard` |
| **Supplier Products** | `GET /supplier/products`, `POST /supplier/products`, `PUT /supplier/products/:id`, `DELETE /supplier/products/:id`, `PUT /supplier/products/:id/toggle-status`, `GET /supplier/categories` |
| **Supplier Inventory** | `GET /supplier/inventory`, `PUT /supplier/inventory/:productId/stock` |
| **Supplier Orders** | `GET /supplier/orders`, `POST /supplier/orders/:id/accept`, `POST /supplier/orders/:id/decline` |
| **Supplier Deliveries** | `GET /supplier/deliveries`, `PUT /supplier/deliveries/:id/status` |
| **Supplier Reports** | `GET /supplier/reports`, `GET /supplier/reports/:reportId/download` |
| **Supplier Notifications** | `GET /supplier/notifications`, `PUT /supplier/notifications/:id/read`, `PUT /supplier/notifications/read-all` |
| **Supplier Profile** | `GET /supplier/profile`, `PUT /supplier/profile`, `GET /supplier/registration-status` |
| **Admin Dashboard** | `GET /admin/dashboard` |
| **Admin Users** | `GET /admin/users`, `POST /admin/users`, `GET /admin/users/:id`, `POST /admin/users/:id/suspend`, `POST /admin/users/:id/reactivate`, `POST /admin/users/:id/reset-password` |
| **Admin Suppliers** | `GET /admin/suppliers`, `GET /admin/suppliers/:id`, `POST /admin/suppliers/:id/approve`, `POST /admin/suppliers/:id/reject`, `POST /admin/suppliers/:id/suspend`, `POST /admin/suppliers/:id/reactivate` |
| **Admin Orders** | `GET /admin/orders`, `GET /admin/orders/:id`, `POST /admin/orders/:id/force-close`, `PUT /admin/orders/:orderId/risk/:alertId/resolve` |
| **Admin Categories** | `GET /admin/categories`, `GET /admin/categories/:id`, `POST /admin/categories`, `PUT /admin/categories/:id`, `PUT /admin/categories/:id/deactivate`, `POST /admin/categories/:id/merge` |
| **Admin Regions** | `GET /admin/regions`, `POST /admin/regions`, `PUT /admin/regions/:id/toggle` |
| **Admin Reports** | `GET /admin/reports`, `POST /admin/reports/generate`, `GET /admin/reports/:id/download` |
| **Admin Settings** | `GET /admin/settings`, `PUT /admin/settings`, `PUT /admin/settings/registration/toggle` |
| **Shared** | `GET /regions`, `GET /categories`, `GET /notifications/unread-count`, `GET /search`, `POST /upload`, `POST /upload/multiple` |
| **Messaging** | `POST /messages`, `GET /messages/:orderId` |
| **WebSocket** | `wss://api.tawreed.com/ws?token=jwt` |
