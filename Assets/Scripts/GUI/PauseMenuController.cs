
public class PauseMenuController : MainMenuBase
{
    public override DialogType DialogType => DialogType.PauseMenu;


    #region Controls

    public void OnButtonContinuePressed()
    {
        SetActive(false);
        WorldControl.GetInstance().SetPause(false);
    }

    public void OnButtonRestartLevelPressed()
    {
        WorldControl.GetInstance().RestartLevel();
        SetActive(false);
    }

    public void OnButtonEndLevelPressed()
    {
        WorldControl.GetInstance().EndLevel();

        DialogsController.GetInstance().ShowDialog(DialogType.StartMenu);
        SetActive(false);
    }

    public void OnButtonSavePressed()
    {
        GameProgress.Save();
    }

    public void OnButtonLoadPressed()
    {
        OnButtonEndLevelPressed();
        GameProgress.Load();
    }
    #endregion
}
