using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;
using UnityEngine.UI;
using DG.Tweening;

public class StaticBackgroundModule : MonoBehaviour, ModuleInterface
{
    private CanvasGroup canvasGroup;

    public bool isEnable { get { return canvasGroup.alpha == 1; } }

    public void Display(bool isOn)
    {
        canvasGroup.alpha = (isOn) ? 1 : 0;

    }

    public void SetUp() {
        canvasGroup = GetComponent<CanvasGroup>();
    }

}
