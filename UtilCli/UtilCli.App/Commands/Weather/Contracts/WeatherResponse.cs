using Newtonsoft.Json;

namespace UtilCli.App.Commands.Weather.Contracts
{
    public class Locale
    {
        public string locale1 { get; set; }
        public string locale2 { get; set; }
        public string locale3 { get; set; }
        public object locale4 { get; set; }
    }

    public class Location
    {
        public List<string> address { get; set; }
        public List<string> adminDistrict { get; set; }
        public List<string> adminDistrictCode { get; set; }
        public List<string> city { get; set; }
        public List<string> country { get; set; }
        public List<string> countryCode { get; set; }
        public List<string> displayName { get; set; }
        public List<string> ianaTimeZone { get; set; }
        public List<double> latitude { get; set; }
        public List<Locale> locale { get; set; }
        public List<double> longitude { get; set; }
        public List<object> neighborhood { get; set; }
        public List<string> placeId { get; set; }
        public List<string> postalCode { get; set; }
        public List<string> postalKey { get; set; }
        public List<bool> disputedArea { get; set; }
        public List<object> disputedCountries { get; set; }
        public List<object> disputedCountryCodes { get; set; }
        public List<object> disputedCustomers { get; set; }
        public List<List<bool>> disputedShowCountry { get; set; }
        public List<string> iataCode { get; set; }
        public List<string> icaoCode { get; set; }
        public List<object> locId { get; set; }
        public List<object> locationCategory { get; set; }
        public List<string> pwsId { get; set; }
        public List<string> type { get; set; }
    }

    public class Data
    {
        public Location location { get; set; }
    }

    public class QueryUtilCli
    {
        public bool loading { get; set; }
        public bool loaded { get; set; }
        public Data data { get; set; }
        public int status { get; set; }
        public string statusText { get; set; }
    }

    public class GetSunV3LocationSearchUrlConfig
    {
        [JsonProperty($"queryUtilCli")]
        public QueryUtilCli queryUtilCli { get; set; }
    }

    public class Dal
    {
        public GetSunV3LocationSearchUrlConfig getSunV3LocationSearchUrlConfig { get; set; }
    }

    public class WeatherResponse
    {
        public Dal dal { get; set; }
    }


}
