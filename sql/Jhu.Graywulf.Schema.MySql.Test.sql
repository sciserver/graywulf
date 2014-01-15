CREATE DATABASE graywulf_schema_test;
USE graywulf_schema_test;

CREATE USER graywulf IDENTIFIED BY 'password';
GRANT ALL on graywulf_schema_test.* to graywulf;

-- CREATE TABLES --

CREATE TABLE `author` (
  `ID` bigint(20) NOT NULL,
  `Name` varchar(50) NOT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE `book` (
  `ID` bigint(20) NOT NULL,
  `Title` varchar(50) DEFAULT NULL,
  `Year` int(11) DEFAULT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE `bookauthor` (
  `BookID` bigint(20) NOT NULL,
  `AuthorID` bigint(20) NOT NULL,
  PRIMARY KEY (`BookID`,`AuthorID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE `tablewithindexes` (
  `ID` int(11) NOT NULL,
  `Data1` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`ID`),
  UNIQUE KEY `Data1_UNIQUE` (`Data1`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE `tablewithprimarykey` (
  `ID` int(11) NOT NULL,
  `Data1` char(10) DEFAULT NULL,
  `Data2` char(10) DEFAULT NULL,
  `Flag` bit(1) DEFAULT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- CREATE VIEWS --

CREATE VIEW `viewcomputedcolumn`
AS
SELECT (`a`.`ID` + `b`.`ID`) AS `id`
FROM (`tablewithprimarykey` `a`
JOIN `tablewithprimarykey` b);

CREATE VIEW `viewcrossjoinonetable`
AS
SELECT `a`.`ID` AS `a_id`,`b`.`ID` AS `b_id`
FROM (`tablewithprimarykey` `a`
JOIN `tablewithprimarykey` `b`);

CREATE VIEW `viewoverprimarykey`
AS
SELECT `t`.`ID` AS `ID`,`t`.`Flag` AS `Flag`,`t`.`Data1` AS `Data1`,`t`.`Data2` AS `Data2`
FROM `tablewithprimarykey` `t`
WHERE (`t`.`Flag` = 1);

CREATE VIEW `viewoversametable`
AS
SELECT `a`.`ID` AS `a_id`,`b`.`ID` AS `b_id`
FROM (`tablewithprimarykey` `a`
JOIN `tablewithprimarykey` `b` ON ((`a`.`ID` = `b`.`ID`)));

CREATE VIEW `viewwithstar`
AS
SELECT `tablewithprimarykey`.`ID` AS `ID`, `tablewithprimarykey`.`Data1` AS `Data1`, `tablewithprimarykey`.`Data2` AS `Data2`, `tablewithprimarykey`.`Flag` AS `Flag`
FROM `tablewithprimarykey`;

-- CREATE STORED PROCEDURES --

DELIMITER $$
CREATE PROCEDURE `StoredProcedure`
(
	param1 int,
	param2 nvarchar(50)
)
SELECT 1$$
DELIMITER ;

-- CREATE FUNCTIONS --

DELIMITER $$
CREATE FUNCTION `ScalarFunction`
(
	param1 varchar(50),
	param2 real
) RETURNS double
RETURN 0$$
DELIMITER ;


/*
delimiter //

CREATE TABLE TableWithPrimaryKey ( 
	ID INT NOT NULL, 
	Data1 NCHAR(10) NULL, 
	Data2 NCHAR(10) NULL, 
	Flag BIT NULL, 
	CONSTRAINT PK_TableWithPrimaryKey PRIMARY KEY (ID ASC) 
) 
ENGINE=InnoDB//

CREATE PROCEDURE spTest (IN hello NVARCHAR(50))
BEGIN
	select @@version;
END//

CREATE TABLE SampleData(
      column_bool		BOOL,
      column_tinyint	TINYINT,
      column_utinyint	TINYINT UNSIGNED,
      column_smallint	SMALLINT,
      column_usmallint	SMALLINT UNSIGNED,
      column_mediumint	MEDIUMINT,
      column_umediumint	MEDIUMINT UNSIGNED,
      column_integer	INTEGER,
      column_uinteger	INTEGER UNSIGNED,
      column_bigint		BIGINT,
      column_ubigint	BIGINT UNSIGNED,
      column_int		INT,
      column_float		FLOAT,
      column_double		DOUBLE,
      column_decimal	DECIMAL(13,0),
      column_date		DATE,
      column_datetime	DATETIME,        
      column_year		YEAR, 
      column_time		TIME,
      column_timestamp	TIMESTAMP,
      column_tinytext	TINYTEXT,
      column_text		TEXT,
      column_mediumtext	MEDIUMTEXT,
      column_longtext	LONGTEXT,
      column_tinyblob	TINYBLOB,
      column_blob		BLOB,
      column_mediumblob	MEDIUMBLOB,
      column_longblob	LONGBLOB,
      column_bit		BIT,
      column_set		Set("alma","korte"),
      column_enum		Enum("also","felso"),
      column_binary		BINARY,
      column_varbinary	VARBINARY(3),
	  column_geometry	geometry,
	  column_char	char,
	  column_nchar	nchar,
	  column_varchar	varchar(4),
	  column_nvarchar	nvarchar(4)
)  
ENGINE=InnoDB//

CREATE TABLE BookAuthor (
	BookID bigint NOT NULL,
	AuthorID bigint NOT NULL,
	CONSTRAINT PK_BookAuthor PRIMARY KEY ( BookID ASC, AuthorID ASC )
)
ENGINE=InnoDB//

CREATE TABLE Book (
	ID bigint NOT NULL,
	Title nvarchar(50) NULL,
	Year int NULL,
	CONSTRAINT PK_Book PRIMARY KEY ( ID ASC )
)
ENGINE=InnoDB//

CREATE TABLE Author (
	ID bigint NOT NULL,
	Name nvarchar(50) NOT NULL,
	CONSTRAINT PK_Author PRIMARY KEY ( ID ASC )
)
ENGINE=InnoDB//

CREATE VIEW ViewWithStar
AS
	SELECT * FROM TableWithPrimaryKey
//

CREATE VIEW ViewOverSameTable
AS
	SELECT a.ID a_id, b.ID b_id
	FROM TableWithPrimaryKey a
	INNER JOIN TableWithPrimaryKey b ON a.id = b.id
//

CREATE VIEW ViewOverPrimaryKey
AS
	SELECT t.ID, t.Flag, t.Data1, t.Data2
	FROM TableWithPrimaryKey t
	WHERE Flag = 1
//

CREATE VIEW ViewCrossJoinOneTable
AS
	SELECT a.ID a_id, b.ID b_id
	FROM TableWithPrimaryKey a
	CROSS JOIN TableWithPrimaryKey b
//

CREATE VIEW ViewComputedColumn
AS
	SELECT a.ID + b.ID id
	FROM TableWithPrimaryKey a
	CROSS JOIN TableWithPrimaryKey b
//
*/