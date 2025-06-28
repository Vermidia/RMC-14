using Content.Shared.Actions;
using Content.Shared.Popups;
using Robust.Shared.Network;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Ghost;

public abstract class SharedRMCGhostRoleCandidateSystem : EntitySystem
{
    [Dependency] private readonly SharedActionsSystem _actions = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly INetManager _net = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly SharedUserInterfaceSystem _ui = default!;
    [Dependency] private readonly IRobustRandom _random = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<RMCGhostRoleCandidateChoiceXenoComponent, ComponentStartup>(OnXenoVotePickerBegin);

        SubscribeLocalEvent<GhostRoleCandidateComponent, RMCGhostRoleOptInActionEvent>(OnOptIn);

        Subs.BuiEvents<RMCGhostRoleCandidatePickerComponent>(RMCPickedGhostUI.Key, subs =>
        {
            subs.Event<RMCPickedGhostBuiMsg>(OnGhostRoleChoice);
        });

    }

    protected virtual void OnXenoVotePickerBegin(Entity<RMCGhostRoleCandidateChoiceXenoComponent> ent, ref ComponentStartup args)
    {

    }

    public void StartChoice(Entity<RMCGhostRoleCandidatePickerComponent> ent, List<EntityUid> possibleCandidates)
    {
        foreach (var mob in possibleCandidates)
        {
            EnsureComp<GhostRoleCandidateComponent>(mob);

            var action = _actions.AddAction(mob, ent.Comp.OptInAction);

            if (action == null)
                continue;

            if (!_actions.TryGetActionData(action, out var data))
                continue;

            if (data.BaseEvent is not RMCGhostRoleOptInActionEvent { } ghostVote)
                continue;

            ghostVote.Source = ent;
            _actions.UpdateAction(action);
            _popup.PopupEntity(Loc.GetString(ent.Comp.CandidatePopup), mob, mob, PopupType.Medium);
        }

        ent.Comp.EndSelectionAt = _timing.CurTime + ent.Comp.OptInTime;
        ent.Comp.Candidates.Clear();
        Dirty(ent);
    }

    private void OnOptIn(Entity<GhostRoleCandidateComponent> ent, ref RMCGhostRoleOptInActionEvent args)
    {
        if (args.Source == null || !TryComp<RMCGhostRoleCandidatePickerComponent>(args.Source, out var picker))
            return;

        picker.Candidates.Add(GetNetEntity(ent));

        Dirty(args.Source.Value, picker);

        RemoveOptInAction(ent, (args.Source.Value, picker));
    }

    public override void Update(float frameTime)
    {
        if (_net.IsClient)
            return;

        var time = _timing.CurTime;

        var query = EntityQueryEnumerator<RMCGhostRoleCandidatePickerComponent>();

        while (query.MoveNext(out var uid, out var picker))
        {
            if (picker.AutoDenyAt != null)
            {
                if (time < picker.AutoDenyAt)
                    continue;

                CandidateMadeChoice((uid, picker), null);
                continue;
            }

            if (picker.PickingDone || picker.EndSelectionAt == null || time < picker.EndSelectionAt)
                continue;

            //Remove the corresponding action if it wasn't gotten rid of already
            var voterQuery = EntityQueryEnumerator<GhostRoleCandidateComponent>();
            while (voterQuery.MoveNext(out var voterUid, out var _))
            {
                RemoveOptInAction(voterUid, (uid, picker));
            }

            picker.PickingDone = true;
            //Go through the canidiates
            PromptCandidate((uid, picker));
        }

    }

    private void PromptCandidate(Entity<RMCGhostRoleCandidatePickerComponent> picker)
    {
        if (picker.Comp.Candidates.Count <= 0)
        {
            //TODO Backup picking
            picker.Comp.AutoDenyAt = null;
            return;
        }

        var candidate = GetEntity(_random.PickAndTake(picker.Comp.Candidates));

        if (!_ui.TryOpenUi(picker.Owner, RMCPickedGhostUI.Key, candidate))
        {
            PromptCandidate(picker);
            return;
        }

        picker.Comp.AutoDenyAt = _timing.CurTime + picker.Comp.AcceptTime;
        Dirty(picker);
    }

    private void RemoveOptInAction(EntityUid user, Entity<RMCGhostRoleCandidatePickerComponent> source)
    {
        foreach (var (actionId, action) in _actions.GetActions(user))
        {
            if (action.BaseEvent is not RMCGhostRoleOptInActionEvent { } voteAction)
                continue;

            if (voteAction.Source != source)
                continue;

            _actions.RemoveAction(user, actionId);
        }
    }

    private void OnGhostRoleChoice(Entity<RMCGhostRoleCandidatePickerComponent> ent, ref RMCPickedGhostBuiMsg args)
    {
        CandidateMadeChoice(ent, args.Actor, args.Confirmed);
    }

    private void CandidateMadeChoice(Entity<RMCGhostRoleCandidatePickerComponent> picker, EntityUid? candidate, bool accepted = false)
    {
        _ui.CloseUi(picker.Owner, RMCPickedGhostUI.Key);

        if (!accepted || candidate == null)
            PromptCandidate(picker);
        else
            MoveCandidate(picker, candidate.Value);

    }

    protected virtual void MoveCandidate(Entity<RMCGhostRoleCandidatePickerComponent> picker, EntityUid chosen)
    {

    }
}
