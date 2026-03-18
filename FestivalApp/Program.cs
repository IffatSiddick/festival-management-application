namespace FestivalApp
{
    internal class Program
    {

        static void Main(string[] args)
        {
            // Database access layer
            SQLRepository2 repo = new SQLRepository2();

            // Business logic layer
            FestivalManager manager = new FestivalManager(repo);

            // Presentation layer
            ConsoleView view = new ConsoleView(manager);
            view.Run();
            Console.ReadKey();
        }
    }
}
