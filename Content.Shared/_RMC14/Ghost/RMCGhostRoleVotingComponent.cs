using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Generic;

namespace Content.Shared._RMC14.Ghost;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class RMCGhostRoleVotingComponent : Component
{
    [DataField]
    public EntProtoId VoteAction = "RMCActionVote";

    [DataField]
    public LocId VotePopup = "rmc-ghost-vote-popup-generic";

    [DataField]
    public LocId VoteMenuText = "rmc-ghost-vote-countdown-generic";

    [DataField, AutoNetworkedField]
    public bool VotingDone = false;

    [DataField(customTypeSerializer: typeof(DictionarySerializer<NetEntity, int>)), AutoNetworkedField]
    public Dictionary<NetEntity, int> Candidates = new();

    [DataField, AutoNetworkedField]
    public TimeSpan VotingTime = TimeSpan.FromSeconds(20);

    [DataField, AutoNetworkedField]
    public TimeSpan VoteEndsAt;
}
