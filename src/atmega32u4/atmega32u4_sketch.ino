const int STATUS_LED = 17;
const int SWITCH_1 = 7;
const int SWITCH_2 = 8;

unsigned long previousMillis = 0;
int step = 0;

const String AUTH_TOKEN = "4F7D9B2A1C8E5G0H";
bool authenticated = false;

struct BlinkStep {
  bool ledOn;
  unsigned long duration;
};

const BlinkStep blinkPattern[] = {
  {true,  300},
  {false, 200},
  {true,  300},
  {false, 30000}
};

const int TOTAL_STEPS = sizeof(blinkPattern) / sizeof(blinkPattern[0]);

void setup() {
  pinMode(STATUS_LED, OUTPUT);
  pinMode(SWITCH_1, OUTPUT);
  pinMode(SWITCH_2, OUTPUT);
  digitalWrite(STATUS_LED, LOW);
  digitalWrite(SWITCH_1, LOW);
  digitalWrite(SWITCH_2, LOW);

  Serial.begin(9600);
}

void loop() {
  handleBlink();
  handleSerial();
}

void handleBlink() {
  unsigned long currentMillis = millis();
  if (currentMillis - previousMillis >= blinkPattern[step].duration) {
    previousMillis = currentMillis;
    digitalWrite(STATUS_LED, blinkPattern[step].ledOn ? HIGH : LOW);
    step = (step + 1) % TOTAL_STEPS;
  }
}

void handleSerial() {
  if (Serial.available()) {
    String command = Serial.readStringUntil('\n');
    command.trim();

    if (command.startsWith("login:")) {
      String token = command.substring(6);
      if (token == AUTH_TOKEN) {
        authenticated = true;
        Serial.println("auth:ok");
      }
      return;
    }

    if (!authenticated) {
      return;
    }

    if (command == "status" && authenticated) {
      bool statusSwitch_1 = digitalRead(SWITCH_1);
      bool statusSwitch_2 = digitalRead(SWITCH_2);

      if (statusSwitch_1 && !statusSwitch_2) {
        Serial.println("switch:1:on, switch:2:off");
      } else if (!statusSwitch_1 && statusSwitch_2) {
        Serial.println("switch:1:off, switch:2:on");
      } else if (!statusSwitch_1 && !statusSwitch_2) {
        Serial.println("switch:1:off, switch:2:off");
      } else if (statusSwitch_1 && statusSwitch_2) {
        Serial.println("switch:1:on, switch:2:on");
      }
    }
    else if (command == "switch:1:on" && authenticated) {
      if (digitalRead(SWITCH_2) == HIGH) {
        Serial.println("cmd:err");
      } else {
        digitalWrite(SWITCH_1, HIGH);
        Serial.println("cmd:ok");
      }
    }
    else if (command == "switch:2:on" && authenticated) {
      if (digitalRead(SWITCH_1) == HIGH) {
        Serial.println("cmd:err");
      } else {
        digitalWrite(SWITCH_2, HIGH);
        Serial.println("cmd:ok");
      }
    }
    else if (command == "switch:a:off" && authenticated) {
      digitalWrite(SWITCH_1, LOW);
      digitalWrite(SWITCH_2, LOW);
      Serial.println("cmd:ok");
    }
  }
}
