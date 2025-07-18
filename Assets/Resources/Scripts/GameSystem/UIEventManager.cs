using UnityEngine.Events;

public static class UIEventManager
{
    public static UnityEvent OnOpenMainMenu = new UnityEvent();
    public static UnityEvent OnCloseMainMenu = new UnityEvent();

    public static UnityEvent OnOpenPauseMenu = new UnityEvent();
    public static UnityEvent OnClosePauseMenu = new UnityEvent();

    public static UnityEvent OnOpenSettingsMenu = new UnityEvent();
    public static UnityEvent OnCloseSettingsMenu = new UnityEvent();

    public static UnityEvent OnSettingSaved = new UnityEvent();

    public static UnityEvent OnQuitToMainMenu = new UnityEvent();
}
