using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace VC_Faction
{
    public class CompAbilityEffect_GiveSong: CompAbilityEffect
    {
        private CompProperties_AbilitySong Props => (CompProperties_AbilitySong) this.props;
        public override void PostApplied(List<LocalTargetInfo> targets, Map map)
        {
            base.PostApplied(targets, map);
            Find.MusicManagerPlay.ForcePlaySong(Props.songDef, true);
        }

    }
}
