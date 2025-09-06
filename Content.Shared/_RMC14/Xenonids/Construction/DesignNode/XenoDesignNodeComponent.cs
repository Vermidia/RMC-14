using Robust.Shared.GameStates;

namespace Content.Shared._RMC14.Xenonids.Construction.DesignNode;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class XenoDesignNodeComponent : Component
{
    [DataField, AutoNetworkedField]
    public Color MarkColor;

    [DataField, AutoNetworkedField]
    public string MarkState = "mark_wall";
}
