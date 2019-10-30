using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;
using UnityEngine.UI;

public class VirtualImageWallModule : MonoBehaviour, ModuleInterface
{
    [SerializeField]
    private RectTransform UIHolder;

    [SerializeField]
    private ImageCard ImageCardPrefab;

    [SerializeField]
    private int CycleTime;

    [SerializeField]
    private int LineSpace;

    private Vector2 _ScreenSize;

    private CanvasGroup canvasGroup;

    private int card_width, card_height, desire_row, desire_column;

    private List<ImageCard> imagecard_list;

    private TextureUtility textureUtility;

    private FileUtility fileUtility;

    public void Display(bool isOn)
    {
        canvasGroup.alpha = (isOn) ? 1 : 0;

        if (isOn)
        {
            GenerateImageWall();

        }
    }

    public void SetUp(TextureUtility textureUtility, FileUtility fileUtility, SettingData settingData) {
        this.textureUtility = textureUtility;
        this.fileUtility = fileUtility;
        this.card_width = settingData.card_width;
        this.card_height = settingData.card_height;
        this.desire_row = settingData.desire_rows;
        this.desire_column = settingData.desire_columns;

        imagecard_list = new List<ImageCard>();
        canvasGroup = GetComponent<CanvasGroup>();

        UtilityMethod.ClearChildObject(UIHolder);
    }

    private void GenerateImageWall() {
        _ScreenSize = new Vector2(Screen.width, Screen.height );
        Vector2 cardSize = ImageCardPrefab.rectTransform.rect.size;

        Debug.Log("_ScreenSize " + _ScreenSize);

        int maxCardCol = Mathf.RoundToInt(_ScreenSize.x / cardSize.x);
        int maxCardRow = Mathf.RoundToInt(_ScreenSize.y / cardSize.y);
        float centerX = 0, centerY = 0;
        float startX = centerX - ((maxCardCol - 1) * cardSize.x * 0.5f + ((maxCardCol - 1) * LineSpace * 0.5f)),
              startY = centerY + ((maxCardRow - 1) * cardSize.y * 0.5f + ((maxCardRow - 1) * LineSpace * 0.5f));


        Debug.Log("maxCardCol " + maxCardCol + ", maxCardRow" + maxCardRow);
        Debug.Log("maxCardWidth " + (maxCardCol * cardSize.x) + ", maxCardHeight" + (maxCardRow * cardSize.y));

        for (int x = 0; x < maxCardCol; x++) {
            for (int y = 0; y < maxCardRow; y++)
            {
                ImageCard g_card = GetImageCard();
                float delayTime = Random.Range(0.1f, 1f);
                float posX = startX + (cardSize.x * x) + (x * LineSpace), posY = startY - (cardSize.y * y) - (y * LineSpace);
                FileUtility.ImageData randomImage = fileUtility.GetRandomImage();

                if (FileUtility.IsImageValid(randomImage)) {
                    //imagecard_list.Add(g_card);
                    PrintImageOnScreen(delayTime, posX, posY, randomImage, g_card);
                }
            }
        }
    }

    private void PrintImageOnScreen(float delayTime, float posX, float posY, FileUtility.ImageData imageData, ImageCard imageCard) {
        imageCard.rectTransform.anchoredPosition = new Vector2(posX, posY);
        imageCard.rawImage.enabled = false;

        textureUtility.GetTexture(imageData.url, (Texture texture) =>
        {
            imageCard.rawImage.enabled = true;
            imageCard.rawImage.texture = texture;
        });
    }

    private ImageCard GetImageCard() {
        ImageCard imageCard = UtilityMethod.CreateObjectToParent(UIHolder, ImageCardPrefab.gameObject).GetComponent<ImageCard>();

        return imageCard;
    }


    


}
