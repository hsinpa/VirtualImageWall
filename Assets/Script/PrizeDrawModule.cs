using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;
using UnityEngine.UI;
using DG.Tweening;

public class PrizeDrawModule : MonoBehaviour, ModuleInterface
{
    [SerializeField]
    private ImageCard LeftImageCard;

    [SerializeField]
    private ImageCard RightImageCard;

    [SerializeField]
    private Text CompanyNameText;

    [SerializeField, Range(1, 5)]
    private float DrawTime = 4;

    private CanvasGroup canvasGroup;

    private int CompanyCount;
    private int CompanyIndex;

    private TextureUtility _textureUtility;
    private FileUtility _fileUtility;
    private SettingData _settingData;

    private bool leftFlag = false, rightFlag = false;

    public bool isEnable {get { return canvasGroup.alpha == 1; }}

    public void SetUp(TextureUtility textureUtility, FileUtility fileUtility, SettingData settingData)
    {
        this._textureUtility = textureUtility;
        this._fileUtility = fileUtility;
        this._settingData = settingData;

        CompanyIndex = 0;
        canvasGroup = GetComponent<CanvasGroup>();

        LeftImageCard.draw_phase.gameObject.SetActive(true);
        RightImageCard.draw_phase.gameObject.SetActive(true);

        LeftImageCard.Reset();
        RightImageCard.Reset();
    }

    public void Display(bool isOn)
    {
        canvasGroup.alpha = (isOn) ? 1 : 0;

        CompanyCount = this._fileUtility.CompanyCount;

        SetCompanyInfo(CompanyIndex);

        leftFlag = false;
        rightFlag = false;
    }

    private void ChangeCompany(int direction) {
        CompanyIndex += direction;

        if (CompanyIndex >= CompanyCount)
            CompanyIndex = 0;

        if (CompanyIndex < 0)
            CompanyIndex = CompanyCount - 1;

        SetCompanyInfo(CompanyIndex);
    }

    public void DrawAll() {
        //Refresh Image Libarary
        this._fileUtility.LoadAllImagesFromFolder();
        CompanyCount = this._fileUtility.CompanyCount;

        DrawLeft();
        DrawRight();
    }

    public void DrawLeft() {
        if (leftFlag)
            return;

        leftFlag = true;
        DrawSingleItem(LeftImageCard, () => leftFlag = false);
    }

    public void DrawRight() {
        if (rightFlag)
            return;

        rightFlag = true;
        DrawSingleItem(RightImageCard, () => rightFlag = false);
    }

    private void DrawSingleItem(ImageCard imageCard, System.Action callback) {
        string key = this._fileUtility.GetCompanykeyByIndex(CompanyIndex);
        FileUtility.ImageData picked_imageData = this._fileUtility.GetRandomImageByTag(key, true);

        if (FileUtility.IsImageValid(picked_imageData))
        {
            _fileUtility.MarkPrizeDrawPrivilege(picked_imageData, true);

            int total_generate_num = 15;
            float period = 0.15f;
            float totalTime = (total_generate_num * period) + period;

            RandomGenerateImage(imageCard, key, total_generate_num, period, 0);

            _ = UtilityMethod.DoDelayWork(totalTime, () =>
            {
                SetImageCard(imageCard, picked_imageData);
                callback();
            });
        }
    }

    private void RandomGenerateImage(ImageCard imageCard, string company_key, int total_generate_num, float period, int current_generate_num) {
        if (current_generate_num >= total_generate_num)
            return;

        FileUtility.ImageData imageData = this._fileUtility.GetRandomImageByTag(company_key, false);

        if (FileUtility.IsImageValid(imageData))
        {
            SetImageCard(imageCard, imageData);

            current_generate_num += 1;

            _ = UtilityMethod.DoDelayWork(period, () =>
            {
                RandomGenerateImage(imageCard, company_key, total_generate_num, period, current_generate_num);
            });
        }
    }

    private void SetCompanyInfo(int company_index) {

        string key = this._fileUtility.GetCompanykeyByIndex(company_index);
        CompanyNameText.text = key;
    }

    private void SetImageCard(ImageCard imageCard, FileUtility.ImageData imageData) {
        _textureUtility.GetTexture(imageData.url, (Texture texture) =>
        {
            if (texture == null)
                return;

            imageCard.rawImage.enabled = true;
            imageCard.rawImage.texture = texture;
        });

        imageCard.company_title.text = imageData.company_name;
        imageCard.personnel_title.text = imageData.title_name;
    }
    #region Input Event

    private void Update()
    {
        if (!isEnable)
            return;

        if (Input.GetKeyDown(InputEvent.DrawAll)) {
            DrawAll();
        }

        if (Input.GetKeyDown(InputEvent.DrawLeft))
        {
            DrawLeft();
        }

        if (Input.GetKeyDown(InputEvent.DrawRight))
        {
            DrawRight();
        }

        if (Input.GetKeyDown(InputEvent.ChangeCompanyLeft))
        {
            ChangeCompany(-1);
        }

        if (Input.GetKeyDown(InputEvent.ChangeCompanyRight))
        {
            ChangeCompany(1);
        }
    }

    #endregion



}
