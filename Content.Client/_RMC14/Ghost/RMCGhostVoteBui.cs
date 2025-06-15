using Content.Shared._RMC14.Ghost;
using JetBrains.Annotations;
using Robust.Client.UserInterface;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System.Linq;
using static Robust.Client.UserInterface.Controls.LineEdit;

namespace Content.Client._RMC14.Ghost;

[UsedImplicitly]
public sealed class RMCGhostVoteBui : BoundUserInterface
{
    [Dependency] private readonly IPrototypeManager _prototype = default!;

    [ViewVariables]
    private RMCVoteWindow? _window;

    private readonly List<NetEntity> _candidates = new();

    private readonly RMCGhostRoleVotingComponent? _voting = null;

    public RMCGhostVoteBui(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
        _candidates.Clear();
        _voting = EntMan.GetComponent<RMCGhostRoleVotingComponent>(Owner);
        _candidates = _voting.Candidates.Keys.ToList();

    }

    protected override void Open()
    {
        base.Open();
        Populate();
    }

    private void Populate()
    {
        _window = EnsureWindow();
        _window.EntContainer.DisposeAllChildren();

        foreach (var votie in _candidates)
        {
            var control = new RMCVoteChoiceControl();
            control.Set(EntMan.GetComponent<MetaDataComponent>(EntMan.GetEntity(votie)).EntityName);
            control.Button.OnPressed += _ => SendPredictedMessage(new RMCVoteWindowBuiMsg(votie));

            _window.EntContainer.AddChild(control);
        }
    }

    private RMCVoteWindow EnsureWindow()
    {
        if (_window != null)
            return _window;

        _window = this.CreateWindow<RMCVoteWindow>();
        _window.SearchBar.OnTextChanged += OnSearchBarChanged;
        if (_voting != null)
            _window.SetTimer(_voting);
        return _window;
    }

    private void OnSearchBarChanged(LineEditEventArgs args)
    {
        if (_window is not { Disposed: false })
            return;

        foreach (var child in _window.EntContainer.Children)
        {
            if (child is not RMCVoteChoiceControl control)
                continue;

            if (string.IsNullOrWhiteSpace(args.Text))
                control.Visible = true;
            else
                control.Visible = control.NameLabel.GetMessage()?.Contains(args.Text, StringComparison.OrdinalIgnoreCase) ?? false;
        }
    }
}
