using UnityEngine;
using System.Collections;

public class ActivateObject : MonoBehaviour {

    public GameObject ExplosionObj;
    public GameObject Boxes;

	// Use this for initialization
	void Start () {
	    StartCoroutine("WaitAndExplode");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    IEnumerator WaitAndExplode()
    {
        // suspend execution for 5 seconds
        yield return new WaitForSeconds(5);
        foreach (Transform Child in Boxes.transform)
            Child.GetComponent<Rigidbody>().isKinematic = false; 
        ExplosionObj.SetActive(true);
    }
}
