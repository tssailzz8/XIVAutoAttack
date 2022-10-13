﻿using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Statuses;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XIVAutoAttack.Combos.Melee;

namespace XIVAutoAttack.Actions
{
    internal static class StatusHelper
    {
        public static readonly SortedList<uint, EnemyLocation> ActionLocations = new SortedList<uint, EnemyLocation>()
        {
            {DRGCombo.Actions.FangandClaw.ID, EnemyLocation.Side },
            {DRGCombo.Actions.WheelingThrust.ID, EnemyLocation.Back },
            {MNKCombo.Actions.Demolish.ID, EnemyLocation.Back },
            {MNKCombo.Actions.SnapPunch.ID, EnemyLocation.Side },
            {NINCombo.Actions.TrickAttack.ID, EnemyLocation.Back },
            {NINCombo.Actions.AeolianEdge.ID, EnemyLocation.Back },
            {NINCombo.Actions.ArmorCrush.ID, EnemyLocation.Side },
            {NINCombo.Actions.Suiton.ID, EnemyLocation.Back },
            {RPRCombo.Actions.Gibbet.ID, EnemyLocation.Side },
            {RPRCombo.Actions.Gallows.ID, EnemyLocation.Back },
            {SAMCombo.Actions.Gekko.ID, EnemyLocation.Back },
            {SAMCombo.Actions.Kasha.ID, EnemyLocation.Side },
        };

        internal static uint ToUint(this Color c)
        {
            return (uint)(((c.A << 24) | (c.R << 16) | (c.G << 8) | c.B) & 0xffffffffL);
        }

        internal static bool HaveStatusSelfFromSelf(params ushort[] effectIDs)
        {
            return FindStatusSelfFromSelf(effectIDs).Length > 0;
        }
        internal static float[] FindStatusSelfFromSelf(params ushort[] effectIDs)
        {
            return FindStatusFromSelf(Service.ClientState.LocalPlayer, effectIDs);
        }

        internal static float FindStatusTimeSelfFromSelf(params ushort[] effectIDs)
        {
            return FindStatusTimeFromSelf(Service.ClientState.LocalPlayer, effectIDs);
        }

        internal static float[] FindStatusFromSelf(BattleChara obj, params ushort[] effectIDs)
        {
            uint[] newEffects = effectIDs.Select(a => (uint)a).ToArray();
            return FindStatusFromSelf(obj).Where(status => newEffects.Contains(status.StatusId)).Select(status => status.RemainingTime).ToArray();
        }

        internal static Status[] FindStatusFromSelf(BattleChara obj)
        {
            if (obj == null) return new Status[0];

            return obj.StatusList.Where(status => status.SourceID == Service.ClientState.LocalPlayer.ObjectId).ToArray();
        }

        internal static float FindStatusTimeFromSelf(BattleChara obj, params ushort[] effectIDs)
        {
            var times = FindStatusFromSelf(obj, effectIDs);
            if (times == null || times.Length == 0) return 0;
            return times.Max();
        }
    }
}