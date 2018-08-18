using UnityEngine;
using UnityEngine.UI;

public class TabControl : MonoBehaviour
{

    public Color32 BackgroundSelectColor;
    public Color32 BackgroundUnselectColor;
    public Color32 FontSelectedColor;
    public Color32 FontUnselectedColor;

    public GameObject ButtonMap;
    public GameObject ButtonInventory;
    public GameObject ButtonPolitics;

    public GameObject CanvasMap;
    public GameObject CanvasInventory;
    public GameObject CanvasPolitics;

    public GameObject Player;

    // Use this for initialization
    void Start()
    {
        Button buttonMap = ButtonMap.GetComponent<Button>();
        Button buttonInventory = ButtonInventory.GetComponent<Button>();
        Button buttonPolitics = ButtonPolitics.GetComponent<Button>();

        buttonMap.onClick.AddListener(OpenMap);
        buttonInventory.onClick.AddListener(OpenInventory);
        buttonPolitics.onClick.AddListener(OpenPolitics);
    }

    void OpenMap()
    {
        ButtonMap.GetComponent<Image>().color = BackgroundSelectColor;
        ButtonInventory.GetComponent<Image>().color = BackgroundUnselectColor;
        ButtonPolitics.GetComponent<Image>().color = BackgroundUnselectColor;

        ButtonMap.GetComponentInChildren<Text>().color = FontSelectedColor;
        ButtonPolitics.GetComponentInChildren<Text>().color = FontUnselectedColor;
        ButtonInventory.GetComponentInChildren<Text>().color = FontUnselectedColor;

        CanvasPolitics.SetActive(false);
        CanvasInventory.SetActive(false);
        CanvasMap.GetComponentInChildren<RawImage>().enabled = true;
        Player.SetActive(true);
        Debug.Log("Mapa!");
    }

    void OpenInventory()
    {
        ButtonInventory.GetComponent<Image>().color = BackgroundSelectColor;
        ButtonMap.GetComponent<Image>().color = BackgroundUnselectColor;
        ButtonPolitics.GetComponent<Image>().color = BackgroundUnselectColor;

        ButtonInventory.GetComponentInChildren<Text>().color = FontSelectedColor;
        ButtonMap.GetComponentInChildren<Text>().color = FontUnselectedColor;
        ButtonPolitics.GetComponentInChildren<Text>().color = FontUnselectedColor;

        Player.SetActive(false);
        CanvasPolitics.SetActive(false);
        CanvasMap.GetComponentInChildren<RawImage>().enabled = false;
        CanvasInventory.SetActive(true);
        Debug.Log("Inventário!");
    }

    void OpenPolitics()
    {
        ButtonPolitics.GetComponent<Image>().color = BackgroundSelectColor;
        ButtonMap.GetComponent<Image>().color = BackgroundUnselectColor;
        ButtonInventory.GetComponent<Image>().color = BackgroundUnselectColor;

        ButtonPolitics.GetComponentInChildren<Text>().color = FontSelectedColor;
        ButtonMap.GetComponentInChildren<Text>().color = FontUnselectedColor;
        ButtonInventory.GetComponentInChildren<Text>().color = FontUnselectedColor;

        Player.SetActive(false);
        CanvasInventory.SetActive(false);
        CanvasMap.GetComponentInChildren<RawImage>().enabled = false;
        CanvasPolitics.SetActive(true);

        Debug.Log("Políticos!");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
