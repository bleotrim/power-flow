# Arduino CLI – Installing, Compiling, and Uploading Sketches to Pro Micro

## 1. Installing the Arduino Core

To install the required core for **Arduino AVR** boards, run:

```bash
arduino-cli core update-index
arduino-cli core install arduino:avr
```

### Verifying the installation

```bash
arduino-cli core list
```

Expected output:

```
ID             Installed Latest Name
arduino:avr    1.8.6     1.8.6  Arduino AVR Boards
digistump:avr  1.7.5     1.7.5  Digistump AVR Boards
```

## 2. Connecting the **Pro Micro** Board

Connect the board to the computer via USB.

### 🔎 Checking connected devices

```bash
arduino-cli board list
```

Example output:

```
Port         Protocol Type              Board Name        FQBN                    Core
/dev/ttyACM0 serial   Serial Port (USB) Arduino Leonardo  arduino:avr:leonardo   arduino:avr
```

**Note**: The `FQBN` (Fully Qualified Board Name) is required for the following steps.

## 3. Compiling the Sketch

### Creating a working directory

```bash
mkdir sketch
cd sketch
```

### Creating the `sketch.ino` file

```bash
nano sketch.ino
```

Insert the following example code:

```cpp
void setup() {
    pinMode(4, OUTPUT);
}

void loop() {
    digitalWrite(4, HIGH);
    delay(1000);
    digitalWrite(4, LOW);
    delay(1000);
}
```

### Compiling the sketch

```bash
arduino-cli compile --fqbn arduino:avr:leonardo <sketch_dir_path> --output-dir <sketch_dir_path>
```

This command will create a `build/` folder containing the `.hex` file.

## 4. Uploading the Sketch

### Verifying the serial port

```bash
arduino-cli board list
```

Identify the serial port (e.g., `/dev/ttyACM0`).

### Uploading to the board

```bash
arduino-cli upload -p <serial_port> --fqbn arduino:avr:leonardo <root_sketch_folder_path>
```

## 5. Serial Monitor (optional)

**Serial Monitor** can be opened with:

```bash
arduino-cli monitor -p <serial_port> -c baudrate=<baud_rate_value>
```
