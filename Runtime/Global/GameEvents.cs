public static class GameEvents
{
    public static System.Action<bool> OnTogglePause;
    public static System.Action OnFOVChanged;

    public static void TriggerTogglePause(bool isPaused)
    {
        OnTogglePause?.Invoke(isPaused);
    }

    public static void TriggerFOVChanged()
    {
        OnFOVChanged?.Invoke();
    }
}