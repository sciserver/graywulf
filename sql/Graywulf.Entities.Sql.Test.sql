DECLARE @id entities.[Identity] = entities.[Identity]::Parse('<id name="test" auth="1"><group name="a" role="x" /><group name="b" role="y" /></id>')
DECLARE @acl entities.[EntityAcl] = entities.[EntityAcl]::Parse('<acl owner="test"><user name="@owner"><grant access="all" /></user><user name="test"><grant access="write" /><grant access="read" /></user><group name="testgroup" role="member"><grant access="read" /></group></acl>')

SELECT @id.ToString(), @acl.ToString()

SELECT @acl.ToBinary()

SET @acl = 0x41434C0100000474657374550006406F776E6572412B03616C6C55000474657374412B05777269746555000474657374412B04726561644700097465737467726F757000066D656D626572412B047265616458

SELECT @id.CanRead(@acl.ToBinary())