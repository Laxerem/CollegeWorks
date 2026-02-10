from datetime import date
from database.database import DataBase


def main() -> None:
    database = DataBase("./database/database.db")

    database.initialize()
    database.clear()

    first_group_id = database.groups.add_group("Группа А", 1994, "09.02.02")
    second_group_id = database.groups.add_group("Группа A", 1000, "02.02.02")

    first_id = database.students.add_student("Никита Абичкин", date(2008, 5, 8), first_group_id)
    database.students.delete_student(first_id)

    database.students.add_student("Жоский", date(2008, 5, 8), second_group_id)
    database.students.add_student("Гигачад", date(2008, 5, 8), first_group_id)
    second_id = database.students.add_student("Никита Абичкин", date(2008, 5, 8), first_group_id)
    student = database.students.get_student(second_id)

    student.birthday = date(2008, 5, 10)
    database.students.update_student(student)

    students = database.students.get_students_by_group(first_group_id)

    print("---СТУДЕНТЫ С ГРУППОЙ 1994---")
    for i in range(len(students)):
        print(students[i].to_string())
    print("-----------------------------")
    print()
    print("--СТУДЕНТЫ С КОДОМ СПЕЦИАЛЬНОСТИ 09--")

    students2 = database.students.get_students_by_special_code("09")
    for i in range(len(students2)):
        print(students2[i].to_string())
    
    print("-------------------------------------")

if __name__ == "__main__":
    main()