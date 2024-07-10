CREATE PROCEDURE UpdateOrderItem
	
	@OrderItemId INT,
	@OrderId INT,
	@ProductId INT,
	@Quantity INT,
	@Price DECIMAL(10, 2)
	
AS
BEGIN

	UPDATE OrderItem
	SET
		OrderId = @OrderId,
		ProductId = @ProductId,
		Quantity = @Quantity,
		Price = @Price
	WHERE
		OrderItemId = @OrderItemId

END