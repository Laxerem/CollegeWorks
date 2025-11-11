# Удаляем старые таблицы, если есть
DROP TABLE IF EXISTS students;
DROP TABLE IF EXISTS marks;
DROP TABLE IF EXISTS subjects;

# Создание таблицы студентов
CREATE TABLE IF NOT EXISTS students (
    id INT PRIMARY KEY AUTO_INCREMENT,
    username VARCHAR(30),
    marks_id INT
);

# Создание таблицы предметов
CREATE TABLE IF NOT EXISTS subjects (
    id INT PRIMARY KEY AUTO_INCREMENT,
    item_name VARCHAR(30)
);

# Создание таблицы с оценками
CREATE TABLE IF NOT EXISTS marks (
    id INT PRIMARY KEY AUTO_INCREMENT,
    mark INT CHECK (mark BETWEEN 1 AND 5), # Оценка от 1 до 5
    item_id INT # Внешний ключ к предмету
);

# Связь таблицы студентов с оценками
ALTER TABLE students
ADD CONSTRAINT FOREIGN KEY (marks_id) REFERENCES marks (id);

# Связь таблицы оценок с предметами
ALTER TABLE marks
ADD CONSTRAINT FOREIGN KEY (item_id) REFERENCES subjects (id);

# Заполняем таблицу предметов
INSERT INTO subjects (item_name) VALUES 
("СУБД"),
("ООП"),
("АИСД"),
("Дискретка"),
("Инфа"),
("Git"),
("ОС"),
("Разработка ПО"),
("Дизайн"),
("Физра");

# Заполняем таблицу оценок
INSERT INTO marks (mark, item_id) VALUES 
(5, 1),
(4, 2),
(3, 3),
(5, 4),
(2, 5),
(4, 6),
(5, 7),
(3, 8),
(4, 9),
(5, 10);

# Заполняем таблицу студентов

INSERT INTO students (username, marks_id) VALUES
("АБИЧКИН", 1),
("ВОРОБЬЁВ", 2),
("ТИТОВ", 3),
("МС", 4),
("Журавлёв", 5),
("Лихачёв", 6),
("Тигунов", 7),
("Жеренский", 8),
("Владимир", 9),
("Богатырный", 10);
