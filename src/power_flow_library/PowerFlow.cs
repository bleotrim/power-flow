using System;
using System.Threading.Tasks;
using SerialCommLib;

public class Status
{
    public bool Device_1 { get; set; }
    public bool Device_2 { get; set; }
}

class PowerFlow
{
    private static async Task<bool> TryLogin(SerialCommunicator serial)
    {
        try
        {
            string command = "login:TEST";
            string response = await serial.SendCommandAsync(command, TimeSpan.FromSeconds(3));
            return response == "auth:ok";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Errore] {ex.Message}");
            return false;
        }
    }

    private static async Task<Status?> GetStatus(SerialCommunicator serial)
    {
        try
        {
            string response = await serial.SendCommandAsync("status", TimeSpan.FromSeconds(3));

            if (response == "switch:1:on, switch:2:off")
                return new Status { Device_1 = true, Device_2 = false };
            else if (response == "switch:1:off, switch:2:on")
                return new Status { Device_1 = false, Device_2 = true };
            else if (response == "switch:1:off, switch:2:off")
                return new Status { Device_1 = false, Device_2 = false };
            else if (response == "switch:1:on, switch:2:on")
                return new Status { Device_1 = true, Device_2 = true };
            else
            {
                Console.WriteLine($"[↯] Risposta non riconosciuta: {response}");
                return null;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Errore] {ex.Message}");
            return null;
        }
    }

    public static async Task<bool> TurnOnDevice(int deviceNumber)
    {
        using var serial = new SerialCommunicator("/dev/tty.usbmodem101");

        try
        {
            serial.Open();

            if (!await TryLogin(serial))
            {
                Console.WriteLine("[Errore] Autenticazione fallita.");
                return false;
            }

            string command = $"switch:{deviceNumber}:on";
            string response = await serial.SendCommandAsync(command, TimeSpan.FromSeconds(3));

            if (response == "cmd:ok")
                Console.WriteLine($"[↯] Dispositivo {deviceNumber} acceso con successo.");
            else
                Console.WriteLine($"[↯] Errore nell'accensione del dispositivo {deviceNumber}: {response}");

            return response == "cmd:ok";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Errore] {ex.Message}");
            return false;
        }
    }

    public static async Task<bool> TurnOffAllDevices()
    {
        using var serial = new SerialCommunicator("/dev/tty.usbmodem101");

        try
        {
            serial.Open();

            if (!await TryLogin(serial))
            {
                Console.WriteLine("[Errore] Autenticazione fallita.");
                return false;
            }

            string command = $"switch:a:off";
            string response = await serial.SendCommandAsync(command, TimeSpan.FromSeconds(3));

            if (response == "cmd:ok")
                Console.WriteLine("[↯] Tutti i dispositivi sono stati spenti con successo.");
            else
                Console.WriteLine($"[↯] Errore nello spegnimento: {response}");

            return response == "cmd:ok";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Errore] {ex.Message}");
            return false;
        }
    }
}
