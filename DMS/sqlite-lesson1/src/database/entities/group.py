from datetime import date


class Group:
    def __init__(self, id: int, name: str, year: int, speciality_code: str) -> None:
        self.id = id
        self.name = name
        self.year = year
        self.speciality_code = speciality_code