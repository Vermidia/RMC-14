using Content.Shared.DoAfter;
using Content.Shared.FixedPoint;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Xenonids.Construction.Events;

[Serializable, NetSerializable]
public sealed partial class XenoUpgradeResinDoafterEvent : SimpleDoAfterEvent
{
    [DataField]
    public NetCoordinates Coordinates;

    [DataField]
    public EntProtoId StructureId = "WallXenoResin";

    [DataField]
    public NetEntity? Effect;

    [DataField]
    public FixedPoint2 PlasmaCost;

    public XenoUpgradeResinDoafterEvent(NetCoordinates coordinates, EntProtoId structureId, FixedPoint2 cost, NetEntity? effect = null)
    {
        Coordinates = coordinates;
        StructureId = structureId;
        Effect = effect;
        PlasmaCost = cost;
    }
}
