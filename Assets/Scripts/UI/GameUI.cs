using TMPro;
using UnityEngine;
using Zenject;

public class GameUI : MonoBehaviour
{
    private GameManager m_gm;
    private PlayerLifes m_lifes;
    public TextMeshProUGUI countdownText;
    public TextMeshProUGUI levelProgressText;
    public TextMeshProUGUI playerLifesText;

    private CountdownView m_countdownView;
    private LevelProgressView m_LevelProgress;
    private PlayerLifesView m_LifesView;

    [Inject]
    void Construct(GameManager gm, PlayerLifes lifes)
    {
        m_gm = gm;
        m_lifes = lifes;
    }

    private void Awake()
    {
        m_gm.onLevelCreated += M_gm_onLevelCreated;
        m_LifesView = new PlayerLifesView(m_lifes, playerLifesText);
    }

    private void M_gm_onLevelCreated(Level obj)
    {
        m_countdownView = new CountdownView(countdownText, obj.Countdown);
        m_LevelProgress = new LevelProgressView(obj, levelProgressText);

    }
}

public class PlayerLifesView
{
    private TextMeshProUGUI m_playerLifesText;
    private PlayerLifes m_lifes;

    public PlayerLifesView(PlayerLifes lifes, TextMeshProUGUI playerLifesText)
    {
        m_playerLifesText = playerLifesText;
        m_lifes = lifes;
        lifes.onLifesChanged += Lifes_onLifesChanged;
        Lifes_onLifesChanged();
    }

    private void Lifes_onLifesChanged()
    {
        m_playerLifesText.text = $"lifes: {m_lifes.CurrentLifes}";
    }
}