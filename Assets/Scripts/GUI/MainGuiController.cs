using TMPro;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class MainGuiController : MonoBehaviour
{
    [SerializeField] private TMP_Text m_textMessage;

    private Animator m_animator;

    private readonly int ShowMessageTrigger = Animator.StringToHash("ShowMessage");

    private void Awake()
    {
        m_animator = GetComponent<Animator>();
    }

    private void Start()
    {
        m_textMessage.text = "";
        WorldControl.GetInstance().OnMessage += OnMessage;
    }

    private void OnMessage(string message)
    {
        m_animator.SetTrigger(ShowMessageTrigger);
        m_textMessage.text = message;
    }

    public void OnPauseButtonPressed()
    {
        WorldControl.GetInstance().SetPause(true);
    }
}
