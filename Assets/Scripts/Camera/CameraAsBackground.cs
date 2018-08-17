using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraAsBackground : MonoBehaviour {

    private RawImage image;
    private WebCamTexture cam;

	// Use this for initialization
	void Start () {
        image = GetComponent<RawImage>();
        cam = new WebCamTexture(Screen.width, Screen.height);
        image.texture = cam;
        cam.Play();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
