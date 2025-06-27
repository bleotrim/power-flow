#include "DigiCDC.h"

const byte START_BYTE = 0xAA;
const byte END_BYTE = 0x55;

const byte STATUS_OK = 0x01;
const byte STATUS_ERROR_CMD_INVALID = 0x02;
const byte STATUS_ERROR_CHECKSUM = 0x03;
const byte STATUS_ERROR_CONFLICT = 0x04;

const byte COMMAND_CONNECT_REQUEST = 0x0A;
const byte COMMAND_PB0_ON = 0x11;
const byte COMMAND_PB2_ON = 0x12;
const byte COMMAND_PB0_PB2_OFF = 0x13;

const int PIN_PB0 = 0;
const int PIN_PB2 = 2;

bool deviceReady = false;

unsigned long lastByteReceivedTime = 0;
const unsigned long PACKET_TIMEOUT_MS = 100;

byte calculateChecksum(byte dataByte) {
  return dataByte ^ 0xFF;
}

void sendFeedback(byte statusByte) {
  byte feedbackMessage[4];
  feedbackMessage[0] = START_BYTE;
  feedbackMessage[1] = statusByte;
  feedbackMessage[2] = calculateChecksum(statusByte);
  feedbackMessage[3] = END_BYTE;
  SerialUSB.write(feedbackMessage, 4);
  delay(500);
}

void flushSerialBuffer() {
  while(SerialUSB.available()) {
    SerialUSB.read();
  }
}

void setup() {

  pinMode(PIN_PB0, OUTPUT);
  pinMode(PIN_PB2, OUTPUT);

  digitalWrite(PIN_PB0, LOW);
  digitalWrite(PIN_PB2, LOW);

  SerialUSB.begin();
}

void loop() {
  SerialUSB.poll();

  if (!SerialUSB.isConnected()) {
    if (deviceReady) {
      deviceReady = false;
      digitalWrite(PIN_PB0, LOW);
      digitalWrite(PIN_PB2, LOW);
    }
  }

  if (SerialUSB.available() > 0 && (millis() - lastByteReceivedTime > PACKET_TIMEOUT_MS)) {
    flushSerialBuffer();
  }

  if (SerialUSB.available() > 0) {
      lastByteReceivedTime = millis();
  }

  if (SerialUSB.available() >= 4) {
    byte receivedBytes[4];
    SerialUSB.readBytes(receivedBytes, 4);

    flushSerialBuffer();

    if (receivedBytes[0] == START_BYTE && receivedBytes[3] == END_BYTE) {
      byte commandByte = receivedBytes[1];
      byte receivedChecksum = receivedBytes[2];
      byte calculatedChecksum = calculateChecksum(commandByte);

      if (receivedChecksum == calculatedChecksum) {
        if (!deviceReady) {
          if (commandByte == COMMAND_CONNECT_REQUEST) {
            sendFeedback(STATUS_OK);
            deviceReady = true;
          }
        } else {
          switch (commandByte) {
            case COMMAND_PB0_ON:
              if (digitalRead(PIN_PB2) == HIGH) {
                sendFeedback(STATUS_ERROR_CONFLICT);
              } else {
                digitalWrite(PIN_PB0, HIGH);
                sendFeedback(STATUS_OK);
              }
              break;

            case COMMAND_PB2_ON:
              if (digitalRead(PIN_PB0) == HIGH) {
                sendFeedback(STATUS_ERROR_CONFLICT);
              } else {
                digitalWrite(PIN_PB2, HIGH);
                sendFeedback(STATUS_OK);
              }
              break;

            case COMMAND_PB0_PB2_OFF:
              digitalWrite(PIN_PB0, LOW);
              digitalWrite(PIN_PB2, LOW);
              sendFeedback(STATUS_OK);
              break;

            case COMMAND_CONNECT_REQUEST:
              sendFeedback(STATUS_OK);
              break;

            default:
              sendFeedback(STATUS_ERROR_CMD_INVALID);
              break;
          }
        }
      } else {
      }
    } else {
    }
  }
}