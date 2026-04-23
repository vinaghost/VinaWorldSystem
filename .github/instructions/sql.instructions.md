# SQL Formatting Instructions

These instructions define the required SQL formatting standards for all SQL statements in this repository. All contributors must follow these rules when writing or updating SQL in query classes or other files.

## 1. Keywords
- All SQL keywords (e.g., SELECT, FROM, WHERE, JOIN, ORDER BY, GROUP BY, HAVING, AS, AND, OR, ON, IN, IS, NULL, etc.) must be written in **UPPERCASE**.

## 2. Column and Table Names
- Always wrap column and table names in double quotes ("") if the database requires it for case sensitivity or reserved words. Otherwise, use plain names.
- Use consistent naming conventions (e.g., snake_case or PascalCase) as per the database schema.

## 3. Alignment and Indentation
- Align SQL clauses vertically for readability.
- Indent columns in SELECT, JOIN, and other multi-line clauses by 4 spaces.
- Each major clause (SELECT, FROM, WHERE, JOIN, GROUP BY, ORDER BY, etc.) should start on a new line.
- For multi-line SELECT, put each column on its own line and align them.

Example:

```sql
SELECT
    p.Id           AS PlayerId,
    p.Name         AS PlayerName,
    a.Id           AS AllianceId,
    a.Name         AS AllianceName,
    v.X,
    v.Y,
    v.Tribe,
    v.Population,
    v.IsCapital,
    v.IsCity,
    v.IsHarbor
FROM
    new_villages n
    JOIN Villages v   ON n.VillageId = v.Id
    JOIN Players p    ON v.PlayerId = p.Id
    JOIN Alliances a  ON p.AllianceId = a.Id
WHERE
    v.Population <> 0
ORDER BY
    v.Population DESC;
```

## 4. Joins
- Always specify the type of JOIN (INNER JOIN, LEFT JOIN, etc.) unless using the default (INNER JOIN).
- Place the ON clause on the same line as the JOIN or on a new indented line.

## 5. Parameters
- Use named parameters (e.g., @PlayerId) for all variable inputs.
- Do not concatenate user input directly into SQL strings.

## 6. CTEs and Subqueries
- For Common Table Expressions (CTEs), use WITH and indent the CTE body.
- For subqueries, indent the subquery and align with surrounding SQL.

## 7. Comments
- Use -- for single-line comments and /* ... */ for block comments.
- Place comments above the relevant SQL line or clause.

## 8. General Best Practices
- Avoid SELECT *; always specify columns explicitly.
- Use aliases for tables and columns for clarity.
- Keep SQL statements as simple and readable as possible.
- Avoid deeply nested subqueries when possible.
- Use consistent formatting throughout the codebase.

---

**All SQL code in this repository must follow these formatting rules.**
