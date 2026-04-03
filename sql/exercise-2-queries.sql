WITH Movements AS (
    SELECT
        User_id,
        TipoMov,
        fecha,
        LEAD(TipoMov) OVER (PARTITION BY User_id ORDER BY fecha) AS NextTipoMov,
        LEAD(fecha) OVER (PARTITION BY User_id ORDER BY fecha) AS NextFecha
    FROM ccloglogin
),
Sessions AS (
    SELECT
        User_id,
        DATEDIFF(SECOND, fecha, NextFecha) AS SessionSeconds
    FROM Movements
    WHERE TipoMov = 1
      AND NextTipoMov = 0
      AND NextFecha > fecha
),
Totals AS (
    SELECT
        User_id,
        SUM(SessionSeconds) AS TotalSeconds
    FROM Sessions
    GROUP BY User_id
)
SELECT TOP 1
    User_id,
    CONCAT(
        TotalSeconds / 86400, ' días, ',
        (TotalSeconds % 86400) / 3600, ' horas, ',
        ((TotalSeconds % 86400) % 3600) / 60, ' minutos, ',
        ((TotalSeconds % 86400) % 3600) % 60, ' segundos'
    ) AS TiempoTotal
FROM Totals
ORDER BY TotalSeconds DESC;


WITH Movements AS (
    SELECT
        User_id,
        TipoMov,
        fecha,
        LEAD(TipoMov) OVER (PARTITION BY User_id ORDER BY fecha) AS NextTipoMov,
        LEAD(fecha) OVER (PARTITION BY User_id ORDER BY fecha) AS NextFecha
    FROM ccloglogin
),
Sessions AS (
    SELECT
        User_id,
        DATEDIFF(SECOND, fecha, NextFecha) AS SessionSeconds
    FROM Movements
    WHERE TipoMov = 1
      AND NextTipoMov = 0
      AND NextFecha > fecha
),
Totals AS (
    SELECT
        User_id,
        SUM(SessionSeconds) AS TotalSeconds
    FROM Sessions
    GROUP BY User_id
)
SELECT TOP 1
    User_id,
    CONCAT(
        TotalSeconds / 86400, ' días, ',
        (TotalSeconds % 86400) / 3600, ' horas, ',
        ((TotalSeconds % 86400) % 3600) / 60, ' minutos, ',
        ((TotalSeconds % 86400) % 3600) % 60, ' segundos'
    ) AS TiempoTotal
FROM Totals
ORDER BY TotalSeconds ASC;


WITH Movements AS (
    SELECT
        User_id,
        TipoMov,
        fecha,
        LEAD(TipoMov) OVER (PARTITION BY User_id ORDER BY fecha) AS NextTipoMov,
        LEAD(fecha) OVER (PARTITION BY User_id ORDER BY fecha) AS NextFecha
    FROM ccloglogin
),
Sessions AS (
    SELECT
        User_id,
        YEAR(fecha) AS YearValue,
        MONTH(fecha) AS MonthValue,
        DATEDIFF(SECOND, fecha, NextFecha) AS SessionSeconds
    FROM Movements
    WHERE TipoMov = 1
      AND NextTipoMov = 0
      AND NextFecha > fecha
),
Averages AS (
    SELECT
        User_id,
        YearValue,
        MonthValue,
        CAST(AVG(CAST(SessionSeconds AS BIGINT)) AS BIGINT) AS AverageSeconds
    FROM Sessions
    GROUP BY User_id, YearValue, MonthValue
)
SELECT
    CONCAT(
        'Usuario ', User_id, ' en ',
        CASE MonthValue
            WHEN 1 THEN 'enero'
            WHEN 2 THEN 'febrero'
            WHEN 3 THEN 'marzo'
            WHEN 4 THEN 'abril'
            WHEN 5 THEN 'mayo'
            WHEN 6 THEN 'junio'
            WHEN 7 THEN 'julio'
            WHEN 8 THEN 'agosto'
            WHEN 9 THEN 'septiembre'
            WHEN 10 THEN 'octubre'
            WHEN 11 THEN 'noviembre'
            WHEN 12 THEN 'diciembre'
        END,
        ' ', YearValue, ': ',
        AverageSeconds / 86400, ' días, ',
        (AverageSeconds % 86400) / 3600, ' horas, ',
        ((AverageSeconds % 86400) % 3600) / 60, ' minutos, ',
        ((AverageSeconds % 86400) % 3600) % 60, ' segundos'
    ) AS Resultado
FROM Averages
ORDER BY User_id, YearValue, MonthValue;