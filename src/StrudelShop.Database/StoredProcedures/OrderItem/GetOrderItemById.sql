CREATE PROCEDURE GetOrderItemById
	
	@OrderItemId INT

AS
BEGIN

	SELECT * FROM OrderItem 
	WHERE OrderItemId = @OrderItemId;

END