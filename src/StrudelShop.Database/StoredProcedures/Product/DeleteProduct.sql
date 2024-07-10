CREATE PROCEDURE DeleteProduct

	@ProductId INT
	
AS
BEGIN

	DELETE FROM Product
	WHERE ProductId = @ProductId

END