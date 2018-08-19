using Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TileManager : MonoBehaviour
{

    private bool firstTime = true;

    private float lat, lon;
    private float Oldlat, Oldlon;

    [SerializeField]
    private Texture2D texture;

    [SerializeField]
    private Transform Target;

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

        while (lat == 0 && lon == 0)
        {
            lat = Input.location.lastData.latitude;
            lon = Input.location.lastData.longitude;

            yield return new WaitForSeconds(1f);
        }

        if (firstTime)
        {
            firstTime = false;

            CoordinatesPair body = new CoordinatesPair(lat, lon);

            Debug.Log("Body: " + JsonUtility.ToJson(body));

            byte[] decoded = System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(body).ToString());

            UnityWebRequest request = UnityWebRequest.Post(@"https://politicosgo.herokuapp.com/obras/closeto", "");
            request.uploadHandler = new UploadHandlerRaw(decoded);
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            string response = request.downloadHandler.text;

            string[] obras = response.Split('{');

            for (int i = 1; i < obras.Length; i++)
            {
                double latitude = 0, longitude = 0;
                {
                    int index = obras[i].IndexOf("\"latitude\":");
                    index += 11;

                    int closeIndex = obras[i].IndexOf(",", index);

                    latitude = double.Parse(obras[i].Substring(index, closeIndex - index));
                }

                {
                    int index = obras[i].IndexOf("\"longitude\":");
                    index += 12;

                    int closeIndex = obras[i].IndexOf(",", index);

                    longitude = double.Parse(obras[i].Substring(index, closeIndex - index));
                }

                Debug.Log(latitude + ", " + longitude);

                GameObject obra = GameObject.CreatePrimitive(PrimitiveType.Quad);
                obra.transform.SetParent(Map.transform);
                obra.transform.position.Set(0, 0, 0);

                Vector3 reeff = Quaternion.AngleAxis(lon, -Vector3.up) * Quaternion.AngleAxis(lat, -Vector3.right) * new Vector3(0, 0, 1);
                Vector3 to = Quaternion.AngleAxis((float)longitude, -Vector3.up) * Quaternion.AngleAxis((float)latitude, -Vector3.right) * new Vector3(0, 0, 1);

                //xoff *= 0.300122f;
                //yoff *= 0.123043f;

                obra.transform.Translate(to - reeff);
            }
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

        string url = string.Format(@"http://api.mapbox.com/v4/mapbox.{5}/{0},{1},{2}/{3}x{3}@2x.png?access_token={4}", lon, lat, zoom, 640, "pk.eyJ1IjoiaWNhcm8tbGltYSIsImEiOiJjamt5bHFzZHowa3BsM2xsbTFsdHBsOXRvIn0.tEKqmS22qois9WniVll-og", "emerald");

        WWW wWW = new WWW(url);
        yield return wWW;

        Debug.Log(url);

        if (Oldlat != 0 && Oldlon != 0)
        {
            float x, y;
            Vector3 position = Vector3.zero;

            geodeticOffsetInv(lat * Mathf.Deg2Rad, lon * Mathf.Deg2Rad, Oldlat * Mathf.Deg2Rad, Oldlon * Mathf.Deg2Rad, out x, out y);

            if ((Oldlat - lat) < 0 && (Oldlon - lon) > 0 || (Oldlat - lat) > 0 && (Oldlon - lon) < 0)
            {
                position = new Vector3(x, y, 0);
            }
            else
            {
                position = new Vector3(-x, -y, 0);
            }

            position.x *= 0.300122f;
            position.y *= 0.123043f;

            Target.position = position;
        }

        Map.GetComponent<RawImage>().texture = wWW.texture;

        yield return new WaitForSeconds(2f);

        Oldlat = lat;
        Oldlon = lon;

        while (Oldlat == lat && Oldlon == lon)
        {
            Debug.Log("Não se moveu!");

            lat = Input.location.lastData.latitude;
            lon = Input.location.lastData.longitude;

            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitUntil(() => Oldlat != lat || Oldlon != lon);

        yield return StartCoroutine(loadTiles(18));
    }

    // Update is called once per frame
    void Update()
    {
        Target.position = Vector3.Lerp(Target.position, new Vector3(0, 0, 0), 2.0f * Time.deltaTime);
    }

    float GD_semiMajorAxis = 6378137.000000f;
    float GD_TranMercB = 6356752.314245f;
    float GD_geocentF = 0.003352810664f;

    void geodeticOffsetInv(float refLat, float refLon,
        float lat, float lon,
        out float xOffset, out float yOffset)
    {
        float a = GD_semiMajorAxis;
        float b = GD_TranMercB;
        float f = GD_geocentF;

        float L = lon - refLon;
        float U1 = Mathf.Atan((1 - f) * Mathf.Tan(refLat));
        float U2 = Mathf.Atan((1 - f) * Mathf.Tan(lat));
        float sinU1 = Mathf.Sin(U1);
        float cosU1 = Mathf.Cos(U1);
        float sinU2 = Mathf.Sin(U2);
        float cosU2 = Mathf.Cos(U2);

        float lambda = L;
        float lambdaP;
        float sinSigma;
        float sigma;
        float cosSigma;
        float cosSqAlpha;
        float cos2SigmaM;
        float sinLambda;
        float cosLambda;
        float sinAlpha;
        int iterLimit = 100;
        do
        {
            sinLambda = Mathf.Sin(lambda);
            cosLambda = Mathf.Cos(lambda);
            sinSigma = Mathf.Sqrt((cosU2 * sinLambda) * (cosU2 * sinLambda) +
                (cosU1 * sinU2 - sinU1 * cosU2 * cosLambda) *
                (cosU1 * sinU2 - sinU1 * cosU2 * cosLambda));
            if (sinSigma == 0)
            {
                xOffset = 0.0f;
                yOffset = 0.0f;
                return;  // co-incident points
            }
            cosSigma = sinU1 * sinU2 + cosU1 * cosU2 * cosLambda;
            sigma = Mathf.Atan2(sinSigma, cosSigma);
            sinAlpha = cosU1 * cosU2 * sinLambda / sinSigma;
            cosSqAlpha = 1 - sinAlpha * sinAlpha;
            cos2SigmaM = cosSigma - 2 * sinU1 * sinU2 / cosSqAlpha;
            if (float.IsNaN(cos2SigmaM)) //isNaN
            {
                cos2SigmaM = 0;  // equatorial line: cosSqAlpha=0 (§6)
            }
            float C = f / 16 * cosSqAlpha * (4 + f * (4 - 3 * cosSqAlpha));
            lambdaP = lambda;
            lambda = L + (1 - C) * f * sinAlpha *
                (sigma + C * sinSigma * (cos2SigmaM + C * cosSigma * (-1 + 2 * cos2SigmaM * cos2SigmaM)));
        } while (Mathf.Abs(lambda - lambdaP) > 1e-12 && --iterLimit > 0);

        if (iterLimit == 0)
        {
            xOffset = 0.0f;
            yOffset = 0.0f;
            return;  // formula failed to converge
        }

        float uSq = cosSqAlpha * (a * a - b * b) / (b * b);
        float A = 1 + uSq / 16384 * (4096 + uSq * (-768 + uSq * (320 - 175 * uSq)));
        float B = uSq / 1024 * (256 + uSq * (-128 + uSq * (74 - 47 * uSq)));
        float deltaSigma = B * sinSigma * (cos2SigmaM + B / 4 * (cosSigma * (-1 + 2 * cos2SigmaM * cos2SigmaM) -
            B / 6 * cos2SigmaM * (-3 + 4 * sinSigma * sinSigma) * (-3 + 4 * cos2SigmaM * cos2SigmaM)));
        float s = b * A * (sigma - deltaSigma);

        float bearing = Mathf.Atan2(cosU2 * sinLambda, cosU1 * sinU2 - sinU1 * cosU2 * cosLambda);
        xOffset = Mathf.Sin(bearing) * s;
        yOffset = Mathf.Cos(bearing) * s;
    }
}