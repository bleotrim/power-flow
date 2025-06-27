compile:
arduino-cli compile --fqbn digistump:avr:digispark-tiny <sketch_dir_path> --output-dir ./build

upload:
micronucleus <sketch_dir_path>/build/test_sketch.ino.hex

check serial port in linix:
ls /dev/ttyUSB* /dev/ttyACM* /dev/ttyS* 2>/dev/null