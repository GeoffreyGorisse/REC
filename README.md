# REC: Replay, Export and Capture Tracked Movements in Unity  - Version 0.2.0 (Beta)
*18/03/2025*  
*Contact: Geoffrey Gorisse, PhD, [geoffrey.gorisse@gmail.com](mailto:geoffrey.gorisse@gmail.com)*

REC is an open-source Unity tool designed for developers and researchers to record, export, and replay the movements of virtual reality users and virtual characters. It enables the recording of body tracking data and animations, exports skeletal data in CSV files for analysis, and allows replaying the recorded movements.

Gorisse, G., Christmann, O., & Dubosc, C. (2022, June). REC: A Unity Tool to Replay, Export and Capture Tracked Movements for 3D and Virtual Reality Applications. In _Proceedings of the 2022 International Conference on Advanced Visual Interfaces_ (pp. 1-3).

---

## REC Features Overview
- **Record Movements**: Capture body tracking data or animated character movements.
- **Export & Import**: Export motion data in CSV format for further analysis and re-import it for replaying on virtual characters.
- **Replay Movements**: Play back recorded movements on virtual characters with the same skeleton in real-time.

**Watch the video**: [REC Tool Video](https://youtu.be/JoGoU34bTAk)

---

## Movement Recording
To record a virtual character's movements, tag bones in the Unity hierarchy. The tool records the local rotations of these bones along with the root bone position. The system uses Unity’s tagging system to select bones for recording to allow for full or partial-body tracking.

---

## Data Export and Import
REC exports motion data in CSV format, making it compatible with most statistical analysis software. The exported file includes:
- Character's scale.
- Recording delay in milliseconds (16.6 ms or 60 Hz by default).
- Recording time for each set of data.
- Character's position and rotation.
- Root bone's local position.
- Selected bones' local rotations. 

The exported data can be reloaded from the **StreamingAssets** folder.

---

## Movement Replaying
The tool matches recorded bone names using the tagging system, enabling real-time replay of movements by applying the data to the corresponding bones.

---

## Installation & Setup

1. **Clone the repository** or **download the latest release** from GitHub.
2. Open the project in **Unity** (recommended Unity version: 2020.3 or higher).
3. Enter **Play Mode** in the **REC demo scene**.

---

## License

This project is licensed under the **GNU GPLv3**.  
