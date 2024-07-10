CREATE PROCEDURE UpdateCustomer
	
	@CustomerId INT,
	@FirstName NVARCHAR(50),
	@LastName NVARCHAR(50),
	@Email NVARCHAR(50),
	@PhoneNumber NVARCHAR(50),
	@Address NVARCHAR(50)

AS
BEGIN 
	
	UPDATE Customer
	SET FirstName = @FirstName,
		LastName = @LastName,
		Email = @Email,
		PhoneNumber = @PhoneNumber,
		Address = @Address
	WHERE CustomerId = @CustomerId;

END