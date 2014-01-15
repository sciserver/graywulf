create database graywulf_schema_test;

CREATE USER graywulf WITH PASSWORD 'password';
GRANT ALL PRIVILEGES ON DATABASE graywulf_schema_test TO graywulf;
GRANT ALL ON SCHEMA public TO graywulf;
 
-- CREATE TABLES --

CREATE TABLE "Author"
(
  "ID" bigint NOT NULL,
  "Name" character varying(50),
  CONSTRAINT "PK_Author" PRIMARY KEY ("ID")
);

CREATE TABLE "Book"
(
  "ID" bigint NOT NULL,
  "Title" character varying(50),
  "Year" integer,
  CONSTRAINT "PK_Book" PRIMARY KEY ("ID")
);

CREATE TABLE "BookAuthor"
(
  "BookID" bigint NOT NULL,
  "AuthorID" bigint NOT NULL,
  CONSTRAINT "PK_BookAuthor" PRIMARY KEY ("BookID", "AuthorID")
);

CREATE TABLE "TableWithIndexes"
(
  "ID" integer NOT NULL,
  "Data1" bigint,
  CONSTRAINT "PK_TableWithIndexes" PRIMARY KEY ("ID")
);

CREATE INDEX "IX_TableWithIndexes"
  ON "TableWithIndexes"
  USING btree
  ("Data1");

CREATE TABLE "TableWithPrimaryKey"
(
  id integer NOT NULL,
  data1 character(10),
  data2 character(10),
  flag bit(1),
  CONSTRAINT "PK_TableWithPrimaryKey" PRIMARY KEY (id)
);

-- CREATE VIEWS --

CREATE OR REPLACE VIEW "ViewComputedColumn" AS 
 SELECT a.id + b.id AS id
   FROM "TableWithPrimaryKey" a
  CROSS JOIN "TableWithPrimaryKey" b;

CREATE OR REPLACE VIEW "ViewCrossJoinOneTable" AS 
 SELECT a.id AS a_id, 
    b.id AS b_id
   FROM "TableWithPrimaryKey" a
  CROSS JOIN "TableWithPrimaryKey" b;

CREATE OR REPLACE VIEW "ViewOverPrimaryKey" AS 
 SELECT t.id, 
    t.flag, 
    t.data1, 
    t.data2
   FROM "TableWithPrimaryKey" t
  WHERE t.flag = B'1'::"bit";

CREATE OR REPLACE VIEW "ViewOverSameTable" AS 
 SELECT a.id AS a_id, 
    b.id AS b_id
   FROM "TableWithPrimaryKey" a
   JOIN "TableWithPrimaryKey" b ON a.id = b.id;

CREATE OR REPLACE VIEW "ViewWithStar" AS 
 SELECT "TableWithPrimaryKey".id, 
    "TableWithPrimaryKey".data1, 
    "TableWithPrimaryKey".data2, 
    "TableWithPrimaryKey".flag
   FROM "TableWithPrimaryKey";

-- CREATE FUNCTIONS --

CREATE OR REPLACE FUNCTION "InlineTableValuedFunction"(IN param1 integer, IN param2 integer)
  RETURNS TABLE(column1 integer, column2 integer) AS
$BODY$
	SELECT $1 column1, $2 column2
$BODY$
  LANGUAGE sql VOLATILE
  COST 100
  ROWS 1000;

CREATE OR REPLACE FUNCTION "ScalarFunction"(param1 character varying, param2 real)
  RETURNS real AS
$BODY$
BEGIN
	RETURN param2;
END;
$BODY$
  LANGUAGE plpgsql VOLATILE
  COST 100;



/*

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
      "column_timewithtimezone"				TIMETZ,
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

 comment on function spTest(IN hello VARCHAR(50))  is 'spTestComment';
 comment on COLUMN Book.ID is 'id of user';
 comment on table Author  is 'this is my own table comment';
*/