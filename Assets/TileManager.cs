using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TileManager : MonoBehaviour
{

    private float lat, lon;

    [SerializeField]
    private Texture2D texture;
    
    public GameObject Map;

    IEnumerator Start()
    {
        while (!Input.location.isEnabledByUser)
        {
            Debug.Log("Ta preso aqui!");
            yield return new WaitForSeconds(1f);
        }

        Debug.Log("1!");
        Input.location.Start(10f, 5f);
        Debug.Log("2!");


        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            Debug.Log("Ta preso aqui2!");
            yield return new WaitForSeconds(1f);
            maxWait--;
        }

        if (maxWait < 1)
        {
            yield break;
        }

        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.Log("Entra aqui!");
            yield break;
        }
        else
        {
            lat = Input.location.lastData.latitude;
            lon = Input.location.lastData.longitude;
        }

        StartCoroutine(loadTiles(18));

        while (!Input.location.isEnabledByUser)
        {
            yield return new WaitForSeconds(1f);
        }

        StartCoroutine(Start());
    }

    IEnumerator loadTiles(int zoom)
    {
        lat = Input.location.lastData.latitude;
        lon = Input.location.lastData.longitude;

        string url = string.Format(@"http://api.mapbox.com/v4/mapbox.{5}/{0},{1},{2}/{3}x{3}@2x.png?access_token={4}", lon, lat, zoom, 640, "pk.eyJ1IjoiaWNhcm8tbGltYSIsImEiOiJjamt5bHFzZHowa3BsM2xsbTFsdHBsOXRvIn0.tEKqmS22qois9WniVll-og", "emerald");

        WWW wWW = new WWW(url);
        yield return wWW;

        Debug.Log(url);

        Map.GetComponent<RawImage>().texture = wWW.texture;

        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(loadTiles(18));
    }

    // Update is called once per frame
    void Update()
    {
        /*System.Threading.Thread.Sleep(4000);

        string url = string.Format(@"http://api.mapbox.com/v4/mapbox.{5}/{0},{1},{2}/{3}x{3}@2x.png?access_token={4}", lon, lat, 18, 640, "pk.eyJ1IjoiaWNhcm8tbGltYSIsImEiOiJjamt5bHFzZHowa3BsM2xsbTFsdHBsOXRvIn0.tEKqmS22qois9WniVll-og", "emerald");

        WWW wWW = new WWW(url);

        Debug.Log(url);

        while (!wWW.isDone)
        {
            Debug.Log("Preso...");
            System.Threading.Thread.Sleep(100);
        }

        texture = wWW.texture;

        /*if (tile == null)
        {
            tile = GameObject.CreatePrimitive(PrimitiveType.Plane);
            tile.transform.localScale = Vector3.one * 1;
            tile.GetComponent<Renderer>().material = Material;
            tile.transform.position.Set(0, 0, 0);
            tile.transform.Rotate(new Vector3(-90, 0, 0));
            tile.transform.SetParent(transform);
        }

        tile.GetComponent<Renderer>().material.mainTexture = texture;

        Map.GetComponent<RawImage>().texture = texture;*/
    }
}