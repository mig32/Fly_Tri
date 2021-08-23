using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapSelectorItemController : MonoBehaviour
{
    [SerializeField] private TMP_Text m_textMapName;
    [SerializeField] private Image m_imageMapIcon;

    private int m_mapIdx;

    public event Action<int> OnPressed;

    public bool InitWithMapIdx(int mapIdx)
    {
        m_mapIdx = mapIdx;
        var mapPaths = WorldControl.GetInstance().MapList.GetMapPaths(mapIdx);
        if (mapPaths == null)
        {
            m_textMapName.text = $"MISSING {mapIdx}";
            SetIconSprite(MapsHelper.GetDefaultIcon());
            return false;
        }

        m_textMapName.text = mapPaths.Name;
        SetIconSprite(mapPaths.GetMapIcon());
        return true;
    }

    private void SetIconSprite(Sprite sprite)
    {
        m_imageMapIcon.sprite = sprite;
        m_imageMapIcon.SetNativeSize();
    }

    public void OnButtonPressed()
    {
        OnPressed?.Invoke(m_mapIdx);
    }
}
