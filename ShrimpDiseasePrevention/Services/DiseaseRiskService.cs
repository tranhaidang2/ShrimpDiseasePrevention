using System;
using System.Collections.Generic;
using System.Linq;

namespace ShrimpDiseasePrevention.Services
{
    public class DiseaseRiskService
    {
        public List<string> PredictDiseaseRisk(WeatherForecast weatherForecast)
        {
            var riskPredictions = new List<string>();

            foreach (var forecast in weatherForecast.List)
            {
                double temperature = forecast.Main.Temp;
                double pop = forecast.Pop;
                string weatherCondition = forecast.Weather.FirstOrDefault()?.Main.ToLower();
                string description = forecast.Weather.FirstOrDefault()?.Description.ToLower();
                string dateTime = UnixTimeStampToDateTime(forecast.Dt).ToString("dd-MM-yyyy HH:mm");
                int cloudCoverage = forecast.Clouds?.All ?? 0;
                double windSpeed = forecast.Wind?.Speed ?? 0;
                int visibility = forecast.Visibility;

                string riskMessage;

                if (temperature > 28 && description.Contains("rain") && pop > 0.5)
                {
                    riskMessage = $"{dateTime}: Cảnh báo! Điều kiện thuận lợi cho dịch bệnh vi khuẩn và virus (nhiệt độ > 28°C, trời mưa, xác suất mưa > 50%).";
                }
                else if (temperature > 30)
                {
                    riskMessage = $"{dateTime}: Cảnh báo! Nhiệt độ cao (> 30°C) có thể gây stress nhiệt hoặc giảm oxy hòa tan.";
                }
                else if (temperature < 20)
                {
                    riskMessage = $"{dateTime}: Cảnh báo! Nhiệt độ thấp (< 20°C) có thể làm suy giảm hệ miễn dịch của tôm.";
                }
                else if (windSpeed > 10)
                {
                    riskMessage = $"{dateTime}: Cảnh báo! Gió mạnh (> 10 m/s) có thể gây xáo trộn môi trường nước.";
                }
                else if (visibility < 1000)
                {
                    riskMessage = $"{dateTime}: Cảnh báo! Tầm nhìn thấp (< 1km) có thể ảnh hưởng đến hoạt động nuôi tôm.";
                }
                else
                {
                    riskMessage = $"{dateTime}: Điều kiện thời tiết bình thường, nguy cơ dịch bệnh thấp.";
                }

                riskPredictions.Add(riskMessage);
            }

            return riskPredictions;
        }

        private DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(unixTimeStamp);
            return dateTimeOffset.DateTime;
        }
    }
}
