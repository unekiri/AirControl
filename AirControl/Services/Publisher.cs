using AirControl.Helpers;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirControl.Services
{
    // メッセージ発行クラス
    class Publisher
    {
        private readonly IMqttClient _client;
        private readonly IConnectionHelper _connectionHelper;

        public Publisher(IConnectionHelper connectionHelper)
        {
            var factory = new MqttFactory();
            _connectionHelper = connectionHelper;
            _client = factory.CreateMqttClient();
        }

        // MQTTブローカーに接続する
        public async Task ConnectToBroker(string brokerAddress, int port)
        {
            var options = new MqttClientOptionsBuilder()
                .WithClientId(Guid.NewGuid().ToString())
                .WithTcpServer(brokerAddress, port)
                .Build();
            var retry = 0;

            await _connectionHelper.ConnectWithRetryAsync(_client, options);
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

            await _client.PublishAsync(message, CancellationToken.None);
        }
    }
}
