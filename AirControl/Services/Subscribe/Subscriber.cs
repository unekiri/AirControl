using Microsoft.Extensions.Configuration;
using MQTTnet;
using MQTTnet.Client;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Text.Json;
using AirControl.Helpers;

namespace AirControl.Services.Subscribe
{
    // メッセージ購読クラス
    public class Subscriber
    {
        private readonly IMqttClient _client;
        private readonly IConnectionHelper _connectionHelper;
        private readonly SetSubscribedValue _setSubscribedValue;
        public TaskCompletionSource<bool> MessageReceivedCompletionSource { get; private set; }

        public Subscriber(IConnectionHelper connectionHelper, SetSubscribedValue setSubscribedValue)
        {
            var factory = new MqttFactory();
            _client = factory.CreateMqttClient();
            _connectionHelper = connectionHelper;
            _setSubscribedValue = setSubscribedValue;
        }

        // MQTTブローカーに接続する
        public async Task ConnectToBroker(string brokerAddress, int port)
        {
            var options = new MqttClientOptionsBuilder()
                .WithClientId(Guid.NewGuid().ToString())
                .WithTcpServer(brokerAddress, port)
                .Build();
            var retry = 0;

            // メッセージ受信ハンドラ
            _client.ApplicationMessageReceivedAsync += e =>
            {
                var message = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);
                var data = JsonSerializer.Deserialize<SetSubscribedValue>(message);

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    _setSubscribedValue.UpdateView(data.Type, $"{data.Temperature}", data.Airflow);
                });

                // メッセージを受信したので TaskCompletionSource を完了させる
                MessageReceivedCompletionSource?.TrySetResult(true);
                return Task.CompletedTask;
            };

            await _connectionHelper.ConnectWithRetryAsync(_client, options);
        }

        // MQTTブローカーのトピックから購読する
        public async Task SubscribeAndWaitForMessage(string topic)
        {
            try
            {
                // トピックにサブスクライブ
                await _client.SubscribeAsync(new MqttTopicFilterBuilder()
                    .WithTopic(topic)
                    .Build());

                // メッセージ受信を待つ準備をする
                MessageReceivedCompletionSource = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

                // メッセージが来るまで待機
                await MessageReceivedCompletionSource.Task;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"メッセージを受信できませんでした。 {topic}: {e.Message}");
            }
        }
    }
}