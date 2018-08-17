﻿using System.Collections;
using UnityEngine;

public class TileManager : MonoBehaviour
{

    private float lat, lon;

    private Texture2D texture;
    public GameObject tile;

    public Material Material;

    void Start()
    {
        StartCoroutine(Starti());
    }

    private IEnumerator Starti()
    {
        /*while (!Input.location.isEnabledByUser)
        {
            Debug.Log("Ta preso aqui!");
            yield return new WaitForSeconds(1f);
        }*/

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

        StartCoroutine(Starti());
    }

    IEnumerator loadTiles(int zoom)
    {
        lat = Input.location.lastData.latitude;
        lon = Input.location.lastData.longitude;

        string url = string.Format(@"http://api.mapbox.com/v4/mapbox.{5}/{0},{1},{2}/{3}x{3}@2x.png?access_token={4}", lon, lat, zoom, 640, "pk.eyJ1IjoiaWNhcm8tbGltYSIsImEiOiJjamt5ZzU1b2wwajJpM3BtcHFtbjBvNG5lIn0.jxfMoybiJJItkUTzduWm6Q", "emerald");

        WWW wWW = new WWW(url);
        yield return wWW;

        texture = wWW.texture;

        if (tile == null)
        {
            tile = GameObject.CreatePrimitive(PrimitiveType.Plane);
            tile.transform.localScale = Vector3.one * 1;
            tile.GetComponent<Renderer>().material = Material;
            tile.transform.parent = transform;
        }

        tile.GetComponent<Renderer>().material.mainTexture = texture;

        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(loadTiles(18));
    }

    // Update is called once per frame
    void Update()
    {

    }
}