namespace EmployeeSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();
            // middleware, endpoints, etc.

            app.MapGet("/", () => "initialize Employee System Project");

            app.Run();
        }
    }
}
