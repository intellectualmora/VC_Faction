// Decompiled with JetBrains decompiler
// Type: RimWorld.QuestGen.QuestNode_Root_DistressCall
// Assembly: Assembly-CSharp, Version=1.5.9214.33606, Culture=neutral, PublicKeyToken=null
// MVID: 630E2863-BC9A-4A34-93F2-EFF01E3A9556
// Assembly location: E:\SteamLibrary\steamapps\common\RimWorld\RimWorldWin64_Data\Managed\Assembly-CSharp.dll

using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using RimWorld;
using RimWorld.QuestGen;
using Verse;
using Verse.AI.Group;

namespace VC_Faction
{
    public class QuestPart_AskWelkin : QuestPart
    {

        public string inSignal;
        public Quest quest;
        public override void Notify_QuestSignalReceived(Signal signal)
        {
            base.Notify_QuestSignalReceived(signal);
            if (!(signal.tag == this.inSignal) )
                return;
            this.AskWelkin();
            this.PostAdd();
        }
        private void AskWelkin()
        {
            Map bruhl = Find.Maps.Find(m =>
                m.mapPawns.AllHumanlikeSpawned.Find(p =>
                    p.Name.ToStringFull == PawnCustomize.GenerateWelkin().Name.ToStringFull) != null);
            Pawn welkin = bruhl.mapPawns.AllHumanlikeSpawned.Find(p => p.Name.ToStringFull == PawnCustomize.GenerateWelkin().Name.ToStringFull);
            Lord currentLord = welkin.GetLord();
            if (currentLord != null)
            {
                currentLord.Notify_PawnLost(welkin, PawnLostCondition.ForcedToJoinOtherLord);
            }
            List<Pawn> pawns = new List<Pawn>();
            pawns.Add(welkin);
            LordMaker.MakeNewLord(Find.FactionManager.FirstFactionOfDef(FactionDefOf.VC_Gallia), new LordJob_WaitForTalk(welkin,this.quest), bruhl, pawns);
        }
        public virtual void PostAdd()
        {
        }

    }
}