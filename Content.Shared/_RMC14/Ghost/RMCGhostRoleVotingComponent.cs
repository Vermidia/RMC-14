using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Ghost;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class RMCGhostRoleVotingComponent : Component
{
    [DataField]
    public EntProtoId VoteAction = "RMCActionVote";

    [DataField]
    public LocId VotePopup = "rmc-ghost-vote-popup-generic";

    [DataField, AutoNetworkedField]
    public bool VotingDone = false;

    [DataField]
    public Dictionary<EntityUid, int> Candidates = new();

    [DataField, AutoNetworkedField]
    public TimeSpan VotingTime = TimeSpan.FromSeconds(20);

    [DataField, AutoNetworkedField]
    public TimeSpan VoteEndsAt;
}
