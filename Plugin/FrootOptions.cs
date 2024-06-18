using System.Collections.Generic;
using System.Linq;
using BepInEx.Logging;
using Menu.Remix.MixedUI;
using Menu.Remix.MixedUI.ValueTypes;
using UnityEngine;

namespace BigFroot;

public class FrootOptions : OptionInterface
{
    //private readonly ManualLogSource Logger;
    public static readonly FrootOptions Instance = new();

    public FrootOptions() //(BigFroot modInstance, ManualLogSource loggerSource)
    {
        //Logger = loggerSource;
        FrootOptions.spawnChanceDangleFruit = this.config.Bind<float>("spawnChanceDangleFruit", 0.05f, new ConfigAcceptableRange<float>(0f, 1f));
        FrootOptions.spawnChanceWaterNut = this.config.Bind<float>("spawnChanceWaterNut", 0.05f, new ConfigAcceptableRange<float>(0f, 1f));
        FrootOptions.spawnChanceGooieDuck = this.config.Bind<float>("spawnChanceGooieDuck", 0.05f, new ConfigAcceptableRange<float>(0f, 1f));
        FrootOptions.spawnChanceLillyPuck = this.config.Bind<float>("spawnChanceLillyPuck", 0.05f, new ConfigAcceptableRange<float>(0f, 1f));
        FrootOptions.spawnChanceDandelionPeach = this.config.Bind<float>("spawnChanceDandelionPeach", 0.05f, new ConfigAcceptableRange<float>(0f, 1f));
        FrootOptions.spawnChanceGlowWeed = this.config.Bind<float>("spawnChanceGlowWeed", 0.05f, new ConfigAcceptableRange<float>(0f, 1f));
        FrootOptions.noVisuals = this.config.Bind<bool>("noVisuals", false);
        FrootOptions.vanillaBehavior = this.config.Bind<bool>("vanillaBehavior", true);
    }
    public static bool rotundWorldEnabled;
    public static Configurable<float> spawnChanceDangleFruit;
    public static Configurable<float> spawnChanceWaterNut;
    public static Configurable<float> spawnChanceGooieDuck;
    public static Configurable<float> spawnChanceLillyPuck;
    public static Configurable<float> spawnChanceDandelionPeach;
    public static Configurable<float> spawnChanceGlowWeed;
    public static Configurable<bool> noVisuals;
    public static Configurable<bool> vanillaBehavior;


    private UIelement[] UIArrPlayerOptions;

    public OpFloatSlider pZoomOp;
    public OpCheckBox mpBox1;
    public OpLabel lblOp1;


    public override void Initialize()
    {
        base.Initialize();
        var opTab = new OpTab(this, "Options");
        var chanceTab = new OpTab(this, "Spawn Chances");
        this.Tabs = new[]
        {
            opTab,
            chanceTab
        };


        //I DO THINGS MY WAY
        float lineCount = 580;
        int margin = 20;
        string dsc = "";

        // margin += 150;

        //
        lineCount -= 60;
        dsc = Translate("Disables the custom visuals that make the big fruit look big");
        Tabs[0].AddItems(new UIelement[]
        {
            mpBox1 = new OpCheckBox(FrootOptions.noVisuals, new Vector2(margin, lineCount))
            {description = dsc},
            new OpLabel(mpBox1.pos.x + 30, mpBox1.pos.y+3, Translate("Disable visuals"))
            {description = dsc},
            new OpLabel(165, 575, Translate("Big Froot Options"), bigText: true)
            {alignment = FLabelAlignment.Center},
        });

        lineCount -= 75;
        dsc = Translate("Disable swelling after eating a big fruit (requires Rotund World)");
        Tabs[0].AddItems(new UIelement[]
        {
            mpBox1 = new OpCheckBox(FrootOptions.vanillaBehavior, new Vector2(margin, lineCount))
            {description = dsc},
            new OpLabel(mpBox1.pos.x + 30, mpBox1.pos.y+3, Translate("Disable swelling"))
            {description = dsc}
        });


        //spawn chance tab
        lineCount = 580;
        lineCount -= 60;
        dsc = Translate("How likely it is for Blue Fruits to be big");
        int barLngtInt = 500;
        Tabs[1].AddItems(new UIelement[]
        {
            pZoomOp = new OpFloatSlider(FrootOptions.spawnChanceDangleFruit, new Vector2(margin + 0, lineCount), barLngtInt, 2, false)
            {description = dsc},
            new OpLabel(pZoomOp.pos.x - 20, pZoomOp.pos.y - 15, Translate("Blue Fruit"), bigText: false)
            {alignment = FLabelAlignment.Center},
            new OpLabel(165, 575, Translate("Spawn Chances"), bigText: true)
            {alignment = FLabelAlignment.Center},
        });

        lineCount -= 75;
        dsc = Translate("How likely it is for Bubble Fruits to be big");
        Tabs[1].AddItems(new UIelement[]
        {
            pZoomOp = new OpFloatSlider(FrootOptions.spawnChanceWaterNut, new Vector2(margin + 0, lineCount), barLngtInt, 2, false)
            {description = dsc},
            new OpLabel(pZoomOp.pos.x - 20, pZoomOp.pos.y - 15, Translate("Bubble Fruit"), bigText: false)
            {alignment = FLabelAlignment.Center}
        });

        lineCount -= 75;
        dsc = Translate("How likely it is for Gooieducks to be big");
        Tabs[1].AddItems(new UIelement[]
        {
            pZoomOp = new OpFloatSlider(FrootOptions.spawnChanceGooieDuck, new Vector2(margin + 0, lineCount), barLngtInt, 2, false)
            {description = dsc},
            new OpLabel(pZoomOp.pos.x - 20, pZoomOp.pos.y - 15, Translate("Gooieduck"), bigText: false)
            {alignment = FLabelAlignment.Center}
        });

        lineCount -= 75;
        dsc = Translate("How likely it is for Lilypucks to be big");
        Tabs[1].AddItems(new UIelement[]
        {
            pZoomOp = new OpFloatSlider(FrootOptions.spawnChanceLillyPuck, new Vector2(margin + 0, lineCount), barLngtInt, 2, false)
            {description = dsc},
            new OpLabel(pZoomOp.pos.x - 20, pZoomOp.pos.y - 15, Translate("Lilypuck"), bigText: false)
            {alignment = FLabelAlignment.Center}
        });

        lineCount -= 75;
        dsc = Translate("How likely it is for Dandelion Peaches to be big");
        Tabs[1].AddItems(new UIelement[]
        {
            pZoomOp = new OpFloatSlider(FrootOptions.spawnChanceDandelionPeach, new Vector2(margin + 0, lineCount), barLngtInt, 2, false)
            {description = dsc},
            new OpLabel(pZoomOp.pos.x - 20, pZoomOp.pos.y - 15, Translate("Dandelion Peach"), bigText: false)
            {alignment = FLabelAlignment.Center}
        });

        lineCount -= 75;
        dsc = Translate("How likely it is for Glow Weed to be big");
        Tabs[1].AddItems(new UIelement[]
        {
            pZoomOp = new OpFloatSlider(FrootOptions.spawnChanceGlowWeed, new Vector2(margin + 0, lineCount), barLngtInt, 2, false)
            {description = dsc},
            new OpLabel(pZoomOp.pos.x - 20, pZoomOp.pos.y - 15, Translate("Glow Weed"), bigText: false)
            {alignment = FLabelAlignment.Center}
        });
    }

    public override void Update()
    {
        base.Update();
    }
}