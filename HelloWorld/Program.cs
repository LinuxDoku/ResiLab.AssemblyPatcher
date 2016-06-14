namespace HelloWorld {
    public class Program {
        private static string _helloWorld = "Hello World";

        public static void Main(string[] args) {
            System.Console.WriteLine(_helloWorld);
        }

        public static string GenerateHelloWorld()
        {
            return "Hello World";
        }
    }
}
