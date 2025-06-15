using Robust.Shared.GameStates;

namespace Content.Shared._RMC14.Ghost;

[RegisterComponent, NetworkedComponent]
public sealed partial class RMCGhostRoleVotePickerXenoComponent : Component
{
    [DataField]
    public bool IncludeQueen = false;

    [DataField]
    public bool IncludeSlotless = false;
}
