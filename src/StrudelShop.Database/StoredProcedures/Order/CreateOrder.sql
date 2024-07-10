CREATE PROCEDURE CreateOrder
	
	@CustomerId INT,
	@OrderDate DATETIME,
	@DeliveryDate DATETIME,
	@TotalAmount DECIMAL(10, 2),
	@PaymentStatus NVARCHAR(50)

AS
BEGIN
	
	INSERT INTO [Order] (CustomerId, OrderDate, DeliveryDate, TotalAmount, PaymentStatus)
	VALUES (@CustomerId, @OrderDate, @DeliveryDate, @TotalAmount, @PaymentStatus);

END