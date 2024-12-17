using Microsoft.AspNetCore.Mvc;
using ShrimpDiseasePrevention.Services;
using ShrimpDiseasePrevention.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ShrimpDiseasePrevention.Controllers
{
    public class WeatherController : Controller
    {
        private readonly WeatherService _weatherService;
        private readonly DiseaseRiskService _diseaseRiskService;

        private readonly Dictionary<string, string> _weatherDescriptionTranslations = new Dictionary<string, string>
{
    { "clear sky", "Trời quang" },
    { "few clouds", "Một vài đám mây" },
    { "scattered clouds", "Mây rải rác" },
    { "broken clouds", "Mây rải rác" },
    { "shower rain", "Mưa rào" },
    { "rain", "Mưa" },
    { "overcast clouds", "Mây che phủ" },
    { "thunderstorm", "Giông bão" },
    { "snow", "Tuyết" },
    { "mist", "Sương mù" },
    { "drizzle", "Mưa phùn" },
    { "light rain", "Mưa nhẹ" },
    { "moderate rain", "Mưa vừa" },
    { "heavy intensity rain", "Mưa to" },
    { "very heavy rain", "Mưa rất to" },
    { "extreme rain", "Mưa cực kỳ to" },
    { "freezing rain", "Mưa tuyết" },
    { "light intensity shower rain", "Mưa rào nhẹ" },
    { "heavy intensity shower rain", "Mưa rào nặng" },
    { "ragged shower rain", "Mưa rào không đều" },
    { "thunderstorm with light rain", "Giông bão kèm mưa nhẹ" },
    { "thunderstorm with rain", "Giông bão kèm mưa" },
    { "thunderstorm with heavy rain", "Giông bão kèm mưa to" },
    { "light thunderstorm", "Giông bão nhẹ" },
    { "heavy thunderstorm", "Giông bão mạnh" },
    { "ragged thunderstorm", "Giông bão không đều" },
    { "thunderstorm with drizzle", "Giông bão kèm mưa phùn" },
    { "thunderstorm with light drizzle", "Giông bão kèm mưa phùn nhẹ" },
    { "light snow", "Tuyết nhẹ" },
    { "snow fall", "Rơi tuyết" },
    { "heavy snow", "Tuyết rơi nặng" },
    { "sleet", "Mưa tuyết" },
    { "shower sleet", "Mưa tuyết rào" },
    { "light shower sleet", "Mưa tuyết rào nhẹ" },
    { "heavy shower sleet", "Mưa tuyết rào nặng" },
    { "smoke", "Khói" },
    { "haze", "Sương mù nhẹ" },
    { "dust", "Bụi" },
    { "fog", "Sương mù dày đặc" },
    { "sand", "Cát" },
    { "ash", "Tro" },
    { "squalls", "Gió lốc" },
    { "tornado", "Lốc xoáy" }
};


        public WeatherController(WeatherService weatherService, DiseaseRiskService diseaseRiskService)
        {
            _weatherService = weatherService;
            _diseaseRiskService = diseaseRiskService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Forecast(string city)
        {
            if (string.IsNullOrEmpty(city))
            {
                ViewBag.Error = "Vui lòng nhập tên tỉnh/thành phố.";
                return View("Index");
            }

            // Lấy dữ liệu dự báo thời tiết
            var weatherForecast = await _weatherService.GetWeatherForecastAsync(city);
            if (weatherForecast == null)
            {
                ViewBag.Error = "Không thể lấy dữ liệu thời tiết. Vui lòng thử lại.";
                return View("Index");
            }

            // Dự đoán nguy cơ dịch bệnh
            var diseaseRiskPredictions = _diseaseRiskService.PredictDiseaseRisk(weatherForecast);

            // Lưu dữ liệu vào ViewBag
            ViewBag.City = weatherForecast.City.Name;
            ViewBag.Country = weatherForecast.City.Country;
            ViewBag.DiseaseRisks = diseaseRiskPredictions;

            // Dịch mô tả thời tiết sang tiếng Việt
            foreach (var forecast in weatherForecast.List)
            {
                if (_weatherDescriptionTranslations.ContainsKey(forecast.Weather[0].Description.ToLower()))
                {
                    forecast.Weather[0].Description = _weatherDescriptionTranslations[forecast.Weather[0].Description.ToLower()];
                }
            }

            return View("Forecast", weatherForecast);
        }
    }
}
