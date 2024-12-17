public class WeatherForecast
{
    public CityInfo City { get; set; }
    public List<Forecast> List { get; set; }
}

public class CityInfo
{
    public string Name { get; set; }
    public string Country { get; set; }
}

public class Forecast
{
    public long Dt { get; set; } // Unix timestamp
    public Temperature Main { get; set; }
    public List<Weather> Weather { get; set; }
}

public class Temperature
{
    public double Temp { get; set; }
    public double TempMin { get; set; }
    public double TempMax { get; set; }
}

public class Weather
{
    public string Main { get; set; }
    public string Description { get; set; }
}
