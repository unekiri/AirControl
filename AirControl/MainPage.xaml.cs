using AirControl.Publish;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace AirControl
{
    public partial class MainPage : ContentPage
    {
        public IConfiguration Configuration { get; private set; }

        public MainPage()
        {
            InitializeComponent();
        }

        public MainPage(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        private async Task ConnectAndPublish(dynamic jsonPayload)
        {
            var address = this.Configuration["MqttSettings:Address"];
            var port = int.Parse(this.Configuration["MqttSettings:Port"]);
            var topic = this.Configuration["MqttSettings:Topic"];

            var publisher = new Publisher();
            // MQTTブローカーに接続
            await publisher.ConnectToBroker(address, port);
            // MQTTブローカーにメッセージを送信
            await publisher.Publish(topic, jsonPayload);
        }
        
        private async void Setting(object sender, EventArgs e)
        {
            try
            {
                // 選択された値を取得
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
                await ConnectAndPublish(jsonPayload);
                
                await DisplayAlert("Success", "Turn on", "OK");

                // 既に起動している場合、以下のメッセージを出す
                // await DisplayAlert("Success", "Setting was Changed");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Failed to connect or published a message to MQTT Broker while setting: " + ex.Message, "OK");

                Console.WriteLine(ex.ToString());
            }
        }

        private async void TurnOff(object sender, EventArgs e)
        {
            try
            {
                var jsonPayload = JsonSerializer.Serialize(new { });
                await ConnectAndPublish(jsonPayload);

                await DisplayAlert("Success", "Turn off", "OK");

            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Failed to connect or published a message to MQTT Broker while turning off: " + ex.Message, "OK");

                Console.WriteLine(ex.ToString());
            }
        }
    }
}
