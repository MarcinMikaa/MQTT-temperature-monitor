using MQTTnet;
using System.Text;

namespace MqttTemperatureMonitor.Services
{
    public class MqttServerService
    {
        private readonly IMqttClient _mqttClient;

        public MqttServerService()
        {
            var factory = new MqttClientFactory();
            _mqttClient = factory.CreateMqttClient();
        }

        public async Task StartAsync()
        {
            var options = new MqttClientOptionsBuilder()
                .WithTcpServer("broker.hivemq.com", 1883)
                .WithClientId("Server-" + Guid.NewGuid().ToString())
                .Build();

            await _mqttClient.ConnectAsync(options, CancellationToken.None);

            var subscribeOptions = new MqttClientSubscribeOptionsBuilder()
                .WithTopicFilter("system/temperature")
                .Build();
            await _mqttClient.SubscribeAsync(subscribeOptions, CancellationToken.None);

            _mqttClient.ApplicationMessageReceivedAsync += e =>
            {
                if (e.ApplicationMessage.Topic == "system/temperature")
                {
                    var json = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                    Console.WriteLine($"Received temperature data: {json}");
                }
                return Task.CompletedTask;
            };

            while (true)
            {
                Console.WriteLine("Press Enter to get temperature data...");
                Console.ReadLine();
                var mqttmessage = new MqttApplicationMessageBuilder()
                    .WithTopic("system/request")
                    .WithPayload(Encoding.UTF8.GetBytes("Requesting temperature data"))
                    .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
                    .Build();
                await _mqttClient.PublishAsync(mqttmessage, CancellationToken.None);
                Console.WriteLine("Request sent.");
            }
        }

        public async Task StopAsync()
        {
            var disconnectOptions = new MqttClientDisconnectOptionsBuilder()
                .Build();
            await _mqttClient.DisconnectAsync(disconnectOptions);
        }
    }
}
