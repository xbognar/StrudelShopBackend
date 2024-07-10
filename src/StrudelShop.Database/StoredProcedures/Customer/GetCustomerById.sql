CREATE PROCEDURE GetCustomerById
	
	@CustomerId INT

AS
BEGIN
	
	SELECT * FROM Customer 
	WHERE CustomerId = @CustomerId;

END