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
        database = sqlite3.connect(self.__name)
        self.students.initialize()
        self.groups.initialize()
        database.close()

    def clear(self) -> None:
        database = sqlite3.connect(self.__name)
        self.students.clear()
        self.groups.clear()
        database.close()

    def connect(self) -> sqlite3.Connection:
        return sqlite3.connect(self.__name)