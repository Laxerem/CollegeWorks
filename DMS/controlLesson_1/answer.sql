# Control Lesson

## С INDEX

```SQL
# Создаём таблицу logs
CREATE TABLE IF NOT EXISTS logs(
    log_id INT PRIMARY KEY AUTO_INCREMENT,
    user_id INT,
    action VARCHAR(20),
    timestamp TIMESTAMP
);

# Заполняем данными
SET SESSION cte_max_recursion_depth = 400000;

INSERT INTO logs (user_id, action, timestamp)
WITH RECURSIVE numbers (n) AS (
    SELECT 0
    UNION ALL
    SELECT n + 1 FROM numbers WHERE n < 399999
)
SELECT 
    n + 1,
    CONCAT("action", n),
    DATE('2025-11-10') + INTERVAL FLOOR(RAND() * 1825) DAY
FROM numbers;

# Создаём индексы
CREATE INDEX index_one ON logs (user_id);
CREATE INDEX index_two ON logs (timestamp);

# Проверяем выполнение
EXPLAIN 
SELECT * FROM logs
WHERE timestamp BETWEEN '2025-11-18' AND '2025-12-25';
```

## Без INDEX

```SQL
# Создаём таблицу logs
CREATE TABLE IF NOT EXISTS logs(
    log_id INT PRIMARY KEY AUTO_INCREMENT,
    user_id INT,
    action VARCHAR(20),
    timestamp TIMESTAMP
);

# Заполняем данными
SET SESSION cte_max_recursion_depth = 400000;

INSERT INTO logs (user_id, action, timestamp)
WITH RECURSIVE numbers (n) AS (
    SELECT 0
    UNION ALL
    SELECT n + 1 FROM numbers WHERE n < 399999
)
SELECT 
    n + 1,
    CONCAT("action", n),
    DATE('2025-11-10') + INTERVAL FLOOR(RAND() * 1825) DAY
FROM numbers;

# Проверяем выполнение
EXPLAIN 
SELECT * FROM logs
WHERE timestamp BETWEEN '2025-11-18' AND '2025-12-25';
```