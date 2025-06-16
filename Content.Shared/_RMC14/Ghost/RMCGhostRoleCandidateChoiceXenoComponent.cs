using Content.Shared.Players.PlayTimeTracking;
using Content.Shared.Roles;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Ghost;

[RegisterComponent, NetworkedComponent]
public sealed partial class RMCGhostRoleCandidateChoiceXenoComponent : Component
{
    [DataField]
    public bool IncludeQueen = false;

    [DataField]
    public bool IncludeSlotless = false;

    [DataField]
    public ProtoId<JobPrototype>? JobRequirement;
}
