using AirControl.Helpers;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirControl.Services.Subscribe
{
    public class AtFirstExeSubscribe
    {
        public readonly IConfiguration _configuration;
        private readonly IConnectionHelper _connectionHelper;
        private readonly SetSubscribedValue _setSubscribedValue;

        public AtFirstExeSubscribe(IConfiguration configuration, IConnectionHelper connectionHelper, SetSubscribedValue setSubscribedValue)
        {
            _configuration = configuration;
            _connectionHelper = connectionHelper;
            _setSubscribedValue = setSubscribedValue;
        }

        public async Task Run()
        {
            var address = _configuration["MqttSettings:Address"];
            var port = int.Parse(_configuration["MqttSettings:Port"]);
            var topic = _configuration["MqttSettings:Topic"];

            var subscriber = new Subscriber(this._connectionHelper, this._setSubscribedValue);
            // MQTTブローカーに接続
            await subscriber.ConnectToBroker(address, port);

            // トピックの購読を行い、メッセージが受信されるまで待機
            await subscriber.SubscribeAndWaitForMessage(topic);
        }
    }
}
