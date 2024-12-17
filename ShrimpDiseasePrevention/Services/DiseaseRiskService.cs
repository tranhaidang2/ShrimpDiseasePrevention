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
                string weatherCondition = forecast.Weather.FirstOrDefault()?.Main.ToLower();
                string description = forecast.Weather.FirstOrDefault()?.Description.ToLower();
                string dateTime = UnixTimeStampToDateTime(forecast.Dt).ToString("dd-MM-yyyy HH:mm");

                string riskMessage;

                // Điều kiện dịch bệnh vi khuẩn và virus (nhiệt độ cao, độ ẩm cao, mưa)
                if (temperature > 28 && description.Contains("rain"))
                {
                    riskMessage = $"{dateTime}: Cảnh báo! Điều kiện thuận lợi cho dịch bệnh vi khuẩn và virus (nhiệt độ > 28°C, trời mưa).";
                }
                // Điều kiện gây stress nhiệt và thiếu oxy
                else if (temperature > 30)
                {
                    riskMessage = $"{dateTime}: Cảnh báo! Nhiệt độ cao (> 30°C) có thể gây stress nhiệt hoặc giảm oxy hòa tan.";
                }
                // Điều kiện nhiệt độ thấp
                else if (temperature < 20)
                {
                    riskMessage = $"{dateTime}: Cảnh báo! Nhiệt độ thấp (< 20°C) có thể làm suy giảm hệ miễn dịch của tôm.";
                }
                else
                {
                    riskMessage = $"{dateTime}: Điều kiện thời tiết bình thường, nguy cơ dịch bệnh thấp.";
                }

                riskPredictions.Add(riskMessage);
            }

            return riskPredictions;
        }

        // Chuyển đổi Unix timestamp sang DateTime
        private DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(unixTimeStamp);
            return dateTimeOffset.DateTime;
        }
    }
}
