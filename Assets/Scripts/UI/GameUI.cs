using TMPro;
using UnityEngine;
using Zenject;

public class GameUI : MonoBehaviour
{
    private GameManager m_gm;
    public TextMeshProUGUI countdownText;
    public TextMeshProUGUI levelProgressText;
    private CountdownView m_countdownView;
    private LevelProgressView m_LevelProgress;

    [Inject]
    void Construct(GameManager gm)
    {
        m_gm = gm;
    }

    private void Awake()
    {
        m_gm.onLevelCreated += M_gm_onLevelCreated;
    }

    private void M_gm_onLevelCreated(Level obj)
    {
        m_countdownView = new CountdownView(countdownText, obj.Countdown);
        m_LevelProgress = new LevelProgressView(obj, levelProgressText);
    }
}
