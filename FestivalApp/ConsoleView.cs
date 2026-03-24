using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * MAJOR THINGS TO BE FIXED: HANDLING INVALID INPUT
 */

namespace FestivalApp
{
    internal class ConsoleView
    {
        private FestivalManager manager;

        public ConsoleView(FestivalManager manager)
        {
            this.manager = manager;
        }

        // Main menu loop
        public void Run()
        {
            Console.WriteLine("╔═══════════════════════════════════════╗");
            Console.WriteLine("║    FESTIVAL MANAGEMENT SYSTEM         ║");
            Console.WriteLine("╚═══════════════════════════════════════╝");
            Console.WriteLine();

            bool running = true;

            while (running)
            {
                DisplayMenu();

                string choice = GetInput("Enter your choice (1-8): ");

                if (choice == "1")
                {
                    AddPerformer();
                }
                else if (choice == "2")
                {
                    AddCrew();
                }
                else if (choice == "3")
                {
                    AddVendor();
                }
                else if (choice == "4")
                {
                    FindPerson();
                }
                else if (choice == "5")
                {
                    ViewAllPeople();
                }
                else if (choice == "6")
                {
                    ViewByRole();
                }
                else if (choice == "7")
                {
                    EditPerson();
                }
                else if (choice == "8")
                {
                    DeletePerson();
                }
                else if (choice == "9")
                {
                    Console.WriteLine("\nThank you for using this Festival Management Application!");
                    running = false;
                }
                else
                {
                    Console.WriteLine("\n⚠ Invalid choice!");
                }
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        public void DisplayMenu()
        {
            Console.WriteLine("\n=== MAIN MENU ===");
            Console.WriteLine("1. Add New Performer");
            Console.WriteLine("2. Add New Crew");
            Console.WriteLine("3. Add New Vendor");
            Console.WriteLine("4. View one People");
            Console.WriteLine("5. View All People");
            Console.WriteLine("6. View All People in Role");
            Console.WriteLine("7. Edit Person information");
            Console.WriteLine("8. Delete Person");
            Console.WriteLine("9. Exit");
        }

        public void ShowMessage(string message)
        {
            Console.WriteLine("\nOK: " + message);
        }

        public void ShowError(string message)
        {
            Console.WriteLine("\nError: " + message);
        }

        public string GetInput(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine() ?? "";
        }

        // Add Performer menu
        private void AddPerformer()
        {
            Console.WriteLine("\n=== ADD NEW PERFORMER ===");

            Console.WriteLine("Name: ");
            string name = Console.ReadLine();

            Console.WriteLine("Telephone: ");
            string telephone = Console.ReadLine();

            Console.WriteLine("Email: ");
            string email = Console.ReadLine();

            Console.WriteLine("Fee: ");
            string fee_in = Console.ReadLine();

            int fee;

            if (!int.TryParse(fee_in, out fee))
            {
                Console.WriteLine("\n⚠ Invalid fee format. The fee must whole number.");
                return;
            }

            Console.Write("How many genres does this performer perform in? "); 
            string genre_in = Console.ReadLine();

            int genre_amount;

            if (!int.TryParse(genre_in, out genre_amount))
            {
                Console.WriteLine("\n⚠ Invalid format. You must enter the number of genres the performer performs in.");
                return;
            }

            List<string> genres = new List<string>();
            string genre;

            for (int count = 0; count < genre_amount; count ++)
            { 
                Console.WriteLine("Enter a genre: "); 
                genre = Console.ReadLine() ?? "";
                genres.Add(genre);
            }

            string result = manager.AddPerformer(name, telephone, email, fee, genres);

            Console.WriteLine();
            if (result.StartsWith("SUCCESS"))
            {
                Console.WriteLine(" ✓ " + result.Substring(9));
            }
            else
            {
                Console.WriteLine("⚠ " + result.Substring(7));
            }
        }

        // Add crew menu
        private void AddCrew()
        {
            Console.WriteLine("\n=== ADD NEW CREW ===");

            Console.WriteLine("Name: ");
            string name = Console.ReadLine();

            Console.WriteLine("Telephone: ");
            string telephone = Console.ReadLine();

            Console.WriteLine("Email: ");
            string email = Console.ReadLine();

            Console.WriteLine("Hourly rate: ");
            string rate_in = Console.ReadLine();
            int rate;

            if (!int.TryParse(rate_in, out rate))
            {
                Console.WriteLine("\n⚠ Invalid rate. The hourly rate must a whole number.");
                return;
            }

            Console.WriteLine("Employment type (FULL TIME/ PART TIME): ");
            string employment = Console.ReadLine();

            Console.WriteLine("Weekly hours: ");
            Console.WriteLine("FULL TIME: 25 - 40 hours");
            Console.WriteLine("PART TIME: 1 - 24 hours");
            string hours_in = Console.ReadLine();
            
            int hours;

            if (!int.TryParse(hours_in, out hours))
            {
                Console.WriteLine("\n⚠ Invalid hours. The weekly hours must a whole number.");
                return;
            }

            string result = manager.AddCrew(name, telephone, email, rate, employment, hours);

            Console.WriteLine();
            if (result.StartsWith("SUCCESS"))
            {
                Console.WriteLine(" ✓ " + result.Substring(9));
            }
            else
            {
                Console.WriteLine("⚠ " + result.Substring(7));
            }
        }

        // Add Vendor
        private void AddVendor()
        {
            Console.WriteLine("\n=== ADD NEW VENDOR ===");

            Console.Write("Name: ");
            string name = Console.ReadLine();

            Console.Write("Telephone: ");
            string telephone = Console.ReadLine();

            Console.Write("Email: ");
            string email = Console.ReadLine();

            Console.Write("How many products does this vendor sell? ");
            string category_in = Console.ReadLine();

            int category_amount;

            if (!int.TryParse(category_in, out category_amount))
            {
                Console.WriteLine("\n⚠ Invalid format. You must enter the number of categories the vendor sells.");
                return;
            }

            List<string> categories = new List<string>();
            string category;

            for (int count = 0; count < category_amount; count++)
            {
                Console.Write("Enter a category: ");
                category = Console.ReadLine();
                categories.Add(category);
            }

            string result = manager.AddVendor(name, telephone, email, categories);

            Console.WriteLine();
            if (result.StartsWith("SUCCESS"))
            {
                Console.WriteLine(" ✓ " + result.Substring(9));
            }
            else
            {
                Console.WriteLine("⚠ " + result.Substring(7));
            }
        }

        // View all people menu
        private void ViewAllPeople()
        {
            Console.WriteLine("\n=== ALL PEOPLE ===");

            List<Person> people = manager.GetAllPeople();

            if (people.Count == 0)
            {
                Console.WriteLine("No people have been added to the database.");
                return;
            }

            Console.WriteLine("Total people: " + people.Count);
            Console.WriteLine();

            for (int i = 0; i < people.Count; i++)
            {
                Console.WriteLine("--- Person #" + (i + 1) + " ---");
                people[i].DisplayInformation();
                Console.WriteLine();
            }
        }

        // View all people with role menu
        private void ViewByRole()
        {
            Console.WriteLine("\n=== ALL PEOPLE BY ROLE===");

            Console.WriteLine("What role do you want to search for? (PERFORMER/ CREW/ VENDOR)");
            string role = Console.ReadLine();

            List<Person> people = new List<Person>();
            string result = manager.GetByRole(role, out people);

            Console.WriteLine();
            if (result.StartsWith("ERROR"))
            {
                Console.WriteLine("⚠ " + result.Substring(7));
                return;
            }
            else
            {
                if (people.Count == 0)
                {
                    Console.WriteLine("No people in database with this role.");
                    return;
                }

                Console.WriteLine("Total people with this role: " + people.Count);
                Console.WriteLine();

                for (int i = 0; i < people.Count; i++)
                {
                    Console.WriteLine("--- Person #" + (i + 1) + " ---");
                    people[i].DisplayInformation();
                    Console.WriteLine();
                }
            }
        }

        // Edit person menu
        private void EditPerson()
        {
            Console.WriteLine("\n=== EDIT PERSON ===");
            Console.Write("Enter Person ID to edit: ");
            string id_in = Console.ReadLine();
            int id;

            if (!int.TryParse(id_in, out id))
            {
                Console.WriteLine("\n⚠ Invalid ID. The ID must be a number.");
                return;
            }

            Console.WriteLine("These are the field names all people have:");
            Console.WriteLine("name, telephone, email");

            Console.WriteLine("Performers has: fee");

            Console.WriteLine("Crew have these unique field names: hourly_rate, employment, weekly_hours");

            Console.Write("Enter the field to edit: ");
            string field = Console.ReadLine();

            Console.Write("Enter the value to replace the original value: ");
            string value = Console.ReadLine();

            string result = manager.EditPerson(id, field, value);

            Console.WriteLine();
            if (result.StartsWith("SUCCESS"))
            {
                Console.WriteLine(" ✓ " + result.Substring(9));
            }
            else
            {
                Console.WriteLine("⚠ " + result);
            }
        }

        // Delete person menu
        private void DeletePerson()
        {
            Console.WriteLine("\n=== DELETE PERSON ===");
            Console.Write("Enter Person ID to edit: ");
            string id_in = Console.ReadLine();
            int id;

            if (!int.TryParse(id_in, out id))
            {
                Console.WriteLine("\n⚠ Invalid id. The ids must be a number.");
                return;
            }

            string result = manager.DeletePerson(id);

            Console.WriteLine();
            if (result.StartsWith("SUCCESS"))
            {
                Console.WriteLine(" ✓ " + result.Substring(9));
            }
            else
            {
                Console.WriteLine("⚠ " + result.Substring(7));
            }
        }

        private void FindPerson()
        {
            Console.WriteLine("\n=== FIND PERSON ===");
            Console.Write("Enter name of person you want to find: ");
            string name = Console.ReadLine();

            List<Person> people = new List<Person>();
            string result = manager.FindPersonByName(name, out people);

            Console.WriteLine();
            if (result.StartsWith("ERROR"))
            {
                Console.WriteLine(" ✓ " + result.Substring(7));
            }
            else
            {
                for (int i = 0; i < people.Count; i++)
                {
                    Console.WriteLine("--- Person #" + (i + 1) + " ---");
                    people[i].DisplayInformation();
                    Console.WriteLine();
                }
            }
        }
    }
}
