using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapDescriptionMenuController : BaseDialog
{
    [SerializeField] private Image m_imageIcon;
    [SerializeField] private Image m_imageMiniMap;
    [SerializeField] private TMP_Text m_textMapName;
    [SerializeField] private string m_stringGravity;
    [SerializeField] private TMP_Text m_textMapGravity;
    [SerializeField] private string m_stringDrag;
    [SerializeField] private TMP_Text m_textMapDrag;
    [SerializeField] private string m_strinFriction;
    [SerializeField] private TMP_Text m_textMapFriction;
    [SerializeField] private string m_strinBounciness;
    [SerializeField] private TMP_Text m_textMapBounciness;
    [SerializeField] private TMP_Text m_textMapDescription;

    private int m_mapIdx = -1;

    public override DialogType DialogType => DialogType.MapDescriptionMenu;

    public void SetMapIdx(int mapIdx)
    {
        m_mapIdx = mapIdx;

        var mapPaths = WorldControl.GetInstance().MapList.GetMapPaths(mapIdx);
        if (mapPaths == null)
        {
            SetActive(false);
            return;
        }

        var mapInfo = Resources.Load<MapInfo>(mapPaths.PrefabPath);

        m_imageIcon.sprite = mapInfo.m_mapIcon;
        m_imageMiniMap.sprite = mapInfo.m_miniImage;
        m_textMapName.text = mapInfo.m_mapName;
        m_textMapDescription.text = mapInfo.m_mapDiscription;
        m_textMapGravity.text = string.Format(m_stringGravity, mapInfo.m_mapGravity);
        m_textMapDrag.text = string.Format(m_stringDrag, mapInfo.m_mapDrag);
        m_textMapFriction.text = string.Format(m_strinFriction, mapInfo.m_mapFriction);
        m_textMapBounciness.text = string.Format(m_strinBounciness, mapInfo.m_mapBounciness);
    }

    public void OnButtonStartPressed()
    {
        if (WorldControl.GetInstance() == null)
        {
            return;
        }

        WorldControl.GetInstance().LoadLevel(m_mapIdx);
        SetActive(false);
    }

    public void OnButtonBackPressed()
    {
        DialogsController.GetInstance().ShowDialog(DialogType.MapSelectorMenu);
        SetActive(false);
    }
}
