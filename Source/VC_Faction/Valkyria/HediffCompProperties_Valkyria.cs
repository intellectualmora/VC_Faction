using System;
using RimWorld;
using Verse;

namespace Valkyria
{
    public class HediffCompProperties_Valkyria : HediffCompProperties
    {
        public SongDef songDef;
        public float overlightRadius;
        public float glowRadius = 14f;
        public ColorInt glowColor = new ColorInt((int)byte.MaxValue, (int)byte.MaxValue, (int)byte.MaxValue, 0) * 1.45f;
        public bool colorPickerEnabled;
        public bool darklightToggle;
        public Type compGlowerClass;
        public HediffCompProperties_Valkyria() => this.compClass = typeof(HediffComp_Valkyria);

    }
}
