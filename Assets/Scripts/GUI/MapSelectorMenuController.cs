using UnityEngine;
using UnityEngine.UI;

public class MapSelectorMenuController : BaseDialog
{
    [SerializeField] private MapSelectorItemController m_mapItemPrefab;
    [SerializeField] private ScrollRect m_scroll;

    public override DialogType DialogType => DialogType.MapSelectorMenu;

    private void Start()
    {
        UpdateScrollContent();
    }

    private void UpdateScrollContent()
    {
        ClearScroll();

        var mapList = WorldControl.GetInstance().MapList;
        for (var i = 0; i < mapList.Count; ++i)
        {
            var itemController = Instantiate(m_mapItemPrefab, m_scroll.content);
            if (itemController.InitWithMapIdx(i))
            {
                itemController.OnPressed += OnMapSelected;
            }
            else
            {
                Destroy(itemController.gameObject);
            }
        }
    }

    private void ClearScroll()
    {
        GameObject[] allChildren = new GameObject[m_scroll.content.childCount];
        var i = 0;
        foreach (RectTransform child in m_scroll.content)
        {
            allChildren[i] = child.gameObject;
            ++i;
        }

        foreach (GameObject child in allChildren)
        {
            Destroy(child);
        }
    }

    private void OnMapSelected(int mapIdx)
    {
        var dialog = DialogsController.GetInstance().ShowDialog(DialogType.MapDescriptionMenu);
        if (dialog != null)
        {
            var mapDescription = (MapDescriptionMenuController)dialog;
            mapDescription.SetMapIdx(mapIdx);
        }

        SetActive(false);
    }

    public void OnButtonBackPressed()
    {
        DialogsController.GetInstance().ShowDialog(DialogType.StartMenu);
        SetActive(false);
    }
}
