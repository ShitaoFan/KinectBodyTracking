using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HitAction : MonoBehaviour
{
    public CameraShaking cameraShaking;

    // Start is called before the first frame update
    void Start()
    {
    }

    void OnTriggerEnter(Collider myCollider)
    {
        //Output the Collider's GameObject's name
        Debug.Log("Hitme");
        cameraShaking.enabled = true;

        // SceneManager.LoadScene("AvatarTracking", LoadSceneMode.Additive);
        
       
    }


}
