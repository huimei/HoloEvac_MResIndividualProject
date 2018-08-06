The aim of this project is projecting the skull's CT scan as a 3D model and the position of skull drill tip to surgeon through HoloLens. Hence, this project requires the use of MicronTracker as an external tracking source instead of just using the build-in camera in HoloLens. The use of external tracker allows the surgeon to be able to move his/hers head freely without having the issue to position the HoloLens' camera in odd position in order to see the marker on skull and skull drill. 

In order to achieve accurate overlay projection, calibration is needed to find out the transformation between the marker on HoloLens to the user's eyes. The calibration process flow is shown as below:
1) In HoloLens, a red cross is displayed on screen with slight horizontal disparity for right and left eye. {Calibration_Disparity}
2) When user aligened the red cross with a target in the world space, he/she will press the clicker.
3) MicronTracker software will record down the position of HoloLens marker and target's marker. {Micron_Calib}
4) User repeat this process with 10 different screen positions and target positions (total 100 points taken).
5) At this stage, calculation is done offline so the technician will have to transfer the data from MicronTracker software to Java code. {CalculateCalibrationMatrix} 
6) Screen points are added into the data and a few methods with different data size are used to determine the best calibration result. Methods used: Direct Linear Transformation(DLT), Least Square (LS), OpenCV estimationAffine3D, DLT and LS with RANSAC.
7) As the last 20set of calibration data are reserved from the beginning, these data are used to calculate the reprojection error of the calibration matrix.
8) Summary of reprojection error are produced as csv file.
9) After getting calibration matrix, the matrix is plug into {Micron_Veri} code.
10) {Calib_Veri) code will run in HoloLens to receive the data from Micron_Veri and project a cube based on the give position and rotation.

Code for acute subdural hematoma evacuation procedure will rely on {HoloEvac_Micron} for MicronTracker side and {HoloEvac} for HoloLens side. --work in progress 

TODO list:
- Work on RANSAC for 3D points instead of 2D
- Connect {HoloEvac} to a server and get skull FBX data and calibration data straight from server
- Connect the flow for calibration into one system instead of multiple individual system
- Auto select the best calibration matrix 

IDE used:
Visual Studio 2015 for C# code
Eclipse Photon for Java
Unity 2017.3.1p4 for Unity

Set-up:
MicronTracker related code requires library from MicronTracker (Get it from the USB provided by MicronTracker)
OpenCV for eclipse: https://docs.opencv.org/2.4/doc/tutorials/introduction/linux_eclipse/linux_eclipse.html
Other libraries used in Java code can be found in "Libraries" folder
Libraries used in C# code can be restored from NuGet packages
