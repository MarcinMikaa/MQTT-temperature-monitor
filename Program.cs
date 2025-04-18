using MqttTemperatureMonitor.Services;
using System.Threading.Tasks;

namespace MqttTemperatureMonitor
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Chosese the role:");
            Console.WriteLine("1. Client (get temperature)");
            Console.WriteLine("2. Server (send temperature)");
            var choice = Console.ReadLine();

            if (choice == "1")
            {
                var clientService = new MqttClientService();
                try
                {
                    await clientService.StartAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Client Error: {ex.Message}");
                }
            }
            else if (choice == "2")
            {
                var serverService = new MqttServerService();
                try
                {
                    await serverService.StartAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Server Error: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Invalid choice. Please select 1 or 2.");
            }
        }
    }
}
