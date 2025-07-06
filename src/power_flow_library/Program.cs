using System;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    private static SerialPort? mySerialPort;
    private static TaskCompletionSource<string>? currentResponseTcs;

    static async Task Main(string[] args)
    {
        try
        {
            SetupSerialPort();

            string loginResponse = await SendCommandAndWaitResponseAsync("login:4F7D9B2A1C8E5G0H", TimeSpan.FromSeconds(5));

            if (loginResponse != "auth:ok")
            {
                Console.WriteLine("[X] Autenticazione fallita: " + loginResponse);
                return;
            }

            Console.WriteLine("[✓] Autenticazione avvenuta con successo.");

            while (true)
            {
                Console.Write("> ");
                string input = Console.ReadLine()?.Trim() ?? "";

                if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
                    break;

                if (!string.IsNullOrEmpty(input))
                {
                    string response = await SendCommandAndWaitResponseAsync(input, TimeSpan.FromSeconds(3));
                    Console.WriteLine($"[⇄] Risposta: {response}");
                }
            }
        }
        finally
        {
            Cleanup();
        }
    }

    private static void SetupSerialPort()
    {
        string portName = "/dev/tty.usbmodem1101";

        mySerialPort = new SerialPort(portName)
        {
            BaudRate = 9600,
            Parity = Parity.None,
            StopBits = StopBits.One,
            DataBits = 8,
            Handshake = Handshake.None,
            RtsEnable = true,
            ReadTimeout = 2000,
            WriteTimeout = 1000
        };

        mySerialPort.DataReceived += DataReceivedHandler;

        if (!mySerialPort.IsOpen)
        {
            mySerialPort.Open();
            Console.WriteLine($"[↯] Porta seriale {portName} aperta.");
        }
    }

    private static async Task<string> SendCommandAndWaitResponseAsync(string command, TimeSpan timeout)
    {
        if (mySerialPort?.IsOpen != true)
            throw new InvalidOperationException("Porta seriale non disponibile.");

        currentResponseTcs = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);

        try
        {
            mySerialPort.WriteLine(command);
            Console.WriteLine($"[TX] → {command}");

            using var cts = new CancellationTokenSource(timeout);
            using (cts.Token.Register(() => currentResponseTcs.TrySetResult("timeout")))
            {
                string response = await currentResponseTcs.Task;
                return response;
            }
        }
        catch (Exception ex)
        {
            return $"errore: {ex.Message}";
        }
    }

    private static void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
    {
        try
        {
            var sp = (SerialPort)sender;
            string inData = sp.ReadLine().Trim();

            Console.WriteLine($"[RX] ← {inData}");

            currentResponseTcs?.TrySetResult(inData);
        }
        catch (Exception ex)
        {
            currentResponseTcs?.TrySetResult($"errore_ricezione: {ex.Message}");
        }
    }

    private static void Cleanup()
    {
        if (mySerialPort?.IsOpen == true)
        {
            mySerialPort.Close();
            Console.WriteLine("[↯] Porta seriale chiusa.");
        }
    }
}
