namespace CryptoSearch
{
    class Program
    {
        static async Task Main(string[] args)
        {
            ArgumentNullException.ThrowIfNull(args);

            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Title = "CryptoSearch - Новая эра поиска существующих Bitcoin адресов";

            ConsoleUI ui = new();
            await ui.RunAsync();
        }
    }
}