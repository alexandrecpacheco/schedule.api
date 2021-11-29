namespace Schedule.Domain
{
    public class ApiSettings
    {
        public Environment Environment { get; set; } = new Environment();
    }

    public class Environment
    {
        public string CurrentEnvironment { get; set; }

        public bool IsProduction()
        {
            return CurrentEnvironment == "Production";
        }

        public bool IsStaging()
        {
            return CurrentEnvironment == "Staging";
        }

        public bool Development()
        {
            return CurrentEnvironment == "Development";
        }
    }
}
