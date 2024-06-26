﻿using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Client.UserInterface.XAML;

namespace Content.Client._RMC14.Xenonids.Evolution;

[GenerateTypedNameReferences]
public sealed partial class XenoDevolveWindow : DefaultWindow
{
    public XenoDevolveWindow()
    {
        RobustXamlLoader.Load(this);
    }
}
