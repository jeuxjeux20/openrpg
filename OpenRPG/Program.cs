namespace OpenRPG
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Bot.Instance.Start().GetAwaiter().GetResult();
        }
    }
}