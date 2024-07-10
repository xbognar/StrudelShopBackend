CREATE PROCEDURE DeleteOrder
	
	@OrderId INT

AS
BEGIN

	DELETE FROM [Order] 
	WHERE OrderId = @OrderId;

END