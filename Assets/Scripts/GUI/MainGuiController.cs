using TMPro;
using UnityEngine;

public class MainGuiController : MonoBehaviour
{
    [SerializeField] private TMP_Text m_textScoreAmount;

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
}
