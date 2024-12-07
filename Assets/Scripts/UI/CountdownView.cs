using TMPro;

public class CountdownView
{
    private TextMeshProUGUI uiText;
    private Countdown m_countdown;

    public CountdownView(TextMeshProUGUI countdownText, Countdown countdown)
    {
        uiText = countdownText;
        m_countdown = countdown;

        Bind(countdown);
    }

    public void Bind(Countdown countdown)
    {
        if (m_countdown != null)
            countdown.onTimeChanged -= Countdown_onTimeChanged;

        m_countdown = countdown;
        countdown.onTimeChanged += Countdown_onTimeChanged;
    }

    private void Countdown_onTimeChanged()
    {
        uiText.text = (m_countdown.duration - m_countdown.currentTime).ToString("0.0");
    }
}
