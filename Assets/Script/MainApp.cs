using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MainApp : MonoBehaviour
{

    [SerializeField]
    private VirtualImageWallModule virtualImageWall;

    [SerializeField]
    private PrizeDrawModule prizeDrawModule;

    public void Start() {
        SettingData settingData = ParseSettingData();

        Debug.Log("settingData root_folder " + settingData.root_folder);
        Debug.Log("settingData card_height " + settingData.card_height);
        Debug.Log("settingData card_width " + settingData.card_width);
        Debug.Log("settingData desire_columns " + settingData.desire_columns);
        Debug.Log("settingData desire_rows " + settingData.desire_rows);

        if (!string.IsNullOrEmpty(settingData.root_folder)) {
            virtualImageWall.SetUp(settingData.card_width, settingData.card_height, settingData.desire_rows, settingData.desire_columns);
            virtualImageWall.Display(true);
        }
    }

    private SettingData ParseSettingData() {
        string settingPath = Application.streamingAssetsPath + "/Setting.json";

        if (File.Exists(settingPath)) {
            return JsonUtility.FromJson<SettingData>( File.ReadAllText(settingPath) );
        }

        return default(SettingData);
    }

}
