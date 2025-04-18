using MQTTnet;
using MqttTemperatureMonitor.Models;
using System.Text;
using System.Text.Json;

namespace MqttTemperatureMonitor.Services
{
    public class MqttClientService
    {
        private readonly IMqttClient _mqttClient;
        private readonly HardwareMonitorService _hardwareMonitor;

        public MqttClientService()
        {
            var factory = new MqttClientFactory();
            _mqttClient = factory.CreateMqttClient();
            _hardwareMonitor = new HardwareMonitorService();
        }

        public async Task StartAsync()
        {
            var options = new MqttClientOptionsBuilder()
                .WithTcpServer("broker.hivemq.com", 1883)
                .WithClientId("Client-" + Guid.NewGuid().ToString())
                .Build();

            await _mqttClient.ConnectAsync(options, CancellationToken.None);

            var subscribeOptions = new MqttClientSubscribeOptionsBuilder()
                .WithTopicFilter("system/request")
                .Build();
            await _mqttClient.SubscribeAsync(subscribeOptions, CancellationToken.None);

            _mqttClient.ApplicationMessageReceivedAsync += async e =>
            {
                if (e.ApplicationMessage.Topic == "system/request")
                {
                    Console.WriteLine("Received request for system information.");
                    await PublishTemperatureAsync();
                }
            };

            while (true)
            {
                await PublishTemperatureAsync();
                await Task.Delay(10000);
            }
        }

        private async Task PublishTemperatureAsync()
        {
            var temperature = _hardwareMonitor.GetCpuTemperature();
            var message = new TemperatureMessage
            {
                DeviceId = "client1",
                Temperature = temperature,
                Unit = "Celsius",
                Timestamp = DateTime.UtcNow.ToString("o")
            };
            var json = JsonSerializer.Serialize(message);
            var mqttMesage = new MqttApplicationMessageBuilder()
                .WithTopic("system/temperature")
                .WithPayload(Encoding.UTF8.GetBytes(json))
                .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
                .Build();
            await _mqttClient.PublishAsync(mqttMesage, CancellationToken.None);
            Console.WriteLine($"Published temperature: {json}°C");
        }

        public async Task StopAsync()
        {
            var disconnectOptions = new MqttClientDisconnectOptionsBuilder()
                .Build();
            await _mqttClient.DisconnectAsync(disconnectOptions);
        }
    }
}
