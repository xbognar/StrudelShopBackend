CREATE PROCEDURE GetTotalSalesByDateRange

  @StartDate DATETIME,
  @EndDate DATETIME

AS
BEGIN
  
  SELECT SUM(TotalAmount) AS TotalSales
  FROM [Order]
  WHERE OrderDate BETWEEN @StartDate AND @EndDate;

END;
