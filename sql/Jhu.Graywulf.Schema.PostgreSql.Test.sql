create database Graywulf_Test;

CREATE USER graywulf WITH PASSWORD 'XqA98Pdt.';
GRANT ALL PRIVILEGES ON DATABASE Graywulf_Test to graywulf;
GRANT ALL ON SCHEMA public TO graywulf;
 
CREATE TABLE TableWithPrimaryKey ( 
	ID INT NOT NULL, 
	Data1 NCHAR(10) NULL, 
	Data2 NCHAR(10) NULL, 
	Flag BIT NULL, 
	CONSTRAINT PK_TableWithPrimaryKey PRIMARY KEY ( ID)  
);

CREATE TABLE SampleData(
	"column_smallint"			SMALLINT,
	"column_integer"			INTEGER,
	"column_bigint"				BIGINT,
	"column_int"				INT,
	"column_decimal"			DECIMAL(13,0),
	"column_numeric"			NUMERIC,
	"column_real"				REAL,
	"column_doubleprecision"	DOUBLE PRECISION,
	"column_smallserial"		SMALLSERIAL,
	"column_serial"				SERIAL,
	"column_bigserial"			BIGSERIAL,
	"column_money"				MONEY,
	"column_charactervarying"	CHARACTER VARYING(10),
	  "column_varchar"			VARCHAR(10),
	  "column_character"		CHARACTER(10),
	  "column_char"				CHAR(10),
      "column_text"				TEXT,
      "column_bytea"			BYTEA,
      "column_timestamp"		TIMESTAMP,
      "column_timestampwithtimezone"	TIMESTAMPTZ,
      "column_date"				DATE,
      "column_time"				TIME,
      "column_time"				TIMETZ,
      "column_interval"			INTERVAL,
      "column_bool"				BOOLEAN,
      "column_point"			POINT,
      "column_line"				LINE,
      "column_lseg"				LSEG,
      "column_box"				BOX,
      "column_path"				PATH,
      "column_polygon"			POLYGON,
      "column_circle"			CIRCLE,
      "column_cidr"				CIDR,
      "column_inet"				INET,
      "column_macaddr"			MACADDR,
      "column_bit"				BIT(2),
      "column_bitvarying"		BIT VARYING(2),
      "column_tsvector"			TSVECTOR,
      "column_uuid"				UUID,
      "column_xml"				XML,
      "column_json"				JSON,
      "column_arrayinteger"		integer[],
      "column_int4range"		INT4RANGE,
      "column_int8range"		INT8RANGE,
      "column_numrange"			NUMRANGE,
      "column_tsrange"			TSRANGE,
      "column_tstzrange"		TSTZRANGE,
      "column_daterange"		DATERANGE,
      "column_oid"				OID
);        

CREATE TABLE BookAuthor (
	BookID bigint NOT NULL,
	AuthorID bigint NOT NULL,
	CONSTRAINT PK_BookAuthor PRIMARY KEY ( BookID , AuthorID  )
);

CREATE TABLE Book (
	ID bigint NOT NULL,
	Title varchar(50) NULL,
	Year int NULL,
	CONSTRAINT PK_Book PRIMARY KEY ( ID  )
);

CREATE TABLE Author (
	ID bigint NOT NULL,
	Name varchar(50) NOT NULL,
	CONSTRAINT PK_Author PRIMARY KEY ( ID  )
);

CREATE VIEW ViewWithStar
AS
	SELECT * FROM TableWithPrimaryKey
;

CREATE VIEW ViewOverSameTable
AS
	SELECT a.ID a_id, b.ID b_id
	FROM TableWithPrimaryKey a
	INNER JOIN TableWithPrimaryKey b ON a.id = b.id
;


CREATE VIEW ViewCrossJoinOneTable
AS
	SELECT a.ID a_id, b.ID b_id
	FROM TableWithPrimaryKey a
	CROSS JOIN TableWithPrimaryKey b
;

CREATE VIEW ViewComputedColumn
AS
	SELECT a.ID + b.ID id
	FROM TableWithPrimaryKey a
	CROSS JOIN TableWithPrimaryKey b
;

CREATE OR REPLACE FUNCTION  spTest(IN hello VARCHAR(50))
  RETURNS void AS
  $BODY$
  BEGIN
	SELECT version();
 end
 $BODY$
 LANGUAGE 'plpgsql';
 
 comment on function spTest(IN hello VARCHAR(50))  is 'spTestComment';
 comment on COLUMN Book.ID is 'id of user';
 comment on table Author  is 'this is my own table comment';
