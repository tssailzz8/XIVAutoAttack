using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using System.Collections.Generic;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;

namespace XIVAutoAttack.Combos.Tank;

internal abstract class WARCombo<TCmd> : JobGaugeCombo<WARGauge, TCmd> where TCmd : Enum
{

    public sealed override uint[] JobIDs => new uint[] { 21, 3 };
    internal sealed override bool HaveShield => Player.HaveStatus(StatusIDs.Defiance);
    private protected override BaseAction Shield => Defiance;

    public static readonly BaseAction
        //�ػ�
        Defiance = new(48, shouldEndSpecial: true),

        //����
        HeavySwing = new(31),

        //�ײ���
        Maim = new(37),

        //����ն �̸�
        StormsPath = new(42),

        //������ �츫
        StormsEye = new(45)
        {
            OtherCheck = b => Player.WillStatusEndGCD(3, 0, true, StatusIDs.SurgingTempest),
        },

        //�ɸ�
        Tomahawk = new(46)
        {
            FilterForTarget = b => TargetFilter.ProvokeTarget(b),
        },

        //�͹�
        Onslaught = new(7386, shouldEndSpecial: true)
        {
            ChoiceTarget = TargetFilter.FindTargetForMoving,
        },

        //����    
        Upheaval = new(7387)
        {
            BuffsNeed = new ushort[] { StatusIDs.SurgingTempest },
        },

        //��ѹ��
        Overpower = new(41),

        //��������
        MythrilTempest = new(16462),

        //Ⱥɽ¡��
        Orogeny = new(25752),

        //ԭ��֮��
        InnerBeast = new(49)
        {
            OtherCheck = b => !Player.WillStatusEndGCD(3, 0, true, StatusIDs.SurgingTempest) && (JobGauge.BeastGauge >= 50 || Player.HaveStatus(StatusIDs.InnerRelease)),
        },

        //��������
        SteelCyclone = new(51)
        {
            OtherCheck = InnerBeast.OtherCheck,
        },

        //ս��
        Infuriate = new(52)
        {
            BuffsProvide = new[] { StatusIDs.InnerRelease },
            OtherCheck = b => TargetFilter.GetObjectInRadius(TargetUpdater.HostileTargets, 5).Length > 0 && JobGauge.BeastGauge < 50,
        },

        //��
        Berserk = new(38)
        {
            OtherCheck = b => TargetFilter.GetObjectInRadius(TargetUpdater.HostileTargets, 5).Length > 0,
        },

        //ս��
        ThrillofBattle = new(40),

        //̩Ȼ����
        Equilibrium = new(3552),

        //ԭ��������
        NascentFlash = new(16464)
        {
            ChoiceTarget = TargetFilter.FindAttackedTarget,
        },

        ////ԭ����Ѫ��
        //Bloodwhetting = new BaseAction(25751),

        //����
        Vengeance = new(44)
        {
            BuffsProvide = Rampart.BuffsProvide,
            OtherCheck = BaseAction.TankDefenseSelf,
        },

        //ԭ����ֱ��
        RawIntuition = new(3551)
        {
            BuffsProvide = Rampart.BuffsProvide,
            OtherCheck = BaseAction.TankDefenseSelf,
        },

        //����
        ShakeItOff = new(7388, true),

        //����
        Holmgang = new(43)
        {
            OtherCheck = BaseAction.TankBreakOtherCheck,
        },

        ////ԭ���Ľ��
        //InnerRelease = new BaseAction(7389),

        //���ı���
        PrimalRend = new(25753)
        {
            BuffsNeed = new[] { StatusIDs.PrimalRendReady },
        };
}
