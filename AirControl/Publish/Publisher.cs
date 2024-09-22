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
                .WithClientId(brokerAddress)
                .WithTcpServer(brokerAddress, port)
                .Build();

            await this.Client.ConnectAsync(options, CancellationToken.None);
        }

        public async Task SentMessage(string topic)
        {
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload("Hello World!")
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtMostOnce)
                .Build();

            await this.Client.PublishAsync(message, CancellationToken.None);
        }
    }
}
