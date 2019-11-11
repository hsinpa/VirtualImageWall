using UnityEngine;
using UnityEngine.UI;

public class ImageCard : MonoBehaviour
{
    public RectTransform rectTransform;

    public Image rawImage;

    public Image background;

    public Image draw_phase;

    public Text personnel_title;

    public Text company_title;

    public Utility.FileUtility.ImageData imageData;

    public void Reset()
    {
        rawImage.enabled = false;
        personnel_title.text = "";
        company_title.text = "";
    }
}
