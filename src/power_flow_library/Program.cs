using System;
using System.IO.Ports;
using System.Text;
using System.Threading;

class Program
{
    private static SerialPort serialPort;
    private static bool handshakeSuccess = false;

    static void Main()
    {
        string portName = "/dev/ttyACM1"; // WARNING: to be changed with correct port name
        serialPort = new SerialPort(portName, 9600);
        serialPort.DataReceived += SerialDataReceived;
        serialPort.Open();
        Console.WriteLine($"Porta {portName} aperta");

        SendCommand(0x0A);
        Console.WriteLine("Handshake inviato...");

        int timeout = 3000;
        int waited = 0;
        while (!handshakeSuccess && waited < timeout)
        {
            Thread.Sleep(100);
            waited += 100;
        }

        if (!handshakeSuccess)
        {
            Console.WriteLine("Handshake error");
            serialPort.Close();
            return;
        }

        Console.WriteLine("Handshake ok. Press 0 (PB0), 1 (PB2), O (shut down both), Q (exit)");

        while (true)
        {
            Console.Write("> ");
            string input = Console.ReadLine().Trim().ToUpper();
            if (input == "Q") break;

            switch (input)
            {
                case "0": SendCommand(0x11); break; // PB0 ON
                case "1": SendCommand(0x12); break; // PB2 ON
                case "O": SendCommand(0x13); break; // OFF
                default:
                    Console.WriteLine("Not valid command. Use 0, 1, O, or Q to exit.");
                    break;
            }
        }

        serialPort.Close();
        Console.WriteLine("Connection closed.");
    }

    private static void SendCommand(byte command)
    {
        byte[] packet = new byte[4];
        packet[0] = 0xAA;
        packet[1] = command;
        packet[2] = (byte)(command ^ 0xFF);
        packet[3] = 0x55;

        serialPort.Write(packet, 0, 4);
        Console.WriteLine($"Send comaand: 0x{command:X2}");
    }

    private static void SerialDataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        try
        {
            while (serialPort.BytesToRead >= 4)
            {
                byte[] buffer = new byte[4];
                serialPort.Read(buffer, 0, 4);

                // Valida pacchetto
                if (buffer[0] != 0xAA || buffer[3] != 0x55)
                {
                    Console.WriteLine("Not recieved correct packet");
                    return;
                }

                byte status = buffer[1];
                byte checksum = buffer[2];

                if ((byte)(status ^ 0xFF) != checksum)
                {
                    Console.WriteLine("Wrong checksum");
                    return;
                }

                if (!handshakeSuccess && status == 0x01)
                {
                    handshakeSuccess = true;
                }

                // Interpreta risposta
                string msg = status switch
                {
                    0x01 => "OK",
                    0x02 => "Not valid command",
                    0x03 => "Wrong Checksum",
                    0x04 => "Conflict (PB0/PB2 one of them already on)",
                    _    => $"Unknown code: 0x{status:X2}"
                };

                Console.WriteLine($"-> Answer: {msg}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Recieved error: {ex.Message}");
        }
    }
}
