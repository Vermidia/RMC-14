using Content.Shared._RMC14.Ghost;
using JetBrains.Annotations;
using Robust.Client.UserInterface;

namespace Content.Client._RMC14.Ghost;

[UsedImplicitly]
public sealed class RMCPickedGhostBui(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
    [ViewVariables]
    private RMCPickedGhostWindow? _window;

    protected override void Open()
    {
        base.Open();
        _window = this.CreateWindow<RMCPickedGhostWindow>();
        if (EntMan.TryGetComponent<RMCGhostRoleCandidatePickerComponent>(Owner, out var comp))
            _window.SetPicker(comp);
        _window.DenyButton.OnPressed += _ =>
        {
            SendPredictedMessage(new RMCPickedGhostBuiMsg(false));
            _window.Close();
        };
        _window.ConfirmButton.OnPressed += _ =>
        {
            SendPredictedMessage(new RMCPickedGhostBuiMsg(true));
            _window.Close();
        };
    }
}
