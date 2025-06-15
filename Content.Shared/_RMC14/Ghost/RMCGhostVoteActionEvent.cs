using Content.Shared.Actions;

namespace Content.Shared._RMC14.Ghost;

public sealed partial class RMCGhostVoteActionEvent : InstantActionEvent
{
    [DataField]
    public EntityUid? Source;
}
