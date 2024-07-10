CREATE PROCEDURE GetCustomerOrderHistory
  
  @CustomerID INT

AS
BEGIN
  
  SELECT o.OrderID, o.OrderDate, o.TotalAmount, o.DeliveryDate, o.PaymentStatus
  FROM [Order] o
  WHERE o.CustomerID = @CustomerID;

END;
