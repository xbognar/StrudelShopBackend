CREATE PROCEDURE GetOrdersByDateRange

	@StartDate DATETIME,
	@EndDate DATETIME

AS
BEGIN

	SELECT * FROM [Order]
	WHERE OrderDate BETWEEN @StartDate AND @EndDate;

END