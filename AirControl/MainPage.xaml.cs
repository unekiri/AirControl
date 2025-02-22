using AirControl.Helpers;
using AirControl.Services;
using AirControl.Services.Subscribe;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace AirControl
{
    public partial class MainPage : ContentPage
    {
        private readonly IConfiguration _configuration;
        private readonly IConnectionHelper _connectionHelper;
        public MainPage(IConfiguration configuration, IConnectionHelper connectionHelper, SetSubscribedValue setSubscribedValue)
        {
            InitializeComponent();
            _configuration = configuration;
            _connectionHelper = connectionHelper;
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

                await DisplayAlert("Success", "設定を切替ました。", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "設定の切替ができませんでした。: " + ex.Message, "OK");

                Console.WriteLine(ex.ToString());
            }
        }

        private async Task ConnectToBrokerAndExePublish(dynamic jsonPayload)
        {
            var address = _configuration["MqttSettings:Address"];
            var port = int.Parse(_configuration["MqttSettings:Port"]);
            var topic = _configuration["MqttSettings:Topic"];

            var publisher = new Publisher(_connectionHelper);
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
                await DisplayAlert("Error", "正常にシャットダウンできませんでした。: " + ex.Message, "OK");

                Console.WriteLine(ex.ToString());
            }
        }
    }
}
