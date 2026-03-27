# СУБД

## Создаём и заполняем таблицы

```sql
DROP TABLE IF EXISTS orders CASCADE;
DROP TABLE IF EXISTS users CASCADE;

CREATE TABLE users (
    id SERIAL PRIMARY KEY,
    username VARCHAR(50) NOT NULL,
    email VARCHAR(100) UNIQUE NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE orders (
    id SERIAL PRIMARY KEY,
    user_id INT NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    cost DECIMAL(10, 2) NOT NULL,
    order_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- НИЖНИЙ РЕГИСТР
INSERT INTO users (username, email)
SELECT 
    'user_' || i, 
    'user_' || i || '@example.com'
FROM generate_series(1, 25000) AS i;

-- ВЕРХНИЙ РЕГИСТР
INSERT INTO users (username, email)
SELECT 
    'USER_' || i, 
    'USER_' || i || '@EXAMPLE.COM'
FROM generate_series(1, 25000) AS i;

-- В перемешку
INSERT INTO users (username, email)
SELECT 
    'UseR_' || i, 
    'User_' || i || '@example.COM'
FROM generate_series(1, 25000) AS i;

INSERT INTO users (username, email)
SELECT 
    'User_' || i, 
    'User_' || i || '@ExaMPle.com'
FROM generate_series(1, 25000) AS i;

INSERT INTO orders (user_id, cost)
SELECT 
    floor(random() * 100000 + 1)::int, 
    round((random() * 1000 + 10)::numeric, 2) 
FROM generate_series(1, 300000);
```


Индекс на overcase или apercase 
Что то там на нижний регистр и на верхний регистор
Какие то запросы к верхнему регистеру и explain

c 15 по 23 декабря 25 года
создаёшь индекс на статус
Индексы на apper и lower

Все виды строк с регистрами

## Проверка без индексов

```sql
EXPLAIN ANALYZE
SELECT * FROM orders WHERE cost > 500;

/* 
"Seq Scan on orders  (cost=0.00..5661.00 rows=152982 width=22) (actual time=0.010..26.680 rows=153152.00 loops=1)"
"  Filter: (cost > '500'::numeric)"
"  Rows Removed by Filter: 146848"
"  Buffers: shared hit=1911"
"Planning:"
"  Buffers: shared hit=28"
"Planning Time: 0.176 ms"
"Execution Time: 30.836 ms"
*/

EXPLAIN ANALYZE SELECT * FROM orders WHERE user_id = 150 AND cost > 500;

/*
"Gather  (cost=1000.00..5558.26 rows=2 width=22) (actual time=10.520..19.843 rows=2.00 loops=1)"
"  Workers Planned: 1"
"  Workers Launched: 1"
"  Buffers: shared hit=1911"
"  ->  Parallel Seq Scan on orders  (cost=0.00..4558.06 rows=1 width=22) (actual time=10.295..13.004 rows=1.00 loops=2)"
"        Filter: ((cost > '500'::numeric) AND (user_id = 150))"
"        Rows Removed by Filter: 149999"
"        Buffers: shared hit=1911"
"Planning Time: 0.119 ms"
"Execution Time: 19.868 ms"
*/

EXPLAIN ANALYZE SELECT * FROM users WHERE email = 'user1@example.com';

/*
"Index Scan using users_email_key on users  (cost=0.42..8.44 rows=1 width=44) (actual time=0.018..0.019 rows=0.00 loops=1)"
"  Index Cond: ((email)::text = 'user1@example.com'::text)"
"  Index Searches: 1"
"  Buffers: shared hit=3"
"Planning:"
"  Buffers: shared hit=43 dirtied=1"
"Planning Time: 0.172 ms"
"Execution Time: 0.034 ms"
*/

EXPLAIN ANALYZE SELECT * FROM users WHERE lower(email) = 'user1@example.com';

/*
"Seq Scan on users  (cost=0.00..2435.00 rows=500 width=44) (actual time=31.126..31.126 rows=0.00 loops=1)"
"  Filter: (lower((email)::text) = 'user1@example.com'::text)"
"  Rows Removed by Filter: 100000"
"  Buffers: shared hit=935"
"Planning Time: 0.049 ms"
"Execution Time: 31.141 ms"
*/

EXPLAIN ANALYZE SELECT * FROM users WHERE upper(email) = 'USER1@EXAMPLE.COM';

/*
"Seq Scan on users  (cost=0.00..2435.00 rows=500 width=44) (actual time=30.738..30.738 rows=0.00 loops=1)"
"  Filter: (upper((email)::text) = 'USER1@EXAMPLE.COM'::text)"
"  Rows Removed by Filter: 100000"
"  Buffers: shared hit=935"
"Planning Time: 0.079 ms"
"Execution Time: 30.758 ms"
*/

```

## Использование индексов

### 1. Исследование с обычными индексами 

```sql
CREATE INDEX idx_orders_order_cost ON orders(cost DESC);

EXPLAIN ANALYZE
SELECT * FROM orders WHERE cost > 500;
/*
Слишком много записей - postgres делает seq scan
"Seq Scan on orders  (cost=0.00..5661.00 rows=153022 width=22) (actual time=0.012..26.170 rows=152851.00 loops=1)"
"  Filter: (cost > '500'::numeric)"
"  Rows Removed by Filter: 147149"
"  Buffers: shared hit=1911"
"Planning Time: 0.082 ms"
"Execution Time: 30.297 ms"
*/

EXPLAIN ANALYZE
SELECT * FROM orders WHERE cost > 950;
/*
Записей не так много - postgres использует дерево индексов
"Bitmap Heap Scan on orders  (cost=340.86..2478.37 rows=18121 width=22) (actual time=2.167..6.384 rows=17956.00 loops=1)"
"  Recheck Cond: (cost > '950'::numeric)"
"  Heap Blocks: exact=1911"
"  Buffers: shared hit=1911 read=52"
"  ->  Bitmap Index Scan on idx_orders_order_cost  (cost=0.00..336.33 rows=18121 width=0) (actual time=1.988..1.988 rows=17956.00 loops=1)"
"        Index Cond: (cost > '950'::numeric)"
"        Index Searches: 1"
"        Buffers: shared read=52"
"Planning Time: 0.119 ms"
"Execution Time: 6.906 ms"
*/
```

### 2. Исследование с составными индексами

```sql
CREATE INDEX idx_composite_orders_order_cost ON orders(user_id, cost DESC);

EXPLAIN ANALYZE SELECT * FROM orders WHERE user_id = 150 AND cost > 500;
/*
"Bitmap Heap Scan on orders  (cost=4.44..12.28 rows=2 width=22) (actual time=0.146..0.148 rows=1.00 loops=1)"
"  Recheck Cond: ((user_id = 150) AND (cost > '500'::numeric))"
"  Heap Blocks: exact=1"
"  Buffers: shared hit=1 read=3"
"  ->  Bitmap Index Scan on idx_composite_orders_order_cost  (cost=0.00..4.44 rows=2 width=0) (actual time=0.135..0.135 rows=1.00 loops=1)"
"        Index Cond: ((user_id = 150) AND (cost > '500'::numeric))"
"        Index Searches: 1"
"        Buffers: shared read=3"
"Planning:"
"  Buffers: shared hit=18 read=1"
"Planning Time: 0.327 ms"
"Execution Time: 0.168 ms"
*/
```

### 3. Исследование с функциональными индексами

```sql
CREATE INDEX idx_composite_users_email_upper ON users(UPPER(email));
CREATE INDEX idx_composite_users_email_lower ON users(LOWER(email));

EXPLAIN ANALYZE SELECT * FROM users WHERE upper(email) = 'USER1@EXAMPLE.COM';
/*
"Bitmap Heap Scan on users  (cost=12.29..829.58 rows=500 width=44) (actual time=0.060..0.061 rows=0.00 loops=1)"
"  Recheck Cond: (upper((email)::text) = 'USER1@EXAMPLE.COM'::text)"
"  Buffers: shared hit=3"
"  ->  Bitmap Index Scan on idx_composite_users_email_upper  (cost=0.00..12.17 rows=500 width=0) (actual time=0.050..0.050 rows=0.00 loops=1)"
"        Index Cond: (upper((email)::text) = 'USER1@EXAMPLE.COM'::text)"
"        Index Searches: 1"
"        Buffers: shared hit=3"
"Planning:"
"  Buffers: shared hit=76"
"Planning Time: 0.526 ms"
"Execution Time: 0.110 ms"
*/

EXPLAIN ANALYZE SELECT * FROM users WHERE lower(email) = 'user1@example.com';

/*
"Bitmap Heap Scan on users  (cost=12.29..829.58 rows=500 width=44) (actual time=0.041..0.041 rows=0.00 loops=1)"
"  Recheck Cond: (lower((email)::text) = 'user1@example.com'::text)"
"  Buffers: shared hit=3"
"  ->  Bitmap Index Scan on idx_composite_users_email_lower  (cost=0.00..12.17 rows=500 width=0) (actual time=0.025..0.025 rows=0.00 loops=1)"
"        Index Cond: (lower((email)::text) = 'user1@example.com'::text)"
"        Index Searches: 1"
"        Buffers: shared hit=3"
"Planning:"
"  Buffers: shared hit=2"
"Planning Time: 0.129 ms"
"Execution Time: 0.064 ms"
*/

```
