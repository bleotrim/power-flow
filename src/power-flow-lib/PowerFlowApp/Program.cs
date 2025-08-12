using CommandLine;

class Program
{
    static async Task<int> Main(string[] args)
    {
        var powerFlow = new PowerFlow();

        return await Parser.Default.ParseArguments<TurnOnOptions, TurnOffOptions, CustomCommandOptions>(args)
            .MapResult(
                async (TurnOnOptions opts) =>
                {
                    Console.WriteLine($"Turning on device {opts.DeviceNumber}...");
                    bool success = await powerFlow.TurnOnDevice(opts.DeviceNumber);
                    return success ? 0 : 1;
                },
                async (TurnOffOptions opts) =>
                {
                    Console.WriteLine("Turning off all devices...");
                    bool success = await powerFlow.TurnOffAllDevices();
                    return success ? 0 : 1;
                },
                async (CustomCommandOptions opts) =>
                {
                    Console.WriteLine($"Sending custom command: {opts.Command}");
                    bool success = await powerFlow.SendCustomCommand(opts.Command);
                    return success ? 0 : 1;
                },
                errs => Task.FromResult(1)
            );
    }
}
