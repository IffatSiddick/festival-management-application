namespace FestivalApp
{
    using System;

    internal class Vendor : Person
    {
        public List<String> ProductCategories { get; }

        public Vendor(int personID, string name, string number, string email, List<string> categories) 
            : base(personID, name, number, email, "VENDOR")
        {
            ProductCategories = categories;
        }

        public override void DisplayInformation()
        {
            Console.WriteLine($"ID: {PersonID}");
            Console.WriteLine($"Name: {Name}");
            Console.WriteLine($"Telephone: {Telephone}");
            Console.WriteLine($"Email: {Email}");
            Console.WriteLine($"Product Category/Categories:");

            foreach (String product in ProductCategories)
            {
                Console.WriteLine(product);
            }
        }
    }
}


