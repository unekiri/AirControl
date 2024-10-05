using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirControl.Subscribe
{
    public class SubscribedMessage
    {
        public string Type { get; set; }
        public string Temperature { get; set; }
        public string Airflow { get; set; }
        public void UpdateUI(string type, string temperature, string airflow)
        {
            this.Type = type;
            this.Temperature = temperature;
            this.Airflow = airflow;
        }
    }
}
