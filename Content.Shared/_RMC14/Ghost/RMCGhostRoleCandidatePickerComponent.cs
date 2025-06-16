using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Generic;

namespace Content.Shared._RMC14.Ghost;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class RMCGhostRoleCandidatePickerComponent : Component
{
    [DataField]
    public EntProtoId OptInAction = "RMCActionOptIn";

    [DataField]
    public LocId CandidatePopup = "rmc-ghost-picker-popup-generic";

    [DataField]
    public bool PickingDone = false;

    [DataField(customTypeSerializer:typeof(ListSerializers<NetEntity>)), AutoNetworkedField]
    public List<NetEntity> Candidates = new();

    [DataField, AutoNetworkedField]
    public TimeSpan OptInTime = TimeSpan.FromSeconds(5);

    [DataField, AutoNetworkedField]
    public TimeSpan? EndSelectionAt;
}
