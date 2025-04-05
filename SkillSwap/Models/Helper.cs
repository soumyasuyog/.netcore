namespace SkillSwap.Models
{
    public class Helper
    {
        public static string GetApiUrl ()
        {
            IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            return config.GetValue<string>("APIbasePath");
        }
    }
}
