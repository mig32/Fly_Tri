using UnityEngine;
using UnityEngine.UI;

public abstract class MainMenuBase : BaseDialog
{
    [SerializeField] private Slider m_sliderSound;
    [SerializeField] private Slider m_sliderMusic;

    protected override void OnActivated()
    {
        m_sliderSound.value = WorldControl.GetInstance().GetFXVolume();
        m_sliderMusic.value = WorldControl.GetInstance().GetMusicVolume();
    }

    public void OnSoundValueChanged(Slider slider)
    {
        WorldControl.GetInstance().SetFXVolume(slider.value);
    }

    public void OnMusicValueChanged(Slider slider)
    {
        WorldControl.GetInstance().SetMusicVolume(slider.value);
    }
}
