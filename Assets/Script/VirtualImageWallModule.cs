using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;
using UnityEngine.UI;
using DG.Tweening;
using Pooling;
using System.Threading.Tasks;

public class VirtualImageWallModule : MonoBehaviour, ModuleInterface
{
    RectTransform rectTransform;

    [SerializeField]
    private RectTransform UIHolder;

    [SerializeField]
    private ImageCard ImageCardPrefab;

    private int CycleTime;

    private int LineSpace, MaxColumn, MaxRow;

    [SerializeField]
    private Animator logo_holo_animator;

    [SerializeField]
    private float fadeInOutTime = 0.4f;

    [SerializeField, Range(0.1f, 1)]
    private float maxPerImageDelayTime = 0.8f;

    private Vector2 _ScreenSize;

    private CanvasGroup canvasGroup;

    private int card_width, card_height;

    private List<ImageCard> imagecard_list;

    private TextureUtility textureUtility;

    private FileUtility fileUtility;

    private System.Action EventImageWallShowComplete;

    public bool isEnable { get { return canvasGroup.alpha == 1; } }

    System.Threading.CancellationTokenSource cancelToken;

    public void Display(bool isOn)
    {
        canvasGroup.alpha = (isOn) ? 1 : 0;

        if (isOn)
        {
            cancelToken = new System.Threading.CancellationTokenSource();

            logo_holo_animator.SetTrigger(EventFlag.Animation.Reset);

            EventImageWallShowComplete += OnImageWallShowComplete;

            GenerateImageWall();
        }
        else {
            logo_holo_animator.SetTrigger(EventFlag.Animation.Reset);
            Debug.Log("Reset");
            cancelToken.Cancel();
            cancelToken.Dispose();
            cancelToken = null;

            EventImageWallShowComplete -= OnImageWallShowComplete;
        }
    }

    public void SetUp(TextureUtility textureUtility, FileUtility fileUtility, SettingData settingData) {
        rectTransform = GetComponent<RectTransform>();
        _ScreenSize = new Vector2(rectTransform.rect.width, rectTransform.rect.height);

        this.textureUtility = textureUtility;
        this.fileUtility = fileUtility;
        this.card_width = settingData.card_width;
        this.card_height = settingData.card_height;
        this.CycleTime = settingData.image_wall_cycle_time;
        this.LineSpace = settingData.image_wall_space;
        this.MaxColumn = settingData.max_column;
        this.MaxRow = settingData.max_row;
        //this.LineSpace = 0;

        Debug.Log("rectTransform " + rectTransform.sizeDelta);
        Debug.Log("rectTransform " + rectTransform.rect);

        ImageCardPrefab.rectTransform.sizeDelta = new Vector2(this.card_width, this.card_height);

        imagecard_list = new List<ImageCard>();
        canvasGroup = GetComponent<CanvasGroup>();

        int queueLength = 150;
        PoolManager.instance.CreatePool(ImageCardPrefab.gameObject, PoolingID.ImageCard, queueLength);

        UtilityMethod.ClearChildObject(UIHolder);
    }

    private void GenerateImageWall() {

        if (!isEnable)
            return;

        //Find if any new image is being added
        fileUtility.LoadAllImagesFromFolder();

        //_ScreenSize = new Vector2(Screen.width, Screen.height );
        Vector2 cardSize = new Vector2(card_width, card_height);

        Debug.Log("_ScreenSize " + _ScreenSize);

        int maxCardCol = Mathf.FloorToInt((_ScreenSize.x + LineSpace )/ cardSize.x) -1;
        int maxCardRow = Mathf.FloorToInt((_ScreenSize.y + LineSpace) / cardSize.y);

        maxCardCol = Mathf.Clamp(maxCardCol, maxCardCol, this.MaxColumn);
        maxCardRow = Mathf.Clamp(maxCardRow, maxCardRow, this.MaxRow);

        float centerX = 0, centerY = 0;
        float startX = centerX - ((maxCardCol - 1) * cardSize.x * 0.5f + ((maxCardCol - 1) * LineSpace * 0.5f)),
              startY = centerY + ((maxCardRow - 1) * cardSize.y * 0.5f + ((maxCardRow - 1) * LineSpace * 0.5f));

        Debug.Log("maxCardCol " + maxCardCol + ", maxCardRow" + maxCardRow);
        Debug.Log("maxCardWidth " + (maxCardCol * cardSize.x) + ", maxCardHeight" + (maxCardRow * cardSize.y));

        for (int x = 0; x < maxCardCol; x++) {
            for (int y = 0; y < maxCardRow; y++)
            {
                ImageCard g_card = GetImageCard();
                float delayTime = Random.Range(0.1f, maxPerImageDelayTime);
                float posX = startX + (cardSize.x * x) + (x * LineSpace), posY = startY - (cardSize.y * y) - (y * LineSpace);
                FileUtility.ImageData randomImage = fileUtility.GetRandomImage(true);

                if (FileUtility.IsImageValid(randomImage)) {
                    //imagecard_list.Add(g_card);
                    g_card.imageData = randomImage;
                    PrintImageOnScreen(delayTime, posX, posY, randomImage, g_card);
                }
            }
        }

        var task = UtilityMethod.DoDelayWork(maxPerImageDelayTime + 0.1f, () =>
        {
            OnImageWallShowComplete();
        }, cancelToken.Token);
    }

    private async void PrintImageOnScreen(float delayTime, float posX, float posY, FileUtility.ImageData imageData, ImageCard imageCard) {
        imageCard.rectTransform.anchoredPosition = new Vector2(posX, posY);
        imageCard.rawImage.enabled = false;
        imageCard.rawImage.color = new Color(1, 1, 1, 0);

        await UtilityMethod.DoDelayWork(delayTime, () =>
        {
            textureUtility.GetTexture(imageData.url, (Texture texture) =>
            {
                if (imageCard == null) return;
                imageCard.rawImage.enabled = true;
                imageCard.rawImage.texture = texture;

                imageCard.rawImage.DOFade(1, fadeInOutTime);

                fileUtility.MarkImageVisibility(imageCard.imageData, false);
            });
        }, cancelToken.Token); 
    }

    private void FadeOutImages() {
        if (!isEnable)
            return;

        ImageCard[] images = transform.GetComponentsInChildren<ImageCard>();
        int imageLength = images.Length;

        for (int i = 0; i < imageLength; i++) {
            float delayTime = Random.Range(0.1f, maxPerImageDelayTime);
            FadeSingleImage(delayTime, images[i]);
        }

        //Call Brink animation
        _ = UtilityMethod.DoDelayWork(maxPerImageDelayTime, () =>
        {
            logo_holo_animator.SetTrigger(EventFlag.Animation.Blink);
        }, cancelToken.Token);

        //Call Regenerate image wall
        int animationTime = 2;
        _ = UtilityMethod.DoDelayWork(animationTime + maxPerImageDelayTime, () =>
        {
            GenerateImageWall();
        }, cancelToken.Token);
    }

    private async void FadeSingleImage(float delayTime, ImageCard imageCard) {

        Vector2 targetPos = Vector2.zero;

        await UtilityMethod.DoDelayWork(delayTime, () =>
        {
            imageCard.DORestart();
            imageCard.rawImage.DOFade(0, fadeInOutTime);
            imageCard.rectTransform.DOAnchorPos(targetPos, fadeInOutTime).OnComplete(()=> {
                imageCard.rawImage.enabled = false;

                PoolManager.instance.Destroy(imageCard.gameObject);
            });
        }, cancelToken.Token);
    }

    private ImageCard GetImageCard() {
        ImageCard imageCard = PoolManager.instance.ReuseObject(PoolingID.ImageCard).GetComponent<ImageCard>();
        
        UtilityMethod.InsertObjectToParent(transform, imageCard.gameObject);

        return imageCard;
    }

    private void OnImageWallShowComplete() {
        if (!isEnable)
            return;

        _ = UtilityMethod.DoDelayWork(CycleTime, () =>
        {
            FadeOutImages();
        }, cancelToken.Token);
    }
}
