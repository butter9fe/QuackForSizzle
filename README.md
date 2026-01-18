# QuackForSizzle 🍳🦆

**Cook with real pans. Win virtual glory.**

QuackForSizzle is an innovative cooking game that bridges the physical and digital worlds by transforming real cooking pans into game controllers. Built with custom hardware and Unity, players use actual panning motions to flip pancakes, sear steaks, and master culinary challenges in an immersive gaming experience.

## 🎮 Features

### Physical Controllers
- **Custom Pan Hardware**: Real cooking pans retrofitted with embedded sensors
- **Gyroscope Integration**: Precise motion tracking for flipping, tilting, and panning movements
- **NFC Technology**: Seamless food item recognition and interaction
- **Real-World Physics**: Your actual pan movements translate directly into in-game actions

### Gameplay
- **Interactive Cooking Challenges**: Flip pancakes, sear steaks, and complete timed cooking tasks
- **Motion-Based Controls**: Use natural cooking motions to control your character
- **Progressive Difficulty**: Master basic flips before tackling advanced culinary techniques
- **Haptic Feedback**: Feel the sizzle through responsive game mechanics

## 🛠️ Technical Stack

- **Game Engine**: Unity
- **Hardware**: Custom IoT pan controllers
  - Gyroscope sensors for motion detection
  - NFC readers for item recognition
  - Wireless communication modules
- **Languages**: C# (Unity), Embedded C/C++ (Hardware)

## 🚀 Getting Started

### Prerequisites
- Unity 2020.3 LTS or later
- QuackForSizzle pan controller hardware
- Compatible operating system (Windows/macOS/Linux)

### Installation

1. Clone the repository:
```bash
git clone https://github.com/butter9fe/QuackForSizzle.git
cd QuackForSizzle
```

2. Open the project in Unity:
   - Launch Unity Hub
   - Click "Add" and select the cloned project folder
   - Open the project with Unity 2020.3 LTS or later

3. Connect your pan controller:
   - Ensure the pan controller is powered on
   - Pair the device via Bluetooth/USB (see hardware documentation)
   - Calibrate sensors in the game settings menu

4. Run the game:
   - Open the main scene in `Assets/Scenes/MainGame.unity`
   - Press the Play button in Unity Editor
   - Start cooking!

## 🎯 How to Play

1. **Calibrate Your Pan**: Hold the pan level and press the calibration button
2. **Select Your Challenge**: Choose from various cooking mini-games
3. **Cook with Motion**: Use real flipping motions to interact with virtual food
4. **Master the Timing**: Perfect your flips to achieve the highest scores
5. **Progress Through Levels**: Unlock new recipes and challenges

### Controls
- **Tilt Pan**: Move your character or adjust cooking temperature
- **Flip Motion**: Execute cooking actions (flip pancakes, toss vegetables)
- **NFC Tap**: Select ingredients or special items
- **Pan Shake**: Activate special abilities or power-ups

## 📁 Project Structure

```
QuackForSizzle/
├── Assets/
│   ├── Scenes/          # Unity scenes
│   ├── Scripts/         # Game logic and controller integration
│   ├── Prefabs/         # Reusable game objects
│   ├── Materials/       # Textures and shaders
│   └── Audio/           # Sound effects and music
├── Hardware/
│   ├── Firmware/        # Pan controller firmware
│   ├── Schematics/      # Circuit diagrams
│   └── Docs/            # Hardware documentation
└── README.md
```

## 🔧 Hardware Setup

The QuackForSizzle pan controller includes:
- **Gyroscope Module**: Tracks 6-axis motion (pitch, roll, yaw)
- **NFC Reader**: RC522 module for item detection
- **Microcontroller**: ESP32 or Arduino-compatible board
- **Battery**: Rechargeable lithium-ion power supply
- **Communication**: Bluetooth Low Energy for wireless connectivity

For detailed hardware assembly instructions, see `Hardware/Docs/assembly.md`.

## 🤝 Contributing

We welcome contributions! Whether you want to:
- Add new cooking challenges
- Improve motion detection algorithms
- Design new hardware modifications
- Fix bugs or enhance performance

Please read our contributing guidelines and submit a pull request.

## 📜 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🙏 Acknowledgments

- Thanks to all playtesters who helped refine the pan motion controls
- Inspired by the joy of cooking and the creativity of gamers everywhere
- Built with passion for innovative gaming experiences

## 📞 Contact

- **GitHub**: [@butter9fe](https://github.com/butter9fe)
- **Issues**: [Report bugs or request features](https://github.com/butter9fe/QuackForSizzle/issues)

---

**Ready to sizzle?** Grab your pan and start cooking! 🍳✨