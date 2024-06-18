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
        FrootOptions.spawnChance = this.config.Bind<float>("spawnChance", 0.05f, new ConfigAcceptableRange<float>(0.01f, 0.5f));
        FrootOptions.noVisuals = this.config.Bind<bool>("noVisuals", false);
    }

    public static Configurable<float> spawnChance;
    public static Configurable<bool> noVisuals;
    

    private UIelement[] UIArrPlayerOptions;

    public OpFloatSlider pZoomOp;
    public OpCheckBox mpBox1;
    public OpLabel lblOp1;


    public override void Initialize()
    {
        var opTab = new OpTab(this, "Options");
        this.Tabs = new[]
        {
            opTab
        };
		
		
		//I DO THINGS MY WAY
		float lineCount = 580;
		int margin = 20;
		string dsc = "";
		
		// margin += 150;
		lineCount -= 60;
		//OpCheckBox mpBox1;
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
        dsc = Translate("How likely it is for a normal blue fruit to spawn as a big fruit");
        int barLngtInt = 500;
        Tabs[0].AddItems(new UIelement[]
        {
            pZoomOp = new OpFloatSlider(FrootOptions.spawnChance, new Vector2(margin + 0, lineCount), barLngtInt, 2, false) {description = dsc},
            new OpLabel(pZoomOp.pos.x - 20, pZoomOp.pos.y - 15, Translate("Spawn Chance"), bigText: false)
            {alignment = FLabelAlignment.Center}
        });
    }
}