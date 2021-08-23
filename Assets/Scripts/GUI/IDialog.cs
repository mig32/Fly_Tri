public interface IDialog
{
    public DialogType DialogType { get; }
    public bool IsActive { get; }
    public void SetActive(bool isActive);
}
