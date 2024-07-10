CREATE TABLE OrderItem
(
	OrderItemId INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	OrderId INT NOT NULL,
	ProductId INT NOT NULL,
	Quantity INT NOT NULL,
	Price DECIMAL(10,2) NOT NULL,
	
	FOREIGN KEY (OrderId) REFERENCES [Order](OrderId),
	FOREIGN KEY ([ProductId]) REFERENCES [Product]([ProductId])
)
