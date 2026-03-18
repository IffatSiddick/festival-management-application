namespace FestivalApp
{
    using System;

    internal class Person
    {
        public int PersonID { get; private set; }
        public string Name { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }

        public Person(int personID, string user_name, string user_number, string user_email, string user_role)
        {
            PersonID = personID;
            Name = user_name;
            Telephone = user_number;
            Email = user_email;
            Role = user_role;
        }

        public virtual void DisplayInformation()
        {
            Console.WriteLine($"ID: {PersonID}");
            Console.WriteLine($"Name: {Name}");
            Console.WriteLine($"Telephone: {Telephone}");
            Console.WriteLine($"Email: {Email}");
            Console.WriteLine($"Role: {Role}");
        }
    }


}

