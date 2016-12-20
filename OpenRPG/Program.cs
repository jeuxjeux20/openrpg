namespace OpenRPG
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            new Bot().Start().GetAwaiter().GetResult();
        }
    }
}