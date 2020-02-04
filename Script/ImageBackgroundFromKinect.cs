using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageBackgroundFromKinect : MonoBehaviour
{
    public GUITexture backgroundImage;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        KinectManager manager = KinectManager.Instance;

        if (manager && manager.IsInitialized())
        {
            //backgroundImage.renderer.material.mainTexture = manager.GetUsersClrTex();
            if (backgroundImage && (backgroundImage.texture == null))
            {
                backgroundImage.texture = manager.GetUsersClrTex();
            }
        }
    }
}
