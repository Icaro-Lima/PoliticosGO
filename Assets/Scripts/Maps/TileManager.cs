using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{

    private float lat, lon;

    private Texture2D texture;
    private GameObject tile;

    // Use this for initialization
    IEnumerator Start()
    {
        while (!Input.location.isEnabledByUser)
        {
            yield return new WaitForSeconds(1f);
        }

        Input.location.Start(10f, 5f);

        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1f);
            maxWait--;
        }

        if (maxWait < 1)
        {
            yield break;
        }

        if (Input.location.status == LocationServiceStatus.Failed)
        {
            yield break;
        }
        else
        {
            lat = Input.location.lastData.latitude;
            lon = Input.location.lastData.longitude;
        }

        StartCoroutine(loadTiles(18));

        while (Input.location.isEnabledByUser)
        {
            yield return new WaitForSeconds(1f);
        }

        StartCoroutine(Start());
    }

    IEnumerator loadTiles(int zoom)
    {
        lat = Input.location.lastData.latitude;
        lon = Input.location.lastData.longitude;

        string url = string.Format();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
