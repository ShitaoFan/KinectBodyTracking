using UnityEngine;
using System.Collections;

public class HeadTracking : MonoBehaviour {

	public KinectWrapper.NuiSkeletonPositionIndex TrackedJoint = KinectWrapper.NuiSkeletonPositionIndex.Head;
	public GameObject OverlayObject;

	public float smoothFactor = 5f;

    public float verticalOffSet = 0.0f;
	
	public GUIText debugText;


	// Use this for initialization
	void Start () {

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
							scaleY = (float)posJoint.y + verticalOffSet;
							scaleZ = -1.0f * (float)posJoint.z;


						
						//						Vector3 localPos = new Vector3(scaleX * 10f - 5f, 0f, scaleY * 10f - 5f); // 5f is 1/2 of 10f - size of the plane
						//						Vector3 vPosOverlay = backgroundImage.transform.TransformPoint(localPos);
						//                      Vector3 vPosOverlay = BottomLeft + ((vRight * scaleX) + (vUp * scaleY));
						
						if(debugText)
						{
							debugText.GetComponent<GUIText>().text = "Tracked user ID: " + userId;  // new Vector2(scaleX, scaleY).ToString();
						}
						
						if(OverlayObject)
						{
							Vector3 vPosOverlay = new Vector3(scaleX, scaleY, scaleZ);
							OverlayObject.transform.position = Vector3.Lerp(OverlayObject.transform.position, vPosOverlay, smoothFactor * Time.deltaTime);
						}
					}
				}
				
			}
			
		}
	}
}
