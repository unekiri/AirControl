using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirControl.Subscribe
{
    public class AtFirstExeSubscribe
    {
        public readonly IConfiguration _configuration;
        private readonly SetSubscribedValue _setSubscribedValue;

        public AtFirstExeSubscribe(IConfiguration configuration, SetSubscribedValue setSubscribedValue)
        {
            this._configuration = configuration;
            this._setSubscribedValue = setSubscribedValue;
        }

        public async Task Run()
        {
            var address = this._configuration["MqttSettings:Address"];
            var port = int.Parse(this._configuration["MqttSettings:Port"]);
            var topic = this._configuration["MqttSettings:Topic"];

            var subscriber = new Subscriber(this._setSubscribedValue);
            // MQTTブローカーに接続
            await subscriber.ConnectToBroker(address, port);

            // トピックの購読を行い、メッセージが受信されるまで待機
            await subscriber.SubscribeAndWaitForMessage(topic);
        }
    }
}
