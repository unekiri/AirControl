using AirControl.Publish;
using AirControl.Subscribe;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace AirControl
{
    public partial class MainPage : ContentPage
    {
        private readonly IConfiguration _configuration;
        public MainPage(IConfiguration configuration, SetSubscribedValue setSubscribedValue)
        {
            InitializeComponent();
            this._configuration = configuration;
            this.BindingContext = setSubscribedValue;
        }

        // 選択した値をブローカーに発行する
        private async void PushSelectedValue(object sender, EventArgs e)
        {
            try
            {
                // 選択した値を取得
                var selectedType = typePicker.SelectedItem?.ToString();
                var selectedTemperature = temperaturePicker.SelectedItem?.ToString();
                var selectedAirflow = airflowPicker.SelectedItem?.ToString();

                // Json形式でpayloadを作成
                var payload = new
                {
                    Type = selectedType,
                    Temperature = selectedTemperature,
                    Airflow = selectedAirflow
                };

                var jsonPayload = JsonSerializer.Serialize(payload);
                await ConnectToBrokerAndExePublish(jsonPayload);

                await DisplayAlert("Success", "Completed the configuration", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Failed to connect or published a message to MQTT Broker while setting: " + ex.Message, "OK");

                Console.WriteLine(ex.ToString());
            }
        }

        private async Task ConnectToBrokerAndExePublish(dynamic jsonPayload)
        {
            var address = this._configuration["MqttSettings:Address"];
            var port = int.Parse(this._configuration["MqttSettings:Port"]);
            var topic = this._configuration["MqttSettings:Topic"];

            var publisher = new Publisher();
            // MQTTブローカーに接続
            await publisher.ConnectToBroker(address, port);
            // MQTTブローカーにメッセージを送信
            await publisher.ExePublish(topic, jsonPayload);
        }

        // 電源を切る
        private async void TurnOff(object sender, EventArgs e)
        {
            try
            {
                var jsonPayload = JsonSerializer.Serialize(new { });
                await ConnectToBrokerAndExePublish(jsonPayload);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Failed to connect or published a message to MQTT Broker while turning off: " + ex.Message, "OK");

                Console.WriteLine(ex.ToString());
            }
        }
    }
}
