using UnityEngine;
using UnityEngine.UI;

public class TurnOverQuestion : MonoBehaviour
{
    [SerializeField] private ImmobilityStageManager stageManager;

    [SerializeField] private Button btnCorrect;
    [SerializeField] private Button btnWrong;

    [SerializeField] private Text feedbackText; // optional

    private void Awake()
    {
        if (btnCorrect) btnCorrect.onClick.AddListener(OnCorrect);
        if (btnWrong) btnWrong.onClick.AddListener(OnWrong);
    }

    private void OnEnable()
    {
        if (feedbackText) feedbackText.text = "";
    }

    private void OnCorrect()
    {
        if (feedbackText) feedbackText.text = "";
        stageManager.OnTurnOverAnsweredCorrect();
    }

    private void OnWrong()
    {
        if (feedbackText) feedbackText.text = "Wrong. Try again.";
    }
}
