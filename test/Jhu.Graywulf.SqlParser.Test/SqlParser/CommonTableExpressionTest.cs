SET STATEMENT:

works

SET @myvar = 'This is a test'
SET  @NewBalance  =  10
SET  @NewBalance  =  @NewBalance  *  10
SET @rows = (SELECT COUNT(*) FROM Sales.Customer)
SET @p=point.SetXY(23.5, 23.5) 												-- it didn't works perfect, because it looks like udfIdentifier

didn't works

SET @p.X = @p.X + 1.1											 			-- in expression didn't recognize .X argument/field
SET @p.SetXY(22, 23)

############################################################################################
UPDATE STATEMENT

works

UPDATE dbo.Table2 
SET dbo.Table2.ColB = dbo.Table2.ColB + dbo.Table1.ColB
FROM dbo.Table2 
INNER JOIN dbo.Table1 
ON (dbo.Table2.ColA = dbo.Table1.ColA)
---------------------------------------------
UPDATE dbo.Table1 
SET c2 = c2 + d2 
FROM dbo.Table2 
WHERE e2>0
---------------------------------------------
UPDATE Cities
SET Location = CONVERT(Point, '12.3:46.2')
WHERE Name = 'Anchorage'
---------------------------------------------	
UPDATE Cities																		--udtcolumnnameargument name is not the best for this, need to rename
SET Location.SetXY(23.5, 23.5)
WHERE Name = 'Anchorage'
---------------------------------------------
UPDATE Cities
SET Location.X = 23.5																--Location is not table name, and X is not column name....
WHERE Name = 'Anchorage'
---------------------------------------------
UPDATE Sales.SalesPerson
SET Bonus = 6000, CommissionPct = .10, SalesQuota = NULL
---------------------------------------------
UPDATE Production.Product
SET Color = 'Metallic Red'
WHERE Name='Road-250%' AND Color = 'Red'
---------------------------------------------
UPDATE TOP (10) HumanResources.Employee
SET VacationHours = VacationHours * 1.25
---------------------------------------------
UPDATE Production.Product
SET ListPrice = ListPrice * 2
---------------------------------------------
UPDATE Production.Product
SET ListPrice += @NewPrice
WHERE Color = 'Red'
---------------------------------------------
UPDATE Production.ScrapReason 
SET Name += ' - tool malfunction'
WHERE ScrapReasonID BETWEEN 10 and 12
---------------------------------------------
UPDATE Production.Location
SET CostRate = DEFAULT
WHERE CostRate > 20.00
---------------------------------------------
UPDATE Person.vStateProvinceCountryRegion
SET CountryRegionName = 'United States of America'
WHERE CountryRegionName = 'United States'
---------------------------------------------
UPDATE sr
SET sr.Name += ' - tool malfunction'
FROM Production.ScrapReason AS sr
JOIN Production.WorkOrder AS wo 
     ON sr.ScrapReasonID = wo.ScrapReasonID
     AND wo.ScrappedQty > 300
---------------------------------------------
UPDATE Sales.SalesPerson
SET SalesYTD = SalesYTD + SubTotal
FROM Sales.SalesPerson AS sp
JOIN Sales.SalesOrderHeader AS so
    ON sp.BusinessEntityID = so.SalesPersonID
    AND so.OrderDate = 10
---------------------------------------------
UPDATE Production.Document
SET DocumentSummary = N'Replacing NULL value'
WHERE Title = N'Crank Arm and Tire Maintenance'
---------------------------------------------
UPDATE dbo.Cities																		udtcolumnnameargument name is not okay
SET Location = CONVERT(Point, '12.3:46.2')
WHERE Name = 'Anchorage'
---------------------------------------------

---------------------------------------------
didn't work


WITH cte AS (SELECT * FROM @x)															--the method or operation is not implemented
UPDATE x SET Value = y.Value
FROM cte AS x
INNER JOIN @y AS y ON y.ID = x.ID
-----------------------------------------------------------------------------
WITH cte AS (SELECT * FROM @x)															--the method or operation is not implemented
UPDATE cte   
SET Value = y.Value
FROM cte AS x 
INNER JOIN @y AS y ON y.ID = x.ID
-----------------------------------------------------------------------------
WITH Parts(AssemblyID, ComponentID, PerAssemblyQty, EndDate, ComponentLevel) AS				--with statement doest work properly
(
    SELECT b.ProductAssemblyID, b.ComponentID, b.PerAssemblyQty,
        b.EndDate, 0 AS ComponentLevel
    FROM Production.BillOfMaterials AS b
    WHERE b.ProductAssemblyID = 800
          AND b.EndDate IS NULL
    UNION ALL
    SELECT bom.ProductAssemblyID, bom.ComponentID, p.PerAssemblyQty,
        bom.EndDate, ComponentLevel + 1
    FROM Production.BillOfMaterials AS bom 
        INNER JOIN Parts AS p
        ON bom.ProductAssemblyID = p.ComponentID
        AND bom.EndDate IS NULL
)
UPDATE Production.BillOfMaterials
SET PerAssemblyQty = c.PerAssemblyQty * 2
FROM Production.BillOfMaterials AS c
JOIN Parts AS d ON c.ProductAssemblyID = d.AssemblyID
WHERE d.ComponentLevel = 0


---------------------------------------------------------------------------------
UPDATE Sales.SalesPerson																-- after plus(+) Select doesn't work
SET SalesYTD = SalesYTD + 
    (SELECT SUM(so.SubTotal) 
     FROM Sales.SalesOrderHeader AS so
     WHERE so.OrderDate = (SELECT MAX(OrderDate)
                           FROM Sales.SalesOrderHeader AS so2
                           WHERE so2.SalesPersonID = so.SalesPersonID)
     AND Sales.SalesPerson.BusinessEntityID = so.SalesPersonID
     GROUP BY so.SalesPersonID)
----------------------------------------------------------------------------	 
UPDATE @MyTableVar
SET NewVacationHours = e.VacationHours + 20,
    ModifiedDate = GETDATE()
FROM HumanResources.Employee AS e 
WHERE e.BusinessEntityID = EmpID
----------------------------------------------------------------------------------
UPDATE Sales.SalesPerson																-- after plus(+) Select doesn't work
SET SalesYTD = SalesYTD + 
    (SELECT SUM(so.SubTotal) 
     FROM Sales.SalesOrderHeader AS so
     WHERE so.OrderDate = (SELECT MAX(OrderDate)
                           FROM Sales.SalesOrderHeader AS so2
                           WHERE so2.SalesPersonID = so.SalesPersonID)
     AND Sales.SalesPerson.BusinessEntityID = so.SalesPersonID
     GROUP BY so.SalesPersonID)
----------------------------------------------------------------------------------
UPDATE Production.Document
SET DocumentSummary .WRITE(N'Carefully inspect and maintain the tires and crank arms.',0,NULL)
WHERE Title = N'Crank Arm and Tire Maintenance'
----------------------------------------------------------------------------------
UPDATE dbo.Cities																		-- location is not a schema name, attribute recognising
SET Location.X = 23.5
WHERE Name = 'Anchorage'
----------------------------------------------------------------------------------



###########################################################################
INSERT STATEMENT

works

INSERT INTO Cities (Location)
VALUES ( CONVERT(Point, '12.3:46.2') )
------------------------------------------------------------------
INSERT INTO Cities (Location)
VALUES ( dbo.CreateNewPoint(x, y) )
------------------------------------------------------------------
INSERT INTO Production.UnitMeasure
VALUES ('FT', 'Feet', '20080414')
------------------------------------------------------------------
INSERT INTO Production.UnitMeasure
VALUES ('FT2', 'Square Feet ', '20080923'), ('Y', 'Yards', '20080923'), ('Y3', 'Cubic Yards', '20080923')
------------------------------------------------------------------
INSERT INTO Production.UnitMeasure (Name, UnitMeasureCode,
    ModifiedDate)
VALUES ('Square Yards', 'Y2', GETDATE())
------------------------------------------------------------------
INSERT INTO dbo.T1 (column_4) 
    VALUES ('Explicit value')
------------------------------------------------------------------
INSERT INTO dbo.T1 (column_2, column_4) 
    VALUES ('Explicit value', 'Explicit value')
------------------------------------------------------------------
INSERT INTO T1 DEFAULT VALUES
------------------------------------------------------------------
INSERT T1 VALUES ('Row #1')
------------------------------------------------------------------
INSERT T1 (column_2) VALUES ('Row #2')
------------------------------------------------------------------
INSERT INTO T1 (column_1,column_2) 
    VALUES (-99, 'Explicit identity value')
------------------------------------------------------------------
INSERT INTO dbo.T1 (column_2) 
    VALUES (NEWID())
------------------------------------------------------------------
INSERT INTO dbo.Points (PointValue) VALUES (CONVERT(Point, '3,4'))
------------------------------------------------------------------
INSERT INTO dbo.Points (PointValue) VALUES (CONVERT(Point, '1,5'))
------------------------------------------------------------------
INSERT INTO dbo.EmployeeSales
    SELECT 'SELECT', sp.BusinessEntityID, c.LastName, sp.SalesYTD 
    FROM Sales.SalesPerson AS sp
    INNER JOIN Person.Person AS c
        ON sp.BusinessEntityID = c.BusinessEntityID
    WHERE sp.BusinessEntityID LIKE '2%'
    ORDER BY sp.BusinessEntityID, c.LastName
------------------------------------------------------------------
INSERT INTO HumanResources.NewEmployee 
    SELECT EmpID, LastName, FirstName, Phone, 
           Address, City, StateProvince, PostalCode, CurrentFlag
    FROM EmployeeTemp
------------------------------------------------------------------
INSERT INTO V1 
    VALUES ('Row 1',1)
------------------------------------------------------------------

------------------------------------------------------------------
didn't works

INSERT INTO dbo.Points (PointValue) VALUES (CAST ('1,99' AS Point))
----------------------------------------------------------------------
INSERT INTO dbo.EmployeeSales 
EXECUTE dbo.uspGetEmployeeSales
----------------------------------------------------------------------
INSERT INTO dbo.EmployeeSales 
EXECUTE 
('
SELECT ''EXEC STRING'', sp.BusinessEntityID, c.LastName, 
    sp.SalesYTD 
    FROM Sales.SalesPerson AS sp 
    INNER JOIN Person.Person AS c
        ON sp.BusinessEntityID = c.BusinessEntityID
    WHERE sp.BusinessEntityID LIKE ''2%''
    ORDER BY sp.BusinessEntityID, c.LastName
')
----------------------------------------------------------------------
INSERT INTO @MyTableVar (LocationID, CostRate, ModifiedDate)					--didn't recognise local variable
    SELECT LocationID, CostRate, GETDATE() FROM Production.Location
    WHERE CostRate > 0
----------------------------------------------------------------------
INSERT INTO Sales.SalesHistory WITH (TABLOCK)									--bulk loading data not implemented yet
    (SalesOrderID, 
     SalesOrderDetailID,
     CarrierTrackingNumber, 
     OrderQty, 
     ProductID, 
     SpecialOfferID, 
     UnitPrice, 
     UnitPriceDiscount,
     LineTotal, 
     rowguid, 
     ModifiedDate)
SELECT * FROM Sales.SalesOrderDetail
----------------------------------------------------------------------
INSERT INTO Production.ZeroInventory (DeletedProductID, RemovedOnDate)					-- in from clause it's not implemented MERGE...
SELECT ProductID, GETDATE()
FROM
(   MERGE Production.ProductInventory AS pi
    USING (SELECT ProductID, SUM(OrderQty) FROM Sales.SalesOrderDetail AS sod
           JOIN Sales.SalesOrderHeader AS soh
           ON sod.SalesOrderID = soh.SalesOrderID
           AND soh.OrderDate = '20070401'
           GROUP BY ProductID) AS src (ProductID, OrderQty)
    ON (pi.ProductID = src.ProductID)
    WHEN MATCHED AND pi.Quantity - src.OrderQty <= 0
        THEN DELETE
    WHEN MATCHED
        THEN UPDATE SET pi.Quantity = pi.Quantity - src.OrderQty
    OUTPUT $action, deleted.ProductID) AS Changes (Action, ProductID)
WHERE Action = 'DELETE';
IF @@ROWCOUNT = 0
PRINT 'Warning: No rows were inserted

#########################################################################################################
WITH STATEMENT

works

WITH Sales_CTE (SalesPersonID, SalesOrderID, SalesYear)
AS
-- Define the CTE query.
(
    SELECT SalesPersonID, SalesOrderID, YEAR(OrderDate) AS SalesYear
    FROM Sales.SalesOrderHeader
    WHERE SalesPersonID IS NOT NULL
)
----------------------------------------------------------------------
WITH Sales_CTE (SalesPersonID, NumberOfOrders)
AS
(
    SELECT SalesPersonID, COUNT(*)
    FROM Sales.SalesOrderHeader
    WHERE SalesPersonID IS NOT NULL
    GROUP BY SalesPersonID
)
----------------------------------------------------------------------
WITH Sales_CTE (SalesPersonID, TotalSales, SalesYear)
AS
-- Define the first CTE query.
(
    SELECT SalesPersonID, SUM(TotalDue) AS TotalSales, YEAR(OrderDate) AS SalesYear
    FROM Sales.SalesOrderHeader
    WHERE SalesPersonID IS NOT NULL
       GROUP BY SalesPersonID, YEAR(OrderDate)

)
,   -- Use a comma to separate multiple CTE definitions.

-- Define the second CTE query, which returns sales quota data by year for each sales person.
Sales_Quota_CTE (BusinessEntityID, SalesQuota, SalesQuotaYear)
AS
(
       SELECT BusinessEntityID, SUM(SalesQuota) AS SalesQuota, YEAR(QuotaDate) AS SalesQuotaYear
       FROM Sales.SalesPersonQuotaHistory
       GROUP BY BusinessEntityID, YEAR(QuotaDate)
)
----------------------------------------------------------------------
WITH DirectReports(ManagerID, EmployeeID, Title, EmployeeLevel) AS 
(
    SELECT ManagerID, EmployeeID, Title, 0 AS EmployeeLevel
    FROM dbo.MyEmployees 
    WHERE ManagerID IS NULL
    UNION ALL
    SELECT e.ManagerID, e.EmployeeID, e.Title, EmployeeLevel + 1
    FROM dbo.MyEmployees AS e
        INNER JOIN DirectReports AS d
        ON e.ManagerID = d.EmployeeID 
)
----------------------------------------------------------------------
WITH DirectReports(ManagerID, EmployeeID, Title, EmployeeLevel) AS 
(
    SELECT ManagerID, EmployeeID, Title, 0 AS EmployeeLevel
    FROM dbo.MyEmployees 
    WHERE ManagerID IS NULL
    UNION ALL
    SELECT e.ManagerID, e.EmployeeID, e.Title, EmployeeLevel + 1
    FROM dbo.MyEmployees AS e
        INNER JOIN DirectReports AS d
        ON e.ManagerID = d.EmployeeID 
)
----------------------------------------------------------------------
WITH DirectReports(Name, Title, EmployeeID, EmployeeLevel, Sort)
AS (SELECT CONVERT(varchar(255), e.FirstName + ' ' + e.LastName),
        e.Title,
        e.EmployeeID,
        1,
        CONVERT(varchar(255), e.FirstName + ' ' + e.LastName)
    FROM dbo.MyEmployees AS e
    WHERE e.ManagerID IS NULL
    UNION ALL
    SELECT CONVERT(varchar(255), REPLICATE ('|    ' , EmployeeLevel) +
        e.FirstName + ' ' + e.LastName),
        e.Title,
        e.EmployeeID,
        EmployeeLevel + 1,
        CONVERT (varchar(255), RTRIM(Sort) + '|    ' + FirstName + ' ' + 
                 LastName)
    FROM dbo.MyEmployees AS e
    JOIN DirectReports AS d ON e.ManagerID = d.EmployeeID
    )
----------------------------------------------------------------------
WITH cte (EmployeeID, ManagerID, Title) as
(
    SELECT EmployeeID, ManagerID, Title
    FROM dbo.MyEmployees
    WHERE ManagerID IS NOT NULL
  UNION ALL
    SELECT cte.EmployeeID, cte.ManagerID, cte.Title
    FROM cte 
    JOIN  dbo.MyEmployees AS e 
        ON cte.ManagerID = e.EmployeeID
)
----------------------------------------------------------------------
WITH cte (EmployeeID, ManagerID, Title)
AS
(
    SELECT EmployeeID, ManagerID, Title
    FROM dbo.MyEmployees
    WHERE ManagerID IS NOT NULL
  UNION ALL
    SELECT  e.EmployeeID, e.ManagerID, e.Title
    FROM dbo.MyEmployees AS e
    JOIN cte ON e.ManagerID = cte.EmployeeID
)
----------------------------------------------------------------------
WITH Parts(AssemblyID, ComponentID, PerAssemblyQty, EndDate, ComponentLevel) AS
(
    SELECT b.ProductAssemblyID, b.ComponentID, b.PerAssemblyQty,
        b.EndDate, 0 AS ComponentLevel
    FROM Production.BillOfMaterials AS b
    WHERE b.ProductAssemblyID = 800
          AND b.EndDate IS NULL
    UNION ALL
    SELECT bom.ProductAssemblyID, bom.ComponentID, p.PerAssemblyQty,
        bom.EndDate, ComponentLevel + 1
    FROM Production.BillOfMaterials AS bom 
        INNER JOIN Parts AS p
        ON bom.ProductAssemblyID = p.ComponentID
        AND bom.EndDate IS NULL
)
----------------------------------------------------------------------
WITH Parts(AssemblyID, ComponentID, PerAssemblyQty, EndDate, ComponentLevel) AS
(
    SELECT b.ProductAssemblyID, b.ComponentID, b.PerAssemblyQty,
        b.EndDate, 0 AS ComponentLevel
    FROM Production.BillOfMaterials AS b
    WHERE b.ProductAssemblyID = 800
          AND b.EndDate IS NULL
    UNION ALL
    SELECT bom.ProductAssemblyID, bom.ComponentID, p.PerAssemblyQty,
        bom.EndDate, ComponentLevel + 1
    FROM Production.BillOfMaterials AS bom 
        INNER JOIN Parts AS p
        ON bom.ProductAssemblyID = p.ComponentID
        AND bom.EndDate IS NULL
)
----------------------------------------------------------------------
WITH Generation (ID) AS
(
-- First anchor member returns Bonnie's mother.
    SELECT Mother 
    FROM dbo.Person
    WHERE Name = 'Bonnie'
UNION
-- Second anchor member returns Bonnie's father.
    SELECT Father 
    FROM dbo.Person
    WHERE Name = 'Bonnie'
UNION ALL
-- First recursive member returns male ancestors of the previous generation.
    SELECT Person.Father
    FROM Generation, Person
    WHERE Generation.ID=Person.ID
UNION ALL
-- Second recursive member returns female ancestors of the previous generation.
    SELECT Person.Mother
    FROM Generation, dbo.Person
    WHERE Generation.ID=Person.ID
)
----------------------------------------------------------------------

----------------------------------------------------------------------
didn't works



WITH vw AS
 (
    SELECT itmIDComp, itmID
    FROM @t1

    UNION ALL

    SELECT itmIDComp, itmID
    FROM @t2
) 
,r AS
 (
    SELECT t.itmID AS itmIDComp
           , NULL AS itmID
           ,CAST(0 AS bigint) AS N
           ,1 AS Lvl
    FROM (SELECT 1 UNION ALL SELECT 2 UNION ALL SELECT 3 UNION ALL SELECT 4) AS t (itmID) 

UNION ALL

SELECT t.itmIDComp
    , t.itmID
    , ROW_NUMBER() OVER(PARTITION BY t.itmIDComp ORDER BY t.itmIDComp, t.itmID) AS N
    , Lvl + 1
FROM r 
    JOIN vw AS t ON t.itmID = r.itmIDComp
) 
----------------------------------------------------------------------
DECLARE STATEMENT

works

DECLARE @MyCounter int
-----------------------------------------------------------------------
DECLARE @a int,  @b int,@c int
-----------------------------------------------------------------------
DECLARE @FirstNameVariable nvarchar,
   @PostalCodeVariable nvarchar
-----------------------------------------------------------------------
-----------------------------------------------------------------------


didn't works


DECLARE @MyTableVar table(
    EmpID int NOT NULL,
    OldVacationHours int,
    NewVacationHours int,
    ModifiedDate datetime)
-----------------------------------------------------------------------
-----------------------------------------------------------------------
###############################################################################
OVER CLAUSE

works

OVER(PARTITION BY PostalCode ORDER BY SalesYTD)
-----------------------------------------------------------------------
OVER ( PARTITION BY SalesOrderID)
-----------------------------------------------------------------------
OVER (PARTITION BY TerritoryID 
ORDER BY DATEPART(yy,ModifiedDate) 
)
-----------------------------------------------------------------------
OVER (ORDER BY DATEPART(yy,ModifiedDate) )
-----------------------------------------------------------------------
OVER (PARTITION BY TerritoryID ORDER BY DATEPART(yy,ModifiedDate) ROWS BETWEEN CURRENT ROW AND 1 FOLLOWING )
-----------------------------------------------------------------------
OVER (PARTITION BY TerritoryID ORDER BY DATEPART(yy,ModifiedDate)  ROWS UNBOUNDED PRECEDING)
-----------------------------------------------------------------------
-----------------------------------------------------------------------

-----------------------------------------------------------------------
didn't work

OVER(PARTITION BY PostalCode ORDER BY SalesYTD DESC)

###########################################################################################
CREATE VIEW STATEMENT

works

CREATE VIEW Customers
AS
--Select from local member table.
SELECT *
FROM CompanyData.dbo.Customers_33
UNION ALL
--Select from member table
SELECT *
FROM CompanyData.dbo.Customers_66
UNION ALL
--Select from mmeber table
SELECT *
FROM CompanyData.dbo.Customers_99
-----------------------------------------------------------------------
CREATE VIEW hiredate_view
 AS 
SELECT p.FirstName, p.LastName, e.BusinessEntityID, e.HireDate
FROM HumanResources.Employee e 
JOIN Person.Person AS p ON e.BusinessEntityID = p.BusinessEntityID
-----------------------------------------------------------------------
CREATE VIEW Purchasing.PurchaseOrderReject
WITH ENCRYPTION
AS
SELECT PurchaseOrderID, ReceivedQty, RejectedQty, 
    RejectedQty / ReceivedQty AS RejectRatio, DueDate
FROM Purchasing.PurchaseOrderDetail
WHERE RejectedQty / ReceivedQty > 0
AND DueDate > CONVERT(DATETIME,'20010630',101)
-----------------------------------------------------------------------
CREATE VIEW dbo.SeattleOnly
AS
SELECT p.LastName, p.FirstName, e.JobTitle, a.City, sp.StateProvinceCode
FROM HumanResources.Employee e
	INNER JOIN Person.Person p
	ON p.BusinessEntityID = e.BusinessEntityID
    INNER JOIN Person.BusinessEntityAddress bea 
    ON bea.BusinessEntityID = e.BusinessEntityID 
    INNER JOIN Person.Address a 
    ON a.AddressID = bea.AddressID
    INNER JOIN Person.StateProvince sp 
    ON sp.StateProvinceID = a.StateProvinceID
WHERE a.City = 'Seattle'
WITH CHECK OPTION
-----------------------------------------------------------------------
CREATE VIEW Sales.SalesPersonPerform
AS
SELECT TOP (100) SalesPersonID, SUM(TotalDue) AS TotalSales
FROM Sales.SalesOrderHeader
WHERE OrderDate > CONVERT(DATETIME,'20001231',101)
GROUP BY SalesPersonID
-----------------------------------------------------------------------
CREATE VIEW dbo.all_supplier_view
WITH SCHEMABINDING
AS
SELECT supplyID, supplier
FROM dbo.SUPPLY1
UNION ALL
SELECT supplyID, supplier
FROM dbo.SUPPLY2
UNION ALL
SELECT supplyID, supplier
FROM dbo.SUPPLY3
UNION ALL
SELECT supplyID, supplier
FROM dbo.SUPPLY4
-----------------------------------------------------------------------

-----------------------------------------------------------------------
didn't works
#######################################################################################
DROP VIEW STATEMENT

works

DROP VIEW dbo.Reorder
-----------------------------------------------------------------------
DROP VIEW dbo.Reorder, alma
########################################################################################
DROP TABLE STATEMENT

works

DROP TABLE ProductVendor1
-----------------------------------------------------------------------
DROP TABLE AdventureWorks2012.dbo.SalesPerson2
-----------------------------------------------------------------------
DROP TABLE alma, korte

#######################################################################################
TRUNCATE TABLE

works

TRUNCATE TABLE HumanResources.JobCandidate
-----------------------------------------------------------------------
TRUNCATE TABLE Job.HumanResources.JobCandidate
#######################################################################################
FREETEXT PREDICATE

works

SELECT Description 
FROM Production.ProductDescription 
WHERE FREETEXT(Description, @SearchWord)
-----------------------------------------------------------------------
SELECT Title
FROM Production.Document
WHERE FREETEXT (Document, 'vital safety components' )
-----------------------------------------------------------------------
-----------------------------------------------------------------------
-----------------------------------------------------------------------
-----------------------------------------------------------------------