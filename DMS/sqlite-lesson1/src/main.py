from datetime import date
from database.database import DataBase


def main() -> None:
    database = DataBase("./database/database.db")

    database.clear()
    database.initialize()

    database.groups.add_group("Группа А", 1994)
    database.groups.add_group("Группа А", 1000)

    first_id = database.students.add_student("Никита Абичкин", date(2008, 5, 8), 1994)
    database.students.delete_student(first_id)

    database.students.add_student("Жоский", date(2008, 5, 8), 1000)
    database.students.add_student("Гигачад", date(2008, 5, 8), 1994)
    second_id = database.students.add_student("Никита Абичкин", date(2008, 5, 8), 1994)
    student = database.students.get_student(second_id)

    student.birthday = date(2008, 5, 10)
    database.students.update_student(student)

    students = database.students.get_students_by_group(1994)

    for i in range(len(students)):
        print(students[i].to_string())

if __name__ == "__main__":
    main()