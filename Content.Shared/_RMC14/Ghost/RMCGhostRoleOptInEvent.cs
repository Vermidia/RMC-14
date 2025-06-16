using Content.Shared.Actions;

namespace Content.Shared._RMC14.Ghost;

public sealed partial class RMCGhostRoleOptInActionEvent : InstantActionEvent
{
    [DataField]
    public EntityUid? Source;
}
