using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartMenuController : MainMenuBase
{
    public override DialogType DialogType => DialogType.StartMenu;

    #region Controls
    public void OnButtonSelectLevelPressed()
    {
        DialogsController.GetInstance().ShowDialog(DialogType.MapSelectorMenu);
        SetActive(false);
    }

    public void OnButtonLoadPressed()
    {
        GameProgress.Load();
    }

    public void OnButtonResetPressed()
    {
        GameProgress.Clear();
    }

    public void OnButtonExitPressed()
    {
        Application.Quit();
    }
    #endregion
}
