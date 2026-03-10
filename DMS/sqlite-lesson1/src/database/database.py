import sqlite3

from datetime import datetime
from database.repositories.groups_repository import GroupsRepository
from database.repositories.students_repository import StudentsRepository
from database.entities.student import Student


class DataBase:
    def __init__(self, file_path: str) -> None:
        self.__name = file_path
        self.students = StudentsRepository(self)
        self.groups = GroupsRepository(self)
    
    def initialize(self) -> None:
        self.students.initialize()
        self.groups.initialize()

    def clear(self) -> None:
        self.students.clear()
        self.groups.clear()

    def connect(self) -> sqlite3.Connection:
        connection = sqlite3.connect(self.__name)
        connection.execute('PRAGMA foreign_keys = ON')
        return connection