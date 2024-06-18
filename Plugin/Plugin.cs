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

[BepInPlugin("willowwisp.bigfroot", "Big Froot", "1.1.1")]



public class BigFroot : BaseUnityPlugin
{
    public static BigFroot instance;
    //private FrootOptions Options;

    public void OnEnable()
    {
        try
        {
            On.RainWorld.OnModsInit += RainWorldOnOnModsInit;
            #region dangle fruit
            On.DangleFruit.ctor += DangleFruit_ctor;
            On.DangleFruit.BitByPlayer += DangleFruit_BitByPlayer;
            On.DangleFruit.InitiateSprites += DangleFruit_InitiateSprites;
            On.DangleFruit.ApplyPalette += DangleFruit_ApplyPalette;
            #endregion
            #region bubble fruit
            On.WaterNut.ctor += WaterNut_ctor;
            On.WaterNut.InitiateSprites += WaterNut_InitiateSprites;
            On.WaterNut.ApplyPalette += WaterNut_ApplyPalette;
            On.WaterNut.Swell += WaterNut_Swell;
            On.Rock.HitSomething += WaterNut_HitSomething;
            On.SwollenWaterNut.ctor += SwollenWaterNut_ctor;
            On.SwollenWaterNut.BitByPlayer += SwollenWaterNut_BitByPlayer;
            On.SwollenWaterNut.InitiateSprites += SwollenWaterNut_InitiateSprites;
            On.SwollenWaterNut.DrawSprites += SwollenWaterNut_DrawSprites;
            On.SwollenWaterNut.ApplyPalette += SwollenWaterNut_ApplyPalette;
            #endregion
            #region gooieduck
            On.MoreSlugcats.GooieDuck.ctor += GooieDuck_ctor;
            On.MoreSlugcats.GooieDuck.BitByPlayer += GooieDuck_BitByPlayer;
            On.MoreSlugcats.GooieDuck.InitiateSprites += GooieDuck_InitiateSprites;
            On.MoreSlugcats.GooieDuck.DrawSprites += GooieDuck_DrawSprites;
            On.MoreSlugcats.GooieDuck.ApplyPalette += GooieDuck_ApplyPalette;
            #endregion
            #region lilypuck
            On.MoreSlugcats.LillyPuck.ctor += LillyPuck_ctor;
            On.MoreSlugcats.LillyPuck.Thrown += LillyPuck_Thrown;
            On.MoreSlugcats.LillyPuck.BitByPlayer += LillyPuck_BitByPlayer;
            On.MoreSlugcats.LillyPuck.InitiateSprites += LillyPuck_InitiateSprites;
            On.MoreSlugcats.LillyPuck.ApplyPalette += LillyPuck_ApplyPalette;
            #endregion
            #region dandelion peach
            On.MoreSlugcats.DandelionPeach.ctor += DandelionPeach_ctor;
            On.MoreSlugcats.DandelionPeach.BitByPlayer += DandelionPeach_BitByPlayer;
            On.MoreSlugcats.DandelionPeach.InitiateSprites += DandelionPeach_InitiateSprites;
            On.MoreSlugcats.DandelionPeach.ApplyPalette += DandelionPeach_ApplyPalette;
            #endregion
            #region glow weed
            On.MoreSlugcats.GlowWeed.ctor += GlowWeed_ctor;
            On.MoreSlugcats.GlowWeed.Update += GlowWeed_Update;
            On.PhysicalObject.TerrainImpact += GlowWeed_TerrainImpact;
            On.MoreSlugcats.GlowWeed.BitByPlayer += GlowWeed_BitByPlayer;
            On.MoreSlugcats.GlowWeed.InitiateSprites += GlowWeed_InitiateSprites;
            On.MoreSlugcats.GlowWeed.DrawSprites += GlowWeed_DrawSprites;
            On.MoreSlugcats.GlowWeed.ApplyPalette += GlowWeed_ApplyPalette;
            #endregion
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
    #region dangle fruit    (nothing outlandish just fill 5 food pips)
    private void DangleFruit_ctor(On.DangleFruit.orig_ctor orig, DangleFruit self, AbstractPhysicalObject abstractPhysicalObject)
    {
        orig(self, abstractPhysicalObject);

        if (self.GetFroot().isJumbo && !FrootOptions.noVisuals.Value)
        {
            self.bodyChunks[0].mass *= 2f;
            self.bites = 6;
        }
    }

    private void DangleFruit_BitByPlayer(On.DangleFruit.orig_BitByPlayer orig, DangleFruit self, Creature.Grasp grasp, bool eu)
    {
        if (self.GetFroot().isJumbo)
        {
            if (self.bites == 1)
            {
                if (!FrootOptions.vanillaBehavior.Value)
                {
                    (grasp.grabber as Player).GetCat().swell = true;
                    (grasp.grabber as Player).GetCat().bloatTimer = 15f;
                }
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
    #endregion
    #region bubble fruit    (TODO: snail pop, stuns for a bit)
    private void WaterNut_ctor(On.WaterNut.orig_ctor orig, WaterNut self, AbstractPhysicalObject abstractPhysicalObject)
    {
        orig(self, abstractPhysicalObject);

        if (self.GetFroot().isJumbo && !FrootOptions.noVisuals.Value)
        {
            self.bodyChunks[0].mass *= 2f;
        }
    }
    //extra damage
    private bool WaterNut_HitSomething(On.Rock.orig_HitSomething orig, Rock self, SharedPhysics.CollisionResult result, bool eu)
    {
        //theres gotta be a better way...................
        if (self is WaterNut && self.GetFroot().isJumbo && !FrootOptions.noVisuals.Value)
        {
            if (result.obj == null)
            {
                return false;
            }
            if (self.thrownBy is Scavenger && (self.thrownBy as Scavenger).AI != null)
            {
                (self.thrownBy as Scavenger).AI.HitAnObjectWithWeapon(self, result.obj);
            }
            self.vibrate = 20;
            self.ChangeMode(Weapon.Mode.Free);
            if (result.obj is Creature)
            {
                float stunBonus = 45f;
                if (ModManager.MMF && MMF.cfgIncreaseStuns.Value && (result.obj is Cicada || result.obj is LanternMouse || (ModManager.MSC && result.obj is Yeek)))
                {
                    stunBonus = 90f;
                }
                if (ModManager.MSC && self.room.game.IsArenaSession && self.room.game.GetArenaGameSession.chMeta != null)
                {
                    stunBonus = 90f;
                }
                //all this just to change the damage value...
                (result.obj as Creature).Violence(self.firstChunk, self.firstChunk.vel * self.firstChunk.mass, result.chunk, result.onAppendagePos, Creature.DamageType.Blunt, 0.5f, stunBonus); 
            }
            else if (result.chunk != null)
            {
                result.chunk.vel += self.firstChunk.vel * self.firstChunk.mass / result.chunk.mass;
            }
            else if (result.onAppendagePos != null)
            {
                (result.obj as PhysicalObject.IHaveAppendages).ApplyForceOnAppendage(result.onAppendagePos, self.firstChunk.vel * self.firstChunk.mass);
            }
            self.firstChunk.vel = self.firstChunk.vel * -0.5f + Custom.DegToVec(Random.value * 360f) * Mathf.Lerp(0.1f, 0.4f, Random.value) * self.firstChunk.vel.magnitude;
            self.room.PlaySound(SoundID.Rock_Hit_Creature, self.firstChunk);
            if (result.chunk != null)
            {
                self.room.AddObject(new ExplosionSpikes(self.room, result.chunk.pos + Custom.DirVec(result.chunk.pos, result.collisionPoint) * result.chunk.rad, 5, 2f, 4f, 4.5f, 30f, new Color(1f, 1f, 1f, 0.5f)));
            }
            self.SetRandomSpin();
            return true;
        }
        else
        {
            return orig(self, result, eu);
        }
    }

    private void WaterNut_InitiateSprites(On.WaterNut.orig_InitiateSprites orig, WaterNut self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        orig(self, sLeaser, rCam);

        if (self.GetFroot().isJumbo && !FrootOptions.noVisuals.Value)
        {
            for (int i = 0; i < 2; i++)
            {
                sLeaser.sprites[i].scaleX *= 1.6f;
                sLeaser.sprites[i].scaleY *= 1.6f;
            }
        }
    }

    private void WaterNut_ApplyPalette(On.WaterNut.orig_ApplyPalette orig, WaterNut self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        orig(self, sLeaser, rCam, palette);

        if (self.GetFroot().isJumbo && !FrootOptions.noVisuals.Value)
        {
            sLeaser.sprites[0].color = palette.blackColor;
            self.color = Color.Lerp(new Color(0.4f, 0f, 1f), palette.blackColor, Mathf.Lerp(0f, 0.5f, rCam.PaletteDarkness()));
        }
    }
    //make sure big rocks always pop into big bubbles
    private void WaterNut_Swell(On.WaterNut.orig_Swell orig, WaterNut self)
    {
        if (self.GetFroot().isJumbo)
        {
            if (self.grabbedBy.Count > 0)
            {
                self.grabbedBy[0].Release();
            }
            self.AbstrNut.swollen = true;
            self.room.PlaySound(SoundID.Water_Nut_Swell, self.firstChunk.pos);
            SwollenWaterNut swollenWaterNut = new SwollenWaterNut(self.abstractPhysicalObject);
            swollenWaterNut.plop = 0.01f;
            swollenWaterNut.lastPlop = 0f;
            swollenWaterNut.rotation = self.rotation;
            swollenWaterNut.lastRotation = self.lastRotation;
            swollenWaterNut.addAbstractEntity = true;
            self.room.AddObject(swollenWaterNut);
            swollenWaterNut.GetFroot().isJumbo = true; // man.
            swollenWaterNut.firstChunk.HardSetPosition(self.firstChunk.pos);
            swollenWaterNut.AbstrConsumable.isFresh = self.AbstrNut.isFresh;
            self.Destroy();
        }
        else
        {
            orig(self);
        }
    }

    private void SwollenWaterNut_ctor(On.SwollenWaterNut.orig_ctor orig, SwollenWaterNut self, AbstractPhysicalObject abstractPhysicalObject)
    {
        orig(self, abstractPhysicalObject);

        if (self.GetFroot().isJumbo && !FrootOptions.noVisuals.Value)
        {
            self.bodyChunks[0].mass *= 2f;
            self.bites = 6;
        }
    }

    private void SwollenWaterNut_BitByPlayer(On.SwollenWaterNut.orig_BitByPlayer orig, SwollenWaterNut self, Creature.Grasp grasp, bool eu)
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

    private void SwollenWaterNut_InitiateSprites(On.SwollenWaterNut.orig_InitiateSprites orig, SwollenWaterNut self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        orig(self, sLeaser, rCam);

        if (self.GetFroot().isJumbo && !FrootOptions.noVisuals.Value)
        {
            for (int i = 0; i < 3; i++)
            {
                sLeaser.sprites[i].scaleX *= 1.6f;
                sLeaser.sprites[i].scaleY *= 1.75f;
            }
        }
    }

    private void SwollenWaterNut_DrawSprites(On.SwollenWaterNut.orig_DrawSprites orig, SwollenWaterNut self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        //its a bit more complicated than just changing the pallete because of the squishing animation :/
        if (self.GetFroot().isJumbo && !FrootOptions.noVisuals.Value)
        {
            Vector2 pos = Vector2.Lerp(self.firstChunk.lastPos, self.firstChunk.pos, timeStacker);
            Vector2 v = Vector3.Slerp(self.lastRotation, self.rotation, timeStacker);
            self.lastDarkness = self.darkness;
            self.darkness = rCam.room.Darkness(pos) * (1f - rCam.room.LightSourceExposure(pos));
            if (self.darkness != self.lastDarkness)
            {
                self.ApplyPalette(sLeaser, rCam, rCam.currentPalette);
            }
            for (int i = 0; i < 3; i++)
            {
                sLeaser.sprites[i].x = pos.x - camPos.x;
                sLeaser.sprites[i].y = pos.y - camPos.y;
            }
            sLeaser.sprites[0].rotation = Custom.VecToDeg(v);
            sLeaser.sprites[1].rotation = Custom.VecToDeg(v);
            sLeaser.sprites[2].alpha = (1f - self.darkness) * (1f - self.firstChunk.submersion);
            float num = Mathf.Lerp(self.lastPlop, self.plop, timeStacker);
            num = Mathf.Lerp(0f, 1f + Mathf.Sin(num * (float)System.Math.PI), num);
            sLeaser.sprites[2].scaleX = (1.9f * Custom.LerpMap(self.bites, 6f, 1f, 1f, 0.2f) * 1f + Mathf.Lerp(self.lastProp, self.prop, timeStacker) / 20f) * num;
            sLeaser.sprites[2].scaleY = (1.95f * Custom.LerpMap(self.bites, 6f, 1f, 1f, 0.2f) * 1f - Mathf.Lerp(self.lastProp, self.prop, timeStacker) / 20f) * num;
            if (self.blink > 0 && UnityEngine.Random.value < 0.5f)
            {
                sLeaser.sprites[0].color = new Color(1f, 1f, 1f);
            }
            else
            {
                sLeaser.sprites[0].color = self.color;
            }
            if (self.slatedForDeletetion || self.room != rCam.room)
            {
                sLeaser.CleanSpritesAndRemove();
            }
        }
        else
        {
            orig(self, sLeaser, rCam, timeStacker, camPos);
        }
    }

    private void SwollenWaterNut_ApplyPalette(On.SwollenWaterNut.orig_ApplyPalette orig, SwollenWaterNut self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        orig(self, sLeaser, rCam, palette);

        if (self.GetFroot().isJumbo && !FrootOptions.noVisuals.Value)
        {
            self.color = palette.blackColor;
            sLeaser.sprites[1].color = Color.Lerp(new Color(0.4f, 0f, 1f), palette.blackColor, Mathf.Lerp(0f, 0.5f, rCam.PaletteDarkness()));
            sLeaser.sprites[2].color = Color.Lerp(palette.waterColor1, palette.waterColor2, 0.5f);
        }
    }
    #endregion
    #region gooieduck       (idk. just fills 6 food pips amd worm grass still hates it)
    private void GooieDuck_ctor(On.MoreSlugcats.GooieDuck.orig_ctor orig, GooieDuck self, AbstractPhysicalObject abstractPhysicalObject)
    {
        orig(self, abstractPhysicalObject);

        if (self.GetFroot().isJumbo && !FrootOptions.noVisuals.Value)
        {
            self.bodyChunks[0].mass *= 2f;
            self.bites = 6; //hardcoded nonsense :(
        }
    }

    private void GooieDuck_BitByPlayer(On.MoreSlugcats.GooieDuck.orig_BitByPlayer orig, GooieDuck self, Creature.Grasp grasp, bool eu)
    {
        if (self.GetFroot().isJumbo)
        {
            if (self.bites == 1)
            {
                if (!FrootOptions.vanillaBehavior.Value)
                {
                    (grasp.grabber as Player).GetCat().swell = true;
                    (grasp.grabber as Player).GetCat().bloatTimer = 15f;
                }
                if (FrootOptions.noVisuals.Value)
                    (grasp.grabber as Player).AddFood(2);
            }
            else if (self.bites <= 5 && !FrootOptions.noVisuals.Value)
            {
                (grasp.grabber as Player).AddFood(1);
            }
        }
        orig(self, grasp, eu);
    }

    private void GooieDuck_InitiateSprites(On.MoreSlugcats.GooieDuck.orig_InitiateSprites orig, GooieDuck self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {

        if (self.GetFroot().isJumbo && !FrootOptions.noVisuals.Value)
        {
            self.segmentCount = Mathf.Max(6, (int)(Random.value * 5f) + 4);
        }
        orig(self, sLeaser, rCam);

        if (self.GetFroot().isJumbo && !FrootOptions.noVisuals.Value)
        {
            sLeaser.sprites[self.StringSnapPercent.Length].scaleX *= 2.2f;
            sLeaser.sprites[self.StringSnapPercent.Length].scaleY *= 1.65f;
            for (int j = 0; j < self.segmentCount; j++)
            {
                sLeaser.sprites[self.StringSnapPercent.Length + 1 + j].scaleX = 0.35f;
                sLeaser.sprites[self.StringSnapPercent.Length + 1 + j].scaleY = 1.9f;
            }
        }
    }
    
    private void GooieDuck_DrawSprites(On.MoreSlugcats.GooieDuck.orig_DrawSprites orig, GooieDuck self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        orig(self, sLeaser, rCam, timeStacker, camPos);

        if (self.GetFroot().isJumbo && !FrootOptions.noVisuals.Value)
        {

            for (int j = 0; j < self.segmentCount + 1; j++)
            {
                if (j == 0)
                {
                    sLeaser.sprites[self.StringSnapPercent.Length].scaleX = 0.85f * 2.2f + Mathf.Sin(self.PulserA) / 6f;
                    sLeaser.sprites[self.StringSnapPercent.Length].scaleY = 0.85f * 1.65f + Mathf.Sin(self.PulserA) / 6f;
                    continue;
                }
                sLeaser.sprites[self.StringSnapPercent.Length + j].scaleX = 0.35f + Mathf.Sin(self.PulserB) / 8f;
                sLeaser.sprites[self.StringSnapPercent.Length + j].scaleY = 1.9f + Mathf.Sin(self.PulserB + self.PulserA) / 10f;
            }
        }
    }

    private void GooieDuck_ApplyPalette(On.MoreSlugcats.GooieDuck.orig_ApplyPalette orig, GooieDuck self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        orig(self, sLeaser, rCam, palette);

        if (self.GetFroot().isJumbo && !FrootOptions.noVisuals.Value)
        {
            self.CoreColor = Color.Lerp(new Color(0.35f, 0.95f, 1f), palette.blackColor, self.darkness / 3f);
        }
    }
    #endregion
    #region lillypuck       (you get 3 uses!! and deals a bit more damage!!!! but velocity dies off a bit. comes in crazy ass colors!)
    private void LillyPuck_ctor(On.MoreSlugcats.LillyPuck.orig_ctor orig, LillyPuck self, AbstractPhysicalObject abstractPhysicalObject, World world)
    {
        orig(self, abstractPhysicalObject, world);

        if (self.GetFroot().isJumbo && !FrootOptions.noVisuals.Value)
        {
            self.bodyChunks[0].mass = 0.5f;
            self.AbstrLillyPuck.bites = 6;
            self.spearDamageBonus = 1f;

            float value = UnityEngine.Random.value;
            if (value > 0.21 && value < 0.42) value -= 0.21f;
            HSLColor hSLColor = new HSLColor(value, 1f, 0.6f);
            self.flowerColor = Custom.HSL2RGB(hSLColor.hue, hSLColor.saturation, hSLColor.lightness);
            self.LightRad = UnityEngine.Random.Range(250f, 300f);
        }
    }

    private void LillyPuck_Thrown(On.MoreSlugcats.LillyPuck.orig_Thrown orig, LillyPuck self, Creature thrownBy, Vector2 thrownPos, Vector2? firstFrameTraceFromPos, IntVector2 throwDir, float frc, bool eu)
    {
        //base.Thrown(thrownBy, thrownPos, firstFrameTraceFromPos, throwDir, frc, eu);
        #region how do i call the base virtual method helpppppppppppppppppppp
        self.thrownBy = thrownBy;
        self.thrownPos = thrownPos;
        self.throwDir = throwDir;
        self.firstFrameTraceFromPos = firstFrameTraceFromPos;
        self.changeDirCounter = 3;
        self.ChangeOverlap(newOverlap: true);
        self.firstChunk.MoveFromOutsideMyUpdate(eu, thrownPos);
        if (throwDir.x != 0)
        {
            self.firstChunk.vel.y = thrownBy.mainBodyChunk.vel.y * 0.5f;
            self.firstChunk.vel.x = thrownBy.mainBodyChunk.vel.x * 0.2f;
            self.firstChunk.vel.x += (float)throwDir.x * 40f * frc;
            self.firstChunk.vel.y += ((self is Spear) ? 1.5f : 3f); // I KNOW SHUT UP!!!!!
        }
        else
        {
            if (throwDir.y == 0)
            {
                self.ChangeMode(Weapon.Mode.Free);
                return;
            }
            self.firstChunk.vel.x = thrownBy.mainBodyChunk.vel.x * 0.5f;
            self.firstChunk.vel.y = (float)throwDir.y * 40f * frc;
        }
        if (frc >= 1f)
        {
            self.overrideExitThrownSpeed = 0f;
        }
        else
        {
            self.overrideExitThrownSpeed = Mathf.Min(self.exitThrownModeSpeed, frc * 20f);
        }
        self.ChangeMode(Weapon.Mode.Thrown);
        self.setRotation = throwDir.ToVector2();
        self.rotationSpeed = 0f;
        self.meleeHitChunk = null;
        #endregion

        if (self.AbstrLillyPuck.bites >= 3) //all this just to change this one operator -_- talk about compromising compatibility right?
        {
            self.room?.PlaySound(SoundID.Slugcat_Throw_Spear, self.firstChunk);
            self.ChangeMode(Weapon.Mode.Thrown);
            return;
        }
        self.room?.PlaySound(SoundID.Slugcat_Throw_Rock, self.firstChunk);
        self.spinning = true;
        self.firstChunk.vel *= Mathf.Lerp(0.3f, 0.75f, Mathf.InverseLerp(0f, 6f, self.AbstrLillyPuck.bites)); //and this i guess
        self.ChangeMode(Weapon.Mode.Free);
    }

    private void LillyPuck_BitByPlayer(On.MoreSlugcats.LillyPuck.orig_BitByPlayer orig, LillyPuck self, Creature.Grasp grasp, bool eu)
    {
        if (self.GetFroot().isJumbo)
        {
            if (self.AbstrLillyPuck.bites == 1)
            {
                if (!FrootOptions.vanillaBehavior.Value)
                {
                    (grasp.grabber as Player).GetCat().swell = true;
                    (grasp.grabber as Player).GetCat().bloatTimer = 15f;
                }
                if (FrootOptions.noVisuals.Value)
                    (grasp.grabber as Player).AddFood(4);
            }
            else if (self.AbstrLillyPuck.bites <= 5 && !FrootOptions.noVisuals.Value)
            {
                (grasp.grabber as Player).AddFood(1);
            }
        }

        orig(self, grasp, eu);
    }

    private void LillyPuck_InitiateSprites(On.MoreSlugcats.LillyPuck.orig_InitiateSprites orig, LillyPuck self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        orig(self, sLeaser, rCam);

        if (self.GetFroot().isJumbo && !FrootOptions.noVisuals.Value)
        {
            sLeaser.sprites[0].scaleX *= 1.95f;
            for (int i = 1; i < self.flowerLeavesCount+1; i++)
            {
                sLeaser.sprites[i].scaleX *= 1.2f;
                sLeaser.sprites[i].scaleY *= 1.1f;
            }
        }
    }

    private void LillyPuck_ApplyPalette(On.MoreSlugcats.LillyPuck.orig_ApplyPalette orig, LillyPuck self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        orig(self, sLeaser, rCam, palette);

        if (self.GetFroot().isJumbo && !FrootOptions.noVisuals.Value)
        {
            self.color = Color.Lerp(new Color(0.04f, 0.5f, 0.25f), palette.blackColor, self.darkness);
            sLeaser.sprites[0].color = self.color;
            
            for (int i = 0; i < self.flowerLeavesCount; i++)
            {
                Color a = Color.Lerp(self.color, self.flowerColor, Mathf.Clamp(self.lightFade, 0.3f - 0.3f * self.darkness, 1f));
                sLeaser.sprites[1 + i * 2].color = Color.Lerp(a, self.color, (float)i / ((float)self.flowerLeavesCount / 2f));
                sLeaser.sprites[2 + i * 2].color = Color.Lerp(sLeaser.sprites[1 + i * 2].color, Color.Lerp(new Color(1f, 1f, 1f), palette.blackColor, self.darkness), self.darkness / 20f);
            }
        }
    }
    #endregion
    #region dandelion peach (TODO: instead of making you heavier (requires rotund world) it makes you lighter for a bit)
    private void DandelionPeach_ctor(On.MoreSlugcats.DandelionPeach.orig_ctor orig, DandelionPeach self, AbstractPhysicalObject abstractPhysicalObject)
    {
        orig(self, abstractPhysicalObject);

        if (self.GetFroot().isJumbo && !FrootOptions.noVisuals.Value)
        {
            self.bodyChunks[0].mass *= 2f;
            self.bites = 6;
        }
    }

    private void DandelionPeach_BitByPlayer(On.MoreSlugcats.DandelionPeach.orig_BitByPlayer orig, DandelionPeach self, Creature.Grasp grasp, bool eu)
    {
        if (self.GetFroot().isJumbo)
        {
            if (self.bites == 1)
            {
                if (!FrootOptions.vanillaBehavior.Value)
                {
                    (grasp.grabber as Player).GetCat().swell = true;
                    (grasp.grabber as Player).GetCat().bloatTimer = 15f;
                }
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

    private void DandelionPeach_InitiateSprites(On.MoreSlugcats.DandelionPeach.orig_InitiateSprites orig, DandelionPeach self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        self.puffCount = Mathf.Max(7, (int)(Random.value * 10f));

        orig(self, sLeaser, rCam);

        if (self.GetFroot().isJumbo && !FrootOptions.noVisuals.Value)
        {
            for (int i = 0; i < 2; i++)
            {
                sLeaser.sprites[i].scaleX *= 1.8f;
                sLeaser.sprites[i].scaleY *= 1.6f;
            }
            for (int i = 0; i < self.puffCount; i++)
            {
                sLeaser.sprites[3 + i].scale = 1.7f + Mathf.Sin(self.AbstrConsumable.ID.RandomSeed + i * self.puffCount) / 10f;
            }
        }
    }

    private void DandelionPeach_ApplyPalette(On.MoreSlugcats.DandelionPeach.orig_ApplyPalette orig, DandelionPeach self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        orig(self, sLeaser, rCam, palette);

        if (self.GetFroot().isJumbo && !FrootOptions.noVisuals.Value)
        {
            self.color = Color.Lerp(new Color(0.69f, 0.59f, 0.96f), palette.blackColor, Mathf.Pow(self.darkness, 2f));
            sLeaser.sprites[0].color = self.color;
            sLeaser.sprites[1].color = Color.Lerp(Color.Lerp(palette.fogColor, new Color(1f, 1f, 1f), 0.5f), palette.blackColor, self.darkness);
            sLeaser.sprites[2].color = Color.Lerp(self.color, sLeaser.sprites[1].color, 0.1f);
            //todo: change the stalk color to match these. good luck lmfaooo
        }
    }
    #endregion
    #region glow weed       (has the unintentional behavior of glowing A LOT and apparently being very sought after by scavs. rumor has it they sell up to over half a pearl...)
    private void GlowWeed_ctor(On.MoreSlugcats.GlowWeed.orig_ctor orig, GlowWeed self, AbstractPhysicalObject abstractPhysicalObject)
    {
        orig(self, abstractPhysicalObject);
        if (self.GetFroot().isJumbo && !FrootOptions.noVisuals.Value)
        {
            self.bodyChunks[0].mass *= 3f;
            self.bites = 6;
            self.bounce = 0.4f;
        }
    }

    private void GlowWeed_Update(On.MoreSlugcats.GlowWeed.orig_Update orig, GlowWeed self, bool eu)
    {
        orig(self, eu);
        //make glow weed squishy!
        self.GetFroot().lastProp = self.GetFroot().prop;
        self.GetFroot().prop += self.GetFroot().propSpeed;
        self.GetFroot().propSpeed *= 0.85f;
        self.GetFroot().propSpeed -= self.GetFroot().prop / 10f;
        self.GetFroot().prop = Mathf.Clamp(self.GetFroot().prop, -15f, 15f);
        if (self.grabbedBy.Count == 0)
        {
            self.GetFroot().prop += (self.firstChunk.lastPos.x - self.firstChunk.pos.x) / 15f;
            self.GetFroot().prop -= (self.firstChunk.lastPos.y - self.firstChunk.pos.y) / 15f;
        }
        self.GetFroot().lastPlop = self.GetFroot().plop;
        if (self.GetFroot().plop > 0f && self.GetFroot().plop < 1f)
        {
            self.GetFroot().plop = Mathf.Min(1f, self.GetFroot().plop + 0.1f);
        }
    }

    private void GlowWeed_TerrainImpact(On.PhysicalObject.orig_TerrainImpact orig, PhysicalObject self, int chunk, IntVector2 direction, float speed, bool firstContact)
    {
        //make glow weed squishy!
        orig(self, chunk, direction, speed, firstContact);
        if (self is GlowWeed)
        {
            if (direction.y != 0)
            {
                (self as GlowWeed).GetFroot().prop += speed;
                (self as GlowWeed).GetFroot().propSpeed += speed / 10f;
            }
            else
            {
                (self as GlowWeed).GetFroot().prop -= speed;
                (self as GlowWeed).GetFroot().propSpeed -= speed / 10f;
            }
            if (speed > 1.2f && firstContact)
            {
                Vector2 pos = (self as GlowWeed).firstChunk.pos + direction.ToVector2() * (self as GlowWeed).firstChunk.rad;
                for (int i = 0; i < Mathf.RoundToInt(Custom.LerpMap(speed, 1.2f, 6f, 2f, 5f, 1.2f)); i++)
                {
                    (self as GlowWeed).room.AddObject(new WaterDrip(pos, Custom.RNV() * (2f + speed) * UnityEngine.Random.value * 0.5f + -direction.ToVector2() * (3f + speed) * 0.35f, waterColor: true));
                }
                (self as GlowWeed).room.PlaySound(SoundID.Swollen_Water_Nut_Terrain_Impact, pos, Custom.LerpMap(speed, 1.2f, 6f, 0.2f, 1f), 1f);
            }
        }
    }

    private void GlowWeed_BitByPlayer(On.MoreSlugcats.GlowWeed.orig_BitByPlayer orig, GlowWeed self, Creature.Grasp grasp, bool eu)
    {
        if (self.GetFroot().isJumbo)
        {
            if (self.bites == 1)
            {
                if (!FrootOptions.vanillaBehavior.Value)
                {
                    (grasp.grabber as Player).GetCat().swell = true;
                    (grasp.grabber as Player).GetCat().bloatTimer = 15f;
                }
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

    private void GlowWeed_InitiateSprites(On.MoreSlugcats.GlowWeed.orig_InitiateSprites orig, GlowWeed self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        orig(self, sLeaser, rCam);

        if (self.GetFroot().isJumbo && !FrootOptions.noVisuals.Value)
        {
            for (int i = 0; i < 5; i++)
            {
                sLeaser.sprites[i].scaleX *= 1.8f;
                sLeaser.sprites[i].scaleY *= 1.55f;
            }
        }
    }

    //full rewrite of the drawing function to make them squish!
    private void GlowWeed_DrawSprites(On.MoreSlugcats.GlowWeed.orig_DrawSprites orig, GlowWeed self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        if (self.GetFroot().isJumbo && !FrootOptions.noVisuals.Value)
        {
            float scaleX = 1.8f;
            float scaleY = 1.55f;
            float num = (float)self.bites / 6f;
            Vector2 vector = Vector2.Lerp(self.firstChunk.lastPos, self.firstChunk.pos, timeStacker);
            Vector2 vector2 = Vector3.Slerp(self.lastRotation, self.rotation, timeStacker);
            sLeaser.sprites[0].x = vector.x - camPos.x;
            sLeaser.sprites[0].y = vector.y - camPos.y;
            sLeaser.sprites[0].rotation = Custom.VecToDeg(vector2);
            sLeaser.sprites[0].alpha = 0.6f + rCam.PaletteDarkness() / 2f;
            float num2 = Mathf.Lerp(self.GetFroot().lastPlop, self.GetFroot().plop, timeStacker);
            num2 = Mathf.Lerp(0f, 1f + Mathf.Sin(num2 * (float)System.Math.PI), num2);
            sLeaser.sprites[0].scaleX = (1.2f * scaleX * num + Mathf.Lerp(self.GetFroot().lastProp, self.GetFroot().prop, timeStacker) / 20f) * num2;
            sLeaser.sprites[0].scaleY = (1.6f * scaleY * num - Mathf.Lerp(self.GetFroot().lastProp, self.GetFroot().prop, timeStacker) / 20f) * num2;
            sLeaser.sprites[1].x = vector.x - camPos.x;
            sLeaser.sprites[1].y = vector.y - camPos.y;
            sLeaser.sprites[1].rotation = Custom.VecToDeg(vector2);
            sLeaser.sprites[1].scaleX = 0.9f * scaleX * num;
            sLeaser.sprites[1].scaleY = 1.3f * scaleY * num;
            sLeaser.sprites[2].x = vector.x - camPos.x;
            sLeaser.sprites[2].y = vector.y - camPos.y;
            sLeaser.sprites[2].rotation = Custom.VecToDeg(vector2);
            sLeaser.sprites[2].scaleX = 0.9f * scaleX * num;
            sLeaser.sprites[2].scaleY = 1.3f * scaleY * num;
            if (self.blink > 0 && UnityEngine.Random.value < 0.5f)
            {
                sLeaser.sprites[1].color = self.blinkColor;
                sLeaser.sprites[2].color = self.blinkColor;
                sLeaser.sprites[3].color = self.blinkColor;
                sLeaser.sprites[4].color = self.blinkColor;
            }
            else
            {
                sLeaser.sprites[1].color = self.color;
                sLeaser.sprites[2].color = self.color;
                sLeaser.sprites[3].color = Color.Lerp(self.color, rCam.currentPalette.blackColor, 0.4f);
                sLeaser.sprites[4].color = Color.Lerp(self.color, rCam.currentPalette.blackColor, 0.4f);
            }
            vector2 = Custom.DirVec(default(Vector2), vector2);
            sLeaser.sprites[3].x = vector.x + vector2.x * (10f * num) - camPos.x;
            sLeaser.sprites[3].y = vector.y + vector2.y * (10f * num) - camPos.y;
            sLeaser.sprites[3].rotation = sLeaser.sprites[0].rotation;
            sLeaser.sprites[4].x = vector.x + vector2.x * (-10f * num) - camPos.x;
            sLeaser.sprites[4].y = vector.y + vector2.y * (-10f * num) - camPos.y;
            sLeaser.sprites[4].rotation = sLeaser.sprites[0].rotation;
            if (self.slatedForDeletetion || self.room != rCam.room)
            {
                sLeaser.CleanSpritesAndRemove();
            }
        }
        else
        {
            //orig(self, sLeaser, rCam, timeStacker, camPos);
            float num = (float)self.bites / 3f;
            Vector2 vector = Vector2.Lerp(self.firstChunk.lastPos, self.firstChunk.pos, timeStacker);
            Vector2 vector2 = Vector3.Slerp(self.lastRotation, self.rotation, timeStacker);
            sLeaser.sprites[0].x = vector.x - camPos.x;
            sLeaser.sprites[0].y = vector.y - camPos.y;
            sLeaser.sprites[0].rotation = Custom.VecToDeg(vector2);
            sLeaser.sprites[0].alpha = 0.6f + rCam.PaletteDarkness() / 2f;
            sLeaser.sprites[0].scaleX = 1.2f * num;
            sLeaser.sprites[0].scaleY = 1.6f * num;
            float num2 = Mathf.Lerp(self.GetFroot().lastPlop, self.GetFroot().plop, timeStacker);
            num2 = Mathf.Lerp(0f, 1f + Mathf.Sin(num2 * (float)System.Math.PI), num2);
            sLeaser.sprites[0].scaleX = (1.2f * num + Mathf.Lerp(self.GetFroot().lastProp, self.GetFroot().prop, timeStacker) / 20f) * num2;
            sLeaser.sprites[0].scaleY = (1.6f * num - Mathf.Lerp(self.GetFroot().lastProp, self.GetFroot().prop, timeStacker) / 20f) * num2;
            sLeaser.sprites[1].x = vector.x - camPos.x;
            sLeaser.sprites[1].y = vector.y - camPos.y;
            sLeaser.sprites[1].rotation = Custom.VecToDeg(vector2);
            sLeaser.sprites[1].scaleX = 0.9f * num;
            sLeaser.sprites[1].scaleY = 1.3f * num;
            sLeaser.sprites[2].x = vector.x - camPos.x;
            sLeaser.sprites[2].y = vector.y - camPos.y;
            sLeaser.sprites[2].rotation = Custom.VecToDeg(vector2);
            sLeaser.sprites[2].scaleX = 0.9f * num;
            sLeaser.sprites[2].scaleY = 1.3f * num;
            if (self.blink > 0 && UnityEngine.Random.value < 0.5f)
            {
                sLeaser.sprites[1].color = self.blinkColor;
                sLeaser.sprites[2].color = self.blinkColor;
                sLeaser.sprites[3].color = self.blinkColor;
                sLeaser.sprites[4].color = self.blinkColor;
            }
            else
            {
                sLeaser.sprites[1].color = self.color;
                sLeaser.sprites[2].color = self.color;
                sLeaser.sprites[3].color = Color.Lerp(self.color, rCam.currentPalette.blackColor, 0.4f);
                sLeaser.sprites[4].color = Color.Lerp(self.color, rCam.currentPalette.blackColor, 0.4f);
            }
            vector2 = Custom.DirVec(default(Vector2), vector2);
            sLeaser.sprites[3].x = vector.x + vector2.x * (10f * num) - camPos.x;
            sLeaser.sprites[3].y = vector.y + vector2.y * (10f * num) - camPos.y;
            sLeaser.sprites[3].rotation = sLeaser.sprites[0].rotation;
            sLeaser.sprites[4].x = vector.x + vector2.x * (-10f * num) - camPos.x;
            sLeaser.sprites[4].y = vector.y + vector2.y * (-10f * num) - camPos.y;
            sLeaser.sprites[4].rotation = sLeaser.sprites[0].rotation;
            if (self.slatedForDeletetion || self.room != rCam.room)
            {
                sLeaser.CleanSpritesAndRemove();
            }
        }
    }

    private void GlowWeed_ApplyPalette(On.MoreSlugcats.GlowWeed.orig_ApplyPalette orig, GlowWeed self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        orig(self, sLeaser, rCam, palette);

        if (self.GetFroot().isJumbo && !FrootOptions.noVisuals.Value)
        {
            self.color = new Color(0.7f, 1f, 0.5f);
            sLeaser.sprites[0].color = Color.Lerp(palette.waterColor1, palette.waterColor2, 0.5f);
        }
    }
    #endregion
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

        public float prop;

        public float lastProp;

        public float propSpeed;

        public float plop;

        public float lastPlop;

        public FrootStatus(PlayerCarryableItem fruit)
        {
            if (fruit is DangleFruit or WaterNut or SwollenWaterNut or GooieDuck or LillyPuck or DandelionPeach or GlowWeed) //should this even be checked for??
            {
                Random.State state = Random.state;
                Random.InitState(fruit.abstractPhysicalObject.ID.RandomSeed);

                // UnityEngine.Random.seed = self.abstractPhysicalObject.ID.RandomSeed;
                if (fruit is DangleFruit)
                    this.isJumbo = UnityEngine.Random.value < FrootOptions.spawnChanceDangleFruit.Value;
                if (fruit is WaterNut or SwollenWaterNut)
                    this.isJumbo = UnityEngine.Random.value < FrootOptions.spawnChanceWaterNut.Value;
                if (fruit is GooieDuck)
                    this.isJumbo = UnityEngine.Random.value < FrootOptions.spawnChanceGooieDuck.Value;
                if (fruit is LillyPuck)
                    this.isJumbo = UnityEngine.Random.value < FrootOptions.spawnChanceLillyPuck.Value;
                if (fruit is DandelionPeach)
                    this.isJumbo = UnityEngine.Random.value < FrootOptions.spawnChanceDandelionPeach.Value;
                if (fruit is GlowWeed)
                {
                    this.prop = 0f;
                    this.lastProp = 0f;
                    this.plop = 1f;
                    this.lastPlop = 1f;
                    this.isJumbo = UnityEngine.Random.value < FrootOptions.spawnChanceGlowWeed.Value;
                }
                Random.state = state;
            }
        }
    }

    // This part lets you access the stored stuff by simply doing "self.GetFroot()" in Plugin.cs or everywhere else!
    private static readonly ConditionalWeakTable<PlayerCarryableItem, FrootStatus> CWT = new();
    public static FrootStatus GetFroot(this PlayerCarryableItem fruit) => CWT.GetValue(fruit, _ => new(fruit));
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