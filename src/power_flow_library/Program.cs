using System;
using System.Threading.Tasks;
using SerialCommLib;

class Program
{
    static async Task Main(string[] args)
    {
        bool result = PowerFlow.TurnOnDevice(1).GetAwaiter().GetResult();

        if (result)
            Console.WriteLine("Dispositivo acceso.");
        else
            Console.WriteLine("Errore durante l'accensione.");

        return;

        // Uncomment the following lines to run the interactive console
        using var serial = new SerialCommunicator("/dev/tty.usbmodem101");

        try
        {
            serial.Open();
            Console.WriteLine("[↯] Porta seriale aperta.");

            while (true)
            {
                Console.Write("> ");
                string input = Console.ReadLine()?.Trim() ?? "";

                if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
                    break;

                if (!string.IsNullOrEmpty(input))
                {
                    string response = await serial.SendCommandAsync(input, TimeSpan.FromSeconds(3));
                    Console.WriteLine($"[⇄] Risposta: {response}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Errore] {ex.Message}");
        }
    }
}
