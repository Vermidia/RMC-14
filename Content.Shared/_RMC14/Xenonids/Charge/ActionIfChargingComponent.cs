using Robust.Shared.GameStates;

namespace Content.Shared._RMC14.Xenonids.Charge;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class ActionIfChargingComponent : Component
{
    [DataField, AutoNetworkedField]
    public LocId Popup = "rmc-xeno-toggle-charge-cant";

    /// <summary>
    /// If not blocked it's required for the action to work
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool Block = true;
}
