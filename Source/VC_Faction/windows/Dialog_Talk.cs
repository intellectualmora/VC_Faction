using UnityEngine;
using Verse;
using RimWorld;
namespace VC_Faction
{
    public class Dialog_Talk : Window
    {
        public override Vector2 InitialSize => new Vector2(600f, 400f);
        private Texture2D Portrait;
        public string outSignal1;
        public string outSignal2;
        public string content;
        public string choice1;
        public string choice2;
        public Dialog_Talk( Texture2D portrait,  string content, string choice1, string outSignal1, string choice2, string outSignal2) : base(new TalkWindowDrawing()) // 传入自定义背景绘图类
        {
            this.doCloseX = false;
            this.absorbInputAroundWindow = true;
            this.forcePause = true;
            this.closeOnCancel = true;
            this.doCloseButton = true;
            this.outSignal1 = outSignal1;
            this.outSignal2 = outSignal2;
            this.Portrait = portrait;
            this.content = content;
            this.choice1 = choice1;
            this.choice2 = choice2;
            // this.optionalTitle = "自定义窗口";
        }

        public override void DoWindowContents(Rect inRect)
        {
            Text.Font = GameFont.Small;
            GUI.color = Color.black;

            Widgets.Label(new Rect(inRect.x + inRect.width / 2f - 40f, inRect.y + 60f, 300f, 300f), content);
            GUI.color = Color.white;
            Rect logoRect = new Rect(inRect.x + 23f, inRect.y + 60f,137f,160f);
            GUI.DrawTexture(logoRect, Portrait, ScaleMode.ScaleAndCrop, alphaBlend: true);
            if (!(choice1 == ""|| choice1 == null))
            {
                if (Widgets.ButtonText(new Rect(inRect.x + inRect.width / 2f - 40f, inRect.y + 250f, 150f, 30f),
                        choice1))
                {
                    if(!(outSignal1 == "" || outSignal1 == null))
                        Find.SignalManager.SendSignal(new Signal(outSignal1));
                    Close();
                }
            }
            if (!(choice2 == "" || outSignal2 == null))
            {
                if (Widgets.ButtonText(new Rect(inRect.x + inRect.width / 2f - 40f, inRect.y + 290f, 150f, 30f),
                        choice2))
                {
                    if (!(outSignal2 == "" || outSignal2 == null))
                        Find.SignalManager.SendSignal(new Signal(outSignal2));
                    Close();
                }
            }
        }
    }
}