using UnityEngine;
using System.Collections;

public class ObjectControlTracking : MonoBehaviour {

	public KinectWrapper.NuiSkeletonPositionIndex TrackedJoint = KinectWrapper.NuiSkeletonPositionIndex.Head;
	public GameObject OverlayObject;

	public float smoothFactor = 5f;


	// Use this for initialization
	void Start () {
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
					Vector3 posJoint = manager.GetRawSkeletonJointPos(userId, iJointIndex);
					Quaternion dirJoint = manager.GetJointOrientation(userId, iJointIndex, false);
					
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

							scaleX = (float)posJoint.x;
							scaleY = (float)posJoint.y;
							scaleZ = -1.0f*(float)posJoint.z + 5.0f; //Reverse the Z axis and introduce an offset



						if(OverlayObject)
						{
							Vector3 vPosOverlay = new Vector3(scaleX, scaleY, scaleZ);
							OverlayObject.transform.position = Vector3.Lerp(OverlayObject.transform.position, vPosOverlay, smoothFactor * Time.deltaTime);

                            //We freeze the rotation
							//Quaternion vDirOverlay = new Quaternion(dirJoint.x, dirJoint.y, dirJoint.z, dirJoint.w);
							//OverlayObject.transform.rotation = Quaternion.Lerp(OverlayObject.transform.rotation, vDirOverlay, smoothFactor * Time.deltaTime);
						}
					}
				}
				
			}
			
		}
	}
}
