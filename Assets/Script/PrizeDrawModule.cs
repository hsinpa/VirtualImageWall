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

    [SerializeField]
    private RawImage PeopleWallBG;

    private CanvasGroup canvasGroup;

    private int CompanyCount;
    private int CompanyIndex;

    private TextureUtility _textureUtility;
    private FileUtility _fileUtility;
    private SettingData _settingData;

    private bool leftFlag = false, rightFlag = false;

    public bool isEnable {get { return canvasGroup.alpha == 1; }}

    private Dictionary<string, string> CompanynameMapper;

    public void SetUp(TextureUtility textureUtility, FileUtility fileUtility, SettingData settingData)
    {
        this._textureUtility = textureUtility;
        this._fileUtility = fileUtility;
        this._settingData = settingData;

        CompanyIndex = 0;
        canvasGroup = GetComponent<CanvasGroup>();

        LeftImageCard.draw_phase.gameObject.SetActive(true);
        LeftImageCard.background.enabled = true;

        RightImageCard.draw_phase.gameObject.SetActive(true);
        RightImageCard.background.enabled = true;

        LeftImageCard.Reset();
        RightImageCard.Reset();

        _textureUtility.GetTexture(settingData.lucky_draw_background, (Sprite t) =>
        {
            PeopleWallBG.texture = t.texture;
        });
         
        CompanynameMapper = new Dictionary<string, string>() {
            {EventFlag.CompanyMap.C1_ID,  EventFlag.CompanyMap.C1_Fullname},
            {EventFlag.CompanyMap.C2_ID,  EventFlag.CompanyMap.C2_Fullname},
            {EventFlag.CompanyMap.C3_ID,  EventFlag.CompanyMap.C3_Fullname}
        };
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

        //if (CompanyIndex >= CompanyCount)
        //    CompanyIndex = 0;

        //if (CompanyIndex < 0)
        //    CompanyIndex = CompanyCount - 1;

        CompanyIndex = Mathf.Clamp(CompanyIndex, 0, CompanyCount-1);

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

            int total_generate_num = _settingData.draw_flip_num;
            float period = _settingData.draw_flip_period;

            float totalTime = (total_generate_num * period) + period;

            RandomGenerateImage(imageCard, key, total_generate_num, period, 0);

            _ = UtilityMethod.DoDelayWork(totalTime, () =>
            {
                SetImageCard(imageCard, picked_imageData);
                callback();
            });
        } else
        {
            callback();
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
        CompanyNameText.text = GetCompanyFullName(key);
    }

    private void SetImageCard(ImageCard imageCard, FileUtility.ImageData imageData) {
        _textureUtility.GetTexture(imageData.url, (Sprite texture) =>
        {
            if (texture == null)
                return;
            imageCard.rawImage.preserveAspect = true;
            imageCard.rawImage.enabled = true;
            imageCard.rawImage.sprite = texture;
        });

        imageCard.company_title.text = GetCompanyFullName(imageData.company_name);
        imageCard.personnel_title.text = imageData.title_name;
    }

    private string GetCompanyFullName(string id) {
        if (CompanynameMapper.TryGetValue(id, out string c_name)) {
            return c_name;
        }

        return id;
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
