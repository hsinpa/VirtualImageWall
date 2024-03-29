﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;
using UnityEngine.UI;
using DG.Tweening;
using Pooling;
using System.Threading.Tasks;

public class VirtualImageWallModule : BaseImageWallModule, ModuleInterface
{
    public override void SetUp(TextureUtility textureUtility, FileUtility fileUtility, SettingData settingData)
    {
        base.SetUp(textureUtility, fileUtility, settingData);
        this.Looping = true;
        this.AnimationTriggerKey = EventFlag.Animation.Blink;
    }
}
