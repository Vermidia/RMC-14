using Robust.Shared.GameStates;

namespace Content.Shared._RMC14.Xenonids.Reflect;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class XenoReflectComponent : Component
{
    [DataField, AutoNetworkedField]
    public TimeSpan ReflectTime = TimeSpan.FromSeconds(4);

    [DataField, AutoNetworkedField]
    public TimeSpan? ReflectEndAt;

    [DataField, AutoNetworkedField]
    public float ReflectProbOn = 1.0f;

    [DataField, AutoNetworkedField]
    public float ReflectProbOff = 0;

    [DataField, AutoNetworkedField]
    public Color AuraColor = Color.FromHex("#0000FF55");
}
