using TMPro;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class MainGuiController : MonoBehaviour
{
    [SerializeField] private TMP_Text m_textScoreAmount;
    [SerializeField] private TMP_Text m_textMessage;

    private Animator m_animator;

    private readonly int ShowMessageTrigger = Animator.StringToHash("ShowMessage");

    private void Awake()
    {
        m_animator = GetComponent<Animator>();
    }

    private void Start()
    {
        m_textScoreAmount.text = "";
        m_textMessage.text = "";
        WorldControl.GetInstance().OnScoreUpdated += OnScoreUpdated;
        WorldControl.GetInstance().OnMessage += OnMessage;
    }

    private void OnScoreUpdated()
    {
        if (GameProgress.tempScore == 0)
        {
            m_textScoreAmount.text = $"{GameProgress.score}";
            return;
        }

        string plus = GameProgress.tempScore > 0 ? "+" : "";
        m_textScoreAmount.text = $"{GameProgress.score} ({plus}{GameProgress.tempScore})";
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
