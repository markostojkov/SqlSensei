namespace WebApplication1
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            _ = builder.Services.AddControllers();
            _ = builder.Services.AddEndpointsApiExplorer();

            var app = builder.Build();

            _ = app.MapControllers();

            app.Run();
        }
    }
}