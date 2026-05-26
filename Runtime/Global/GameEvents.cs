public static class GameEvents
{
    public static System.Action<bool> OnTogglePause;
    public static System.Action<bool> OnToggleTab;
    public static System.Action OnFOVChanged;
    public static System.Action<float, string> OnObjectPaintingCompleted;
    public static System.Action<float> OnCurrentMoneyChanged;

    public static void TriggerTogglePause(bool isPaused)
    {
        OnTogglePause?.Invoke(isPaused);
    }

    public static void TriggerFOVChanged()
    {
        OnFOVChanged?.Invoke();
    }

    public static void TriggerToggleTab(bool isTabOpen)
    {
        OnToggleTab?.Invoke(isTabOpen);
    }

    public static void TriggerObjectPaintingCompleted(float cashReward, string rewardStatID)
    {
        OnObjectPaintingCompleted?.Invoke(cashReward, rewardStatID);
    }

    public static void TriggerCurrentMoneyChanged(float newAmount)
    {
        OnCurrentMoneyChanged?.Invoke(newAmount);
    }
}