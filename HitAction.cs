using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HitAction : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void OnTriggerEnter(Collider myCollider)
    {
        //Output the Collider's GameObject's name
        Debug.Log("Hitme");
        // SceneManager.LoadScene("AvatarTracking", LoadSceneMode.Additive);
       
    }


}
