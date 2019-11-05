using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;
using UnityEngine.UI;
using DG.Tweening;
using Pooling;
using System.Threading.Tasks;

public class StagnateWallModule : BaseImageWallModule, ModuleInterface
{

    public override void SetUp(TextureUtility textureUtility, FileUtility fileUtility, SettingData settingData)
    {
        base.SetUp(textureUtility, fileUtility, settingData);

        this.CycleTime = 1;
        this.Looping = false;
        this.AnimationTriggerKey = EventFlag.Animation.Stagnate;
    }

}
