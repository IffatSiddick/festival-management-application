namespace FestivalApp
{
    using System;
    using System.Xml.Linq;

    internal class Crew : Person
    {
        public int Rate { get; set; }
        public string Employment { get; private set; }
        public int Hours { get; private set; }

        public Crew(int personID, string name, string number, string email, 
            int user_rate, string employment, int user_hours)
            : base(personID, name, number, email, "CREW")
        {
            Rate = user_rate;
            Employment = employment;
            Hours = user_hours;
        }

        public void set_employment()
        {
            Console.WriteLine("What is the employment type?");
            Console.WriteLine("Enter F for full time or P for part time: ");

            string input = Console.ReadLine() ?? "";
    
            bool set = false;

            while (set == false)
            {
                if (input == "F")
                {
                    Employment = "Full time";
                    set = true;
                }
                else if (input == "P")
                {
                    Employment = "Part time";
                    set = true;
                }
                else
                {
                    Console.WriteLine("You have entered incorrectly. Please try again.");
                    Console.WriteLine("Enter F for full time or P for part time: ");
                    input = Console.ReadLine() ?? "";
                }
            }
        }

        public string set_hours(int hours)
        {
            if (Employment == "F" && (hours < 25 || hours > 40))
            {
                return "As a full time employee the hours must be between 25 - 40 hours per week.";
            }
            else if (Employment == "P" && (hours > 25 || hours < 1))
            {
                return "As a part time employee the hours must be between 1 - 24 hours per week.";
            }
            else
            {
                // set hours in db
                return "The crew members weekly hours have been set";
            }
        }

        public override void DisplayInformation()
        {
            Console.WriteLine($"ID: {PersonID}");
            Console.WriteLine($"Name: {Name}");
            Console.WriteLine($"Telephone: {Telephone}");
            Console.WriteLine($"Email: {Email}");
            Console.WriteLine($"Hourly rate: {Rate}");
            Console.WriteLine($"Employment: {Employment}");
            Console.WriteLine($"Weekly Hours: {Hours}");
        }
    }
}

