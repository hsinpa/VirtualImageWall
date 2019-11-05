using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Utility;

public class MainApp : MonoBehaviour
{
    [SerializeField]
    private ImageCard ImageCardPrefab;

    [SerializeField]
    private StagnateWallModule stagnateWallModule;

    [SerializeField]
    private VirtualImageWallModule virtualImageWall;

    [SerializeField]
    private PrizeDrawModule prizeDrawModule;

    [SerializeField]
    private TextureUtility textureUtility;

    private FileUtility fileUtility;

    public void Start() {
        SettingData settingData = ParseSettingData();

        Debug.Log("settingData root_folder " + settingData.root_folder);
        Debug.Log("settingData card_height " + settingData.card_height);
        Debug.Log("settingData card_width " + settingData.card_width);

        SetPooling(settingData);

        if (!string.IsNullOrEmpty(settingData.root_folder)) {
            fileUtility = new FileUtility();
            fileUtility.SetTargetFolder(settingData.root_folder);

            stagnateWallModule.SetUp(textureUtility, fileUtility, settingData);
            virtualImageWall.SetUp(textureUtility, fileUtility, settingData);
            prizeDrawModule.SetUp(textureUtility, fileUtility, settingData);

            virtualImageWall.Display(true);
            prizeDrawModule.Display(false);
            stagnateWallModule.Display(false);
        }
    }

    private void SetPooling(SettingData setting) {
        ImageCardPrefab.rectTransform.sizeDelta = new Vector2(setting.card_width, setting.card_height);

        int queueLength = (setting.max_column * setting.max_row * 2);
        Pooling.PoolManager.instance.CreatePool(ImageCardPrefab.gameObject, PoolingID.ImageCard, queueLength);
    }

    private SettingData ParseSettingData() {
        string settingPath = Application.streamingAssetsPath + "/Setting.json";

        if (File.Exists(settingPath)) {
            return JsonUtility.FromJson<SettingData>( File.ReadAllText(settingPath) );
        }

        return default(SettingData);
    }

    private void Update()
    {
        if (Input.GetKeyDown(InputEvent.SwitchMode) && !stagnateWallModule.isEnable) {
            if (prizeDrawModule.isEnable) {
                prizeDrawModule.Display(false);
                virtualImageWall.Display(true);
            }
            else {
                prizeDrawModule.Display(true);
                virtualImageWall.Display(false);
            }
        }

        if (Input.GetKeyDown(InputEvent.StagnateScene) && !prizeDrawModule.isEnable) {
            if (!stagnateWallModule.isEnable)
            {
                virtualImageWall.Display(false);
                prizeDrawModule.Display(false);
                stagnateWallModule.Display(true);
            }
            else {
                virtualImageWall.Display(true);
                prizeDrawModule.Display(false);
                stagnateWallModule.Display(false);
            }
        }

    }
}
