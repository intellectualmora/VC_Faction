// Decompiled with JetBrains decompiler
// Type: RimWorld.QuestGen.QuestNode_GeneratePawn
// Assembly: Assembly-CSharp, Version=1.5.9214.33606, Culture=neutral, PublicKeyToken=null
// MVID: 630E2863-BC9A-4A34-93F2-EFF01E3A9556
// Assembly location: E:\SteamLibrary\steamapps\common\RimWorld\RimWorldWin64_Data\Managed\Assembly-CSharp.dll

using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace VC_Faction
{
    public class QuestNode_GenerateEdyPawn : QuestNode
    {
        [NoTranslate]
        public SlateRef<string> storeAs;

        protected override void RunInt()
        {
            Pawn edyPawn = PawnCustomize.GenerateEdy(); // 调用你写的静态方法
            if (storeAs.GetValue(QuestGen.slate) != null)
            {
                QuestGen.slate.Set(storeAs.GetValue(QuestGen.slate), edyPawn);
            }
        }

        protected override bool TestRunInt(Slate slate)
        {
            return true; // 允许调试预览通过
        }
    }
}
