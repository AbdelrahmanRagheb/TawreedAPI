create database  [Tawreed];
USE [Tawreed];
GO

-- ============================================================
-- 1. BASE TABLES (No Foreign Dependencies)
-- ============================================================

CREATE TABLE [dbo].[users](
	[id] [uniqueidentifier] NOT NULL,
	[email] [nvarchar](255) NOT NULL,
	[password_hash] [nvarchar](255) NOT NULL,
	[phone] [nvarchar](20) NOT NULL,
	[full_name] [nvarchar](150) NOT NULL,
	[role] [nvarchar](20) NOT NULL, -- Admin, Buyer, Supplier, DeliveryPerson
	[status] [nvarchar](20) NOT NULL, -- Active, Suspended, PendingApproval
	[preferred_lang] [nchar](2) NOT NULL,
	[email_verified] [bit] NOT NULL,
	[phone_verified] [bit] NOT NULL,
	[last_login_at] [datetimeoffset](7) NULL,
	[is_deleted] [bit] NOT NULL,
	[created_at] [datetimeoffset](7) NOT NULL,
	[updated_at] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_users] PRIMARY KEY CLUSTERED ([id] ASC)
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[regions](
	[id] [uniqueidentifier] NOT NULL,
	[name_ar] [nvarchar](100) NOT NULL,
	[name_en] [nvarchar](100) NOT NULL,
	[parent_id] [uniqueidentifier] NULL,
	[is_active] [bit] NOT NULL,
	[created_at] [datetimeoffset](7) NOT NULL,
	[updated_at] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_regions] PRIMARY KEY CLUSTERED ([id] ASC)
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[categories](
	[id] [uniqueidentifier] NOT NULL,
	[name_ar] [nvarchar](100) NOT NULL,
	[name_en] [nvarchar](100) NOT NULL,
	[parent_id] [uniqueidentifier] NULL,
	[icon_url] [nvarchar](500) NULL,
	[sort_order] [int] NOT NULL,
	[is_active] [bit] NOT NULL CONSTRAINT [DF_categories_is_active] DEFAULT (1),
	[is_deleted] [bit] NOT NULL CONSTRAINT [DF_categories_is_deleted] DEFAULT (0),
	[created_at] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_categories] PRIMARY KEY CLUSTERED ([id] ASC)
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[units](
	[id] [uniqueidentifier] NOT NULL,
	[name_ar] [nvarchar](50) NOT NULL,
	[name_en] [nvarchar](50) NOT NULL,
	[symbol] [nvarchar](10) NOT NULL, -- KG, L, PCS, BOX, PACK
 CONSTRAINT [PK_units] PRIMARY KEY CLUSTERED ([id] ASC)
) ON [PRIMARY]
GO

-- ============================================================
-- 2. PROFILE & AUTHENTICATION TABLES
-- ============================================================

CREATE TABLE [dbo].[buyers](
	[user_id] [uniqueidentifier] NOT NULL,
	[business_name] [nvarchar](200) NOT NULL,
	[business_type] [nvarchar](20) NOT NULL,
	[tax_id] [nvarchar](50) NULL,
	[region_id] [uniqueidentifier] NOT NULL,
	[address] [nvarchar](500) NULL,
	[latitude] [decimal](9, 6) NULL,
	[longitude] [decimal](9, 6) NULL,
	[rating_avg] [decimal](3, 2) NOT NULL,
	[created_at] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_buyers] PRIMARY KEY CLUSTERED ([user_id] ASC)
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[suppliers](
	[user_id] [uniqueidentifier] NOT NULL,
	[company_name] [nvarchar](200) NOT NULL,
	[tax_id] [nvarchar](50) NULL,
	[region_id] [uniqueidentifier] NOT NULL,
	[is_approved] [bit] NOT NULL,
	[approved_by] [uniqueidentifier] NULL,
	[approved_at] [datetimeoffset](7) NULL,
	[rating_avg] [decimal](3, 2) NOT NULL,
	[created_at] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_suppliers] PRIMARY KEY CLUSTERED ([user_id] ASC)
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[supplier_approval_logs](
	[id] [uniqueidentifier] NOT NULL,
	[supplier_id] [uniqueidentifier] NOT NULL,
	[action] [nvarchar](20) NOT NULL, -- Approved, Rejected, Suspended, Reactivated
	[actor_id] [uniqueidentifier] NOT NULL,
	[reason] [nvarchar](500) NULL,
	[created_at] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_supplier_approval_logs] PRIMARY KEY CLUSTERED ([id] ASC)
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[supplier_categories](
	[supplier_id] [uniqueidentifier] NOT NULL,
	[category_id] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_supplier_categories] PRIMARY KEY CLUSTERED ([supplier_id], [category_id] ASC)
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[refresh_tokens](
	[id] [uniqueidentifier] NOT NULL,
	[user_id] [uniqueidentifier] NOT NULL,
	[token] [nvarchar](500) NOT NULL,
	[expires_at] [datetimeoffset](7) NOT NULL,
	[created_at] [datetimeoffset](7) NOT NULL,
	[revoked_at] [datetimeoffset](7) NULL,
 CONSTRAINT [PK_refresh_tokens] PRIMARY KEY CLUSTERED ([id] ASC)
) ON [PRIMARY]
GO

-- ============================================================
-- 3. PRODUCTS & PRICING TABLES
-- ============================================================

CREATE TABLE [dbo].[products](
	[id] [uniqueidentifier] NOT NULL,
	[supplier_id] [uniqueidentifier] NOT NULL,
	[category_id] [uniqueidentifier] NOT NULL,
	[name_ar] [nvarchar](200) NOT NULL,
	[name_en] [nvarchar](200) NOT NULL,
	[description_ar] [nvarchar](max) NULL,
	[description_en] [nvarchar](max) NULL,
	[unit_id] [uniqueidentifier] NOT NULL,
	[base_price] [decimal](12, 2) NOT NULL,
	[stock_qty] [int] NOT NULL,
	[is_active] [bit] NOT NULL,
	[is_deleted] [bit] NOT NULL,
	[created_at] [datetimeoffset](7) NOT NULL,
	[updated_at] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_products] PRIMARY KEY CLUSTERED ([id] ASC)
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[product_images](
	[id] [uniqueidentifier] NOT NULL,
	[product_id] [uniqueidentifier] NOT NULL,
	[image_url] [nvarchar](500) NOT NULL,
	[sort_order] [int] NOT NULL,
	[is_cover] [bit] NOT NULL,
 CONSTRAINT [PK_product_images] PRIMARY KEY CLUSTERED ([id] ASC)
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[pricing_tiers](
	[id] [uniqueidentifier] NOT NULL,
	[product_id] [uniqueidentifier] NOT NULL,
	[min_qty] [int] NOT NULL,
	[max_qty] [int] NULL,
	[unit_price] [decimal](12, 2) NOT NULL,
 CONSTRAINT [PK_pricing_tiers] PRIMARY KEY CLUSTERED ([id] ASC)
) ON [PRIMARY]
GO

-- ============================================================
-- 4. GROUP ORDERS & WORKFLOW TABLES
-- ============================================================

CREATE TABLE [dbo].[group_orders](
	[id] [uniqueidentifier] NOT NULL,
	[creator_id] [uniqueidentifier] NOT NULL, -- The buyer initiating the order
	[supplier_id] [uniqueidentifier] NOT NULL, -- Group order is strictly tied to 1 supplier
	[region_id] [uniqueidentifier] NOT NULL,
	[title] [nvarchar](200) NOT NULL,
	[description] [nvarchar](max) NULL,
	[deadline_at] [datetimeoffset](7) NOT NULL,
	[status] [nvarchar](20) NOT NULL, -- Draft, PendingApproval, Declined, Open, Locked, Completed, Cancelled
	[closed_at] [datetimeoffset](7) NULL,
	[total_qty] [int] NOT NULL,
	[created_at] [datetimeoffset](7) NOT NULL,
	[updated_at] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_group_orders] PRIMARY KEY CLUSTERED ([id] ASC)
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[group_order_items](
	[id] [uniqueidentifier] NOT NULL,
	[group_order_id] [uniqueidentifier] NOT NULL,
	[product_id] [uniqueidentifier] NOT NULL,
	[target_qty] [int] NOT NULL,
	[current_qty] [int] NOT NULL,
	[current_tier_price] [decimal](12, 2) NULL, -- Dynamically holds current pricing tier based on accumulated qty
	[locked_unit_price] [decimal](12, 2) NULL, -- Locked in when status becomes "Locked"
	[created_at] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_goi] PRIMARY KEY CLUSTERED ([id] ASC)
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[group_order_participants](
	[id] [uniqueidentifier] NOT NULL,
	[group_order_id] [uniqueidentifier] NOT NULL,
	[buyer_id] [uniqueidentifier] NOT NULL,
	[joined_at] [datetimeoffset](7) NOT NULL,
	[status] [nvarchar](20) NOT NULL, -- Joined, Cancelled, Confirmed, Invoiced
	[cancelled_at] [datetimeoffset](7) NULL,
 CONSTRAINT [PK_gop] PRIMARY KEY CLUSTERED ([id] ASC)
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[participant_items](
	[id] [uniqueidentifier] NOT NULL,
	[participant_id] [uniqueidentifier] NOT NULL,
	[group_order_item_id] [uniqueidentifier] NOT NULL,
	[quantity] [int] NOT NULL,
	[unit_price_at_join] [decimal](12, 2) NOT NULL,
	[final_unit_price] [decimal](12, 2) NULL,
	[line_total] [decimal](12, 2) NULL,
 CONSTRAINT [PK_pi] PRIMARY KEY CLUSTERED ([id] ASC)
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[group_order_events](
	[id] [uniqueidentifier] NOT NULL,
	[group_order_id] [uniqueidentifier] NOT NULL,
	[event_type] [nvarchar](50) NOT NULL, -- Created, SupplierApproved, SupplierDeclined, Opened, Closed, Shipped, Cancelled
	[notes_ar] [nvarchar](500) NULL,
	[notes_en] [nvarchar](500) NULL,
	[created_by] [uniqueidentifier] NOT NULL, -- References the User id who triggered the action
	[created_at] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_group_order_events] PRIMARY KEY CLUSTERED ([id] ASC)
) ON [PRIMARY]
GO

-- ============================================================
-- 5. FULFILLMENT, INVOICING, & SYSTEM TABLES
-- ============================================================

CREATE TABLE [dbo].[invoices](
	[id] [uniqueidentifier] NOT NULL,
	[invoice_number] [nvarchar](50) NOT NULL,
	[group_order_id] [uniqueidentifier] NOT NULL,
	[buyer_id] [uniqueidentifier] NOT NULL,
	[participant_id] [uniqueidentifier] NOT NULL,
	[subtotal] [decimal](12, 2) NOT NULL,
	[delivery_fee] [decimal](12, 2) NOT NULL,
	[discount_amount] [decimal](12, 2) NOT NULL,
	[total] [decimal](12, 2) NOT NULL,
	[payment_method] [nvarchar](10) NOT NULL, -- Cash, Card, Wallet
	[payment_status] [nvarchar](20) NOT NULL, -- Unpaid, Paid, Refunded
	[issued_at] [datetimeoffset](7) NOT NULL,
	[paid_at] [datetimeoffset](7) NULL,
	[shipping_address] [nvarchar](500) NOT NULL, -- Historical address snapshot
	[shipping_latitude] [decimal](9, 6) NULL,
	[shipping_longitude] [decimal](9, 6) NULL,
 CONSTRAINT [PK_invoices] PRIMARY KEY CLUSTERED ([id] ASC)
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[deliveries](
	[id] [uniqueidentifier] NOT NULL,
	[invoice_id] [uniqueidentifier] NOT NULL, -- Strict 1:1 with invoice to represent unique shipment destinations
	[group_order_id] [uniqueidentifier] NOT NULL,
	[supplier_id] [uniqueidentifier] NOT NULL,
	[delivery_person_id] [uniqueidentifier] NULL,
	[status] [nvarchar](20) NOT NULL, -- Pending, Preparing, Shipped, Delivered, Failed
	[scheduled_at] [datetimeoffset](7) NULL,
	[delivered_at] [datetimeoffset](7) NULL,
	[tracking_notes] [nvarchar](max) NULL,
	[shipping_address] [nvarchar](500) NOT NULL, -- Copied from Invoice
	[shipping_latitude] [decimal](9, 6) NULL,
	[shipping_longitude] [decimal](9, 6) NULL,
	[created_at] [datetimeoffset](7) NOT NULL,
	[updated_at] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_deliveries] PRIMARY KEY CLUSTERED ([id] ASC)
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[notifications](
	[id] [uniqueidentifier] NOT NULL,
	[user_id] [uniqueidentifier] NOT NULL,
	[type] [nvarchar](30) NOT NULL,
	[title_ar] [nvarchar](200) NOT NULL,
	[title_en] [nvarchar](200) NOT NULL,
	[body_ar] [nvarchar](max) NULL,
	[body_en] [nvarchar](max) NULL,
	[channel] [nvarchar](20) NOT NULL,
	[is_read] [bit] NOT NULL,
	[read_at] [datetimeoffset](7) NULL,
	[related_order_id] [uniqueidentifier] NULL,
	[created_at] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_notifications] PRIMARY KEY CLUSTERED ([id] ASC)
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[audit_logs](
	[id] [uniqueidentifier] NOT NULL,
	[actor_id] [uniqueidentifier] NOT NULL,
	[action] [nvarchar](100) NOT NULL,
	[entity_type] [nvarchar](50) NOT NULL,
	[entity_id] [uniqueidentifier] NULL,
	[metadata] [nvarchar](max) NULL,
	[created_at] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_audit] PRIMARY KEY CLUSTERED ([id] ASC)
) ON [PRIMARY]
GO

-- ============================================================
-- 6. SYSTEM-WIDE FOREIGN KEY CONSTRAINTS
-- ============================================================

-- Self-referential Parent relationships
ALTER TABLE [dbo].[regions] ADD CONSTRAINT [FK_regions_parent] 
FOREIGN KEY ([parent_id]) REFERENCES [dbo].[regions]([id]);

ALTER TABLE [dbo].[categories] ADD CONSTRAINT [FK_categories_parent] 
FOREIGN KEY ([parent_id]) REFERENCES [dbo].[categories]([id]);

-- User Profiles
ALTER TABLE [dbo].[buyers] ADD CONSTRAINT [FK_buyers_users] 
FOREIGN KEY ([user_id]) REFERENCES [dbo].[users]([id]) ON DELETE CASCADE;

ALTER TABLE [dbo].[buyers] ADD CONSTRAINT [FK_buyers_regions] 
FOREIGN KEY ([region_id]) REFERENCES [dbo].[regions]([id]);

ALTER TABLE [dbo].[suppliers] ADD CONSTRAINT [FK_suppliers_users] 
FOREIGN KEY ([user_id]) REFERENCES [dbo].[users]([id]) ON DELETE CASCADE;

ALTER TABLE [dbo].[suppliers] ADD CONSTRAINT [FK_suppliers_approved_by] 
FOREIGN KEY ([approved_by]) REFERENCES [dbo].[users]([id]);

ALTER TABLE [dbo].[suppliers] ADD CONSTRAINT [FK_suppliers_regions] 
FOREIGN KEY ([region_id]) REFERENCES [dbo].[regions]([id]);

-- Supplier Categories Mapping
ALTER TABLE [dbo].[supplier_categories] ADD CONSTRAINT [FK_sc_suppliers] 
FOREIGN KEY ([supplier_id]) REFERENCES [dbo].[suppliers]([user_id]) ON DELETE CASCADE;

ALTER TABLE [dbo].[supplier_categories] ADD CONSTRAINT [FK_sc_categories] 
FOREIGN KEY ([category_id]) REFERENCES [dbo].[categories]([id]) ON DELETE CASCADE;

-- Supplier Approval Auditing
ALTER TABLE [dbo].[supplier_approval_logs] ADD CONSTRAINT [FK_sal_suppliers] 
FOREIGN KEY ([supplier_id]) REFERENCES [dbo].[suppliers]([user_id]) ON DELETE CASCADE;

ALTER TABLE [dbo].[supplier_approval_logs] ADD CONSTRAINT [FK_sal_users] 
FOREIGN KEY ([actor_id]) REFERENCES [dbo].[users]([id]);

-- Auth Tokens
ALTER TABLE [dbo].[refresh_tokens] ADD CONSTRAINT [FK_refresh_tokens_users] 
FOREIGN KEY ([user_id]) REFERENCES [dbo].[users]([id]) ON DELETE CASCADE;
-- User Setup

ALTER TABLE [dbo].[users] ADD CONSTRAINT [CK_user_roles] 
CHECK ([role] IN ('Admin', 'Buyer', 'Supplier'));
-- Products Setup
ALTER TABLE [dbo].[products] ADD CONSTRAINT [FK_products_suppliers] 
FOREIGN KEY ([supplier_id]) REFERENCES [dbo].[suppliers]([user_id]);

ALTER TABLE [dbo].[products] ADD CONSTRAINT [FK_products_categories] 
FOREIGN KEY ([category_id]) REFERENCES [dbo].[categories]([id]);

ALTER TABLE [dbo].[products] ADD CONSTRAINT [FK_products_units] 
FOREIGN KEY ([unit_id]) REFERENCES [dbo].[units]([id]);

ALTER TABLE [dbo].[product_images] ADD CONSTRAINT [FK_product_images_products] 
FOREIGN KEY ([product_id]) REFERENCES [dbo].[products]([id]) ON DELETE CASCADE;

ALTER TABLE [dbo].[pricing_tiers] ADD CONSTRAINT [FK_pricing_tiers_products] 
FOREIGN KEY ([product_id]) REFERENCES [dbo].[products]([id]) ON DELETE CASCADE;

-- Group Orders
ALTER TABLE [dbo].[group_orders] ADD CONSTRAINT [FK_group_orders_creator] 
FOREIGN KEY ([creator_id]) REFERENCES [dbo].[buyers]([user_id]);

ALTER TABLE [dbo].[group_orders] ADD CONSTRAINT [FK_group_orders_suppliers] 
FOREIGN KEY ([supplier_id]) REFERENCES [dbo].[suppliers]([user_id]);

ALTER TABLE [dbo].[group_orders] ADD CONSTRAINT [FK_group_orders_regions] 
FOREIGN KEY ([region_id]) REFERENCES [dbo].[regions]([id]);

ALTER TABLE [dbo].[group_orders] ADD CONSTRAINT [CK_group_orders_status] 
CHECK ([status] IN ('Draft', 'PendingApproval', 'Declined', 'Open', 'Locked', 'Completed', 'Cancelled'));

-- Group Order Events Timeline
ALTER TABLE [dbo].[group_order_events] ADD CONSTRAINT [FK_goe_group_orders] 
FOREIGN KEY ([group_order_id]) REFERENCES [dbo].[group_orders]([id]) ON DELETE CASCADE;

ALTER TABLE [dbo].[group_order_events] ADD CONSTRAINT [FK_goe_users] 
FOREIGN KEY ([created_by]) REFERENCES [dbo].[users]([id]);

-- Group Order Items Setup
ALTER TABLE [dbo].[group_order_items] ADD CONSTRAINT [FK_goi_group_orders] 
FOREIGN KEY ([group_order_id]) REFERENCES [dbo].[group_orders]([id]) ON DELETE CASCADE;

ALTER TABLE [dbo].[group_order_items] ADD CONSTRAINT [FK_goi_products] 
FOREIGN KEY ([product_id]) REFERENCES [dbo].[products]([id]);

-- Group Order Participants Setup
ALTER TABLE [dbo].[group_order_participants] ADD CONSTRAINT [FK_gop_group_orders] 
FOREIGN KEY ([group_order_id]) REFERENCES [dbo].[group_orders]([id]);

ALTER TABLE [dbo].[group_order_participants] ADD CONSTRAINT [FK_gop_buyers] 
FOREIGN KEY ([buyer_id]) REFERENCES [dbo].[buyers]([user_id]);

ALTER TABLE [dbo].[group_order_participants] ADD CONSTRAINT [CK_gop_status] 
CHECK ([status] IN ('Joined', 'Cancelled', 'Confirmed', 'Invoiced'));

ALTER TABLE [dbo].[group_order_participants] ADD CONSTRAINT [UQ_gop_buyer_group_order] 
UNIQUE ([group_order_id], [buyer_id]);

-- Participant Items
ALTER TABLE [dbo].[participant_items] ADD CONSTRAINT [FK_pi_participants] 
FOREIGN KEY ([participant_id]) REFERENCES [dbo].[group_order_participants]([id]) ON DELETE CASCADE;

ALTER TABLE [dbo].[participant_items] ADD CONSTRAINT [FK_pi_goi] 
FOREIGN KEY ([group_order_item_id]) REFERENCES [dbo].[group_order_items]([id]);

-- Invoices
ALTER TABLE [dbo].[invoices] ADD CONSTRAINT [FK_invoices_group_orders] 
FOREIGN KEY ([group_order_id]) REFERENCES [dbo].[group_orders]([id]);

ALTER TABLE [dbo].[invoices] ADD CONSTRAINT [FK_invoices_buyers] 
FOREIGN KEY ([buyer_id]) REFERENCES [dbo].[buyers]([user_id]);

ALTER TABLE [dbo].[invoices] ADD CONSTRAINT [FK_invoices_participants] 
FOREIGN KEY ([participant_id]) REFERENCES [dbo].[group_order_participants]([id]);

ALTER TABLE [dbo].[invoices] ADD CONSTRAINT [CK_invoices_payment_status] 
CHECK ([payment_status] IN ('Unpaid', 'Paid', 'Refunded'));

-- Deliveries (1 Delivery map directly to 1 Invoice)
ALTER TABLE [dbo].[deliveries] ADD CONSTRAINT [FK_deliveries_invoices] 
FOREIGN KEY ([invoice_id]) REFERENCES [dbo].[invoices]([id]) ON DELETE CASCADE;

ALTER TABLE [dbo].[deliveries] ADD CONSTRAINT [FK_deliveries_group_orders] 
FOREIGN KEY ([group_order_id]) REFERENCES [dbo].[group_orders]([id]);

ALTER TABLE [dbo].[deliveries] ADD CONSTRAINT [FK_deliveries_suppliers] 
FOREIGN KEY ([supplier_id]) REFERENCES [dbo].[suppliers]([user_id]);

ALTER TABLE [dbo].[deliveries] ADD CONSTRAINT [FK_deliveries_delivery_person] 
FOREIGN KEY ([delivery_person_id]) REFERENCES [dbo].[users]([id]);

ALTER TABLE [dbo].[deliveries] ADD CONSTRAINT [CK_deliveries_status] 
CHECK ([status] IN ('Pending', 'Preparing', 'Shipped', 'Delivered', 'Failed'));

ALTER TABLE [dbo].[deliveries] ADD CONSTRAINT [UQ_deliveries_invoice] 
UNIQUE ([invoice_id]);

-- Notifications & Audit Logs
ALTER TABLE [dbo].[notifications] ADD CONSTRAINT [FK_notifications_users] 
FOREIGN KEY ([user_id]) REFERENCES [dbo].[users]([id]) ON DELETE CASCADE;

ALTER TABLE [dbo].[audit_logs] ADD CONSTRAINT [FK_audit_users] 
FOREIGN KEY ([actor_id]) REFERENCES [dbo].[users]([id]);
GO