using UnityEngine;
using System.Collections;

public class BodyPartTracking : MonoBehaviour {

	public KinectWrapper.NuiSkeletonPositionIndex TrackedJoint = KinectWrapper.NuiSkeletonPositionIndex.Head;
	public GameObject OverlayObject;

	public float smoothFactor = 5f;


	// Use this for initialization
	void Start () {
        //Assigned the over
		OverlayObject = transform.gameObject;
	}
	
	void Update() 
	{
		KinectManager manager = KinectManager.Instance;
		
		if(manager && manager.IsInitialized())
		{
			int iJointIndex = (int)TrackedJoint;
			
			if(manager.IsUserDetected())
			{
				uint userId = manager.GetPlayer1ID();
				
				if(manager.IsJointTracked(userId, iJointIndex))
				{
                    //Get the Position of the Skeleton Joint
					Vector3 posJoint = manager.GetRawSkeletonJointPos(userId, iJointIndex);

                    Quaternion quatJoint = manager.GetJointOrientation(userId, iJointIndex, false);
					
					if(posJoint != Vector3.zero)
					{
						// 3d position to depth
						/*Vector2 posDepth = manager.GetDepthMapPosForJointPos(posJoint);
						
						// depth pos to color pos
						Vector2 posColor = manager.GetColorMapPosForDepthPos(posDepth);*/
						
						//float scaleX = (float)posColor.x / KinectWrapper.Constants.ColorImageWidth;
						//float scaleY = 1.0f - (float)posColor.y / KinectWrapper.Constants.ColorImageHeight;

						float scaleX;
						float scaleY;
						float scaleZ;

                            //Revert Z axis as Kinect Z and Unity Z mmight point to contrary direction
							scaleX = (float)posJoint.x;
							scaleY = (float)posJoint.y;
							scaleZ = -1.0f*(float)posJoint.z+5;


						if(OverlayObject)
						{
							Vector3 vPosOverlay = new Vector3(scaleX, scaleY, scaleZ);
							OverlayObject.transform.position = Vector3.Lerp(OverlayObject.transform.position, vPosOverlay, smoothFactor * Time.deltaTime);

                            OverlayObject.transform.rotation = quatJoint;

                        }
					}
				}
				
			}
			
		}
	}
}
