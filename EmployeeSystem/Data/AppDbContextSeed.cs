using EmployeeSystem.Models;

namespace EmployeeSystem.Data
{
    public static class AppDbContextSeed
    {

        public static void Seed(AppDbContext db)
        {
            if (!db.Departments.Any())
            {
                db.Departments.AddRange(
                    new DepartmentModel { Name = "Human Resources", DepartmentSupervisor = "Ahmad" },
                    new DepartmentModel { Name = "Accounting", DepartmentSupervisor = "Saeed" },
                    new DepartmentModel { Name = "IT", DepartmentSupervisor = "Lina" },
                    new DepartmentModel { Name = "Marketing", DepartmentSupervisor = "Omar" },
                    new DepartmentModel { Name = "Sales", DepartmentSupervisor = "Nadia" },
                    new DepartmentModel { Name = "Customer Service", DepartmentSupervisor = "Khaled" },
                    new DepartmentModel { Name = "Research and Development", DepartmentSupervisor = "Mona" },
                    new DepartmentModel { Name = "Production", DepartmentSupervisor = "Yousef" },
                    new DepartmentModel { Name = "Logistics", DepartmentSupervisor = "Rana" },
                    new DepartmentModel { Name = "Legal", DepartmentSupervisor = "Tariq" }
                    );
                db.SaveChanges();
                Console.WriteLine("Done save the change");
            }

            if (!db.Employees.Any())
            {
                var depsByName = db.Departments.ToDictionary(d => d.Name, d => d.Id);
                db.Employees.AddRange(
                    new EmployeeModel
                    {
                        FirstName = "Momen",
                        LastName = "Raddad",
                        Position = "Software Engineer",
                        DateOfBirth = new DateTime(2002, 2, 10),
                        DateOfEmployment = new DateTime(2023, 1, 15),
                        EndOfServiceDate = null,
                        DepartmentId = depsByName["IT"]
                    },
                    new EmployeeModel
                    {
                        FirstName = "Sara",
                        LastName = "Ali",
                        Position = "HR Specialist",
                        DateOfBirth = new DateTime(1995, 5, 20),
                        DateOfEmployment = new DateTime(2020, 3, 10),
                        EndOfServiceDate = null,
                        DepartmentId = depsByName["Human Resources"]
                    },
                    new EmployeeModel
                    {
                        FirstName = "Omar",
                        LastName = "Hassan",
                        Position = "Accountant",
                        DateOfBirth = new DateTime(1990, 8, 15),
                        DateOfEmployment = new DateTime(2018, 7, 5),
                        EndOfServiceDate = null,
                        DepartmentId = depsByName["Accounting"]
                    },
                    new EmployeeModel
                    {
                        FirstName = "Lina",
                        LastName = "Khalil",
                        Position = "Marketing Manager",
                        DateOfBirth = new DateTime(1988, 11, 30),
                        DateOfEmployment = new DateTime(2015, 9, 25),
                        EndOfServiceDate = new DateTime(2023, 12, 31),
                        IsActive = false,
                        DepartmentId = depsByName["Marketing"]
                    },
                    new EmployeeModel
                    {
                        FirstName = "Yousef",
                        LastName = "Abdullah",
                        Position = "Sales Executive",
                        DateOfBirth = new DateTime(1992, 4, 18),
                        DateOfEmployment = new DateTime(2019, 6, 12),
                        EndOfServiceDate = new DateTime(2024, 5, 30),
                        DepartmentId = depsByName["Sales"],
                        IsActive = false
                    },
                    new EmployeeModel
                    {
                        FirstName = "Nadia",
                        LastName = "Saeed",
                        Position = "Customer Service Rep",
                        DateOfBirth = new DateTime(1998, 9, 22),
                        DateOfEmployment = new DateTime(2021, 2, 8),
                        EndOfServiceDate = null,
                        DepartmentId = depsByName["Customer Service"]
                    },
                    new EmployeeModel
                    {
                        FirstName = "Khaled",
                        LastName = "Fahmy",
                        Position = "R&D Scientist",
                        DateOfBirth = new DateTime(1985, 12, 5),
                        DateOfEmployment = new DateTime(2010, 4, 14),
                        EndOfServiceDate = null,
                        DepartmentId = depsByName["Research and Development"]
                    },
                    new EmployeeModel
                    {
                        FirstName = "Rana",
                        LastName = "Youssef",
                        Position = "Logistics Coordinator",
                        DateOfBirth = new DateTime(1993, 3, 27),
                        DateOfEmployment = new DateTime(2017, 11, 3),
                        EndOfServiceDate = null,

                        DepartmentId = depsByName["Logistics"]
                    }

                   );

                db.SaveChanges();
                Console.WriteLine("Done ");

            }



        }
    }
}

