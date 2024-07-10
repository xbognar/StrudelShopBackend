CREATE PROCEDURE DeleteOrderItem
	
	@OrderItemId INT

AS
BEGIN

	DELETE FROM OrderItem
	WHERE OrderItemId = @OrderItemId;

END