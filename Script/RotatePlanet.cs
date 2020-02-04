using UnityEngine;
using System.Collections;

public class RotatePlanet : MonoBehaviour {

    public GameObject earth;
    public GameObject moon;
    public GameObject sun;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        //Rotate earth
        earth.transform.Rotate(0.0f, 0.2f, 0.0f);

        //Rotate Moon
        moon.transform.Rotate(0.0f, 0.3f, 0.0f);

        //Rotate moon around earth
        moon.transform.RotateAround(earth.transform.position, Vector3.up, -0.35f);

        //Rotate sun
        sun.transform.RotateAround(earth.transform.position, Vector3.up, -0.25f);
    }
}
