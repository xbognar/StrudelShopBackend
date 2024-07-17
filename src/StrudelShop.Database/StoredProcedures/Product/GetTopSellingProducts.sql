CREATE PROCEDURE GetTopSellingProducts
  
  @Top INT

AS
BEGIN
  
  SELECT TOP (@Top) p.Name, SUM(oi.Quantity) AS TotalSold
  FROM OrderItem oi
  JOIN Product p ON oi.ProductID = p.ProductID
  GROUP BY p.Name
  ORDER BY TotalSold DESC;

END;
