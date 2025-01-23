using Microsoft.Extensions.Configuration;
using MQTTnet;
using MQTTnet.Client;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Text.Json;

namespace AirControl.Subscribe
{
    // メッセージ購読クラス
    public class Subscriber
    {
        private readonly IMqttClient _client;
        private readonly SetSubscribedValue _setSubscribedValue;
        public  TaskCompletionSource<bool> MessageReceivedCompletionSource { get; private set; }

        public Subscriber(SetSubscribedValue setSubscribedValue)
        {
            var factory = new MqttFactory();
            this._client = factory.CreateMqttClient();
            this._setSubscribedValue = setSubscribedValue;
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
            this._client.ApplicationMessageReceivedAsync += e =>
            {
                var message = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);
                var data = JsonSerializer.Deserialize<SetSubscribedValue>(message);

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    this._setSubscribedValue.UpdateView(data.Type, $"{data.Temperature}", data.Airflow);
                });

                // メッセージを受信したので TaskCompletionSource を完了させる
                MessageReceivedCompletionSource?.TrySetResult(true);
                return Task.CompletedTask;
            };

            while (!this._client.IsConnected && retry < 5)
            {
                try
                {
                    Debug.WriteLine($"Attempting to connect to broker... (Attempt {retry + 1})");
                    await this._client.ConnectAsync(options, CancellationToken.None);
                    Debug.WriteLine("Connected to MQTT broker.");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Connection failed: {ex.Message}. Retrying in 1 second...");
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
                retry++;
            }

            if (!this._client.IsConnected)
            {
                Debug.WriteLine("Failed to connect to the MQTT broker after 5 attempts.");
            }
        }

        // MQTTブローカーのトピックから購読する
        public async Task SubscribeAndWaitForMessage(string topic)
        {
            try
            {
                // トピックにサブスクライブ
                await this._client.SubscribeAsync(new MqttTopicFilterBuilder()
                    .WithTopic(topic)
                    .Build());

                // メッセージ受信を待つ準備をする
                MessageReceivedCompletionSource = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

                // メッセージが来るまで待機
                await MessageReceivedCompletionSource.Task;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error subscribing to topic {topic}: {e.Message}");
            }
        }
    }
}