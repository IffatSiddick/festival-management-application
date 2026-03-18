namespace FestivalApp
{
    using System;

    internal class Vendor : Person
    {
        public List<String> product_categories { get; }

        public Vendor(int personID, string name, string number, string email, List<string> categories) 
            : base(personID, name, number, email, "VENDOR")
        {
            product_categories = categories;
        }

        public List<string> get_product_categoriess()
        {
            return product_categories;
        }

        public void DisplayInformation()
        {
            Console.WriteLine($"ID: {PersonID}");
            Console.WriteLine($"Name: {Name}");
            Console.WriteLine($"Telephone: {Telephone}");
            Console.WriteLine($"Email: {Email}");
            Console.WriteLine($"Product Category/Categories: {product_categories}");
        }
    }
}


