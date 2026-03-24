namespace FestivalApp
{
    using System;

    internal class Performer : Person
    {
        public int Fee { get; set; }
        public List<String> Genres { get; set; }

        public Performer(int personID, string name, string number, string email, 
            int user_fee, List<string> genres)
            : base(personID, name, number, email, "PERFORMER")
        {
            Fee = user_fee;
            Genres = genres;
        }

        public List<string> GetGenres()
        {
            return Genres;
        }

        public override void DisplayInformation()
        {
            Console.WriteLine($"ID: {PersonID}");
            Console.WriteLine($"Name: {Name}");
            Console.WriteLine($"Telephone: {Telephone}");
            Console.WriteLine($"Email: {Email}");
            Console.WriteLine($"Fee: {Fee}");
            Console.WriteLine($"Genre/s:");

            foreach (String genre in Genres)
            {
                Console.WriteLine(genre);
            }
        }
    }
}


