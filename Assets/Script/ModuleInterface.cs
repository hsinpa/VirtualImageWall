using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ModuleInterface
{
    bool isEnable { get; }

    void Display(bool isOn);
}
