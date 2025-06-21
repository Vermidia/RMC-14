using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Ghost;

[Serializable, NetSerializable]
public sealed class RMCPickedGhostBuiMsg : BoundUserInterfaceMessage
{
    [DataField]
    public bool Confirmed;

    public RMCPickedGhostBuiMsg(bool confirmed)
    {
        Confirmed = confirmed;
    }
}

[Serializable, NetSerializable]
public enum RMCPickedGhostUI
{
    Key
}
