using System;
using Microsoft.Kinect.Face;

namespace WS2
{
    partial class KinectControl
    {

        private static int convertDetectionResult(int value)
        {
            switch (value)
            {
                case 1:
                    return -1;
                case 3:
                    return 1;
                default:
                    return 0;
            }
        }

        private static void GetDataFaceFrameResults(FaceFrameResult faceResult)
        {
            // extract each face property information and store it in faceText
            if (faceResult.FaceProperties != null)
            {
                foreach (var item in faceResult.FaceProperties)
                {
                    switch (item.Key)
                    {
                        case FaceProperty.Happy:
                            TempResult.test[0] += convertDetectionResult((int)item.Value);
                            break;
                        case FaceProperty.LookingAway:
                            TempResult.test[1] += convertDetectionResult((int)item.Value);
                            break;
                    }
                }
                    
                //Console.WriteLine("PAUSE");
            }

            
            
            
            /*
            // extract face rotation in degrees as Euler angles
            if (faceResult.FaceRotationQuaternion != null)
            {
                int pitch, yaw, roll;
                ExtractFaceRotationInDegrees(faceResult.FaceRotationQuaternion, out pitch, out yaw, out roll);
                faceText += "FaceYaw : " + yaw + "\n" +
                            "FacePitch : " + pitch + "\n" +
                            "FacenRoll : " + roll + "\n";
            }*/
        }
    }
}