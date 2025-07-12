using System;
using RimWorld;
using Verse;

namespace Valkyria
{
    public class Ability_ValkyriaCannon : Ability
    {
        public Ability_ValkyriaCannon():base()
        {

        }

        public Ability_ValkyriaCannon(Pawn pawn) : base(pawn)
        {

        }

        public Ability_ValkyriaCannon(Pawn pawn, Precept sourcePrecept) : base(pawn, sourcePrecept)
        {

        }

        public Ability_ValkyriaCannon(Pawn pawn, AbilityDef def) : base(pawn, def)
        {

        }

        public Ability_ValkyriaCannon(Pawn pawn, Precept sourcePrecept, AbilityDef def) : base(pawn, sourcePrecept, def)
        {

        }
        public override bool CanCast
        {
            get
            {
                bool canCast = base.CanCast;
                foreach (var hediff in pawn.health.hediffSet.hediffs)
                {
                    if (canCast && hediff.def.defName == "ValkyriaAwaken")
                        return true;
                }
                return false;
            }
        }
        

    }
}
