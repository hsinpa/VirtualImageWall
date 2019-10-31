using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SettingData
{
    public string root_folder;

    public int image_wall_cycle_time;
    public int image_wall_space;

    public int card_width;
    public int card_height;

    public float draw_flip_period;
    public int draw_flip_num;
}
