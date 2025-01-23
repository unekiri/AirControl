using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirControl.Publish
{
    // メッセージ発行クラス
    class Publisher
    {
        private readonly IMqttClient _Client;

        public Publisher()
        {
            var factory = new MqttFactory();
            this._Client = factory.CreateMqttClient();
        }

        // MQTTブローカーに接続する
        public async Task ConnectToBroker(string brokerAddress, int port)
        {
            var options = new MqttClientOptionsBuilder()
                .WithClientId(Guid.NewGuid().ToString())
                .WithTcpServer(brokerAddress, port)
                .Build();
            var retry = 0;

            while (!this._Client.IsConnected && retry < 5)
            {
                try
                {
                    Debug.WriteLine($"Attempting to connect to broker... (Attempt {retry + 1})");
                    await this._Client.ConnectAsync(options, CancellationToken.None);
                    Debug.WriteLine("Connected to MQTT broker.");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Connection failed: {ex.Message}. Retrying in 1 second...");
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
                retry++;
            }

            if (!this._Client.IsConnected)
            {
                Debug.WriteLine("Failed to connect to the MQTT broker after 5 attempts.");
            }
        }

        // MQTTブローカーのトピックに発行する
        public async Task ExePublish(string topic, string payload)
        {
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(payload)
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtMostOnce)
                .WithRetainFlag(true)
                .Build();

            await this._Client.PublishAsync(message, CancellationToken.None);
        }
    }
}
