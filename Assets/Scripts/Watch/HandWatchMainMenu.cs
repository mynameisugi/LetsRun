using UnityEngine;
using static HandWatchController;

public class HandWatchMainMenu : MonoBehaviour
{
    [SerializeField]
    private HandWatchController controller;

    public void OnClockEnter()
        => controller.RequestModeSwitch(Mode.Clock);

    public void OnHorseEnter()
        => controller.RequestModeSwitch(Mode.Horse);

    public void OnInventoryEnter()
        => controller.RequestModeSwitch(Mode.Inventory);

    public void OnSettingEnter()
        => controller.RequestModeSwitch(Mode.Setting);

    public void OnMapEnter()
        => controller.RequestModeSwitch(Mode.Map);

    public void OnTutorialEnter()
        => controller.RequestModeSwitch(Mode.Tutorial);
}
