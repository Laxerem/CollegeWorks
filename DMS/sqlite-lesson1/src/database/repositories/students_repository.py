from __future__ import annotations
from datetime import datetime
from typing import TYPE_CHECKING

from database.repositories.repository import Repository
from database.entities.student import Student

if TYPE_CHECKING:
    from database.database import DataBase


class StudentsRepository(Repository):
    def __init__(self, db: "DataBase"):
        self.__db = db

    def initialize(self) -> None:
        database = self.__db.connect()
        database.execute('PRAGMA foreign_keys = ON') # Enable foreign key constraints
        cursor = database.cursor()

        cursor.execute('''
        CREATE TABLE IF NOT EXISTS Students (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            group_id INTEGER,
            name TEXT NOT NULL,
            birthday TEXT NOT NULL,
            FOREIGN KEY (group_id) REFERENCES Groups(id)
        )
        ''')

        database.commit()
        database.close()

    def clear(self):
        database = self.__db.connect()
        cursor = database.cursor()

        cursor.execute('''
        DELETE FROM Students
        ''')

        database.commit()
        database.close()

    def add_student(self, name: str, birthday: datetime.date, group_id: int) -> int:
        database = self.__db.connect()
        cursor = database.cursor()

        string_data = birthday.strftime('%Y-%m-%d')

        cursor.execute(f'''
        INSERT INTO Students (group_id, name, birthday) VALUES (?, ?, ?)
        ''', (group_id, name, string_data))
        database.commit()
        database.close()

        return cursor.lastrowid

    def delete_student(self, id: int) -> bool:
        database = self.__db.connect()
        cursor = database.cursor()

        cursor.execute('''
        DELETE FROM Students WHERE id = ?
        ''', (id,))
        database.commit()
        database.close()

        return cursor.rowcount > 0

    def get_student(self, id: int) -> Student | None:
        database = self.__db.connect()
        cursor = database.cursor()

        cursor.execute(f'''SELECT * FROM Students WHERE id = ?''', (id,))
        row = cursor.fetchone()

        if (row[0] is None) or (row[1] is None) or (row[2] is None):
            return None

        id = row[0]
        group_id = row[1]
        name = row[2]
        birthday_string = row[3]

        birthday = datetime.strptime(birthday_string, '%Y-%m-%d').date()

        database.close()
        return Student(id, name, birthday, group_id)

    def get_students_by_group(self, group_id: int) -> list[Student]:
        database = self.__db.connect()
        cursor = database.cursor()

        cursor.execute('''
        SELECT * FROM Students WHERE group_id = ?
        ''', (group_id,))
        rows = cursor.fetchall()

        students = []
        for row in rows:
            id = row[0]
            group_id = row[1]
            name = row[2]
            birthday_string = row[3]

            birthday = datetime.strptime(birthday_string, '%Y-%m-%d').date()
            students.append(Student(id, name, birthday, group_id))
            
        database.close()
        return students

    def update_student(self, player: Student) -> bool:
        database = self.__db.connect()
        cursor = database.cursor()

        cursor.execute('''
        UPDATE Students SET name = ?, birthday = ?, group_id = ? WHERE id = ?
        ''', (player.name, player.birthday, player.group_id, player.id))
        database.commit()
        database.close()

        return cursor.rowcount > 0;
