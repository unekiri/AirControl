using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirControl.Subscribe
{
    public class SubscribedMessage : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public string Type { get; set; }
        public string Temperature { get; set; }
        public string Airflow { get; set; }
        public void UpdateUI(string type, string temperature, string airflow)
        {
            this.Type = type;
            this.Temperature = temperature;
            this.Airflow = airflow;
            OnPropertyChanged(nameof(Type));
            OnPropertyChanged(nameof(Temperature));
            OnPropertyChanged(nameof(Airflow));
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
