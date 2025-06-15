using Content.Shared._RMC14.Xenonids;
using Content.Shared._RMC14.Xenonids.Egg;
using Content.Shared.Actions;
using Content.Shared.Mind;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.Network;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Ghost;

public abstract class SharedRMCGhostRoleVotingSystem : EntitySystem
{
    [Dependency] private readonly SharedActionsSystem _actions = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly INetManager _net = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly SharedUserInterfaceSystem _ui = default!;
    [Dependency] private readonly MobStateSystem _mob = default!;
    [Dependency] private readonly SharedMindSystem _mind = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<RMCGhostRoleVotePickerXenoComponent, ComponentStartup>(OnXenoVotePickerBegin);

        SubscribeLocalEvent<GhostRoleVoterComponent, RMCGhostVoteActionEvent>(OnOpenVoteAction);

        Subs.BuiEvents<RMCGhostRoleVotingComponent>(RMCVoteUIKey.Key,
    subs =>
    {
        subs.Event<RMCVoteWindowBuiMsg>(OnVoteReceived);
    });
    }

    private void OnXenoVotePickerBegin(Entity<RMCGhostRoleVotePickerXenoComponent> ent, ref ComponentStartup args)
    {
        if (_net.IsClient)
            return;

        if (!TryComp<RMCGhostRoleVotingComponent>(ent, out var voting))
            return;

        List<EntityUid> candidates = new();
        List<EntityUid> voters = new();

        var query = EntityQueryEnumerator<XenoComponent, MobStateComponent>();

        while (query.MoveNext(out var uid, out var xeno, out var mob))
        {
            if (!_mob.IsAlive(uid))
                continue;

            if (!_mind.TryGetMind(uid, out var _, out var _))
                continue;

            voters.Add(uid);

            if (!ent.Comp.IncludeQueen && HasComp<XenoOvipositorCapableComponent>(uid))
                continue;

            if (!ent.Comp.IncludeSlotless && !xeno.CountedInSlots)
                continue;

            candidates.Add(uid);
        }

        candidates.Sort((a, b) => string.CompareOrdinal(Name(a, MetaData(a)), Name(b, MetaData(b))));

        StartVote((ent, voting), candidates, voters);
    }

    /// <summary>
    /// Gives all voters a ghostRoleVoter comp, and an action to vote.
    /// Currently must be run server and clientside because lists aren't synced
    /// Lists convert ents into EntityUids for consistency between server and client
    /// </summary>
    /// <param name="ent">vote holder entity</param>
    /// <param name="candidates">who can be voted for</param>
    /// <param name="voters">who can vote</param>
    public void StartVote(Entity<RMCGhostRoleVotingComponent> ent, List<EntityUid> candidates, List<EntityUid> voters)
    {
        foreach (var voter in voters)
        {
            EnsureComp<GhostRoleVoterComponent>(voter);

            var action = _actions.AddAction(voter, ent.Comp.VoteAction);

            if (action == null)
                continue;

            if (!_actions.TryGetActionData(action, out var data))
                continue;

            if (data.BaseEvent is not RMCGhostVoteActionEvent { } ghostVote)
                continue;

            ghostVote.Source = ent;
            _actions.UpdateAction(action);
            _popup.PopupEntity(Loc.GetString(ent.Comp.VotePopup), voter, voter, PopupType.Medium);
        }

        ent.Comp.Candidates.Clear();

        foreach (var votie in candidates)
        {
            ent.Comp.Candidates.Add(GetNetEntity(votie), 0);
        }

        ent.Comp.VoteEndsAt = _timing.CurTime + ent.Comp.VotingTime;
        Dirty(ent);
    }

    private void OnOpenVoteAction(Entity<GhostRoleVoterComponent> ent, ref RMCGhostVoteActionEvent args)
    {
        if (args.Source != null)
        {
            _ui.OpenUi(args.Source.Value, RMCVoteUIKey.Key, ent);
        }
    }

    /// <summary>
    /// Tally the vote and remove the action
    /// </summary>
    /// <param name="ent"></param>
    /// <param name="args"></param>
    private void OnVoteReceived(Entity<RMCGhostRoleVotingComponent> ent, ref RMCVoteWindowBuiMsg args)
    {
        if (!ent.Comp.Candidates.ContainsKey(args.Selection))
            return;

        if (_net.IsServer)
            ent.Comp.Candidates[args.Selection]++;

        RemoveVoteAction(args.Actor, ent);
    }

    public override void Update(float frameTime)
    {
        if (_net.IsClient)
            return;

        var time = _timing.CurTime;

        var query = EntityQueryEnumerator<RMCGhostRoleVotingComponent>();

        while (query.MoveNext(out var uid, out var voting))
        {
            if (voting.VotingDone || time < voting.VoteEndsAt)
                continue;

            //Remove the corresponding action if it wasn't gotten rid of already
            var voterQuery = EntityQueryEnumerator<GhostRoleVoterComponent>();
            while (voterQuery.MoveNext(out var voterUid, out var _))
            {
                RemoveVoteAction(voterUid, (uid, voting));
            }

            voting.VotingDone = true;
            //Go through the canidiates
        }

    }

    private void RemoveVoteAction(EntityUid user, Entity<RMCGhostRoleVotingComponent> source)
    {
        foreach (var (actionId, action) in _actions.GetActions(user))
        {
            if (action.BaseEvent is not RMCGhostVoteActionEvent { } voteAction)
                continue;

            if (voteAction.Source != source)
                continue;

            _actions.RemoveAction(user, actionId);
            _ui.CloseUi(source.Owner, RMCVoteUIKey.Key, user);
        }
    }
}
[Serializable, NetSerializable]
public enum RMCVoteUIKey : byte
{
    Key
}

[Serializable, NetSerializable]
public sealed class RMCVoteWindowBuiMsg(NetEntity selection) : BoundUserInterfaceMessage
{
    public readonly NetEntity Selection = selection;
}
