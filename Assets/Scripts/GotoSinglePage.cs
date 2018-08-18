using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GotoSinglePage : MonoBehaviour {

    private AssetBundle myLoadedAssetBundle;

    // Use this for initialization
    void Start () {
        Button button = GetComponent<Button>();

        button.onClick.AddListener(OnClick);
	}

    void OnClick()
    {
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
