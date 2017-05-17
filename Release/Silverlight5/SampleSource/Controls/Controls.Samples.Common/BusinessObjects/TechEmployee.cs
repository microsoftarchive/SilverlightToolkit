// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows.Media.Imaging;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// A person object.
    /// </summary>
    public sealed partial class TechEmployee
    {
        /// <summary>
        /// Gets or sets the name of the person.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the number of check-ins.
        /// </summary>
        public int AssignedBugs { get; set; }

        /// <summary>
        /// Gets or sets the number of resolved bugs.
        /// </summary>
        public int ResolvedBugs { get; set; }

        /// <summary>
        /// Gets or sets the TechEmployee salary.
        /// </summary>
        public double Salary { get; set; }

        /// <summary>
        /// Gets a list of reports.
        /// </summary>
        public IList<TechEmployee> Reports { get; private set; }

        /// <summary>
        /// Initializes a new instance of the TechEmployee class.
        /// </summary>
        public TechEmployee()
        {
            this.Reports = new ObservableCollection<TechEmployee>();
        }

        /// <summary>
        /// Gets all the tech employees of the fictional company.
        /// </summary>
        public static ObservableCollection<TechEmployee> AllTechEmployees
        {
            get
            {
                return new ObservableCollection<TechEmployee>() 
                { 
                    new TechEmployee 
                    { 
                        Name = "John Barner", 
                        Salary = 103000.00, 
                        AssignedBugs = 4, 
                        ResolvedBugs = 2,
                        Reports = 
                            {
                                new TechEmployee 
                                { 
                                    Name = "Terrence Whistler", 
                                    Salary = 92000.00, 
                                    AssignedBugs = 2, 
                                    ResolvedBugs = 3,
                                    Reports = 
                                    {
                                        new TechEmployee 
                                        { 
                                            Name = "Eddie Husain", 
                                            Salary = 92000.00, 
                                            AssignedBugs = 2, 
                                            ResolvedBugs = 3,
                                            Reports = 
                                            {
                                                new TechEmployee { Name = "Shawn Wilcox", Salary = 132342.33, AssignedBugs = 3, ResolvedBugs = 12 },
                                                new TechEmployee { Name = "David Burke", Salary = 341092.00, AssignedBugs = 3, ResolvedBugs = 8 },
                                                new TechEmployee 
                                                { 
                                                    Name = "Ted Li", 
                                                    Salary = 103000.00, 
                                                    AssignedBugs = 4, 
                                                    ResolvedBugs = 2,
                                                    Reports = 
                                                    {
                                                        new TechEmployee { Name = "Donald Bishop", Salary = 92000.00, AssignedBugs = 2, ResolvedBugs = 3 },
                                                        new TechEmployee { Name = "Eliza Bush", Salary = 120000.20, AssignedBugs = 2, ResolvedBugs = 9 },
                                                        new TechEmployee { Name = "Jackie Hennesee", Salary = 132342.33, AssignedBugs = 3, ResolvedBugs = 12 },                                            
                                                    }
                                                }
                                            }
                                        },
                                        new TechEmployee { Name = "Jafar Zhang", Salary = 120000.20, AssignedBugs = 2, ResolvedBugs = 9 },
                                        new TechEmployee { Name = "Ning Anson", Salary = 132342.33, AssignedBugs = 3, ResolvedBugs = 12 },
                                    }
                                },
                                new TechEmployee 
                                { 
                                    Name = "Django Cesar", 
                                    Salary = 120000.20, 
                                    AssignedBugs = 2, 
                                    ResolvedBugs = 9,
                                    Reports =
                                    {
                                        new TechEmployee { Name = "Tim Wakely", Salary = 92000.00, AssignedBugs = 2, ResolvedBugs = 3 },
                                        new TechEmployee { Name = "Persipheny Gold", Salary = 120000.20, AssignedBugs = 2, ResolvedBugs = 9 },
                                        new TechEmployee { Name = "Paris Meijer", Salary = 132342.33, AssignedBugs = 3, ResolvedBugs = 12 },
                                    }
                                },
                                new TechEmployee 
                                { 
                                    Name = "Torrence Perdue", 
                                    Salary = 132342.33, 
                                    AssignedBugs = 3, 
                                    ResolvedBugs = 12,
                                    Reports = 
                                    {
                                        new TechEmployee { Name = "Erika Dyer", Salary = 341092.00, AssignedBugs = 3, ResolvedBugs = 8 },
                                        new TechEmployee { Name = "Tim Wakely", Salary = 103000.00, AssignedBugs = 4, ResolvedBugs = 2 },
                                    }
                                },
                                new TechEmployee { Name = "Amber Johnson", Salary = 341092.00, AssignedBugs = 3, ResolvedBugs = 8 },
                                new TechEmployee { Name = "Suzanne Tumbler", Salary = 103000.00, AssignedBugs = 4, ResolvedBugs = 2 },
                                new TechEmployee 
                                { 
                                    Name = "Adam Achava", 
                                    Salary = 92000.00, 
                                    AssignedBugs = 2, 
                                    ResolvedBugs = 3,
                                    Reports = 
                                        {
                                            new TechEmployee { Name = "Carol Teasley", Salary = 92000.00, AssignedBugs = 2, ResolvedBugs = 3 },
                                            new TechEmployee { Name = "Jim Bonafice", Salary = 120000.20, AssignedBugs = 2, ResolvedBugs = 9 },
                                            new TechEmployee { Name = "Cain Cable", Salary = 132342.33, AssignedBugs = 3, ResolvedBugs = 12 },
                                            new TechEmployee 
                                            { 
                                                Name = "Cairo Cady", 
                                                Salary = 341092.00, 
                                                AssignedBugs = 3, 
                                                ResolvedBugs = 8,
                                                Reports = 
                                                {
                                                    new TechEmployee 
                                                    { 
                                                        Name = "Michael Beasley", 
                                                        Salary = 103000.00, 
                                                        AssignedBugs = 4, 
                                                        ResolvedBugs = 2,
                                                        Reports =
                                                        {
                                                            new TechEmployee { Name = "Jeff Johnson", Salary = 341092.00, AssignedBugs = 3, ResolvedBugs = 8 },
                                                            new TechEmployee { Name = "Jared Duframe", Salary = 103000.00, AssignedBugs = 4, ResolvedBugs = 2 },
                                                            new TechEmployee { Name = "Cynthia Diamond", Salary = 92000.00, AssignedBugs = 2, ResolvedBugs = 3 },
                                                            new TechEmployee { Name = "Paris Bonner", Salary = 120000.20, AssignedBugs = 2, ResolvedBugs = 9 },                                                    
                                                        }
                                                    },
                                                    new TechEmployee { Name = "Kevin Scott", Salary = 92000.00, AssignedBugs = 2, ResolvedBugs = 3 },
                                                    new TechEmployee { Name = "Cinzia Crossley", Salary = 120000.20, AssignedBugs = 2, ResolvedBugs = 9 },
                                                    new TechEmployee { Name = "Cicero Foxley", Salary = 132342.33, AssignedBugs = 3, ResolvedBugs = 12 }
                                                }
                                            },
                                        }
                                },
                                new TechEmployee { Name = "Acotas Abner", Salary = 120000.20, AssignedBugs = 2, ResolvedBugs = 9 },
                                new TechEmployee { Name = "Adahy Adelio", Salary = 132342.33, AssignedBugs = 3, ResolvedBugs = 12 }
                            }
                    },
                };
            }
        }
    }
}