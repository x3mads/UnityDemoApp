# X3M Library Implementation Example Demo

This project serves as an example of how to implement the X3M library to display advertisements in your Unity application.

## Overview

This Unity project demonstrates the integration of the X3M library, showcasing how to effectively implement and display ads using its features and functionalities within a Unity environment.

## Features

- Example implementation of X3M library in Unity
- Demonstration of ad display capabilities in a Unity game/app

## Getting Started

### Prerequisites

For basic use and demonstration purposes, you'll need:

- Unity 2022.3.50f1 or later
- Basic knowledge of Unity development

To test with production ads:

- You will need an X3M account. If you don't have one, you can sign up at [X3M's official website](https://www.x3mads.com/).
- Familiarity with the X3M platform and its configuration process is recommended.

### Installation

1. Clone the repository:

- git clone https://github.com/x3mads/UnityDemoApp.git

2. Open the project in Unity:
- Launch Unity Hub
- Click "Add" and browse to the cloned repository directory
- Select the project and open it

3. Run the project:

### Exporting for Android and Opening in Android Studio

1. Open your project in Unity
2. Go to File > Build Settings
3. Select Android from the platform list and click "Switch Platform" if it's not already selected
4. Click "Player Settings" and configure your project (package name, version, etc.)
5. Ensure you have the Android SDK and JDK properly set up in Unity preferences
6. In Build Settings, check "Export Project" option
7. Click "Export" and choose a location to save your Android project
8. Open Android Studio
9. Select "Open an Existing Project" and navigate to the exported project folder
10. Wait for the project to sync and build in Android Studio

### Exporting for iOS and Opening in Xcode

1. Open your project in Unity
2. Go to File > Build Settings
3. Select iOS from the platform list and click "Switch Platform" if it's not already selected
4. Click "Player Settings" and configure your project (bundle identifier, version, etc.)
5. Click "Build"
6. Choose a location to save your Xcode project
7. Once the export is complete, open Xcode
8. In Xcode, go to File > Open
9. Navigate to the exported Unity-iPhone.xcodeproj file and open it

Note: For iOS, you'll need a Mac with Xcode installed. You may also need an Apple Developer account for testing on physical devices.

## Usage

For detailed information on how to use this X3M library implementation in Unity, please refer to the official X3M Ads documentation:

[X3M Ads Unity Documentation](https://docs.loomit.x3mads.com/docs/Loomit/SDK%20integration%20guide/unity/Prerequisites/)

This documentation provides comprehensive guides and examples for integrating and utilizing the X3M library in your Unity projects.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE)  file for details.

## Acknowledgements

- X3M Library
- Unity Technologies