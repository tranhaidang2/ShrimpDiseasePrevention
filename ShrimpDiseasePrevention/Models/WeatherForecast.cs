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
    public Clouds Clouds { get; set; } // Thông tin mây
    public Wind Wind { get; set; }     // Thông tin gió
    public int Visibility { get; set; } // Tầm nhìn (mét)
    public double Pop { get; set; }    // Xác suất mưa
}

public class Temperature
{
    public double Temp { get; set; }
    public double FeelsLike { get; set; } // Nhiệt độ cảm nhận
    public double TempMin { get; set; }
    public double TempMax { get; set; }
    public int Pressure { get; set; }    // Áp suất
    public int Humidity { get; set; }    // Độ ẩm
}

public class Weather
{
    public string Main { get; set; }
    public string Description { get; set; }
    public string Icon { get; set; }
}

public class Clouds
{
    public int All { get; set; } // Mức độ bao phủ của mây (%)
}

public class Wind
{
    public double Speed { get; set; } // Tốc độ gió (m/s)
    public int Deg { get; set; }      // Hướng gió (độ)
}
