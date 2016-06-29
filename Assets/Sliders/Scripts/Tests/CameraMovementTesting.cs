using Sliders;
using System.Collections;
using UnityEngine;

namespace Sliders.Tests
{
    public class CameraMovementTesting : MonoBehaviour
    {
        public CameraMovement c;
        public float testDriveTime = 0.7F;
        public Vector2 testStart = new Vector2(0, 100);
        public Vector2 testGoal = new Vector2(0, 450);
        public bool testStartToCurrentPos = true;
        private Vector3 testLastPosition;
        private bool testDriveController = false;

        private void testDrive()
        {
            Vector3 testPosition = new Vector3(testGoal.x, testGoal.y, Constants.cameraY);
            testLastPosition = transform.position;
            c.moveCamTo(testPosition, testDriveTime);
        }

        private void testDriveBack()
        {
            if (testStartToCurrentPos)
            {
                c.moveCamTo(testLastPosition, testDriveTime);
            }
            else
            {
                Vector3 testPosition = new Vector3(testStart.x, testStart.y, Constants.cameraY);
                testLastPosition = transform.position;
                c.moveCamTo(testPosition, testDriveTime);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) && !testDriveController && !CameraMovement.cameraMove)
            {
                testDrive();
                testDriveController = true;
            }
            else if (Input.GetKeyDown(KeyCode.Mouse0) && testDriveController && !CameraMovement.cameraMove)
            {
                testDriveBack();
                testDriveController = false;
            }
        }
    }
}