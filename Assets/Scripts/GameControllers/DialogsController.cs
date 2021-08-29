using System.Linq;
using UnityEngine;

public class DialogsController : MonoBehaviour
{
    private IDialog[] m_dialogs;

    private static DialogsController m_instance = null;

    public static DialogsController GetInstance()
    {
        Debug.Assert(m_instance != null, "DialogsController used before initialized");
        return m_instance;
    }

    private void Awake()
    {
        if (m_instance != null)
        {
            Debug.Assert(m_instance == this, "Second DialogsController insance");
            return;
        }

        m_instance = this;
    }

    private void Start()
    {
        m_dialogs = GetComponentsInChildren<IDialog>();
        foreach (var dialog in m_dialogs)
        {
            dialog.SetActive(dialog.DialogType == DialogType.StartMenu);
        }
    }

    private void OnDestroy()
    {
        if (m_instance == this)
        {
            m_instance = null;
        }
    }

    public IDialog ShowDialog(DialogType type)
    {
        var dialog = m_dialogs.FirstOrDefault(dialog => dialog.DialogType == type);
        if (dialog == null)
        {
            Debug.Assert(false, $"Dialog {type} not found");
            return null;
        }

        dialog.SetActive(true);
        return dialog;
    }

    public void CloseAllDialogs()
    {
        foreach (var dialog in m_dialogs)
        {
            if (dialog.IsActive)
            {
                dialog.SetActive(false);
            }
        }
    }

    public void ShowMapDescriptionMenu(int mapIdx)
    {
        var dialog = ShowDialog(DialogType.MapDescriptionMenu);
        if (dialog != null)
        {
            var mapDescription = (MapDescriptionMenuController)dialog;
            mapDescription.SetMapIdx(mapIdx);
        }
    }
}
