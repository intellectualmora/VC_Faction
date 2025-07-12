using System;
using RimWorld;
using UnityEngine;
using Verse;

namespace Valkyria
{
    public class CompGlower_Valkyria : CompGlower
    {
        public bool isBeLit = true;
        protected override bool ShouldBeLitNow
        {
            get
            {
                return isBeLit;
            }
        }
    }
}
