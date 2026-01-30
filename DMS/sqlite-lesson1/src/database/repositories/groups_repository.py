from __future__ import annotations
from typing import TYPE_CHECKING

from database.entities.group import Group
from database.repositories.repository import Repository

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
            year INTEGER INTEGER PRIMARY KEY,
            name TEXT NOT NULL
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

    def add_group(self, name: str, year: int) -> int:
        database = self.__db.connect()
        cursor = database.cursor()

        cursor.execute('''
        INSERT INTO Groups (name, year) VALUES (?, ?)
        ''', (name, year))
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
        name = row[1]
        year = row[2]

        database.close()
        return Group(id, name, year)