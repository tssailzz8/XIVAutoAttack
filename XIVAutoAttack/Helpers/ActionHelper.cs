﻿using FFXIVClientStructs.FFXIV.Client.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XIVAutoAttack.Data;
using Action = Lumina.Excel.GeneratedSheets.Action;


namespace XIVAutoAttack.Helpers
{
    internal static class ActionHelper
    {
        internal const byte GCDCooldownGroup = 58;

        internal static ActionCate GetActinoType(this Action action)
        {
            return (ActionCate)action.ActionCategory.Value.RowId;
        }

        internal static bool IsGeneralGCD(this Action action) => action.CooldownGroup == GCDCooldownGroup;

        internal static bool IsRealGCD(this Action action) => action.IsGeneralGCD() || action.AdditionalCooldownGroup == GCDCooldownGroup;

        internal static byte GetCoolDownGroup(this Action action)
        {
            var group = action.IsGeneralGCD() ? action.AdditionalCooldownGroup : action.CooldownGroup;
            if (group == 0) group = GCDCooldownGroup;
            return group;
        }
    }
}
