CREATE PROCEDURE AddOrderItem
	
	@OrderId INT,
	@ProductId INT,
	@Quantity INT,
	@Price DECIMAL(10, 2)

AS
BEGIN

	INSERT INTO OrderItem (OrderId, ProductId, Quantity, Price)
	VALUES (@OrderId, @ProductId, @Quantity, @Price);

END