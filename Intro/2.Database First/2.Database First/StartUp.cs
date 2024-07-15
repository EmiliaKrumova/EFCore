using Microsoft.EntityFrameworkCore;
using SoftUni.Data;
using SoftUni.Models;
using System.Globalization;
using System.Text;


namespace SoftUni
{
    public class StartUp
    {
       public static void Main(string[] args)
        {
           
            SoftUniContext context = new SoftUniContext();
            Console.WriteLine(GetAddressesByTown(context));

        }

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
