using UnityEngine;
using Verse;
using RimWorld;
using RimWorld.QuestGen;

namespace VC_Faction
{
    public class Dialog_Radio : Window
    {
        public override Vector2 InitialSize => new Vector2(600f, 400f);
        private Texture2D Portrait;
        public string text;
        public string title;
        public string choice1;
        public string choice2;
        public QuestScriptDef quest;
        public float point;
        public RadioSignalManager radioSignalManager;
        public RadioSignal radioSignal;
        public Pawn target;
        public Dialog_Radio(Pawn target,RadioSignalManager radioSignalManager, RadioSignal radioSignal, Texture2D portrait = null, string title = "", string text = "", string choice1 = null, string choice2 = null,QuestScriptDef quest = null,float point = 100) : base(new RadioWindowDrawing()) // 传入自定义背景绘图类
        {
            this.target = target;
            this.doCloseX = false;
            this.absorbInputAroundWindow = true;
            this.forcePause = true;
            this.closeOnCancel = true;
            this.doCloseButton = true;
            this.Portrait = portrait;
            this.title = title;
            this.text = text;
            this.choice1 = choice1;
            this.choice2 = choice2;
            this.quest = quest;
            this.radioSignalManager = radioSignalManager;
            this.radioSignal = radioSignal;
            this.point = point;
        }
        protected virtual void GiveQuest(QuestScriptDef questDef)
        {
            Quest andMakeAvailable = QuestUtility.GenerateQuestAndMakeAvailable(questDef, point);
            if (andMakeAvailable.hidden || !questDef.sendAvailableLetter)
                return;
            QuestUtility.SendLetterQuestAvailable(andMakeAvailable);
        }
        public override void DoWindowContents(Rect inRect)
        {
            Text.Font = GameFont.Medium;
            GUI.color = Color.black;
            Widgets.Label(new Rect(inRect.x + inRect.width / 2f - 110f, inRect.y + 25f, 300f, 60f), title);
            Text.Font = GameFont.Small;
            Widgets.Label(new Rect(inRect.x  + 20f, inRect.y + 55f, 520f, 300f), text);
            GUI.color = Color.white;
            Rect logoRect = new Rect(inRect.x , inRect.y + 185f, 174f*1.5f, 117f * 1.5f);
            if(Portrait != null)
                GUI.DrawTexture(logoRect, Portrait, ScaleMode.ScaleAndCrop, alphaBlend: true);
            if (choice1 != null)
            {
                if (Widgets.ButtonText(new Rect(inRect.x + inRect.width / 2f + 50f, inRect.y + 250f, 150f, 30f),
                        choice1))
                {
                    Slate slate = RimWorld.QuestGen.QuestGen.slate;
                    slate.Set(quest.defName+ "_Accepter",target);
                    GiveQuest(this.quest);
                    radioSignalManager.RemoveRadioSignal(this.radioSignal);
                    Close();
                }
            }

            if (choice2 != null)
            {
                if (Widgets.ButtonText(new Rect(inRect.x + inRect.width / 2f + 50f, inRect.y + 290f, 150f, 30f), choice2))
                {
                    radioSignalManager.RemoveRadioSignal(this.radioSignal);
                    Close();
                }
            }
        }
    }
}