CREATE PROCEDURE CreateCustomer
	
	@FirstName NVARCHAR(50),
	@LastName NVARCHAR(50),
	@Email NVARCHAR(50),
	@PhoneNumber NVARCHAR(50),
	@Address NVARCHAR(50)	

AS
BEGIN
	
	INSERT INTO Customer (FirstName, LastName, Email, PhoneNumber, Address)
	VALUES (@FirstName, @LastName, @Email, @PhoneNumber, @Address);

END