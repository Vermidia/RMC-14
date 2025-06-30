using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Aura;
using Content.Shared.IdentityManagement;
using Content.Shared.Popups;
using Content.Shared.Weapons.Reflect;
using Robust.Shared.Network;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Xenonids.Reflect;

public sealed partial class XenoReflectSystem : EntitySystem
{
    [Dependency] private readonly RMCActionsSystem _rmcActions = default!;
    [Dependency] private readonly SharedAuraSystem _aura = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly INetManager _net = default!;
    public override void Initialize()
    {
        SubscribeLocalEvent<XenoReflectComponent, XenoReflectActionEvent>(OnActionReflect);
    }

    private void OnActionReflect(Entity<XenoReflectComponent> ent, ref XenoReflectActionEvent args)
    {
        if (args.Handled)
            return;

        if (!_rmcActions.TryUseAction(args))
            return;

        args.Handled = true;

        var reflect = EnsureComp<ReflectComponent>(ent);
        reflect.ReflectProb = ent.Comp.ReflectProbOn;

        _aura.GiveAura(ent, ent.Comp.AuraColor, ent.Comp.ReflectTime, 4);
        ent.Comp.ReflectEndAt = _timing.CurTime + ent.Comp.ReflectTime; ;

        if (_net.IsServer)
            _popup.PopupEntity(Loc.GetString("rmc-xeno-reflect-on", ("xeno", Identity.Entity(ent, EntityManager))), ent, PopupType.MediumCaution);

        Dirty(ent, reflect);
        Dirty(ent);

    }

    public override void Update(float frameTime)
    {
        if (_net.IsClient)
            return;

        var time = _timing.CurTime;

        var query = EntityQueryEnumerator<XenoReflectComponent, ReflectComponent>();

        while (query.MoveNext(out var uid, out var xeno, out var reflect))
        {
            if (xeno.ReflectEndAt == null || time < xeno.ReflectEndAt)
                continue;

            reflect.ReflectProb = xeno.ReflectProbOff;
            xeno.ReflectEndAt = null;
        }
    }
}
