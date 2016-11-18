using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AppearOnCall : MonoBehaviour
{
    public int ID = 0;
    [HideInInspector]
    public bool Called = false;
    //Possible components that can be disabled or change color
    private InputField TheInputField;
    private Button TheButton;
    private Image TheImage;
    private Text TheText;
    private Color OriginalColor;

    void Start()
    {
        if (GetComponent<InputField>() != null) TheInputField = GetComponent<InputField>();
        if (GetComponent<Button>() != null) TheButton = GetComponent<Button>();

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
        if (TheInputField != null)
        {
            if (Called) TheInputField.enabled = true;
            else TheInputField.enabled = false;
        }

        if (TheButton != null)
        {
            if (Called) TheButton.enabled = true;
            else TheButton.enabled = false;
        }

        if (TheImage != null)
        {
            if (Called) GetComponent<Image>().color = new Color(OriginalColor.r, OriginalColor.g, OriginalColor.b, OriginalColor.a);
            else GetComponent<Image>().color = new Color(OriginalColor.r, OriginalColor.g, OriginalColor.b, 0);
        }

        if (TheText != null)
        {
            if (Called) GetComponent<Text>().color = new Color(OriginalColor.r, OriginalColor.g, OriginalColor.b, OriginalColor.a);
            else GetComponent<Text>().color = new Color(OriginalColor.r, OriginalColor.g, OriginalColor.b, 0);
        }
    }
}
