using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartMenuController : BaseDialog
{
    [SerializeField] private List<Button> m_inGameMenuButtons;
    [SerializeField] private List<Button> m_startScreenButtons;
    [SerializeField] private Slider m_sliderSound;
    [SerializeField] private Slider m_sliderMusic;

    public override DialogType DialogType => DialogType.StartMenu;

    protected override void OnActivated()
    {
        m_sliderSound.value = WorldControl.GetInstance().GetFXVolume();
        m_sliderMusic.value = WorldControl.GetInstance().GetMusicVolume();
    }

    public void SetIngameMode(bool isIngame)
    {
        m_startScreenButtons.ForEach(button => button.enabled = !isIngame);
        m_inGameMenuButtons.ForEach(button => button.enabled = isIngame);
    }

    #region Controls
    public void OnButtonSelectLevelPressed()
    {
        DialogsController.GetInstance().ShowDialog(DialogType.MapSelectorMenu);
        SetActive(false);
    }

    public void OnButtonContinuePressed()
    {
        SetActive(false);
    }

    public void OnButtonRestartLevelPressed()
    {
        WorldControl.GetInstance().RestartLevel();
        SetActive(false);
    }

    public void OnButtonEndLevelPressed()
    {
        WorldControl.GetInstance().EndLevel();
        WorldControl.GetInstance().StopTimer();

        DialogsController.GetInstance().ShowDialog(DialogType.MapDescriptionMenu);
        SetActive(false);
    }

    public void OnButtonSavePressed()
    {
        GameProgress.Save();
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

    public void OnSoundValueChanged(Slider slider)
    {
        WorldControl.GetInstance().SetFXVolume(slider.value);
    }

    public void OnMusicValueChanged(Slider slider)
    {
        WorldControl.GetInstance().SetMusicVolume(slider.value);
    }
    #endregion
}
