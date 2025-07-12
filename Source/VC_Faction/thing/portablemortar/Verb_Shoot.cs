// Decompiled with JetBrains decompiler
// Type: Verse.Verb_Shoot
// Assembly: Assembly-CSharp, Version=1.5.9214.33606, Culture=neutral, PublicKeyToken=null
// MVID: 630E2863-BC9A-4A34-93F2-EFF01E3A9556
// Assembly location: E:\SteamLibrary\steamapps\common\RimWorld\RimWorldWin64_Data\Managed\Assembly-CSharp.dll

using RimWorld;
using Verse;
namespace VC_Faction
{
    public class Verb_Shoot : Verb_LaunchProjectile
    {
        protected override int ShotsPerBurst => this.verbProps.burstShotCount;

        public override void WarmupComplete()
        {
            base.WarmupComplete();
            if (!(this.currentTarget.Thing is Pawn thing) || thing.Downed || thing.IsColonyMech || !this.CasterIsPawn || this.CasterPawn.skills == null)
                return;
            float num1 = thing.HostileTo(this.caster) ? 170f : 20f;
            float num2 = this.verbProps.AdjustedFullCycleTime((Verb)this, this.CasterPawn);
            this.CasterPawn.skills.Learn(SkillDefOf.Shooting, num1 * num2);
        }

        protected override bool TryCastShot()
        {
            int num = base.TryCastShot() ? 1 : 0;
            if (num == 0)
                return num != 0;
            if (!this.CasterIsPawn)
                return num != 0;
            this.CasterPawn.records.Increment(RecordDefOf.ShotsFired);
            return num != 0;
        }
    }
}
