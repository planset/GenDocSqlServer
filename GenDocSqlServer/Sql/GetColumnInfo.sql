
--
-- https://www.mssqltips.com/sqlservertip/1499/create-a-sql-server-data-dictionary-in-seconds-using-extended-properties/
--

SELECT 
    CAST(clmns.name AS NVARCHAR(35)) as ColumnName,
--    substring(ISNULL(CAST(exprop.value AS VARCHAR(255)),''),1,250) +
--    substring(ISNULL(CAST(exprop.value AS VARCHAR(500)),''),251,250) AS Description,
    exprop.value AS Description,
    CAST(ISNULL(idxcol.index_column_id, 0)AS VARCHAR(20)) AS InPrimaryKey,
    CAST(ISNULL( (SELECT TOP 1 1
        FROM sys.foreign_key_columns AS fkclmn
        WHERE fkclmn.parent_column_id = clmns.column_id
        AND fkclmn.parent_object_id = clmns.object_id
        ), 0) AS VARCHAR(20)) AS IsForeignKey,
    CAST(udt.name AS VARCHAR(15)) AS DataType,
    CAST(CAST(CASE WHEN typ.name IN (N'nchar', N'nvarchar') AND clmns.max_length <> -1
                   THEN clmns.max_length/2
                   ELSE clmns.max_length END AS INT) AS VARCHAR(20)) AS Length,
    CAST(CAST(clmns.precision AS INT) AS VARCHAR(20)) AS NumericPrecision,
    CAST(CAST(clmns.scale AS INT) AS VARCHAR(20)) AS NumericScale,
    CAST(clmns.is_nullable AS VARCHAR(20)) AS Nullable,
    CAST(clmns.is_computed AS VARCHAR(20)) AS Computed,
    CAST(clmns.is_identity AS VARCHAR(20)) AS [Identity],
    isnull(CAST(cnstr.definition AS NVARCHAR(20)),'') AS DefaultValue
FROM sys.tables AS tbl
    INNER JOIN sys.all_columns AS clmns ON clmns.object_id=tbl.object_id
    LEFT OUTER JOIN sys.indexes AS idx ON idx.object_id = clmns.object_id AND 1 =idx.is_primary_key
    LEFT OUTER JOIN sys.index_columns AS idxcol ON idxcol.index_id = idx.index_id
        AND idxcol.column_id = clmns.column_id
        AND idxcol.object_id = clmns.object_id
        AND 0 = idxcol.is_included_column
    LEFT OUTER JOIN sys.types AS udt ON udt.user_type_id = clmns.user_type_id
    LEFT OUTER JOIN sys.types AS typ ON typ.user_type_id = clmns.system_type_id 
		AND typ.user_type_id = typ.system_type_id
    LEFT JOIN sys.default_constraints AS cnstr ON cnstr.object_id=clmns.default_object_id
    LEFT OUTER JOIN sys.extended_properties exprop ON exprop.major_id = clmns.object_id
        AND exprop.minor_id = clmns.column_id
        AND exprop.name = 'MS_Description'
WHERE tbl.name = @TableName 
ORDER BY clmns.column_id ASC
