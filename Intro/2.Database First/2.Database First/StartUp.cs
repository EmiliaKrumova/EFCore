using Microsoft.Data.SqlClient.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using SoftUni.Data;
using SoftUni.Models;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;


namespace SoftUni
{
    public class StartUp
    {
       public static void Main(string[] args)
        {
           
            SoftUniContext context = new SoftUniContext();
            Console.WriteLine(GetAddressesByTownNew(context));

        }

        public static string GetAddressesByTownNew(SoftUniContext context)
        {
            var addresses = context.Addresses
                .Select(a => new
                {
                    a.AddressText,
                    TownName = a.Town.Name,
                    EmployeesCount = context.Employees.Count(e => e.AddressId == a.AddressId)
                })
                .OrderByDescending(a => a.EmployeesCount)
                .ThenBy(a => a.TownName)
                .ThenBy(a => a.AddressText)
                .Take(10)
                .ToList();

            var result = new StringBuilder();
            foreach (var address in addresses)
            {
                result.AppendLine($"{address.AddressText}, {address.TownName} - {address.EmployeesCount} employees");
            }

            return result.ToString().TrimEnd();
        }


        public static string RemoveTown(SoftUniContext context)
        {
            var townToDelete = context.Towns
                .Where(t => t.Name == "Seattle")
                .FirstOrDefault();
            if (townToDelete != null)
            {
                var adressesToDelete = context.Addresses
                    .Where(a => a.TownId == townToDelete.TownId)
                    .ToList();
                int countOfAdresses = adressesToDelete.Count();

                foreach (var adress in adressesToDelete)
                {
                    var employeesInAdres = context.Employees
                        .Where(e => e.AddressId == adress.AddressId)
                        .ToList();

                    foreach (var employee in employeesInAdres)
                    {
                        employee.AddressId = null;
                    }
                }
                context.SaveChanges();
                context.Addresses.RemoveRange(adressesToDelete);
                context.Towns.Remove(townToDelete);
                context.SaveChanges();
                return $"{countOfAdresses} addresses in Seattle were deleted";

            }
            else
            {
                return $"0 addresses in Seattle were deleted";
            }
            
        }
        //Write a program that deletes a town with name "Seattle".
        //Also, delete all addresses that are in those towns. Return the number of addresses that were deleted in format "{count} addresses in Seattle were deleted".
        //There will be employees living at those addresses, which will be a problem when trying to delete the addresses. So, start by setting the AddressId of each employee for the given address to null.
        //After all of them are set to null, you may safely remove all the addresses from the context and finally remove the given town.
        public static string DeleteProjectById(SoftUniContext context)
        {
            var projectToDelete = context.Projects.Find(2);
            if (projectToDelete != null)
            {
                
                var employeeProjects = context.EmployeesProjects
                    .Where(ep => ep.ProjectId == 2)
                    .ToList();

                context.EmployeesProjects.RemoveRange(employeeProjects);
               
                context.Projects.Remove(projectToDelete);
                
                context.SaveChanges();
            }
           
            var projectNames = context.Projects
                .Take(10)
                .Select(p => p.Name)
                .ToList();

            return string.Join(Environment.NewLine, projectNames);
        }
        //Let's delete the project with id 2. Then, take 10 projects and return their names, each on a new line. Remember to restore your database after this task.


        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {
            var employees = context.Employees.
                Where(e=>e.FirstName.ToLower().StartsWith("sa"))
                .Select(e=>new
                {
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    Job = e.JobTitle,
                    Salary = e.Salary
                })
                .OrderBy(e=>e.FirstName)
                .ThenBy(e=>e.LastName)
                .ToList();
            StringBuilder sb = new StringBuilder();
            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} - {employee.Job} - (${employee.Salary:F2})");
            }
            return sb.ToString().TrimEnd();
        }
        //Write a program that finds all employees whose first name starts with "Sa".
        //Return their first, last name, their job title and salary rounded to 2 symbols after the decimal separator in the format given in the example below.
        //Order them by the first name, then by last name (ascending).
        //Constraints
        // Find a way to make your query case -insensitive.
        public static string IncreaseSalaries(SoftUniContext context)
        {
            string[] departmentsToPromote = new []{"Engineering", "Tool Design", "Marketing", "Information Services"};

            var employeesToPromote = context.Employees
                .Where(e =>
                departmentsToPromote.Contains(e.Department.Name))
                .ToList();
            
            
            foreach(var employee in employeesToPromote)
            {
                employee.Salary *= 1.12M;
                
            }
            context.SaveChanges();

            var promotedToPrint = employeesToPromote
                .Select(pr => new
                {
                    FirstName = pr.FirstName,
                    LastName = pr.LastName,                   
                    Salary = pr.Salary
                    

                })
                .OrderBy(pr=>pr.FirstName)
                .ThenBy(pr=>pr.LastName) .ToList();
            StringBuilder sb = new StringBuilder();
            foreach( var employee in promotedToPrint)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} (${employee.Salary:F2})");
            }

            return sb.ToString().TrimEnd();

        }

        //Write a program that increases salaries of all employees that are in the Engineering, Tool Design, Marketing, or Information Services department by 12%.
        //Then return first name, last name and salary (2 symbols after the decimal separator) for those employees, whose salary was increased. Order them by first name (ascending), then by last name (ascending). Format of the output
        public static string GetLatestProjects(SoftUniContext context)
        {
            var projects = context.Projects.
                OrderByDescending(x=> x.StartDate)
                //.ThenByDescending(x => x.StartDate)
                //.ThenBy(x => x.Name)
                .Take(10)
                .OrderBy(x=>x.Name)
                .Select(x => new
                {
                    ProjectName = x.Name,
                    ProjectDescription = x.Description,
                    Startdate = x.StartDate.ToString("M/d/yyyy h:mm:ss tt",CultureInfo.InvariantCulture)
                }).ToList();

            StringBuilder sb = new StringBuilder();
            foreach (var project in projects)
            {
                sb.AppendLine(project.ProjectName);
                sb.AppendLine(project.ProjectDescription);
                sb.AppendLine(project.Startdate.ToString());
            }
            return sb.ToString().TrimEnd();
        }
        //Write a program that returns information about the last 10 started projects. Sort them by name lexicographically and return their name, description and start date, each on a new row. 

        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            var departments = context.Departments
                .Where(d => d.Employees.Count() > 5)
                .OrderBy(d => d.Employees.Count())
                .ThenBy(d => d.Name)
                .Select(d => new
                {
                    deptName = d.Name,
                    managerName = d.Manager.FirstName + " " + d.Manager.LastName,
                    employeesIndept = d.Employees
                    .Select(e => new
                    {
                        employeeFirstName = e.FirstName,
                        employeeLastName = e.LastName,
                        emplJob = e.JobTitle
                    }).OrderBy(e => e.employeeFirstName)
                    .ThenBy(e => e.employeeLastName)
                    .ToArray()

                })
                .ToArray();
            StringBuilder sb = new StringBuilder();          

            foreach (var department in departments)
            {
                sb.AppendLine($"{department.deptName} - {department.managerName}");
                foreach(var e in department.employeesIndept)
                {
                    sb.AppendLine($"{e.employeeFirstName} {e.employeeLastName} - {e.emplJob}");
                }
            }
            return sb.ToString().TrimEnd();
        }
        //Find all departments with more than 5 employees. Order them by employee count (ascending), then by department name (alphabetically).
        //For each department, print the department name and the manager's first and last name on the first row.
        //Then print the first name, the last name and the job title of every employee on a new row. Order the employees by first name (ascending), then by last name (ascending).
        //Format of the output: For each department print it in the format "<DepartmentName> - <ManagerFirstName>  <ManagerLastName>" and for each employee print it in the format "<EmployeeFirstName> <EmployeeLastName> - <JobTitle>".


        public static string GetEmployee147(SoftUniContext context)
        {
            var employee = context.Employees
                .Where(e => e.EmployeeId == 147)
                .Select(e=> new
                {
                    FirstName =   e.FirstName,
                     LastName = e.LastName,
                    JobTitle = e.JobTitle,
                    ProjectsNames = e.EmployeesProjects                
                    .Select(ep => ep.Project.Name)
                    .OrderBy(pn => pn)
                    .ToList()

                })
                .FirstOrDefault();
                StringBuilder sb = new StringBuilder();
            if(employee != null)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} - {employee.JobTitle}");

                foreach(var pr in employee.ProjectsNames)
                {
                    sb.AppendLine(pr);
                }
            }
            return sb.ToString().TrimEnd();
        }

        //Get the employee with id 147.
        //Return only his/her first name, last name, job title and projects (print only their names).
        //The projects should be ordered by name (ascending). Format of the output.

        public static string GetAddressesByTown(SoftUniContext context)
        {
            var result = context.Addresses
                .Select(a => new
                {
                    AddressText = a.AddressText,
                    TownName = a.Town.Name,
                    EmployeesCount = a.Employees.Count()
                })
                .OrderByDescending(a => a.EmployeesCount)
                .ThenBy(a => a.TownName)
                .ThenBy(a => a.AddressText)
                .Take(10)
                .ToArray();
            StringBuilder sb = new StringBuilder();

            foreach (var address in result)
            {
                sb.AppendLine($"{address.AddressText}, {address.TownName} - {address.EmployeesCount} employees");
            }

            return sb.ToString().TrimEnd();
        }
        //Find all addresses, ordered by the number of employees who live there (descending), then by town name (ascending)
        //and finally by address text (ascending).
        //Take only the first 10 addresses.
        //For each address return it in the format "<AddressText>, <TownName> - <EmployeeCount> employees"

        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            var employeesWithProjects = context.Employees
                .Take(10)
                .Select(e => new
                {
                    EmplFirstName = e.FirstName,
                    EmplLastName = e.LastName,
                    ManagerFirstName = e.Manager!.FirstName,
                    ManagerLastName = e.Manager!.LastName,

                    Projects = e.EmployeesProjects
                    .Where(ep => ep.Project.StartDate.Year >= 2001 && ep.Project.StartDate.Year <= 2003)
                    .Select(ep => new
                    {
                        ProjectName = ep.Project.Name,
                        StartDate = ep.Project.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture),
                        EndDate = ep.Project.EndDate.HasValue
                        ? ep.Project.EndDate.Value.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)
                        : "not finished"
                    })
                    .ToArray()
                }).ToArray();

            StringBuilder sb = new StringBuilder();
            foreach (var employee in employeesWithProjects)
            {
                sb
                    .AppendLine($"{employee.EmplFirstName} {employee.EmplLastName} - Manager: {employee.ManagerFirstName} {employee.ManagerLastName}");

                foreach(var project in employee.Projects)
                {
                    sb.AppendLine($"--{project.ProjectName} - {project.StartDate} - {project.EndDate}");
                }
            }
            return sb.ToString().TrimEnd();
        }

        public static string GetEmployeesInPeriodFirstMethod(SoftUniContext context)
        {
                            
                var employees = context.Employees
                    .Include(e => e.Manager)
                    .Include(e => e.EmployeesProjects)
                        .ThenInclude(ep => ep.Project)
                    .OrderBy(e => e.EmployeeId)
                    .Take(10)
                    .ToList();

                var result = new StringBuilder();

                foreach (var employee in employees)
                {
                   
                    result.AppendLine($"{employee.FirstName} {employee.LastName} - Manager: {employee.Manager?.FirstName} {employee.Manager?.LastName}");
                   
                    var projects = employee.EmployeesProjects
                        .Where(ep => ep.Project.StartDate.Year >= 2001 && ep.Project.StartDate.Year <= 2003)
                        .Select(ep => ep.Project)
                        .ToList();

                    if (projects.Any())                    {
                        

                        foreach (var project in projects)
                        {
                            var startDate = project.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
                            var endDate = project.EndDate.HasValue
                                ? project.EndDate.Value.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)
                                : "not finished";

                            result.AppendLine($"--{project.Name} - {startDate} - {endDate}");
                        }
                    }
                }

                return result.ToString().TrimEnd();
            


        }
        //Find the first 10 employees and print each employee's first name, last name, manager's first name and last name. If they have projects started in the period 2001 - 2003 (inclusive), print them with information about their name, start and end date. Then return all of their projects in the format "--<ProjectName> - <StartDate> - <EndDate>", each on a new row. If a project has no end date, print "not finished" instead.



        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            var address = new Address()
            {
                AddressText = "Vitoshka 15",
                TownId = 4

            };
            context.Addresses.Add(address);
            var employee = context.Employees.Where(e=>e.LastName== "Nakov").FirstOrDefault();
            if (employee != null)
            {
                employee.Address = address;
            }
           
            context.SaveChanges();

            var result = string.Join(Environment.NewLine, context.Employees
                .Where(e =>  e.Address != null)
                .OrderByDescending(e=>e.AddressId)                
                .Take(10)
                .Select(e=>e.Address.AddressText));
            return result;
        
        }
        //Create a new address with the text "Vitoshka 15" and TownId = 4. Set that address to the employee with last the name "Nakov".
        //Then order by descending all the employees by their Address' Id, take 10 rows and from them, take the AddressText. Return the results each on a new line:


        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
          var result =  string.Join(Environment.NewLine, context.Employees
              .OrderBy(e=>e.EmployeeId)
              .Select
              (e=> $"{e.FirstName} {e.LastName} {e.MiddleName} {e.JobTitle} {e.Salary:f2}")
              .ToList());

            return result;

            
            
        }

         public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            var result = string.Join(Environment.NewLine, context.Employees
                .Where(e => e.Salary > 50000)
                .OrderBy(e => e.FirstName)
                .Select(e => $"{e.FirstName} - {e.Salary:f2}"));
            return result;
        }

        //Your task is to extract all employees with salary over 50000. Return their first names and salaries in format "{firstName} – {salary}". Salary must be rounded to 2 symbols, after the decimal separator. Sort them alphabetically by first name.
        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            var result = string.Join(Environment.NewLine, context.Employees
                .Where(e => e.Department.Name == "Research and Development")
                .OrderBy(e => e.Salary)
                .ThenByDescending(e => e.FirstName)
                .Select(e => $"{e.FirstName} {e.LastName} from {e.Department.Name} - ${e.Salary:f2}"));

            return result;

        }
        //Extract all employees from the Research and Development department. Order them by salary (in ascending order), then by first name (in descending order). Return only their first name, last name, department name and salary rounded to 2 symbols, after the decimal separator in the format shown below:


    }
}
