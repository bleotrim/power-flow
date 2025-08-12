using System;
using System.Threading.Tasks;
using SerialCommLib;

public class PowerFlow
{
    public string PortName { get; set; } = "/dev/ttyACM0";
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(5);
    private async Task<bool> TryLogin(SerialCommunicator serial)
    {
        try
        {
            string command = "login:TEST";
            string response = await serial.SendCommandAsync(command, Timeout);

            if (response == "auth:ok")
                Console.WriteLine("Authentication successful.");
            else
                Console.WriteLine($"Authentication failed: {response}");

            return response == "auth:ok";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Errore] {ex.Message}");
            return false;
        }
    }

    public async Task<bool> TurnOnDevice(int deviceNumber)
    {
        using var serial = new SerialCommunicator(PortName);

        try
        {
            serial.Open();

            if (!await TryLogin(serial)) return false;

            string command = $"switch:{deviceNumber}:on";
            string response = await serial.SendCommandAsync(command, Timeout);

            if (response == "cmd:ok")
                Console.WriteLine($"Device {deviceNumber} turned on.");
            else
                Console.WriteLine($"Error: {response}");

            return response == "cmd:ok";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> TurnOffAllDevices()
    {
        using var serial = new SerialCommunicator(PortName);

        try
        {
            serial.Open();

            if (!await TryLogin(serial)) return false;

            string command = $"switch:a:off";
            string response = await serial.SendCommandAsync(command, Timeout);

            if (response == "cmd:ok")
                Console.WriteLine("All devices turned off.");
            else
                Console.WriteLine($"Error: {response}");

            return response == "cmd:ok";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> SendCustomCommand(string command)
    {
        using var serial = new SerialCommunicator(PortName);

        try
        {
            serial.Open();

            if (!await TryLogin(serial)) return false;

            string response = await serial.SendCommandAsync(command, Timeout);
            Console.WriteLine($"Custom command response: {response}");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return false;
        }
    }
}
