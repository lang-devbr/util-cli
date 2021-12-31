namespace UtilCli.App.Commands.Weather.Contracts
{
    public class Params
    {
        public string query { get; set; }
        public string language { get; set; }
        public string locationType { get; set; }
    }

    public class WeatherRequest
    {
        public string name { get; set; }
        public Params @params { get; set; }
    }


}
