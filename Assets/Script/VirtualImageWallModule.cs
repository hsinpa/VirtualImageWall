using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

public class VirtualImageWallModule : MonoBehaviour, ModuleInterface
{
    [SerializeField]
    private RectTransform UIHolder;

    [SerializeField]
    private GameObject ImageCardPrefab;

    [SerializeField]
    private int CycleTime;

    private Vector2 _ScreenSize;

    private CanvasGroup canvasGroup;

    private int card_width, card_height, desire_row, desire_column;

    private List<ImageCard> imagecard_list;

    public void Display(bool isOn)
    {
        canvasGroup.alpha = (isOn) ? 1 : 0;

        if (isOn)
        {
            GenerateImageWall();

        }
    }

    public void SetUp(int card_width, int card_height, int desire_row, int desire_column) {
        this.card_width = card_width;
        this.card_height = card_height;
        this.desire_row = desire_row;
        this.desire_column = desire_column;

        imagecard_list = new List<ImageCard>();
        canvasGroup = GetComponent<CanvasGroup>();

        UtilityMethod.ClearChildObject(UIHolder);
    }

    private void GenerateImageWall() {
        _ScreenSize = new Vector2(Screen.width, Screen.height );

        float height = Camera.main.orthographicSize * 2.0f;
        float width = height * Screen.width / Screen.height;

        Debug.Log("_ScreenSize " + _ScreenSize);

        ImageCard firstCard = GetImageCard();
        firstCard.rectTransform.position = new Vector3(_ScreenSize.x, _ScreenSize.y, 0);

        ImageCard secondCard = GetImageCard();
        secondCard.rectTransform.anchoredPosition = new Vector2(_ScreenSize.x /2, 0);
    }

    private ImageCard GetImageCard() {
        ImageCard imageCard = UtilityMethod.CreateObjectToParent(UIHolder, ImageCardPrefab).GetComponent<ImageCard>();

        return imageCard;
    }


    


}
