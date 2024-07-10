CREATE PROCEDURE GetOrderDetails
	
	@OrderId INT

AS
BEGIN

	SELECT 
		oi.OrderItemId, 
		p.Name AS ProductName, 
		oi.Quantity, 
		p.Price, 
		(oi.Quantity * p.Price) AS TotalPrice, 
		o.OrderDate, 
		o.DeliveryDate, 
		o.TotalAmount AS OrderTotalAmount, 
		o.PaymentStatus, 
		c.FirstName, 
		c.LastName, 
		c.Email, 
		c.PhoneNumber, 
		c.Address
	FROM 
		OrderItem oi
	JOIN 
		Product p ON oi.ProductId = p.ProductId
	JOIN 
		[Order] o ON oi.OrderId = o.OrderId
	JOIN 
		Customer c ON o.CustomerID = c.CustomerID
	WHERE 
		oi.OrderId = @OrderId;

END
