using Robust.Shared.GameStates;

namespace Content.Shared._RMC14.Xenonids.Fortify;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class ActionIfFortifiedComponent : Component
{
    [DataField, AutoNetworkedField]
    public LocId Popup = "rmc-xeno-fortify-cant";

    /// <summary>
    /// If not blocked it's required for the action to work
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool Block = true;
}
