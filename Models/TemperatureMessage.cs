namespace MqttTemperatureMonitor.Models
{
    public class TemperatureMessage
    {
        public string? DeviceId { get; set; }
        public double Temperature { get; set; }
        public string? Unit { get; set; }
        public string? Timestamp { get; set; }
    }
}
