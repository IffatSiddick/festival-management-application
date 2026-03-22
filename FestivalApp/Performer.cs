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

        public List<string> get_genres()
        {
            return Genres;
        }

        public void set_genre(int user_amount)
        {
            int amount = user_amount;
            // need to send this to something else to go through and add all genres
            for (int count = 0; count < user_amount; count++)
            {
                Console.WriteLine("What genre does the performer perform in?");
                string Genre = Console.ReadLine() ?? "";
                Genres.Add(Genre);
            }
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


