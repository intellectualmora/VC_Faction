// Decompiled with JetBrains decompiler
// Type: RimWorld.LordToil_ManClosestTurrets
// Assembly: Assembly-CSharp, Version=1.5.9214.33606, Culture=neutral, PublicKeyToken=null
// MVID: 630E2863-BC9A-4A34-93F2-EFF01E3A9556
// Assembly location: E:\SteamLibrary\steamapps\common\RimWorld\RimWorldWin64_Data\Managed\Assembly-CSharp.dll

using Verse;
using Verse.AI;
using Verse.AI.Group;
using RimWorld;
namespace VC_Faction
{
    public class LordToil_ManClosestTurrets : LordToil
    {
        public override void UpdateAllDuties()
        {
            for (int index = 0; index < this.lord.ownedPawns.Count; ++index)
                this.lord.ownedPawns[index].mindState.duty = new PawnDuty(DutyDefOf.VC_ManClosestTurret, (LocalTargetInfo)this.lord.ownedPawns[index].Position);
        }
    }
}
