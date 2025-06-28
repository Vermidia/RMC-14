using Content.Shared._RMC14.Ghost;
using Content.Shared._RMC14.Xenonids.Egg;
using Content.Shared._RMC14.Xenonids;
using Content.Shared.Mobs.Components;
using Content.Shared.Mind;
using Content.Shared.Mobs.Systems;
using Robust.Shared.Network;
using Content.Server.Players.PlayTimeTracking;
using Robust.Server.Player;
using Content.Server.Ghost.Roles;
using Content.Shared.Ghost.Roles.Components;
using Content.Server.Ghost.Roles.Components;

namespace Content.Server._RMC14.Ghost;

public sealed class RMCGhostRoleCandidateSystem : SharedRMCGhostRoleCandidateSystem
{
    [Dependency] private readonly MobStateSystem _mob = default!;
    [Dependency] private readonly SharedMindSystem _mind = default!;
    [Dependency] private readonly PlayTimeTrackingSystem _tracking = default!;
    [Dependency] private readonly IPlayerManager _player = default!;
    [Dependency] private readonly GhostRoleSystem _ghost = default!;

    protected override void OnXenoVotePickerBegin(Entity<RMCGhostRoleCandidateChoiceXenoComponent> ent, ref ComponentStartup args)
    {
        if (!TryComp<RMCGhostRoleCandidatePickerComponent>(ent, out var picker))
            return;

        List<EntityUid> candidates = new();

        var query = EntityQueryEnumerator<XenoComponent, MobStateComponent>();

        while (query.MoveNext(out var uid, out var xeno, out var mob))
        {
            if (!_mob.IsAlive(uid))
                continue;

            if (!_mind.TryGetMind(uid, out var _, out var _))
                continue;

            if (!ent.Comp.IncludeQueen && HasComp<XenoOvipositorCapableComponent>(uid))
                continue;

            if (!ent.Comp.IncludeSlotless && !xeno.CountedInSlots)
                continue;

            if (ent.Comp.JobRequirement != null && (!_player.TryGetSessionByEntity(uid, out var ses) || !_tracking.IsAllowed(ses, ent.Comp.JobRequirement)))
                continue;

            candidates.Add(uid);
        }

        //TODO RMC14 ghosts

        StartChoice((ent, picker), candidates);
    }

    protected override void MoveCandidate(Entity<RMCGhostRoleCandidatePickerComponent> picker, EntityUid chosen)
    {
        if (!_player.TryGetSessionByEntity(chosen, out var session))
            return;

        if (!TryComp<GhostRoleComponent>(picker, out var role) || !TryComp<GhostRoleMobSpawnerComponent>(picker, out var spawner))
            return;

        _ghost.Takeover(session, role.Identifier);
    }
}
