using TMPro;

public class LevelProgressView
{
    private Level m_Level;
    private TextMeshProUGUI m_text;

    public LevelProgressView(Level level, TextMeshProUGUI text)
    {
        m_Level = level;
        m_text = text;
        m_Level.onLevelProgressChanged += M_Level_onLevelProgressChanged1;
        M_Level_onLevelProgressChanged1(0);
    }

    private void M_Level_onLevelProgressChanged1(float obj)
    {
        m_text.text = $"{(m_Level.GetProgressPercent()).ToString("0.0")} %";
    }
}
