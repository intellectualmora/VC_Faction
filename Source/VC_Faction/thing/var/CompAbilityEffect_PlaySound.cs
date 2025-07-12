using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace VC_Faction
{
    public class CompAbilityEffect_PlaySound : CompAbilityEffect_WithDuration
    {

        private CompProperties_PlaySound Props => (CompProperties_PlaySound) this.props;
        public override void PostApplied(List<LocalTargetInfo> targets, Map map)
        {
            base.PostApplied(targets, map);
            Find.MusicManagerPlay.ForcePlaySong(Props.songDef, true);
        }


    }
}
