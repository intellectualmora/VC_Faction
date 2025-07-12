// Decompiled with JetBrains decompiler
// Type: RimWorld.CompMannable
// Assembly: Assembly-CSharp, Version=1.5.9214.33606, Culture=neutral, PublicKeyToken=null
// MVID: 630E2863-BC9A-4A34-93F2-EFF01E3A9556
// Assembly location: E:\SteamLibrary\steamapps\common\RimWorld\RimWorldWin64_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;
using RimWorld;
namespace VC_Faction
{
    public class CompMannable : RimWorld.CompMannable
    {
        public CompProperties_Mannable Props => (CompProperties_Mannable)props;

        public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn pawn)
        {
            CompMannable compMannable = this;
            if (pawn.RaceProps.ToolUser && pawn.CanReserveAndReach((LocalTargetInfo)(Thing)compMannable.parent, PathEndMode.InteractionCell, Danger.Deadly))
            {
                if (compMannable.Props.manWorkType != WorkTags.None && pawn.WorkTagIsDisabled(compMannable.Props.manWorkType))
                {
                    if (compMannable.Props.manWorkType == WorkTags.Violent)
                        yield return new FloatMenuOption((string)("CannotManThing".Translate((NamedArgument)compMannable.parent.LabelShort, (NamedArgument)(Thing)compMannable.parent) + " (" + "IsIncapableOfViolenceLower".Translate((NamedArgument)pawn.LabelShort, (NamedArgument)(Thing)pawn) + ")"), (Action)null);
                }
                else
                    yield return new FloatMenuOption((string)"OrderManThing".Translate((NamedArgument)compMannable.parent.LabelShort, (NamedArgument)(Thing)compMannable.parent), (Action)(() => pawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(JobDefOf.VC_ManTurret, (LocalTargetInfo)(Thing)this.parent))));
            }
        }
    }
}
