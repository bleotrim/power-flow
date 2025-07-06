Install core:
- arduino-cli core update-index
- arduino-cli core install arduino:avr

Check installation:
- arcuino-cli core list

This has to return something like:

ID            Installed Latest Name
arduino:avr   1.8.6     1.8.6  Arduino AVR Boards
digistump:avr 1.7.5     1.7.5  Digistump AVR Boards

Connect Pro Micro:
Check the connected device:
- arduino-cli board list

This has to return something like:

Port         Protocol Type              Board Name       FQBN                 Core
/dev/ttyACM0 serial   Serial Port (USB) Arduino Leonardo arduino:avr:leonardo arduino:avr

The FQBN (Fully Qualified Baord Name) is one of the parameters needed next

---------------------------------------------------------------------------------------

Compile a sketch:

- create folder with the sketch name:
ie. mkdir sketch

- enter the folder:
cd sketch

- create a sketch:
nano sketch.ino

sketch example:

void() {
    pinMode(4, OUTPUT);
}

loop() {
    digitalWrite(4, HIGH);
    delay(1000);
    digitalWrite(4, LOW);
    delay(1000);
}

- compile a sketch:
arduino-cli compile --fqbn arduino:avr:leonardo <sketch_dir_path> --output-dir <sketch_dir_path>

This creates a folder build with the hex file.

-------------------------------------------------------------------------------------------

Upload a sketch:

Check the connected device and take the searil port name:
- arduino-cli board list

Upload the sketch:
- arduino-cli upload -p <serial_port> --fqbn arduino:avr:leonardo <root_sketch_folder_path>

--------------------------------------------------------------------------------------------

To use serial monitor if the sketch is using serial communication use:
- arduino-cli monitor -p <serial_port> -c baudrate=<baud_rate_value>