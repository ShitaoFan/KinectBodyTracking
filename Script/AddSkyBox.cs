using UnityEngine;
using System.Collections;

public class AddSkyBox : MonoBehaviour {

	// Use this for initialization
	void Start () {
	

	}
	
	// Update is called once per frame
	void Update () {
	
		for (int i = 0; i < transform.childCount; i++ )
		{
			if(transform.GetChild(i).gameObject.GetComponent<Skybox>() == null)
			{
				transform.GetChild(i).gameObject.AddComponent<Skybox>();
				transform.GetChild(i).gameObject.GetComponent<Skybox>().material = transform.gameObject.GetComponent<Skybox>().material;

			}
		}
	}
}
