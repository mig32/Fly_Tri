using UnityEngine;

public abstract class BaseDialog : MonoBehaviour, IDialog
{
    public bool IsActive => gameObject.activeSelf;
    public void SetActive(bool isActive)
    {
        if (IsActive != isActive)
        {
            gameObject.SetActive(isActive);
        }

        if (isActive)
        {
            transform.SetAsLastSibling();
            OnActivated();
        }
    }

    public virtual DialogType DialogType { get; }
    protected virtual void OnActivated()
    {
    }
}
