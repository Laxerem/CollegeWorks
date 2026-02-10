from datetime import date


class Student:
    def __init__(self, id: int, name: str, birthday: date, group_id: int):
        self.id = id
        self.name = name
        self.birthday = birthday
        self.group_id = group_id

    def to_string(self) -> str:
        formated_data = self.birthday.strftime('%Y-%m-%d')
        return (f"ID: {self.id}\n"
                f"Name: {self.name}\n"
                f"Birthday: {formated_data}\n"
                f"Group ID: {self.group_id}")