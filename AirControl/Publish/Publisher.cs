using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirControl.Publish
{
    public class Publisher
    {
        public IMqttClient Client { get; private set; }

        public Publisher()
        {
            var factory = new MqttFactory();
            this.Client = factory.CreateMqttClient();
        }

        public async Task Connect(string brokerAddress, int port)
        {
            var options = new MqttClientOptionsBuilder()
                .WithClientId(Guid.NewGuid().ToString())
                .WithTcpServer(brokerAddress, port)
                .Build();
            var retry = 0;

            while (!this.Client.IsConnected && retry < 5)
            {
                try
                {
                    Console.WriteLine($"Attempting to connect to broker... (Attempt {retry + 1})");
                    await this.Client.ConnectAsync(options, CancellationToken.None);
                    Console.WriteLine("Connected to MQTT broker.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Connection failed: {ex.Message}. Retrying in 1 second...");
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
                retry++;
            }

            if (!this.Client.IsConnected)
            {
                Console.WriteLine("Failed to connect to the MQTT broker after 5 attempts.");
            }
        }

        public async Task Publish(string topic)
        {
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload("Hello World!")
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtMostOnce)
                .WithRetainFlag(true)
                .Build();

            await this.Client.PublishAsync(message, CancellationToken.None);
        }
    }
}
