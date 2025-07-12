// Decompiled with JetBrains decompiler
// Type: RimWorld.CompEquippableAbility
// Assembly: Assembly-CSharp, Version=1.5.9214.33606, Culture=neutral, PublicKeyToken=null
// MVID: 630E2863-BC9A-4A34-93F2-EFF01E3A9556
// Assembly location: E:\SteamLibrary\steamapps\common\RimWorld\RimWorldWin64_Data\Managed\Assembly-CSharp.dll

using Verse;
using RimWorld;
using System.Collections.Generic;
using System.Linq;

namespace VC_Faction
{
    public class CompApparelAbility : CompEquippable
    {
        private Pawn pawn;
        private int cooldownTicksRemaining = 0;
        private CompProperties_ApparelAbility Props => this.props as CompProperties_ApparelAbility;

    
        public override void Notify_Equipped(Pawn pawn)
        {
            this.pawn = pawn;
            pawn.abilities.GainAbility(this.Props.abilityDef);
            if (cooldownTicksRemaining > 0)
            {
                pawn.abilities.AllAbilitiesForReading.Find(ab => ab.def == this.Props.abilityDef)
                    .StartCooldown(cooldownTicksRemaining);
            }

            pawn.abilities.Notify_TemporaryAbilitiesChanged();
        }

        public override void Notify_Unequipped(Pawn pawn)
        {
            Ability abillity = pawn.abilities.AllAbilitiesForReading.Find(ab => ab.def == this.Props.abilityDef);
            if (abillity.HasCooldown)
            {
                cooldownTicksRemaining = abillity.CooldownTicksRemaining;
            }
            else
            {
                cooldownTicksRemaining = 0;
            }
            pawn.abilities.RemoveAbility(this.Props.abilityDef);
            pawn.abilities.Notify_TemporaryAbilitiesChanged();
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            if (Scribe.mode != LoadSaveMode.PostLoadInit || this.pawn == null)
                return;
        }
    }
}
