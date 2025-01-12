using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Hooking;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Game;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;

namespace XIVAutoAttack.SigReplacers;

internal sealed class IconReplacer : IDisposable
{
    public record CustomComboGroup(uint jobId, uint[] classJobIds, ICustomCombo[] combos);

    private delegate ulong IsIconReplaceableDelegate(uint actionID);

    private delegate uint GetIconDelegate(IntPtr actionManager, uint actionID);

    private delegate IntPtr GetActionCooldownSlotDelegate(IntPtr actionManager, int cooldownGroup);

    public static ICustomCombo RightNowCombo
    {
        get
        {
            if (Service.ClientState.LocalPlayer == null) return null;

            foreach (var combos in CustomCombos)
            {
                if (!combos.classJobIds.Contains(Service.ClientState.LocalPlayer.ClassJob.Id)) continue;
                return GetChooseCombo(combos);
            }

            return null;
        }
    }

    internal static ICustomCombo GetChooseCombo(CustomComboGroup group)
    {
        var id = group.jobId;
        if(Service.Configuration.ComboChoices.TryGetValue(id, out var choice))
        {
            foreach (var combo in group.combos)
            {
                if( combo.Author == choice)
                {
                    return combo;
                }
            }
        }
        return group.combos[0];
    }

    internal static BaseAction[] AllBaseActions => RightComboBaseActions.Union(GeneralBaseAction).ToArray();

    internal static BaseAction[] RightComboBaseActions
    {
        get
        {
            var combo = RightNowCombo;
            if (combo == null) return new BaseAction[0];
            return GetActions(combo, combo.GetType());
        }
    }

    internal static BaseAction[] GeneralBaseAction
    {
        get
        {
            var combo = RightNowCombo;
            if (combo == null) return new BaseAction[0];
            return GetActions(combo, typeof(CustomComboActions));
        }
    }

    private static BaseAction[] GetActions(ICustomCombo combo, Type type)
    {
        return (from field in type.GetFields()
                where field.IsStatic && typeof(BaseAction).IsAssignableFrom(field.FieldType)
                select (BaseAction)field.GetValue(combo) into act
                orderby act.ID
                select act).ToArray();
    }

    private static SortedList<Role, CustomComboGroup[]> _customCombosDict;
    internal static SortedList<Role, CustomComboGroup[]> CustomCombosDict
    {
        get
        {
            if (_customCombosDict == null)
            {
                SetStaticValues();
            }
            return _customCombosDict;
        }
    }
    private static CustomComboGroup[] _customCombos;
    private static CustomComboGroup[] CustomCombos
    {
        get
        {
            if (_customCombos == null)
            {
                SetStaticValues();
            }
            return _customCombos;
        }
    }

    private readonly Hook<IsIconReplaceableDelegate> isIconReplaceableHook;

    private readonly Hook<GetIconDelegate> getIconHook;

    private IntPtr actionManager = IntPtr.Zero;


    public IconReplacer()
    {
        unsafe
        {
            getIconHook = Hook<GetIconDelegate>.FromAddress((IntPtr)ActionManager.fpGetAdjustedActionId, GetIconDetour);
        }
        isIconReplaceableHook = Hook<IsIconReplaceableDelegate>.FromAddress(Service.Address.IsActionIdReplaceable, IsIconReplaceableDetour);

        getIconHook.Enable();
        isIconReplaceableHook.Enable();
    }



    private static void SetStaticValues()
    {
        
        _customCombos = (from t in Assembly.GetAssembly(typeof(ICustomCombo)).GetTypes()
                         where t.GetInterfaces().Contains(typeof(ICustomCombo)) && !t.IsAbstract && !t.IsInterface
                         select (ICustomCombo)Activator.CreateInstance(t) into combo
                         group combo by combo.JobIDs[0] into comboGrp
                         select new CustomComboGroup(comboGrp.Key, comboGrp.First().JobIDs, SetCombos(comboGrp.ToArray())))
                         .ToArray();

        _customCombosDict = new SortedList<Role, CustomComboGroup[]>
            (_customCombos.GroupBy(g => g.combos[0].Role).ToDictionary(set => set.Key, set => set.OrderBy(i => i.jobId).ToArray()));
    }

    private static ICustomCombo[] SetCombos(ICustomCombo[] combos)
    {
        var result = new List<ICustomCombo>(combos.Length);

        foreach (var combo in combos)
        {
            if(!result.Any(c => c.Author == combo.Author))
            {
                result.Add(combo);
            }
        }
        return result.ToArray();
    }

    public void Dispose()
    {
        getIconHook.Dispose();
        isIconReplaceableHook.Dispose();
    }

    internal uint OriginalHook(uint actionID)
    {
        return getIconHook.Original.Invoke(actionManager, actionID);
    }

    private unsafe uint GetIconDetour(IntPtr actionManager, uint actionID)
    {
        this.actionManager = actionManager;
        return RemapActionID(actionID);
    }

    internal static BaseAction KeyActionID => CustomComboActions.Repose;

    private uint RemapActionID(uint actionID)
    {
        PlayerCharacter localPlayer = Service.ClientState.LocalPlayer;

        if (localPlayer == null || actionID != KeyActionID.ID || Service.Configuration.NeverReplaceIcon)
            return OriginalHook(actionID);

        return ActionUpdater.NextAction?.AdjustedID ?? OriginalHook(actionID);

    }

    private ulong IsIconReplaceableDetour(uint actionID)
    {
        return 1uL;
    }

    internal static bool AutoAttackConfig(string str, string str1)
    {
        switch (str)
        {
            case "setall":
                {
                    foreach (var item in CustomCombos)
                    {
                        foreach (var combo in item.combos)
                        {
                            combo.IsEnabled = true;
                        }
                    }
                    Service.ChatGui.Print("All SET");
                    Service.Configuration.Save();
                    break;
                }
            case "unsetall":
                {
                    foreach (var item in CustomCombos)
                    {
                        foreach (var combo in item.combos)
                        {
                            combo.IsEnabled = false;
                        }
                    }
                    Service.ChatGui.Print("All UNSET");
                    Service.Configuration.Save();
                    break;
                }
            default:
                return true;
        }
        Service.Configuration.Save();
        return false;
    }
}
