using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace VC_Faction
{
    public class CompProperties_Retreat : CompProperties_EffectWithDest
    {
        public IntRange stunTicks;
        public float maxBodySize = 3.5f;
    }
}
