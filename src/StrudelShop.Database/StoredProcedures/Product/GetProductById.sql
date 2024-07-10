CREATE PROCEDURE GetProductById
	
	@ProductId INT

AS
BEGIN
	
	SELECT * FROM Product 
	WHERE ProductId = @ProductId;

END