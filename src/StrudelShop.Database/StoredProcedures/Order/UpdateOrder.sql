CREATE PROCEDURE UpdateOrder
	
	@OrderId INT,
	@CustomerId INT,
	@OrderDate DATETIME,
	@DeliveryDate DATETIME,
	@TotalAmount DECIMAL(10, 2),
	@PaymentStatus VARCHAR(50)

AS
BEGIN

	Update [Order]
	SET CustomerId = @CustomerId,
		OrderDate = @OrderDate,
		DeliveryDate = @DeliveryDate,
		TotalAmount = @TotalAmount,
		PaymentStatus = @PaymentStatus
	WHERE OrderId = @OrderId

END
	