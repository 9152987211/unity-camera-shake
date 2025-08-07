# ScreenShake for Unity
An easy to use and lightweight screen shake system for Unity.
Supports both rotation and position shaking with customizable strength, frequency and duration.

## Installation
Simply download the ScreenShake.unitypackage file and double click it to import Screen Shake into your project.

## Usage
1. Add the 'ScreenShake' script to the Camera or GameObject you want to shake.
2. To trigger the screen shake, use the following code with your own parameters:

```
ScreenShake.Instance.StartShake(duration, strength, frequency, shakeType);
```

For example:

```
ScreenShake.Instance.StartShake(0.3f, 1.5f, 75.0f, ShakeType.Rotation);
```
