USE [master]
GO

DROP DATABASE [MomNKidStore]
GO

CREATE DATABASE [MomNKidStore]
GO

USE [MomNKidStore]
GO

CREATE TABLE [dbo].[Account](
	[accountId] [int] PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[roleId] [int] NOT NULL,
	[Email] [nvarchar](64) NOT NULL,
	[password] [nvarchar](Max) NOT NULL,
	[status] [Bit] NOT NULL
)
GO

CREATE TABLE [dbo].[Customer](
	[customerId] [int] PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[accountId] [int] UNIQUE NOT NULL,
	[userName] [nvarchar](50) NOT NULL,
	[Phone] [nvarchar](10),
	[Address] [nvarchar](30) ,
	[dob] [date],
	[point] [int] CHECK (point >= 0),
	[status] [Bit] NOT NULL
	CONSTRAINT FK_accountId_Customer FOREIGN KEY ([accountId]) REFERENCES [dbo].[Account]([accountId])
)
GO

CREATE TABLE [dbo].[ProductCategory](
	[productCategoryId] [int] PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[productCategoryName] [nvarchar](50)	NOT NULL,
	[productCategoryStatus] [bit] NOT NULL
)
GO

CREATE TABLE [dbo].[Product](
	[productId] [int] PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[productCategoryId] [int] NOT NULL,
	[productName] [nvarchar](50)	NOT NULL,
	[productInfor] [nvarchar](250) NOT NULL,
	[productPrice] [decimal](13,2) NOT NULL CHECK ([productPrice] >= 0),
	[productQuantity] [int] NOT NULL CHECK ([productQuantity] >= 0),
	[productStatus] [int] NOT NULL
	CONSTRAINT FK_productCategoryId_Product FOREIGN KEY ([productCategoryId]) REFERENCES [dbo].[ProductCategory]([productCategoryId])
)
GO

-- Create trigger to update status based on quantity
CREATE TRIGGER trg_UpdateProductStatus
ON [dbo].[Product]
AFTER UPDATE
AS
BEGIN
    UPDATE [dbo].[Product]
    SET [productStatus] = 0
    WHERE [productQuantity] = 0 AND [productId] IN (SELECT [productId] FROM inserted);
END;
GO

CREATE TABLE [dbo].[ImageProduct](
	[imageId] [int] PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[productId] [int] NOT NULL,
	[imageProduct] [nvarchar](Max) NOT NULL
	CONSTRAINT FK_productId_ImageProduct FOREIGN KEY ([productId]) REFERENCES [dbo].[Product]([productId])
)
GO

CREATE TABLE [dbo].[Cart](
	[cartId] [int] PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[productId] [int] NOT NULL,
	[customerId] [int] NOT NULL,
	[cartQuantity] [int] NOT NULL,
	[status] [bit] NOT NULL
	CONSTRAINT FK_productId_Cart FOREIGN KEY ([productId]) REFERENCES [dbo].[Product]([productId]),
	CONSTRAINT FK_customerId_Cart FOREIGN KEY ([customerId]) REFERENCES [dbo].[Customer]([customerId])
)
GO

CREATE TABLE [dbo].[Blog](
	[blogId] [int] PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[blogTitle] [nvarchar](50) NOT NULL,
	[blogContent] [nvarchar](350) NOT NULL,
	[blogImage] [nvarchar](MAX),
	[status] [bit] NOT NULL
)
GO

CREATE TABLE [dbo].[BlogProduct](
	[blogProductId] [int] PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[blogId] [int]  NOT NULL,
	[productId] [int] NOT NULL,
	[status] [bit] NOT NULL
	CONSTRAINT FK_productId_BlogProduct FOREIGN KEY ([productId]) REFERENCES [dbo].[Product]([productId]),
	CONSTRAINT FK_blogId_BlogProduct FOREIGN KEY ([blogId]) REFERENCES [dbo].[Blog]([blogId])
)
GO

CREATE TABLE [dbo].[VoucherOfShop](
	[voucherId] [int] PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[voucherValue] [float] NOT NULL,
	[StartDate] [Date] NOT NULL,
	[voucherQuantity] int NOT NULL CHECK ([voucherQuantity] >= 0),
	[EndDate] [Date] NOT NULL,
	[status] [bit] NOT NULL
)
GO

CREATE TABLE [dbo].[Order](
	[orderId] [int] PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[customerId] [int] NOT NULL,
	[voucherId] int,
	[exchangedPoint] int,
	[orderDate] [DateTime] NOT NULL,
	[TotalPrice] [decimal](13,2) NOT NULL CHECK([TotalPrice] >= 0),
	[status] [int] NOT NULL
	CONSTRAINT FK_customerId_Order FOREIGN KEY ([customerId]) REFERENCES [dbo].[Customer]([customerId]),
	CONSTRAINT FK_voucherId_Order FOREIGN KEY ([voucherId]) REFERENCES [dbo].[VoucherOfShop]([voucherId])
)
GO

CREATE TABLE [dbo].[OrderDetail](
	[orderDetailId] [int] PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[orderId] [int] NOT NULL,
	[productId] [int] NOT NULL,
	[orderQuantity] [int] NOT NULL,
	[productPrice] [float] NOT NULL,
	[status] [bit] NOT NULL
	CONSTRAINT FK_productId_OrderDetail FOREIGN KEY ([productId]) REFERENCES [dbo].[Product]([productId]),
	CONSTRAINT FK_orderId_OrderDetail FOREIGN KEY ([orderId]) REFERENCES [dbo].[Order]([orderId]),
)
GO

CREATE TABLE [dbo].[Payment] (
	PaymentId INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
	orderId INT UNIQUE NOT NULL,
	PaymentMethod NVARCHAR(100),
	BankCode NVARCHAR(MAX),
	BankTranNo NVARCHAR(MAX),
	CardType NVARCHAR(MAX),
	PaymentInfo NVARCHAR(MAX),
	PayDate DATETIME,
	TransactionNo NVARCHAR(MAX),
	TransactionStatus INT,
	PaymentAmount DECIMAL(13,2),
	CONSTRAINT FK_orderId_Payments FOREIGN KEY (orderId) REFERENCES [dbo].[Order](orderId)
)
GO

CREATE TABLE [dbo].[Feedback](
	[feedbackId] [int] PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[customerId] [int] NOT NULL,
	[productId] [int] NOT NULL,
	[feedbackContent] [nvarchar](250) NOT NULL,
    [rateNumber] [float] NOT NULL CHECK([rateNumber] >= 0 AND [rateNumber] <= 5),
	[status] [bit] NOT NULL
	CONSTRAINT FK_customerId_Feedback FOREIGN KEY ([customerId]) REFERENCES [dbo].[Customer]([customerId]),
	CONSTRAINT FK_productId_Feedback FOREIGN KEY ([productId]) REFERENCES [dbo].[Product]([productId])
)
GO

create table ChatRequest (
	MessageId INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[customerId] INT NOT NULL,
	Type NVARCHAR(256) NOT NULL,
	Content NVARCHAR(MAX),
	SendTime DATETIME,
	Status INT NOT NULL
)
GO