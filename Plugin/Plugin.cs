//using System;
using System.Collections.Generic;
using System.Security;
using System.Security.Permissions;
using BepInEx;
using UnityEngine;
using MoreSlugcats;
using RWCustom;
using System.Runtime.CompilerServices;

#pragma warning disable CS0618
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618

namespace BigFroot;

[BepInPlugin("willowwisp.bigfroot", "Big Froot", "1.0.2")]



public class BigFroot : BaseUnityPlugin
{
    public static BigFroot instance;
    //private FrootOptions Options;

    public void OnEnable()
    {
        try
        {
            On.RainWorld.OnModsInit += RainWorldOnOnModsInit;

            On.DangleFruit.ctor += DangleFruit_ctor;
            On.DangleFruit.BitByPlayer += DangleFruit_BitByPlayer;
            On.DangleFruit.InitiateSprites += DangleFruit_InitiateSprites;
            On.DangleFruit.ApplyPalette += DangleFruit_ApplyPalette;

            On.Player.Update += Player_Update;
        }
        catch (System.Exception arg)
        {
            base.Logger.LogError(string.Format("Failed to initialize", arg));
            throw;
        }
    }

    private bool IsInit;
    private static bool rotundWorldEnabled;
    private void RainWorldOnOnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
    {
        orig(self);
        try
        {
            MachineConnector.SetRegisteredOI("willowwisp.bigfroot", FrootOptions.Instance);

            if (IsInit) return;

            for (int i = 0; i < ModManager.ActiveMods.Count; i++)
            {
                if (ModManager.ActiveMods[i].id == "willowwisp.bellyplus")
                    rotundWorldEnabled = true;
            }

            
            IsInit = true;
        }
        catch (System.Exception ex)
        {
            Logger.LogError(ex);
            throw;
        }
    }



    private void DangleFruit_ctor(On.DangleFruit.orig_ctor orig, DangleFruit self, AbstractPhysicalObject abstractPhysicalObject)
    {
        orig(self, abstractPhysicalObject);

        if (self.GetFroot().isJumbo && !FrootOptions.noVisuals.Value)
        {
            self.bodyChunks[0].mass *= 2f;
            self.bites = 6;
            // this.bounce = 0.2f;
        }
    }

    private void DangleFruit_BitByPlayer(On.DangleFruit.orig_BitByPlayer orig, DangleFruit self, Creature.Grasp grasp, bool eu)
    {
        if (self.GetFroot().isJumbo)
        {
            if (self.bites == 1)
            {
                (grasp.grabber as Player).GetCat().swell = true;
                (grasp.grabber as Player).GetCat().bloatTimer = 15f;
                if (FrootOptions.noVisuals.Value)
                    (grasp.grabber as Player).AddFood(4);
            }
            else if (self.bites <= 5 && !FrootOptions.noVisuals.Value)
            {
                (grasp.grabber as Player).AddFood(1);
            }
        }
            
        orig(self, grasp, eu);
    }

    private void DangleFruit_InitiateSprites(On.DangleFruit.orig_InitiateSprites orig, DangleFruit self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        orig(self, sLeaser, rCam);

        if (self.GetFroot().isJumbo && !FrootOptions.noVisuals.Value)
        {
            for (int i = 0; i < 2; i++)
            {
                sLeaser.sprites[i].scaleX *= 2.2f;
                sLeaser.sprites[i].scaleY *= 1.65f;
            }
        }
    }

    private void DangleFruit_ApplyPalette(On.DangleFruit.orig_ApplyPalette orig, DangleFruit self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        orig(self, sLeaser, rCam, palette);

        if (self.GetFroot().isJumbo && !FrootOptions.noVisuals.Value)
        {
            self.color = Color.Lerp(self.color, palette.blackColor, 0.6f);
            //REDUCE THE SHADOW AMMOUNT
            sLeaser.sprites[0].color = Color.Lerp(self.color, palette.blackColor, 0.9f); //palette.blackColor;
        }
    }

    private void Player_Update(On.Player.orig_Update orig, Player self, bool eu)
    {
        orig(self, eu);
        if (!self.dead && self.GetCat().swell && rotundWorldEnabled)
        {
            self.GetCat().bloatCount -= 1f;
            if (self.GetCat().bloatCount <= 0f)
            {
                self.AddFood(1);
                self.GetCat().bloatTimer *= 1.25f;
                self.GetCat().bloatCount = self.GetCat().bloatTimer;
            }
        }
    }
}


public static class FrootStatusClass
{
    public class FrootStatus
    {
        public bool isJumbo;

        public FrootStatus(DangleFruit fruit)
        {
            Random.State state = Random.state;
            Random.InitState(fruit.abstractPhysicalObject.ID.RandomSeed);

            // UnityEngine.Random.seed = self.abstractPhysicalObject.ID.RandomSeed;
            if (UnityEngine.Random.value < FrootOptions.spawnChance.Value) //0.05f)
                this.isJumbo = true;

            Random.state = state;
        }
    }

    // This part lets you access the stored stuff by simply doing "self.GetFroot()" in Plugin.cs or everywhere else!
    private static readonly ConditionalWeakTable<DangleFruit, FrootStatus> CWT = new();
    public static FrootStatus GetFroot(this DangleFruit fruit) => CWT.GetValue(fruit, _ => new(fruit));
}



public static class BellyClass
{
    public class Belly
    {
        public bool swell = false;
        public float bloatCount = 0f;
        public float bloatTimer = 12f;
    }

    private static readonly ConditionalWeakTable<Player, Belly> CWT = new();
    public static Belly GetCat(this Player player) => CWT.GetValue(player, _ => new());
}