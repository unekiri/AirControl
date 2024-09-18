using AirControl.Publish;
using Microsoft.Extensions.Configuration;

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
            InitializeComponent();
            this.Configuration = configuration;
        }

        private async void SendMessageToMqttBroker(object sender, EventArgs e)
        {
            try
            {
                var address = this.Configuration["MqttSettings:Address"];
                var port = int.Parse(this.Configuration["MqttSettings:Port"]);
                var topic = this.Configuration["MqttSettings:Topic"];

                var publisher = new Publisher();
                // MQTTブローカーに接続
                await publisher.Connect(address, port);
                // MQTTブローカーにメッセージを送信
                await publisher.Publish(topic);

                await DisplayAlert("Success", "Connected to MQTT Broker and Sent a message", "OK");

            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Failed to connect or published a message to MQTT Broker: " + ex.Message, "OK");

                Console.WriteLine(ex.ToString());
            }
        }
    }

}
