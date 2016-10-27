using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AppearOnCall : MonoBehaviour
{
    public bool Called = false;
    private Image TheImage;
    private Text TheText;
    private Color OriginalColor;

    void Start()
    {
        if (GetComponent<Image>() != null)
        {
            TheImage = GetComponent<Image>();
            OriginalColor = GetComponent<Image>().color;
        }

        if (GetComponent<Text>() != null)
        {
            TheText = GetComponent<Text>();
            OriginalColor = GetComponent<Text>().color;
        }
    }

    void Update()
    {
        if (GetComponent<Image>() != null)
        {
            if (Called) GetComponent<Image>().color = new Color(OriginalColor.r, OriginalColor.g, OriginalColor.b, OriginalColor.a);
            else GetComponent<Image>().color = new Color(OriginalColor.r, OriginalColor.g, OriginalColor.b, 0);
        }

        if (GetComponent<Text>() != null)
        {
            if (Called) GetComponent<Text>().color = new Color(OriginalColor.r, OriginalColor.g, OriginalColor.b, OriginalColor.a);
            else GetComponent<Text>().color = new Color(OriginalColor.r, OriginalColor.g, OriginalColor.b, 0);
        }
    }
}
