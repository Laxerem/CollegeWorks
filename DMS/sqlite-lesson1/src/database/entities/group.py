from datetime import date


class Group:
    def __init__(self, id: int, name: str, year: int) -> None:
        self.id = id
        self.name = name
        self.year = year