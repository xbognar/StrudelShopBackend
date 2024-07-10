CREATE PROCEDURE UpdateProduct
	
	@ProductId INT,
	@Name NVARCHAR(100),
	@Description NVARCHAR(255),
	@Price DECIMAL(10, 2),
	@ImageURL NVARCHAR(255)

AS
BEGIN

	UPDATE Product
	SET Name = @Name,
		Description = @Description,
		Price = @Price,
		ImageURL = @ImageURL
	WHERE ProductId = @ProductId;

END