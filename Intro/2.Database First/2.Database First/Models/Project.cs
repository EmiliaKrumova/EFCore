using System;
using System.Collections.Generic;

namespace SoftUni.Models
{
    public partial class Project
    {
        public Project()
        {
            // Initialize correct collection
            EmployeesProjects = new HashSet<EmployeeProject>();
        }

        public int ProjectId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        // This collection is NOT from Employees, But from EmployeesProjects
        public virtual ICollection<EmployeeProject> EmployeesProjects { get; set; }
    }
}
