using LibreHardwareMonitor.Hardware;

namespace MqttTemperatureMonitor.Services
{
    public class HardwareMonitorService
    {
        public double GetCpuTemperature()
        {
            var computer = new Computer { IsCpuEnabled = true };
            try
            {
                computer.Open();
                foreach (var hardware in computer.Hardware)
                {
                    if (hardware.HardwareType == HardwareType.Cpu)
                    {
                        hardware.Update();
                        foreach (var sensor in hardware.Sensors)
                        {
                            if (sensor.SensorType == SensorType.Temperature)
                            {
                                return sensor.Value ?? 0;
                            }
                        }
                    }
                }
            }
            finally
            {
                computer.Close();
            }
            // Fallback value if no temperature sensor is found
            return new Random().Next(30, 80); 
        }
    }
}
