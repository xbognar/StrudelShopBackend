CREATE PROCEDURE GetOrderById

	@OrderId INT	
	
AS
BEGIN

	SELECT * FROM [Order] 
	WHERE OrderId = @OrderId;

END
