using CommandLine;

[Verb("turn-on", HelpText = "Tuns on a specific device.")]
public class TurnOnOptions
{
    [Option("device", Required = true, HelpText = "Device number to turn on.")]
    public int DeviceNumber { get; set; }
}

[Verb("turn-off", HelpText = "Turns off all devices.")]
public class TurnOffOptions
{
}

[Verb("custom-command", HelpText = "Sends a custom command to the device.")]
public class CustomCommandOptions
{
    [Option("command", Required = true, HelpText = "Command to send to the device.")]
    public string Command { get; set; } = string.Empty;
}
