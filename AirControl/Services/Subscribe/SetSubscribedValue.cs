using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirControl.Services.Subscribe
{
    // 設定値を変更するクラス
    public class SetSubscribedValue : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public string? Type { get; set; }
        public string? Temperature { get; set; }
        public string? Airflow { get; set; }

        public void UpdateView(string type, string temperature, string airflow)
        {
            Type = type;
            Temperature = temperature;
            Airflow = airflow;

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
