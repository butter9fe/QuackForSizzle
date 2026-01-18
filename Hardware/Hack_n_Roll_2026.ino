#include <Wire.h>
#include <Adafruit_MPU6050.h>
#include <Adafruit_Sensor.h>

#include "BluetoothSerial.h"

#include <SPI.h>
#include <MFRC522.h>

#define SDA_PIN 21
#define SCL_PIN 22
#define BTN_PIN 15

#define SS_PIN 5
#define RST_PIN 0

Adafruit_MPU6050 mpu;
BluetoothSerial SerialBT;

float lastAz = 0;
const char *deviceName = "Pan_1";
//const char *deviceName = "Pan_2";
unsigned long lastImpactMs = 0;
const float DEAD_X = 1.5;
const float DEAD_Y = 1.0;
const unsigned long DEBOUNCE_MS = 80;
bool btnLast = HIGH;
unsigned long btnLastChange = 0;

/*
enum Direction {
  NONE, // no change in state
  NEUTRAL, // rest position
  LEFT,
  RIGHT,
  UP,
  DOWN,
  FLIP
};
*/

enum Direction {
  NONE, // no change in state
  NEUTRAL, // rest position
  LEFT,
  RIGHT,
  UP,
  DOWN
};

Direction lastSent = NONE;

Direction detectGesture(float ax, float ay, float az) {
  static unsigned long lastTime = 0;
  unsigned long now = millis();

  float dz = az - lastAz; // change in acceleration in z direction
  lastAz = az;

  if (now - lastTime < 250) return NONE; // cooldown

  if (ax > 4.0)  { lastTime = now; return RIGHT; }
  if (ax < -4.0) { lastTime = now; return LEFT; }
  if (ay > 2.0)  { lastTime = now; return DOWN; }
  if (ay < -2.0) { lastTime = now; return UP; }
  // if (dz > 3.0) { lastTime = now; return FLIP; }
  if (abs(ax) < DEAD_X && abs(ay) < DEAD_Y) {
    return NEUTRAL;
  }
  // if (dz > 6.0) {   // sudden upward acceleration
  //   lastTime = now;
  //   lastAz = az;
  //   return FLIP;
  // }
  //lastAz = az;
  return NONE;
}

// to send to unity
const char* dirToStr(Direction d) {
  switch (d) {
    case NEUTRAL: return "NEUTRAL";
    case LEFT: return "LEFT";
    case RIGHT: return "RIGHT";
    case UP: return "UP";
    case DOWN: return "DOWN";
    //case FLIP: return "FLIP";
    default: return "NONE";
  }
}

bool checkButtonPressed() {
  bool reading = digitalRead(BTN_PIN);
  unsigned long now = millis();

  // debounce: only accept changes if stable for DEBOUNCE_MS
  if (reading != btnLast && (now - btnLastChange) > DEBOUNCE_MS) {
    btnLastChange = now;
    btnLast = reading;

    // pressed = LOW (because pullup)
    if (reading == LOW) return true;
  }
  return false;
}

// Cards

// Food 1
const String CARD_1 = "9F CE 63 1C";
const String CARD_2 = "69 BC 04 04";

// Food 2
const String CARD_3 = "B3 7F 87 40";
const String CARD_4 = "3A 92 65 A6";

int Food = 0;

String CurrentSide = "NONE";

// Tags
const String TAG_1 = "51 14 C1 01";
const String TAG_2 = "FC 2D 44 18";

MFRC522 rfid(SS_PIN, RST_PIN);  // Instance of the class

// Identify which card was scanned
const char* identifyCard(String hexString) {
  // Remove spaces and convert to uppercase for comparison
  hexString.trim();
  
  // Compare against known cards
  if (hexString == CARD_1) return "CARD_1";
  if (hexString == CARD_2) return "CARD_2";
  if (hexString == CARD_3) return "CARD_3";
  if (hexString == CARD_4) return "CARD_4";
  if (hexString == TAG_1) return "TAG_1";
  if (hexString == TAG_2) return "TAG_2";
  
  return "UNKNOWN";
}

// Helper routine to dump a byte array as hex values to Serial. 
String getHexString(byte *buffer, byte bufferSize) {
  String hexString = "";
  for (byte i = 0; i < bufferSize; i++) {
    hexString += (buffer[i] < 0x10 ? " 0" : " ");
    hexString += String(buffer[i], HEX);
  }
  hexString.toUpperCase();
  return hexString;
}



void setup() {
  Serial.begin(115200);
  

  // MPU6050
  Wire.begin(SDA_PIN, SCL_PIN);
  if (!mpu.begin()) {
    Serial.println("MPU6050 not found!");
    while (1) delay(10);
  }
  Serial.println("MPU6050 OK");

  // MFRC522 RFID
  SPI.begin();      // Init SPI bus
  rfid.PCD_Init();  // Init MFRC522

  Serial.println(F("This code scan the MIFARE Classsic NUID."));

  // Bluetooth
  SerialBT.begin(deviceName);  // Initiate Bluetooth device with name in parameter
  Serial.printf("The device started with name \"%s\", now you can pair it with Bluetooth!\n", deviceName);

}

void loop() {
  String PreviousSide = CurrentSide;

  // Reset the loop if no new card present on the sensor/reader. This saves the entire process when idle.
  if (rfid.PICC_IsNewCardPresent() & rfid.PICC_ReadCardSerial()){
    MFRC522::PICC_Type piccType = rfid.PICC_GetType(rfid.uid.sak);

    // Check is the PICC of Classic MIFARE type
    if (piccType != MFRC522::PICC_TYPE_MIFARE_MINI && piccType != MFRC522::PICC_TYPE_MIFARE_1K && piccType != MFRC522::PICC_TYPE_MIFARE_4K) {
      Serial.println(F("Your tag is not of type MIFARE Classic."));
      return;
    };

    Serial.print(F("The NUID tag is:"));
    String uidHex = getHexString(rfid.uid.uidByte, rfid.uid.size);
    CurrentSide = uidHex;
    Serial.print(CurrentSide);
    Serial.print(F(" - "));
    Serial.println(identifyCard(CurrentSide));

    // Halt PICC
    rfid.PICC_HaltA();

    // Stop encryption on PCD
    rfid.PCD_StopCrypto1();

    if ((PreviousSide != CurrentSide)){
      int NewFood = Food;
      if ((identifyCard(CurrentSide) == "CARD_1") || (identifyCard(CurrentSide) == "CARD_2")) NewFood = 1;
      if ((identifyCard(CurrentSide) == "CARD_3") || (identifyCard(CurrentSide) == "CARD_4")) NewFood = 2;

      if (Food == 0){
        Serial.println("food INITIATED");
        Food = NewFood;
        return;
      }

      if (NewFood != Food) {
        //Serial.println("SWAP");
        Serial.println("food SWAPPED");
        Food = NewFood;
      }
      else {
        Serial.println("FLIP");
        SerialBT.println("FLIP");
      }
      return;
    }
  }

  sensors_event_t a, g, temp;
  mpu.getEvent(&a, &g, &temp);

  Direction d = detectGesture(
    a.acceleration.x,
    a.acceleration.y,
    a.acceleration.z
  );

  if (d != NONE && d != lastSent) {
    Serial.println(dirToStr(d));
    SerialBT.println(dirToStr(d));
    lastSent = d;
  }
  delay(50);
}