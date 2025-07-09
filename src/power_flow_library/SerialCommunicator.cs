using System;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;

namespace SerialCommLib
{
    public class SerialCommunicator : IDisposable
    {
        private SerialPort? serialPort;
        private TaskCompletionSource<string>? currentResponseTcs;

        public bool IsOpen => serialPort?.IsOpen == true;

        public SerialCommunicator(string portName, int baudRate = 9600)
        {
            serialPort = new SerialPort(portName)
            {
                BaudRate = baudRate,
                Parity = Parity.None,
                StopBits = StopBits.One,
                DataBits = 8,
                Handshake = Handshake.None,
                RtsEnable = true,
                ReadTimeout = 2000,
                WriteTimeout = 1000
            };

            serialPort.DataReceived += DataReceivedHandler;
        }

        public void Open()
        {
            if (serialPort == null)
                throw new InvalidOperationException("SerialPort non inizializzato.");

            if (!serialPort.IsOpen)
            {
                serialPort.Open();
            }
        }

        public void Close()
        {
            if (serialPort?.IsOpen == true)
            {
                serialPort.Close();
            }
        }

        public async Task<string> SendCommandAsync(string command, TimeSpan timeout)
        {
            if (serialPort?.IsOpen != true)
                throw new InvalidOperationException("Porta seriale non disponibile.");

            currentResponseTcs = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);

            try
            {
                serialPort.WriteLine(command);

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

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                var sp = (SerialPort)sender;
                string inData = sp.ReadLine().Trim();
                currentResponseTcs?.TrySetResult(inData);
            }
            catch (Exception ex)
            {
                currentResponseTcs?.TrySetResult($"errore_ricezione: {ex.Message}");
            }
        }

        public void Dispose()
        {
            Close();
            serialPort?.Dispose();
        }
    }
}
