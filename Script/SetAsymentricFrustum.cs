using UnityEngine;
using System.Collections;

public struct myCameraStruct
{
	public Transform transformCamera;
	
	//Frustum Parameters
	public float leftFrustum;
	public float rightFrustum;
	public float bottomFrustum;
	public float topFrustum;
	
	public float nearZ;
	public float farZ;
	
	public float fovy;
	public float aspect;
	public float screenZ;
	
	public float frustumXShift;
	public float frustumYShift;
};

public struct myScreenStruct
{
	public Vector3 ScreenPosition;
	//Screen size Parameters
	public float right;
	public float left;
	public float top;
	public float bottom;
};

public class SetAsymentricFrustum : MonoBehaviour {
	
	public float InterOcularDistance;
	//Default values
	public float DistanceTrackingOriginFromScreen;
	private float convergenceAngleFactor;
	public float FieldOfView;
	
	//Camera Setup Values
	public float Nearclip;
	public float FarClip;
	
	//Show Debug Frustum
	public bool showFrustum;
	public bool showFocalPt;
	
	//Used when tracking enabled or 1 to one scale
	public bool IsTracked;
	
	//Screen Values
	public float ScreenWidth;
	public float ScreenHeight;
	private float calculatedHalfVertFoV;
    public float screenVerticalOffset = 0.0f;

    //Only 2 Camera
    myCameraStruct[] myCamStruct = new myCameraStruct[2];
	
	//Only one screen
	myScreenStruct[] myScrStruct = new myScreenStruct[1];
	
	void Awake()
	{
		CreateScreenSetup();
	}

	void CreateScreenSetup()
	{
		if(IsTracked)
			myScrStruct[0].ScreenPosition = new Vector3 (0.0f,ScreenHeight/2 +screenVerticalOffset,DistanceTrackingOriginFromScreen);
		else
			myScrStruct[0].ScreenPosition = new Vector3 (0.0f,0.0f, DistanceTrackingOriginFromScreen);
		
		myScrStruct[0].right = ScreenWidth/2;
		myScrStruct[0].left = -ScreenWidth/2;
		myScrStruct[0].top = ScreenHeight/ 2 + screenVerticalOffset;
		myScrStruct[0].bottom = -ScreenHeight/ 2 + screenVerticalOffset;
		
		//Simulate the Real Physical Screen
		GameObject myScreen  = GameObject.CreatePrimitive(PrimitiveType.Plane);
		//Place Screen on a Layer so it is not rendered by none of the Cameras
		myScreen.layer = LayerMask.NameToLayer("Screen");
		//Disable colider so it does not hamper the selection with mouse
		myScreen.transform.GetComponent<Collider>().enabled = false;
		myScreen.name = "Screen";
		
		myScreen.transform.Rotate (new Vector3(90.0f,180.0f,0.0f));
		myScreen.transform.position = myScrStruct[0].ScreenPosition;
		myScreen.transform.localScale = new Vector3(ScreenWidth/10, 0.0f, ScreenHeight/10);
		
		calculatedHalfVertFoV = Mathf.Atan(myScrStruct[0].top/DistanceTrackingOriginFromScreen);
	}

	// Use this for initialization
	void Start () {

		CreateStereoSetup("right");
		CreateStereoSetup("left");

		//Translate each camera to be assigned for both eyes
		for (int i = 0; i < transform.childCount; i++)
		{
			if(transform.GetChild(i).name.Contains("leftCam"))
			{
				transform.GetChild(i).Translate(Vector3.left * InterOcularDistance/2);
				convergenceAngleFactor = 1.0f;//Left Camera
			}
			if (transform.GetChild(i).name.Contains("rightCam"))
			{
				transform.GetChild(i).Translate(Vector3.right * InterOcularDistance/2);
				convergenceAngleFactor = -1.0f;//Right Camera
			}
			//Setup Frustum Parameters - Useful when no tracking
			SetAsymetricFrustumParameters(i);
		}
	}
	
	void CreateStereoSetup(string side)
	{
		int sideFactor = 0;
		if (side == "left") 
			sideFactor = -1;
		if (side == "right") 
			sideFactor = 1;
		
		GameObject myCameraObject = GameObject.Find(side + "Cam");
		Camera myCamera = myCameraObject.transform.GetComponent<Camera>();
		//myCameraObject.transform.parent = GameObject.Find("StereoCamera").transform;
		//myCameraObject.transform.localPosition = new Vector3(0.0f,0.0f,0.0f);
		myCamera.cullingMask &=  ~(1 << LayerMask.NameToLayer("Stereo"));
		myCamera.cullingMask &=  ~(1 << LayerMask.NameToLayer("Screen"));
		myCamera.depth = -1;
		
		if (IsTracked)
			myCamera.aspect = (float)(ScreenWidth/ScreenHeight);
		else
			//Camera aspect Ratio
			myCamera.aspect = (float)(Screen.width/2)/Screen.height;
		
		//Camera Parameters
		myCamera.nearClipPlane = Nearclip;
		myCamera.farClipPlane = FarClip;
		
		if (IsTracked)
			myCamera.fieldOfView = calculatedHalfVertFoV*Mathf.Rad2Deg*2;
		else
			myCamera.fieldOfView = FieldOfView;
		
		/*RenderTexture rt = new RenderTexture(1024,1024,0);
		rt.name = "RenderTexture" + side;
		rt.depth = 24;
		//Assign Render Texture to Camera
		myCamera.targetTexture = rt;*/
		
		//CreateDisplayPlane for Orthographic Camera
		/*GameObject myPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
		myPlane.name = "myPlane"+side;
		myPlane.transform.parent = GameObject.Find("Main Camera").transform;*/
		
		//Place the plane on the Stereo Layer
		/*myPlane.layer = LayerMask.NameToLayer("Stereo");
		
		myPlane.renderer.material.mainTexture = rt;
		myPlane.transform.Rotate (new Vector3(90.0f,180.0f,0.0f));
		myPlane.transform.localScale = new Vector3(0.1f,0.1f,0.1f);
		myPlane.transform.Translate(new Vector3(sideFactor * 10 * 0.1f/2,0.0f,0.0f),Space.World);*/
	}
	
	// Update is called once per frame
	void Update () {
		
		for (int i = 0; i < transform.childCount; i++)
		{
			if(IsTracked)//Necessary update each frame
				SetAsymetricFrustumParameters(i);
			SetPerspectiveOffCenter(i,myCamStruct[i].leftFrustum,myCamStruct[i].rightFrustum,myCamStruct[i].bottomFrustum,myCamStruct[i].topFrustum,myCamStruct[i].nearZ,myCamStruct[i].farZ);
			
			if (IsTracked)
			{
				if(showFocalPt)
					Debug.DrawLine(myCamStruct[i].transformCamera.position,myScrStruct[0].ScreenPosition,Color.red);
			}
			else
			{
				if(showFocalPt)
					Debug.DrawLine(myCamStruct[i].transformCamera.position,myCamStruct[i].transformCamera.parent.position + new Vector3(0.0f,0.0f,myCamStruct[i].screenZ),Color.red);
			}
			
			//Debug Function to Draw frustum volume
			//Draw Top Right
			if(showFrustum)
			{
				Debug.DrawRay(myCamStruct[i].transformCamera.position, new Vector3(myCamStruct[i].rightFrustum,myCamStruct[i].topFrustum,myCamStruct[i].nearZ)*1000, Color.white);
				//Draw Top Left
				Debug.DrawRay(myCamStruct[i].transformCamera.position, new Vector3(myCamStruct[i].leftFrustum,myCamStruct[i].topFrustum,myCamStruct[i].nearZ)*1000, Color.white);
				//Draw Bottom Right
				Debug.DrawRay(myCamStruct[i].transformCamera.position, new Vector3(myCamStruct[i].rightFrustum,myCamStruct[i].bottomFrustum,myCamStruct[i].nearZ)*1000, Color.white);
				//Draw Bottom Left
				Debug.DrawRay(myCamStruct[i].transformCamera.position, new Vector3(myCamStruct[i].leftFrustum,myCamStruct[i].bottomFrustum,myCamStruct[i].nearZ)*1000, Color.white);
			}
		}
	}
	
	void SetAsymetricFrustumParameters(int i)
	{
		myCamStruct[i].transformCamera = transform.GetChild(i); 
		
		//Get current Frustum values
		myCamStruct[i].screenZ = DistanceTrackingOriginFromScreen - transform.position.z;//DistanceTrackingOriginFromScreen;
		if(IsTracked)
			myCamStruct[i].fovy = 2*Mathf.Atan((ScreenHeight/2)/myCamStruct[i].screenZ)*Mathf.Rad2Deg;//transform.GetChild(i).camera.fieldOfView;
		else
			myCamStruct[i].fovy = transform.GetChild(i).GetComponent<Camera>().fieldOfView;
		myCamStruct[i].aspect = transform.GetChild(i).GetComponent<Camera>().aspect;
		
		myCamStruct[i].nearZ = transform.GetChild(i).GetComponent<Camera>().nearClipPlane;
		myCamStruct[i].farZ = transform.GetChild(i).GetComponent<Camera>().farClipPlane;
		
		
		
		if (IsTracked)
		{
			//myCamStruct[i].frustumXShift = convergenceAngleFactor * (-myCamStruct[i].transformCamera.position.x)* myCamStruct[i].nearZ/myCamStruct[i].screenZ;
			myCamStruct[i].frustumXShift = convergenceAngleFactor * (myCamStruct[i].transformCamera.position.x)* myCamStruct[i].nearZ/myCamStruct[i].screenZ;
			myCamStruct[i].frustumYShift = (ScreenHeight/2 + screenVerticalOffset - myCamStruct[i].transformCamera.position.y)* myCamStruct[i].nearZ/myCamStruct[i].screenZ;
		}
		else
		{
			//Instead of using InterOcularDistance/2 we can also use the local coordinates of the camera
			myCamStruct[i].frustumXShift = convergenceAngleFactor * InterOcularDistance/2 * myCamStruct[i].nearZ/myCamStruct[i].screenZ;
			myCamStruct[i].frustumYShift = 0.0f;
		}
		
		//Symetric Frustum values
		float top;
		float right;
		
		top = myCamStruct[i].nearZ * Mathf.Tan(Mathf.Deg2Rad * myCamStruct[i].fovy/2);
		right = myCamStruct[i].aspect * top;
		
		//Setup Asymetric frustum parameters
		myCamStruct[i].topFrustum = top+ myCamStruct[i].frustumYShift;
		myCamStruct[i].bottomFrustum = -top+ myCamStruct[i].frustumYShift;
		myCamStruct[i].leftFrustum = -right + myCamStruct[i].frustumXShift;
		myCamStruct[i].rightFrustum = right + myCamStruct[i].frustumXShift;
	}
	
	void SetPerspectiveOffCenter(int i,float left, float right, float bottom, float top, float near, float far)
	{
		float x = 2.0F * near / (right - left);
		float y = 2.0F * near / (top - bottom);
		float a = (right + left) / (right - left);
		float b = (top + bottom) / (top - bottom);
		float c = -(far + near) / (far - near);
		float d = -(2.0F * far * near) / (far - near);
		float e = -1.0F;
		Matrix4x4 m = new Matrix4x4();
		m[0, 0] = x;
		m[0, 1] = 0;
		m[0, 2] = a;
		m[0, 3] = 0;
		m[1, 0] = 0;
		m[1, 1] = y;
		m[1, 2] = b;
		m[1, 3] = 0;
		m[2, 0] = 0;
		m[2, 1] = 0;
		m[2, 2] = c;
		m[2, 3] = d;
		m[3, 0] = 0;
		m[3, 1] = 0;
		m[3, 2] = e;
		m[3, 3] = 0;
		transform.GetChild(i).GetComponent<Camera>().projectionMatrix = m;
	}
	

	

}

