using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileProcessingFunctions
{
    public class WeatherData
    {
        public string Country { get; set; }
        public string City { get; set; }
        public string Date { get; set; }
        public int Temperature { get; set; }
        public int Humidity { get; set; }
        public int WindSpeed { get; set; }
        public string WindDirection { get; set; }
        public int Precipitation { get; set; }
        public int CloudCover { get; set; }
        public int Visibility { get; set; }
        public int Pressure { get; set; }
        public int DewPoint { get; set; }
        public int UVIndex { get; set; }
        public string Sunrise { get; set; }
        public string Sunset { get; set; }
        public string Moonrise { get; set; }
        public string Moonset { get; set; }
        public string MoonPhase { get; set; }
        public string Conditions { get; set; }
        public string Icon { get; set; }
    }
}
