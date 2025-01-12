﻿using System;
using System.Linq;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;

namespace XIVAutoAttack.Combos.CustomCombo
{
    internal class CustomComboActions
    {

        public static readonly BaseAction
            //昏乱
            Addle = new BaseAction(7560u)
            {
                TargetStatus = new ushort[] { 1203 },
            },

            //即刻咏唱
            Swiftcast = new BaseAction(7561u)
            {
                BuffsProvide = new ushort[]
                {
                    StatusIDs.Swiftcast1,
                    StatusIDs.Swiftcast2,
                    StatusIDs.Swiftcast3,
                    StatusIDs.Triplecast,
                    StatusIDs.Dualcast,
                }
            },

            //康复
            Esuna = new BaseAction(7568)
            {
                ChoiceTarget = (tars) =>
                {
                    if (TargetUpdater.DyingPeople.Length > 0)
                    {
                        return TargetUpdater.DyingPeople.OrderBy(b => TargetFilter.DistanceToPlayer(b)).First();
                    }
                    else if (TargetUpdater.WeakenPeople.Length > 0)
                    {
                        return TargetUpdater.WeakenPeople.OrderBy(b => TargetFilter.DistanceToPlayer(b)).First();
                    }
                    return null;
                },
            },

            //营救
            Rescue = new BaseAction(7571),

            //沉静
            Repose = new BaseAction(16560),

            //醒梦（如果MP低于6000那么使用）
            LucidDreaming = new BaseAction(7562u)
            {
                OtherCheck = b => Service.ClientState.LocalPlayer.CurrentMp < 6000,
            },

            //内丹
            SecondWind = new BaseAction(7541)
            {
                OtherCheck = b => Service.ClientState.LocalPlayer?.GetHealthRatio() < Service.Configuration.HealthSingleAbility,
            },

            //亲疏自行
            ArmsLength = new BaseAction(7548, shouldEndSpecial: true),

            //铁壁
            Rampart = new BaseAction(7531, true)
            {
                BuffsProvide = new ushort[]
                {
                    StatusIDs.Holmgang, StatusIDs.WillDead, StatusIDs.WalkingDead, StatusIDs.Superbolide, StatusIDs.HallowedGround,
                    StatusIDs.Rampart1, StatusIDs.Rampart2, StatusIDs.Rampart3,
                    //原初的直觉和血气
                    StatusIDs.RawIntuition, StatusIDs.Bloodwhetting,
                    //复仇
                    StatusIDs.Vengeance,
                    //预警
                    StatusIDs.Sentinel,
                    //暗影墙
                    StatusIDs.ShadowWall, StatusIDs.DarkMind,
                    //伪装
                    StatusIDs.Camouflage, StatusIDs.Nebula, StatusIDs.HeartofStone,
                },
                OtherCheck = BaseAction.TankDefenseSelf,
            },

            //挑衅
            Provoke = new BaseAction(7533)
            {
                FilterForTarget = b => TargetFilter.ProvokeTarget(b),
            },

            //雪仇
            Reprisal = new BaseAction(7535),

            //退避
            Shirk = new BaseAction(7537, true)
            {
                ChoiceTarget = friends =>
                {
                    var tanks = TargetFilter.GetJobCategory(friends, Role.防护);
                    if (tanks == null || tanks.Length == 0) return null;
                    return tanks[0];
                },
            },

            //浴血
            Bloodbath = new BaseAction(7542)
            {
                OtherCheck = SecondWind.OtherCheck,
            },

            //牵制
            Feint = new BaseAction(7549)
            {
                TargetStatus = new ushort[] { 1195 },
            },

            //插言
            Interject = new BaseAction(7538),

            //下踢
            LowBlow = new BaseAction(7540)
            {
                OtherCheck = b => !b.IsBoss(),
            },

            //扫腿
            LegSweep = new BaseAction(7863),

            //伤头
            HeadGraze = new BaseAction(7551),

            //沉稳咏唱
            Surecast = new BaseAction(7559, shouldEndSpecial: true),

            //真北
            TrueNorth = new BaseAction(7546, shouldEndSpecial: true)
            {
                BuffsProvide = new ushort[] { StatusIDs.TrueNorth },
            };

        private protected virtual BaseAction Raise => null;
        private protected virtual BaseAction Shield => null;
    }
}
