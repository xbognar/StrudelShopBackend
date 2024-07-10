CREATE PROCEDURE CreateProduct

	@Name NVARCHAR(100),
	@Description NVARCHAR(255),
	@Price DECIMAL(10, 2),
	@ImageURL NVARCHAR(255)

AS
BEGIN

	INSERT INTO Product (Name, Description, Price, ImageURL)
	VALUES (@Name, @Description, @Price, @ImageURL);

END