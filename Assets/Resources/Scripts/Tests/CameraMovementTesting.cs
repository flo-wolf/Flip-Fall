using Sliders;
using Sliders.Cam;
using System.Collections;
using UnityEngine;

namespace Sliders.Tests
{
    public class CameraMovementTesting : MonoBehaviour
    {
        public CamMove cm;
        public float testDriveTime = 0.7F;
        public Vector2 testStart = new Vector2(0, 100);
        public Vector2 testGoal = new Vector2(0, 450);
        public bool testStartToCurrentPos = true;
        private Vector3 testLastPosition;
        private bool testDriveController = false;

        private void testDrive()
        {
            Vector3 testPosition = new Vector3(testGoal.x, testGoal.y, Constants.cameraZ);
            testLastPosition = transform.position;
            CamMove.moveCamTo(testPosition, testDriveTime);
        }

        private void testDriveBack()
        {
            if (testStartToCurrentPos)
            {
                CamMove.moveCamTo(testLastPosition, testDriveTime);
            }
            else
            {
                Vector3 testPosition = new Vector3(testStart.x, testStart.y, Constants.cameraZ);
                testLastPosition = transform.position;
                CamMove.moveCamTo(testPosition, testDriveTime);
            }
        }

        private void Update()
        {
            if (CamMove.IsResting())
            {
                if (Input.GetKeyDown(KeyCode.Mouse0) && !testDriveController)
                {
                    testDrive();
                    testDriveController = true;
                }
                else if (Input.GetKeyDown(KeyCode.Mouse0) && testDriveController)
                {
                    testDriveBack();
                    testDriveController = false;
                }
            }
        }
    }
}