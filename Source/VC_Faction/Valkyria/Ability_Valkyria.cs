using System;
using RimWorld;
using Verse;

namespace Valkyria
{
    public class Ability_Valkyria : Ability
    {
        public Ability_Valkyria():base()
        {

        }

        public Ability_Valkyria(Pawn pawn) : base(pawn)
        {

        }

        public Ability_Valkyria(Pawn pawn, Precept sourcePrecept) : base(pawn, sourcePrecept)
        {

        }

        public Ability_Valkyria(Pawn pawn, AbilityDef def) : base(pawn, def)
        {

        }

        public Ability_Valkyria(Pawn pawn, Precept sourcePrecept, AbilityDef def) : base(pawn, sourcePrecept, def)
        {

        }
        public override AcceptanceReport CanCast
        {
            get
            {
                AcceptanceReport canCast = base.CanCast;
                foreach (var thing in pawn.equipment.AllEquipmentListForReading)
                {
                    if (canCast && thing.def.defName.Contains("Spear_Valkyria") && pawn.gender == Gender.Female)
                    {
                        return (AcceptanceReport)true;
                    }
                }
                return (AcceptanceReport) false;
            }
        }

    }
}
