using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        var powerFlow = new PowerFlow();

        Console.WriteLine("1: Turn on device 1");
        Console.WriteLine("2: turn on device 2");
        Console.WriteLine("3: turn off all devices");
        Console.WriteLine("4: custom command");
        Console.Write("Select an option: ");
        
        string input = Console.ReadLine();

        switch (input)
        {
            case "1":
                await powerFlow.TurnOnDevice(1);
                break;
            case "2":
                await powerFlow.TurnOnDevice(2);
                break;
            case "3":
                await powerFlow.TurnOffAllDevices();
                break;
            case "4":
                Console.Write("Prompt custom command: ");
                string customCommand = Console.ReadLine();
                await powerFlow.SendCustomCommand(customCommand);
                break;
            default:
                Console.WriteLine("Not a valid option.");
                break;
        }
    }
}
