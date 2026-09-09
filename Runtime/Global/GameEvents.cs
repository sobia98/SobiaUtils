public static class GameEvents
{
    public static System.Action<bool> OnTogglePause;
    public static System.Action<bool> OnToggleTab;
    public static System.Action OnFOVChanged;
    public static System.Action<short, string> OnObjectPaintingCompleted;
    public static System.Action<short> OnCurrentMoneyChanged;
    public static System.Action<float> OnPlayerMovementSpeedChanged;
    public static System.Action<float> OnJumpCooldownChanged;
    public static System.Action<bool> OnSprintingChanged;
    public static System.Action<bool> OnJumpingChanged;

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

    public static void TriggerObjectPaintingCompleted(short cashReward, string rewardStatID)
    {
        OnObjectPaintingCompleted?.Invoke(cashReward, rewardStatID);
    }

    public static void TriggerCurrentMoneyChanged(short newAmount)
    {
        OnCurrentMoneyChanged?.Invoke(newAmount);
    }

    public static void TriggerPlayerMovementSpeedChanged(float newSpeedMultiplier)
    {
        OnPlayerMovementSpeedChanged?.Invoke(newSpeedMultiplier);
    }

    public static void TriggerJumpCooldownChanged(float newCooldown)
    {
        OnJumpCooldownChanged?.Invoke(newCooldown);
    }

    public static void TriggerSprintingChanged(bool isSprinting)
    {
        OnSprintingChanged?.Invoke(isSprinting);
    }

    public static void TriggerJumpChanged(bool isJumping)
    {
        OnJumpingChanged?.Invoke(isJumping);
    }
}