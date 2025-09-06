using Content.Shared.FixedPoint;
using Robust.Shared.GameStates;

namespace Content.Shared._RMC14.Projectiles.Reflect;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class RMCReflectiveComponent : Component //TODO RMC14 projectile reflect fail damage modifier
{
    [DataField, AutoNetworkedField]
    public FixedPoint2 ReflectChance = 0.75;

    [DataField, AutoNetworkedField]
    public FixedPoint2 DamageMult = 0.5;

    [DataField, AutoNetworkedField]
    public Angle Spread = Angle.FromDegrees(30);

    [DataField, AutoNetworkedField]
    public FixedPoint2 Accuracy = 35;
}
