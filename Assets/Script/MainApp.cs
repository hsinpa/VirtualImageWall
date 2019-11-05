using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Utility;

public class MainApp : MonoBehaviour
{

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

        if (!string.IsNullOrEmpty(settingData.root_folder)) {
            fileUtility = new FileUtility();
            fileUtility.SetTargetFolder(settingData.root_folder);


            virtualImageWall.SetUp(textureUtility, fileUtility, settingData);
            prizeDrawModule.SetUp(textureUtility, fileUtility, settingData);

            virtualImageWall.Display(true);
            prizeDrawModule.Display(false);
        }
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
        if (Input.GetKeyDown(InputEvent.SwitchMode)) {
            if (prizeDrawModule.isEnable) {
                prizeDrawModule.Display(false);
                virtualImageWall.Display(true);
            }
            else {
                prizeDrawModule.Display(true);
                virtualImageWall.Display(false);
            }
        }


        if (Input.GetKeyDown(KeyCode.Alpha4))
            Time.timeScale = 0;
    }
}
