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

