using UnityEngine;
using UnityEngine.UI;

public static class MapsHelper
{
    public static Sprite GetDefaultIcon()
    {
        return Resources.Load<Sprite>("Images/Default_map_icon");
    }

    public static Sprite GetDefaultMiniMap()
    {
        return Resources.Load<Sprite>("Images/Default_mini_map");
    }
}
