using System;
using System.IO.Ports;
using System.Threading;

class Program
{
    private static bool authenticated = false;
    private static SerialPort? mySerialPort;
    static void Main(string[] args)
    {
        SetupSerialPort();
        Authenticate();

        /*while (true)
        {
            Thread.Sleep(1000);
            SendCommand("swtch:2:on");
            Thread.Sleep(1000);
            SendCommand("swtch:off");
            Thread.Sleep(1000);
        }*/
    }

    private static void SetupSerialPort()
    {
        string portName = "/dev/tty.usbmodem1101";
        mySerialPort = new SerialPort(portName);

        mySerialPort.BaudRate = 9600;
        mySerialPort.Parity = Parity.None;
        mySerialPort.StopBits = StopBits.One;
        mySerialPort.DataBits = 8;
        mySerialPort.Handshake = Handshake.None;
        mySerialPort.RtsEnable = true;
        mySerialPort.ReadTimeout = 2000;

        mySerialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
        mySerialPort.Open();
    }

    private static void SendCommand(string command)
    {
        if (mySerialPort.IsOpen)
        {
            mySerialPort.WriteLine(command);
            Console.WriteLine("Comando inviato: " + command);
        }
        else
        {
            Console.WriteLine("La porta seriale non è aperta.");
        }
    }

    private static void Authenticate()
    {
        SendCommand("login:4F7D9B2A1C8E5G0H");

        while (!authenticated)
        {
            Thread.Sleep(100);
        }

        if (authenticated)
        {
            Console.WriteLine("Autenticazione completata con successo.");
        }
        else
        {
            Console.WriteLine("Autenticazione fallita.");
        }
    }

    private static void DataReceivedHandler(
                        object sender,
                        SerialDataReceivedEventArgs e)
    {
        try
        {
            SerialPort sp = (SerialPort)sender;
            Thread.Sleep(100);
            string indata = sp.ReadLine();

            if (indata.Trim() == "auth:ok" && !authenticated)
            {
                authenticated = true;
            }
        }
        catch (TimeoutException)
        {
            Console.WriteLine("Timeout nella lettura seriale.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Errore nella lettura seriale: " + ex.Message);
        }
        finally
        {

        }
    }
}