# Tawreed Platform — Corrected API Specification

> **Base URL**: `/api/v1`  
> **Auth**: JWT Bearer token in `Authorization` header  
> **Localization**: `Accept-Language: en` or `ar` header; all dual fields (`NameAr`/`NameEn`) return the matching language value as a string, **not** as `{en, ar}` object  
> **Pagination**: `?page=1&limit=20` (default page=1, limit=20)  
> **Errors**: `{ "error": { "code": "string", "message": "string", "details?: any } }`

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
  "password": "string"
}
```

_(Role not needed — inferred from `User.Role`)_

**Response `200`:**

```json
{
  "token": "jwt_string",
  "refreshToken": "string",
  "user": {
    "id": "string",
    "name": "string",
    "email": "string",
    "role": "buyer | supplier | admin",
    "avatar": "string (url, nullable)"
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
  "fullName": "string",
  "email": "string",
  "phone": "string",
  "password": "string",
  "businessName": "string",
  "businessType": "string",
  "regionId": "guid",
  "address": "string"
}
```

**Response `201`:**

```json
{
  "token": "jwt_string",
  "refreshToken": "string",
  "user": {
    "id": "string",
    "name": "string",
    "email": "string",
    "role": "buyer"
  }
}
```

> Creates both `User` (with `Role='Buyer'`, `Status='Active'`) and `Buyer` profile in a single transaction.

---

### 1.3 Register (Supplier)

```
POST /auth/register/supplier
```

**Request Body:**

```json
{
  "fullName": "string",
  "email": "string",
  "phone": "string",
  "password": "string",
  "companyName": "string",
  "taxId": "string (optional)",
  "regionId": "guid",
  "categoryIds": ["guid"]
}
```

**Response `201`:**

```json
{
  "token": "jwt_string",
  "refreshToken": "string",
  "user": {
    "id": "string",
    "name": "string",
    "email": "string",
    "role": "supplier"
  },
  "status": "PendingApproval"
}
```

> Creates `User` (with `Role='Supplier'`, `Status='PendingApproval'`), `Supplier` (with `IsApproved=false`), and `SupplierCategory` entries. Account is inactive until admin approval.

---

### 1.4 Logout

```
POST /auth/logout
```

**Headers:** `Authorization: Bearer <token>`  
**Response `200`:** `{ "message": "Logged out" }`

> Revokes all refresh tokens for the user.

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
{ "token": "new_jwt_string", "refreshToken": "new_refresh_string" }
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
  "avatar": "string (url, nullable)",
  "company": "string (nullable)",
  "joinedDate": "2024-01-15",
  "address": "string (nullable)",
  "preferredLang": "en | ar"
}
```

> `company` returns `Buyer.BusinessName` or `Supplier.CompanyName` depending on role.

---

## 2. Buyer APIs

### 2.1 Buyer Dashboard

```
GET /buyer/dashboard
```

**Query Params:** `?regionId=guid` (optional, filters nearby orders by region)

**Response `200`:**

```json
{
  "activeOrders": [
    {
      "id": "string",
      "title": "string",
      "status": "Draft | PendingApproval | Open | Locked | Completed | Cancelled",
      "deadline": "2024-12-20T18:00:00",
      "participantCount": 8,
      "totalValue": 22000,
      "productCount": 2,
      "creatorName": "string",
      "region": "string (name_en)",
      "isCreator": true
    }
  ],
  "nearbyOrders": [
    {
      "id": "string",
      "creatorName": "string",
      "productName": "string",
      "quantity": 50,
      "currentParticipants": 8,
      "deadline": "2024-12-20T18:00:00",
      "region": "string"
    }
  ],
  "notifications": [
    {
      "id": "string",
      "title": "string",
      "message": "string",
      "type": "string",
      "read": false,
      "createdAt": "2024-12-01T10:30:00"
    }
  ],
  "trendingProducts": [
    {
      "id": "string",
      "name": "string",
      "price": 180,
      "imageUrl": "string",
      "categoryName": "string",
      "orderCount": 42
    }
  ],
  "totalSavings": 12450,
  "unreadNotificationCount": 3
}
```

> **Changes from original:**
>
> - `orderNumber` removed (not in DB) — use `id` as identifier
> - `title`, `host`/`creatorName`, `productName`, `categoryName` are plain strings (not `{en, ar}`)
> - `region` returns English name string
> - `status` uses DB values (Draft, Open, Locked, etc.) — not frontend-mapped statuses
> - `currentDiscount`/`projectedFinalCost` removed (computed only at order detail level)
> - `distance`, `savings`, `minJoin`, `maxParticipants`, `unitPrice` removed from nearby orders (not in schema)
> - `deadline` returns full ISO datetime

---

### 2.2 Buyer Orders — List

```
GET /buyer/orders
```

**Query Params:** `?status=Draft|PendingApproval|Open|Locked|Completed|Cancelled&page=1&limit=20`

**Response `200`:**

```json
{
  "orders": [
    {
      "id": "string",
      "title": "string",
      "status": "Draft | PendingApproval | Open | Locked | Completed | Cancelled",
      "createdAt": "2024-06-10T00:00:00",
      "deadline": "2024-06-17T18:00:00",
      "totalOrderValue": 22000,
      "participantCount": 12,
      "productCount": 2,
      "region": "string",
      "creatorName": "string",
      "supplierName": "string"
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

> **Changes:** `orderNumber`, `currentDiscount`, `projectedFinalCost`, `targetQuantity`, `currentQuantity`, `deliveryRegion`, `expectedDelivery`, `host` removed (not in DB or not feasible at list level).

---

### 2.3 Buyer Order — Detail (Group Order)

```
GET /buyer/orders/:id
```

**Response `200`:**

```json
{
  "id": "string",
  "title": "string",
  "description": "string (nullable)",
  "creatorName": "string",
  "region": "string",
  "createdAt": "2024-06-10T00:00:00",
  "deadline": "2024-06-17T18:00:00",
  "deadlinePassed": false,
  "status": "Draft | PendingApproval | Open | Locked | Completed | Cancelled",
  "totalOrderValue": 22000,
  "supplierName": "string",
  "supplierId": "string",
  "products": [
    {
      "productId": "string",
      "productName": "string",
      "currentQuantity": 45,
      "targetQuantity": 60,
      "unit": "KG",
      "currentPrice": 155,
      "originalPrice": 180,
      "discountPercent": 14
    }
  ],
  "participants": [
    {
      "id": "string",
      "name": "string",
      "joinedAt": "2024-06-10T00:00:00",
      "items": [
        { "productId": "string", "quantity": 5, "productName": "string" }
      ]
    }
  ],
  "activities": [
    {
      "id": "string",
      "eventType": "Created | SupplierApproved | SupplierDeclined | Opened | Closed | Shipped | Cancelled",
      "notes": "string (nullable)",
      "createdBy": "string",
      "createdAt": "2024-06-11T09:00:00"
    }
  ]
}
```

> **Changes from original:**
>
> - `orderNumber`, `currentDiscount`, `projectedFinalCost` removed
> - `discountAchieved` flag removed — use `discountPercent > 0`
> - `activities` uses DB values (`eventType`, `notes` not `message{en,ar}`, `createdBy` as name)
> - `supplier.deliveryRegion`, `supplier.expectedDelivery` removed
> - `avatar` on participants removed (no avatar in DB)
> - `deadlinePassed: true/false` added (computed)

---

### 2.4 Join Group Order

```
POST /buyer/orders/:id/join
```

**Request Body:**

```json
{
  "items": [{ "productId": "string", "quantity": 5 }]
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
      "currentQuantity": 50
    }
  ]
}
```

> **Changes:** `discountAchieved` flag removed.

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

> Soft-cancels the participant (sets `Status='Cancelled'`, `CancelledAt`), decreases `GroupOrderItem.CurrentQty`.

---

### 2.6 Update Participation Quantities

```
PUT /buyer/orders/:id/participants/:participantId/items
```

**Request Body:**

```json
{
  "items": [{ "productId": "string", "quantity": 8 }]
}
```

**Response `200`:**

```json
{
  "message": "Quantities updated",
  "updatedProducts": [{ "productId": "string", "currentQuantity": 48 }]
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
  "title": "string",
  "description": "string (optional)",
  "supplierId": "guid",
  "regionId": "guid",
  "deadline": "2026-06-20T18:00:00",
  "items": [
    {
      "productId": "string",
      "targetQuantity": 60
    }
  ]
}
```

> **Changes from original:**
>
> - `title` is plain string, not `{en, ar}`
> - `description` is plain string, not `{en, ar}`
> - `region` replaced with `regionId` (FK to regions table)
> - `supplierId` required (group order is tied to 1 supplier)
> - `deadlineDate` + `deadlineTime` → single `deadline` ISO datetime
> - `visibility`, `notes` removed (not in DB)
> - `unit` removed (comes from product's `Unit`)

**Response `201`:**

```json
{
  "id": "string",
  "status": "Draft",
  "totalOrderValue": 3600,
  "message": "Order created successfully"
}
```

> Created as `Draft` status. Supplier must approve to move to `Open`.

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
  "status": "Draft",
  "savedAt": "2026-06-15T10:30:00",
  "message": "Draft saved successfully"
}
```

> Uses `GroupOrder` with `Status = 'Draft'`. Drafts are user-scoped (only visible to creator).

---

### 2.9 List Drafts

```
GET /buyer/saved
```

**Response `200`:**

```json
{
  "drafts": [
    {
      "id": "string",
      "title": "string",
      "description": "string (nullable)",
      "region": "string",
      "deadline": "2026-06-20T18:00:00",
      "items": [
        {
          "productId": "string",
          "productName": "string",
          "targetQuantity": 60,
          "unit": "KG"
        }
      ],
      "totalCost": 3600,
      "savedAt": "2026-06-15T10:30:00"
    }
  ]
}
```

> **Changes:** Only drafts, no templates (no template concept in DB). Removed `visibility`, `notes`, `deadlineDate`/`deadlineTime`, `price`, `stock`, `category` from items.

---

### 2.10 Delete Draft

```
DELETE /buyer/saved/:id
```

**Response `200`:** `{ "message": "Deleted successfully" }`

> Hard-deletes the `GroupOrder` with `Status = 'Draft'` that belongs to the current user.

---

### 2.11 Publish Draft (Convert to Order)

```
POST /buyer/orders/:id/publish
```

**Response `200`:**

```json
{
  "id": "string",
  "status": "PendingApproval",
  "message": "Draft submitted for supplier approval"
}
```

> Changes status from `Draft` → `PendingApproval`. Supplier can then approve → `Open`.

---

### 2.12 Product Catalog (Buyer)

```
GET /buyer/products
```

**Query Params:** `?search=string&categoryId=guid&inStock=true&page=1&limit=20`

**Response `200`:**

```json
{
  "products": [
    {
      "id": "string",
      "name": "string",
      "description": "string (nullable)",
      "price": 180,
      "imageUrl": "string (nullable)",
      "images": [{ "url": "string", "isCover": true }],
      "categoryName": "string",
      "stock": 200,
      "unit": "KG",
      "supplierName": "string",
      "supplierId": "string"
    }
  ],
  "pagination": { "page": 1, "limit": 20, "total": 8, "totalPages": 1 }
}
```

> **Changes:** `category` is string (name), not `{en, ar}`. Added `unit`, `supplierName`, `supplierId`. `images[]` added (supports multiple product images).

---

### 2.13 Categories List (Buyer filter)

```
GET /buyer/categories
```

**Response `200`:**

```json
{
  "categories": [{ "id": "guid", "name": "Dairy", "productCount": 85 }]
}
```

> **Changes:** Returns `id` + plain `name` string + count, not `{en, ar}` + key.

---

### 2.14 Notifications — List

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
      "title": "string",
      "message": "string (nullable)",
      "type": "string",
      "read": false,
      "createdAt": "2024-12-01T10:30:00"
    }
  ],
  "unreadCount": 3,
  "pagination": { "page": 1, "limit": 20, "total": 6, "totalPages": 1 }
}
```

> **Changes:** `title`/`message` are plain strings (localized at DB query level via `Accept-Language`).

---

### 2.15 Mark Notification as Read

```
PUT /buyer/notifications/:id/read
```

**Response `200`:** `{ "message": "Marked as read" }`

---

### 2.16 Mark All Notifications as Read

```
PUT /buyer/notifications/read-all
```

**Response `200`:** `{ "message": "All marked as read" }`

---

### 2.17 Buyer Profile — Get

```
GET /buyer/profile
```

**Response `200`:**

```json
{
  "name": "Ahmed Al-Rashid",
  "email": "buyer@example.com",
  "phone": "+966 55 123 4567",
  "businessName": "ABC Trading",
  "businessType": "Retail",
  "avatar": "string (url, nullable)",
  "joinedDate": "2024-01-15",
  "address": "string (nullable)",
  "regionName": "string",
  "regionId": "guid",
  "preferredLang": "en | ar"
}
```

> **Changes from original:**
>
> - `company` as `{en, ar}` → single `businessName` string
> - `role` as `{en, ar}` removed (no role title in DB)
> - `address` as `{en, ar}` → single `address` string
> - Added `businessType`, `regionName`, `regionId`, `preferredLang`

---

### 2.18 Buyer Profile — Update

```
PUT /buyer/profile
```

**Request Body:**

```json
{
  "fullName": "string (optional)",
  "phone": "string (optional)",
  "businessName": "string (optional)",
  "address": "string (optional)",
  "avatar": "string (url, optional)",
  "preferredLang": "en | ar (optional)"
}
```

**Response `200`:** `{ "message": "Profile updated", "profile": { /* updated profile */ } }`

---

### 2.19 Product — Get Single

```
GET /buyer/products/:id
```

**Response `200`:** (single Product object, same structure as catalog item)

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
    "ratingAvg": 4.8
  },
  "pendingOrders": [
    {
      "id": "string",
      "title": "string",
      "creatorName": "string",
      "totalAmount": 5900,
      "deadline": "2024-12-20T18:00:00",
      "region": "string",
      "receivedAt": "2024-12-01T00:00:00"
    }
  ],
  "activeGroupOrders": [
    {
      "id": "string",
      "title": "string",
      "participants": 12,
      "totalValue": 22000,
      "deadline": "2024-06-17T18:00:00",
      "status": "Open | Locked"
    }
  ],
  "inventoryAlerts": [
    {
      "productId": "string",
      "productName": "string",
      "stock": 5,
      "status": "low | critical"
    }
  ],
  "deliveryOverview": {
    "pending": 3,
    "preparing": 2,
    "shipped": 1,
    "delivered": 15,
    "failed": 1
  },
  "recentActivity": [
    {
      "action": "string",
      "time": "2024-12-01T10:30:00"
    }
  ]
}
```

> **Changes from original:**
>
> - `growthRate`, `completionRate`, `acceptanceRate`, `deliverySuccessRate`, `avgFulfillmentDays` removed (not in schema)
> - `ordersRequiringAction` → `pendingOrders` (simpler)
> - `items` removed from order objects (detail only)
> - `inventoryAlerts` — `threshold` removed (not in schema), stock < 10 = low, stock = 0 = critical
> - `demandInsights` removed (no trend data)
> - `deliveryOverview` uses DB status values: `Pending, Preparing, Shipped, Delivered, Failed`
> - `delayed` removed (not tracked)
> - `recentActivity` uses full ISO timestamp instead of relative string
> - `buyerName` → `creatorName` (Buyer is always the creator)

---

### 3.2 Supplier Products — List

```
GET /supplier/products
```

**Query Params:** `?search=string&categoryId=guid&status=active|inactive&page=1&limit=20`

**Response `200`:**

```json
{
  "products": [
    {
      "id": "string",
      "name": "string",
      "price": 180,
      "stock": 200,
      "categoryName": "string",
      "categoryId": "guid",
      "isActive": true,
      "imageUrl": "string (nullable)",
      "unit": "KG",
      "unitId": "guid"
    }
  ],
  "pagination": { "page": 1, "limit": 20, "total": 6, "totalPages": 1 }
}
```

> **Changes:** `status` → `isActive` boolean; `category` is string name; `categoryId` added.

---

### 3.3 Supplier Product — Create

```
POST /supplier/products
```

**Request Body:**

```json
{
  "nameAr": "string",
  "nameEn": "string",
  "descriptionAr": "string (optional)",
  "descriptionEn": "string (optional)",
  "basePrice": 180,
  "stockQty": 200,
  "categoryId": "guid",
  "unitId": "guid"
}
```

> **Changes:** Names are separate Ar/En fields (DB stores them separately). `category` is `categoryId` (FK).

**Response `201`:**

```json
{
  "id": "string",
  "name": "string",
  "isActive": true,
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
  "product": {
    /* updated SupplierProduct */
  }
}
```

---

### 3.5 Supplier Product — Delete

```
DELETE /supplier/products/:id
```

**Response `200`:** `{ "message": "Product deleted" }`

> Soft-deletes (sets `IsDeleted = true`).

---

### 3.6 Supplier Product — Toggle Status

```
PUT /supplier/products/:id/toggle-status
```

**Response `200`:**

```json
{
  "message": "Product status changed",
  "isActive": true
}
```

> Toggles `Product.IsActive`.

---

### 3.7 Supplier Categories

```
GET /supplier/categories
```

**Response `200`:**

```json
{
  "categories": [{ "id": "guid", "name": "Dairy" }]
}
```

> Returns categories that have products linked to this supplier. Plain string name.

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
      "name": "string",
      "categoryName": "string",
      "stock": 5,
      "status": "in_stock | low | critical",
      "price": 180,
      "unit": "KG"
    }
  ],
  "summary": {
    "totalProducts": 45,
    "lowStock": 3,
    "criticalStock": 1,
    "inStock": 41
  },
  "pagination": { "page": 1, "limit": 20, "total": 45, "totalPages": 3 }
}
```

> **Changes:** `threshold`, `lastRestocked`, `alerts[]` removed (not in schema). Stock status: stock=0 → critical, stock ≤ 10 → low, else in_stock.

---

### 3.9 Supplier Inventory — Update Stock

```
PUT /supplier/inventory/:productId/stock
```

**Request Body:**

```json
{
  "stockQty": 200
}
```

**Response `200`:**

```json
{
  "message": "Stock updated",
  "product": {
    /* updated product */
  }
}
```

---

### 3.10 Supplier Incoming Orders — List

```
GET /supplier/orders
```

**Query Params:** `?status=PendingApproval|Open|Locked|Completed|Cancelled&page=1&limit=20`

**Response `200`:**

```json
{
  "orders": [
    {
      "id": "string",
      "title": "string",
      "creatorName": "string",
      "buyerCompany": "string (nullable)",
      "totalAmount": 5900,
      "status": "PendingApproval | Open | Locked | Completed | Cancelled",
      "deadline": "2024-12-20T18:00:00",
      "region": "string",
      "receivedAt": "2024-12-01T00:00:00",
      "items": [
        {
          "productId": "string",
          "productName": "string",
          "quantity": 50,
          "unitPrice": 85,
          "lineTotal": 4250
        }
      ]
    }
  ],
  "pagination": { "page": 1, "limit": 20, "total": 5, "totalPages": 1 }
}
```

> **Changes:** Status uses DB values. `buyerCompany` = `Buyer.BusinessName`. `shippingAddress`, `paymentMethod` removed (not available at this level).

---

### 3.11 Supplier Order — Accept

```
POST /supplier/orders/:id/accept
```

**Request Body (optional):**

```json
{
  "notes": "string (optional)"
}
```

**Response `200`:**

```json
{
  "message": "Order accepted",
  "orderStatus": "Open"
}
```

> Changes status from `PendingApproval` → `Open`. Creates a `GroupOrderEvent` with `EventType = 'SupplierApproved'`.

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
  "orderStatus": "Declined"
}
```

> Changes status to `Declined`. Creates a `GroupOrderEvent` with `EventType = 'SupplierDeclined'`.

---

### 3.13 Supplier Deliveries — List

```
GET /supplier/deliveries
```

**Query Params:** `?status=Pending|Preparing|Shipped|Delivered|Failed&page=1&limit=20`

**Response `200`:**

```json
{
  "deliveries": [
    {
      "id": "string",
      "orderId": "string",
      "title": "string",
      "address": "string",
      "status": "Pending | Preparing | Shipped | Delivered | Failed",
      "scheduledAt": "2024-12-05T00:00:00 (nullable)",
      "deliveredAt": "2024-12-05T14:30:00 (nullable)",
      "trackingNotes": "string (nullable)",
      "buyerName": "string",
      "items": [{ "productName": "string", "quantity": 20 }]
    }
  ],
  "pagination": { "page": 1, "limit": 20, "total": 4, "totalPages": 1 }
}
```

> **Changes:** `orderNumber` removed. `carrier` removed (not in DB). `estimatedDate` → `scheduledAt`. `buyerPhone` removed.

---

### 3.14 Supplier Delivery — Update Status

```
PUT /supplier/deliveries/:id/status
```

**Request Body:**

```json
{
  "status": "Pending | Preparing | Shipped | Delivered | Failed",
  "trackingNotes": "string (optional)",
  "scheduledAt": "2024-12-10T00:00:00 (optional)"
}
```

**Response `200`:**

```json
{
  "message": "Delivery status updated",
  "delivery": {
    /* updated Delivery */
  }
}
```

---

### 3.15 Supplier Profile — Get

```
GET /supplier/profile
```

**Response `200`:**

```json
{
  "name": "Mohammed Al-Qahtani",
  "email": "supplier@example.com",
  "phone": "+966 50 987 6543",
  "companyName": "Al-Qahtani Foodstuff",
  "taxId": "string (nullable)",
  "avatar": "string (url, nullable)",
  "joinedDate": "2023-09-01",
  "address": "string (nullable)",
  "regionName": "string",
  "regionId": "guid",
  "ratingAvg": 4.8,
  "isApproved": false,
  "preferredLang": "en | ar"
}
```

> **Changes from original:**
>
> - `company` as `{en, ar}` → single `companyName`
> - `role` as `{en, ar}` removed
> - `rating` → `ratingAvg`
> - `totalProducts`, `ordersFulfilled`, `completionRate`, `acceptanceRate`, `deliverySuccessRate`, `avgFulfillmentDays` removed (not stored)
> - Added `taxId`, `isApproved`, `preferredLang`

---

### 3.16 Supplier Profile — Update

```
PUT /supplier/profile
```

**Request Body:**

```json
{
  "fullName": "string (optional)",
  "phone": "string (optional)",
  "companyName": "string (optional)",
  "avatar": "string (url, optional)",
  "preferredLang": "en | ar (optional)"
}
```

**Response `200`:** `{ "message": "Profile updated", "profile": { /* updated profile */ } }`

---

### 3.17 Supplier Registration Status

```
GET /supplier/registration-status
```

**Response `200`:**

```json
{
  "status": "PendingApproval | Active | Suspended",
  "isApproved": false,
  "approvalLogs": [
    {
      "action": "Approved | Rejected | Suspended | Reactivated",
      "reason": "string (nullable)",
      "actorName": "string",
      "createdAt": "2024-12-01T10:30:00"
    }
  ]
}
```

> **Changes:** `rejectionReason` → use latest `approvalLogs` entry. `documentsStatus`, `reviewedBy`, `reviewedAt` removed (not in DB).

---

### 3.18 Supplier Notifications

```
GET /supplier/notifications
PUT /supplier/notifications/:id/read
PUT /supplier/notifications/read-all
```

(Same structure as Buyer Notifications — 2.14, 2.15, 2.16)

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
    "totalBuyers": 980,
    "totalOrders": 340,
    "pendingSuppliers": 8,
    "activeCategories": 7,
    "newUsersThisMonth": 124
  },
  "pendingSupplierApplications": [
    {
      "id": "string",
      "companyName": "string",
      "contactName": "string",
      "email": "string",
      "category": "string",
      "region": "string",
      "submittedAt": "2024-11-20T00:00:00"
    }
  ],
  "recentOrders": [
    {
      "id": "string",
      "title": "string",
      "buyerName": "string",
      "totalAmount": 5900,
      "status": "Open | Locked | Completed | Cancelled",
      "createdAt": "2024-12-01T00:00:00"
    }
  ]
}
```

> **Changes from original:**
>
> - `totalRevenue` removed (no global revenue aggregation yet)
> - `monthlyGrowth`, `activeBuyers` removed
> - `recentOrders` simplified: `riskLevel` removed, `orderNumber` removed
> - `systemAlerts` removed (not in DB)
> - `topRegions`, `topCategories` removed (defer to reports)
> - `category` returns string name
> - `region` returns string name

---

### 4.2 Admin Users (Buyers) — List

```
GET /admin/users
```

**Query Params:** `?search=string&status=Active|Suspended&page=1&limit=20`

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
      "role": "Buyer",
      "status": "Active | Suspended",
      "businessName": "string (nullable)",
      "region": "string (nullable)",
      "joinedDate": "2024-01-15T00:00:00",
      "lastLoginAt": "2024-12-10T00:00:00 (nullable)"
    }
  ],
  "pagination": { "page": 1, "limit": 20, "total": 980, "totalPages": 49 }
}
```

> **Changes:** `role` is plain string, not `{en, ar}`. `region` is string name. `ordersCreated`, `ordersJoined`, `completedOrders`, `cancelledOrders`, `totalSavings` removed (defer to reports).

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
  "role": "Buyer",
  "status": "Active | Suspended",
  "businessName": "string (nullable)",
  "region": "string (nullable)",
  "joinedDate": "2024-01-15T00:00:00",
  "lastLoginAt": "2024-12-10T00:00:00 (nullable)",
  "suspensionReason": "string (nullable)",
  "ordersCreated": 8,
  "ordersJoined": 42,
  "completedOrders": 38,
  "cancelledOrders": 2
}
```

> **Changes:** `totalSavings`, `activityHistory` removed. `suspensionReason` is not in schema — will be null until implemented.

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
  "status": "Suspended"
}
```

> Sets `User.Status = 'Suspended'`.

---

### 4.5 Admin User — Reactivate

```
POST /admin/users/:id/reactivate
```

**Response `200`:**

```json
{
  "message": "User reactivated",
  "status": "Active"
}
```

> Sets `User.Status = 'Active'`.

---

### 4.6 Admin User — Reset Password

```
POST /admin/users/:id/reset-password
```

**Response `200`:**

```json
{
  "message": "Password reset. Temporary password set.",
  "temporaryPassword": "Temp@123456"
}
```

> Generates a temporary password and updates `User.PasswordHash`.

---

### 4.7 Admin Suppliers — List

```
GET /admin/suppliers
```

**Query Params:** `?search=string&status=Active|PendingApproval|Suspended&categoryId=guid&regionId=guid&page=1&limit=20`

**Response `200`:**

```json
{
  "kpi": {
    "totalSuppliers": 62,
    "pendingApproval": 8,
    "approved": 48,
    "suspended": 4
  },
  "suppliers": [
    {
      "id": "string",
      "companyName": "string",
      "contactName": "string",
      "email": "string",
      "phone": "string",
      "category": "string",
      "status": "Active | PendingApproval | Suspended",
      "region": "string",
      "joinedDate": "2023-09-01T00:00:00",
      "ratingAvg": 4.8,
      "totalProducts": 45,
      "isApproved": true
    }
  ],
  "pagination": { "page": 1, "limit": 20, "total": 62, "totalPages": 4 }
}
```

> **Changes:** `rejected` status removed (use `SupplierApprovalLog` to track rejection). `ordersFulfilled`, `activeOrders`, `completionRate`, `acceptanceRate`, `deliverySuccessRate`, `avgFulfillmentDays` removed (not stored).

---

### 4.8 Admin Supplier — Detail

```
GET /admin/suppliers/:id
```

**Response `200`:**

```json
{
  "id": "string",
  "companyName": "string",
  "contactName": "string",
  "email": "string",
  "phone": "string",
  "categoryNames": ["string"],
  "region": "string",
  "status": "Active | PendingApproval | Suspended",
  "isApproved": true,
  "joinedDate": "2023-09-01T00:00:00",
  "ratingAvg": 4.8,
  "totalProducts": 45,
  "address": "string (nullable)",
  "approvalLogs": [
    {
      "action": "Approved | Rejected | Suspended | Reactivated",
      "reason": "string (nullable)",
      "actorName": "string",
      "createdAt": "2024-12-01T10:30:00"
    }
  ],
  "products": [
    {
      "name": "string",
      "categoryName": "string",
      "stock": 500,
      "unit": "L"
    }
  ]
}
```

> **Changes:** `category` → `categoryNames[]` (supplier can have multiple categories). `businessLicense`, `submittedDocuments`, `activityTimeline` removed (not in DB). `completionRate`, etc. removed.

---

### 4.9 Admin Supplier — Approve

```
POST /admin/suppliers/:id/approve
```

**Response `200`:**

```json
{
  "message": "Supplier approved",
  "status": "Active"
}
```

> Sets `Supplier.IsApproved = true`, `User.Status = 'Active'`, creates `SupplierApprovalLog` with `Action = 'Approved'`.

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
  "status": "Suspended"
}
```

> Sets `User.Status = 'Suspended'`, creates `SupplierApprovalLog` with `Action = 'Rejected'` and `Reason`.

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
  "status": "Suspended"
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
  "status": "Active"
}
```

---

### 4.13 Admin Orders — List

```
GET /admin/orders
```

**Query Params:** `?status=Draft|PendingApproval|Open|Locked|Completed|Cancelled&regionId=guid&page=1&limit=20`

**Response `200`:**

```json
{
  "kpi": {
    "totalOrders": 340,
    "open": 48,
    "locked": 32,
    "completed": 220,
    "cancelled": 25
  },
  "orders": [
    {
      "id": "string",
      "title": "string",
      "buyerName": "string",
      "buyerCompany": "string (nullable)",
      "supplierName": "string",
      "totalAmount": 5900,
      "status": "Draft | PendingApproval | Open | Locked | Completed | Cancelled",
      "region": "string",
      "createdAt": "2024-12-01T00:00:00",
      "deadline": "2024-12-20T18:00:00",
      "participants": 8
    }
  ],
  "pagination": { "page": 1, "limit": 20, "total": 340, "totalPages": 17 }
}
```

> **Changes:** `orderNumber`, `risk`, `revenue` removed. Status uses DB values.

---

### 4.14 Admin Order — Detail

```
GET /admin/orders/:id
```

**Response `200`:**

```json
{
  "id": "string",
  "title": "string",
  "buyer": {
    "id": "string",
    "name": "string",
    "company": "string (nullable)",
    "email": "string",
    "phone": "string"
  },
  "supplier": {
    "id": "string",
    "name": "string",
    "companyName": "string"
  },
  "region": "string",
  "createdAt": "2024-12-01T00:00:00",
  "deadline": "2024-12-20T18:00:00",
  "status": "string",
  "totalOrderValue": 22000,
  "items": [
    {
      "productId": "string",
      "productName": "string",
      "quantity": 45,
      "targetQuantity": 60,
      "unitPrice": 180,
      "totalPrice": 3600
    }
  ],
  "participants": [
    {
      "id": "string",
      "name": "string",
      "joinedAt": "2024-06-10T00:00:00",
      "items": [{ "productId": "string", "quantity": 5 }]
    }
  ],
  "timeline": [
    {
      "eventType": "Created | SupplierApproved | SupplierDeclined | Opened | Closed | Shipped | Cancelled",
      "notes": "string (nullable)",
      "actorName": "string",
      "createdAt": "2024-12-01T10:30:00"
    }
  ]
}
```

> **Changes:** `orderNumber`, `currentDiscount`, `projectedFinalCost`, `paymentMethod`, `shippingAddress` removed. `risk` removed. `discountPercent` removed from items. `timeline` uses DB `GroupOrderEvent` data.

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
  "status": "Cancelled"
}
```

> Sets `GroupOrder.Status = 'Cancelled'`, `ClosedAt = now`. Creates a `GroupOrderEvent` with `EventType = 'Cancelled'` and `notes = reason`.

---

### 4.16 Admin Categories — List

```
GET /admin/categories
```

**Query Params:** `?search=string&page=1&limit=20`

**Response `200`:**

```json
{
  "kpi": {
    "totalCategories": 7,
    "activeCategories": 7
  },
  "categories": [
    {
      "id": "string",
      "nameAr": "string",
      "nameEn": "string",
      "productCount": 85,
      "supplierCount": 12,
      "isActive": true,
      "sortOrder": 1
    }
  ],
  "pagination": { "page": 1, "limit": 20, "total": 7, "totalPages": 1 }
}
```

> **Changes from original:**
>
> - `name` as `{en, ar}` → separate `nameAr`/`nameEn` fields (DB stores them separately)
> - `monthlyOrders`, `growth`, `revenue`, `trend`, `supplierBreakdown`, `topProducts` removed (not in DB)
> - `hotCategories`, `inactiveCategories` removed from KPI
> - Added `sortOrder`

---

### 4.17 Admin Category — Detail

```
GET /admin/categories/:id
```

**Response `200`:**

```json
{
  "id": "string",
  "nameAr": "string",
  "nameEn": "string",
  "productCount": 85,
  "supplierCount": 12,
  "isActive": true,
  "sortOrder": 1,
  "parentId": "guid (nullable)",
  "iconUrl": "string (nullable)"
}
```

> **Changes:** `monthlyOrders`, `growth`, `revenue`, `trend`, `supplierBreakdown`, `topProducts`, `monthlyTrend` removed (not in DB).

---

### 4.18 Admin Category — Create

```
POST /admin/categories
```

**Request Body:**

```json
{
  "nameAr": "string",
  "nameEn": "string",
  "parentId": "guid (optional)",
  "iconUrl": "string (optional)",
  "sortOrder": 0
}
```

**Response `201`:**

```json
{
  "id": "string",
  "nameAr": "string",
  "nameEn": "string",
  "isActive": true,
  "message": "Category created"
}
```

---

### 4.19 Admin Category — Update

```
PUT /admin/categories/:id
```

**Request Body:**

```json
{
  "nameAr": "string (optional)",
  "nameEn": "string (optional)",
  "parentId": "guid (optional)",
  "iconUrl": "string (optional)",
  "sortOrder": 0 (optional),
  "isActive": true (optional)
}
```

**Response `200`:** `{ "message": "Category updated", "category": { /* updated */ } }`

---

### 4.20 Admin Categories — Deactivate

```
PUT /admin/categories/:id/deactivate
```

**Response `200`:** `{ "message": "Category deactivated" }`

> Sets `IsActive = false`.

---

### 4.21 Admin Categories — Activate

```
PUT /admin/categories/:id/activate
```

**Response `200`:** `{ "message": "Category activated" }`

> Sets `IsActive = true`.

---

### 4.22 Admin Regions — List

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
      "nameAr": "string",
      "nameEn": "string",
      "supplierCount": 45,
      "buyerCount": 320,
      "isActive": true
    }
  ]
}
```

> **Changes:** `name` as `{en, ar}` → separate `nameAr`/`nameEn`.

---

### 4.23 Admin Region — Create

```
POST /admin/regions
```

**Request Body:**

```json
{
  "nameAr": "string",
  "nameEn": "string",
  "parentId": "guid (optional)"
}
```

**Response `201`:**

```json
{
  "id": "string",
  "nameAr": "string",
  "nameEn": "string",
  "isActive": true,
  "message": "Region created"
}
```

---

### 4.24 Admin Region — Toggle Active

```
PUT /admin/regions/:id/toggle
```

**Response `200`:**

```json
{
  "message": "Region status changed",
  "isActive": true
}
```

---

### 4.25 Admin — Add User (manually)

```
POST /admin/users
```

**Request Body:**

```json
{
  "fullName": "string",
  "email": "string",
  "phone": "string",
  "password": "string",
  "role": "Buyer | Supplier",
  "businessName": "string (optional)"
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

### 5.1 Regions — List

```
GET /regions
```

**Response `200`:**

```json
{
  "regions": [
    {
      "id": "string",
      "nameAr": "string",
      "nameEn": "string",
      "isActive": true
    }
  ]
}
```

---

### 5.2 Categories — List

```
GET /categories
```

**Response `200`:**

```json
{
  "categories": [
    {
      "id": "string",
      "nameAr": "string",
      "nameEn": "string",
      "isActive": true
    }
  ]
}
```

---

### 5.3 Notifications — Unread Count

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
  "url": "string",
  "filename": "string",
  "size": 102400,
  "mimeType": "image/jpeg"
}
```

---

### 5.5 Search — Global

```
GET /search
```

**Query Params:** `?q=string&type=products|suppliers&page=1&limit=20`

**Response `200`:**

```json
{
  "products": [
    /* Product[] */
  ],
  "suppliers": [
    /* Supplier[] */
  ]
}
```

> **Changes:** `orders` search removed from global search. `suppliers` returns basic info only.

---
