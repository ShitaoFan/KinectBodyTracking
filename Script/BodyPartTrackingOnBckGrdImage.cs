using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartTrackingOnBckGrdImage : MonoBehaviour
{
    public KinectWrapper.NuiSkeletonPositionIndex TrackedJoint = KinectWrapper.NuiSkeletonPositionIndex.HandRight;
    public KinectWrapper.NuiSkeletonPositionIndex SecondTrackedJoint = KinectWrapper.NuiSkeletonPositionIndex.ElbowRight;
    public GameObject OverlayObject;
    public GameObject SecondObj;

    public float smoothFactor = 5f;

    private float distanceToCamera = 10f;


    // Use this for initialization
    void Start()
    {
        if (OverlayObject)
        {
            distanceToCamera = (OverlayObject.transform.position - Camera.main.transform.position).magnitude;
        }
    }

    void Update()
    {
        KinectManager manager = KinectManager.Instance;

        if (manager && manager.IsInitialized())
        {
            int iJointIndex = (int)TrackedJoint;
            int iJoint2Index = (int)SecondTrackedJoint;

            if (manager.IsUserDetected())
            {
                uint userId = manager.GetPlayer1ID();

                if (manager.IsJointTracked(userId, iJointIndex) && manager.IsJointTracked(userId, iJoint2Index))
                {
                    //Get the Position of the Skeleton Joint
                    Vector3 posJoint = manager.GetRawSkeletonJointPos(userId, iJointIndex);
                    Quaternion quatJoint = manager.GetJointOrientation(userId, iJointIndex, false);

                    //For Elbow
                    //Get the Position of the Skeleton Joint
                    Vector3 posJoint2 = manager.GetRawSkeletonJointPos(userId, iJoint2Index);
                    Quaternion quatJoint2 = manager.GetJointOrientation(userId, iJoint2Index, false);

                    //Create an orientation vector from Joint 1 and 2
                    Vector3 directionVector = posJoint - posJoint2;

                    if (posJoint != Vector3.zero)
                    {
                        // 3d position to depth
                        Vector2 posDepth = manager.GetDepthMapPosForJointPos(posJoint);
                        Vector2 posDepth2 = manager.GetDepthMapPosForJointPos(posJoint2);
                        
                        // depth pos to color pos
                        Vector2 posColor = manager.GetColorMapPosForDepthPos(posDepth);
                        Vector2 posColor2 = manager.GetColorMapPosForDepthPos(posDepth2);

                        float scaleX = (float)posColor.x / KinectWrapper.Constants.ColorImageWidth;
                        float scaleY = 1.0f - (float)posColor.y / KinectWrapper.Constants.ColorImageHeight;

                        float scaleX2 = (float)posColor2.x / KinectWrapper.Constants.ColorImageWidth;
                        float scaleY2 = 1.0f - (float)posColor2.y / KinectWrapper.Constants.ColorImageHeight;

                        if (OverlayObject)
                        {
                            distanceToCamera = posJoint.magnitude;

                            Vector3 vPosOverlay = Camera.main.ViewportToWorldPoint(new Vector3(scaleX, scaleY, distanceToCamera));
                            OverlayObject.transform.position = Vector3.Lerp(OverlayObject.transform.position, vPosOverlay, smoothFactor * Time.deltaTime);

                            Vector3 vPosOverlay2 = Camera.main.ViewportToWorldPoint(new Vector3(scaleX2, scaleY2, posJoint2.magnitude));
                            SecondObj.transform.position = Vector3.Lerp(SecondObj.transform.position, vPosOverlay2, smoothFactor * Time.deltaTime);
                           
                            OverlayObject.transform.LookAt(SecondObj.transform.position, Vector3.Cross(OverlayObject.transform.forward,Vector3.up));

                        }
                    }
                }

            }

        }
    }
}
