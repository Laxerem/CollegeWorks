from __future__ import annotations
from typing import TYPE_CHECKING

from database.entities.group import Group
from database.repositories.common.repository import Repository

if TYPE_CHECKING:
    from database.database import DataBase


class GroupsRepository(Repository):
    def __init__(self, db: DataBase):
        self.__db = db

    def initialize(self) -> None:
        database = self.__db.connect()
        cursor = database.cursor()

        cursor.execute('''
        CREATE TABLE IF NOT EXISTS Groups (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            year INTEGER NOT NULL,
            name TEXT NOT NULL,
            speciality_code TEXT NOT NULL
        )
        ''')

        database.commit()
        database.close()
    
    def clear(self):
        database = self.__db.connect()
        cursor = database.cursor()

        cursor.execute('''
        DELETE FROM Groups
        ''')

        database.commit()
        database.close()

    def add_group(self, name: str, year: int, speciality_code: str) -> int:
        database = self.__db.connect()
        cursor = database.cursor()

        cursor.execute('''
        INSERT INTO Groups (name, year, speciality_code) VALUES (?, ?, ?)
        ''', (name, year, speciality_code))
        database.commit()
        database.close()

        return cursor.lastrowid

    def get_group(self, id: int) -> Group | None:
        database = self.__db.connect()
        cursor = database.cursor()

        cursor.execute('''
        SELECT * FROM Groups WHERE id = ?
        ''', (id,))
        row = cursor.fetchone()

        if row is None:
            return None

        id = row[0]
        year = row[1]
        name = row[2]
        speciality_code = row[3]

        database.close()
        return Group(id, name, year, speciality_code)