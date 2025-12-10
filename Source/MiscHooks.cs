using System;
using System.Reflection;
using Celeste;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;

public class MiscHooks
{
    static ILHook MoveBlockController;
    public static void Load()
    {
        On.Celeste.BounceBlock.WindUpPlayerCheck += WindUpPlayerCheck;
        On.Celeste.DashBlock.OnDashed += DashBlockHook;
        On.Monocle.Entity.Awake += GetArrows;
        On.Celeste.Bumper.OnPlayer += BumperHook;
        On.Monocle.Entity.Added += JumpThruAddedHook;
        MoveBlockController = new ILHook(typeof(MoveBlock).GetMethod("Controller", BindingFlags.NonPublic | BindingFlags.Instance).GetStateMachineTarget(), ControllerHook);
    }

    public static void Unload()
    {

        On.Celeste.BounceBlock.WindUpPlayerCheck -= WindUpPlayerCheck;
        On.Celeste.DashBlock.OnDashed -= DashBlockHook;
        On.Monocle.Entity.Awake -= GetArrows;
        On.Celeste.Bumper.OnPlayer -= BumperHook;
        On.Monocle.Entity.Added -= JumpThruAddedHook;
        MoveBlockController?.Dispose();
    }

    private static void JumpThruAddedHook(On.Monocle.Entity.orig_Added orig, Entity self, Scene scene)
    {
        if(self is JumpThru jt)
            scene.Add(new SolidJumpThru(jt));
        orig(self, scene);
    }

    private static void BumperHook(On.Celeste.Bumper.orig_OnPlayer orig, Bumper self, Player player)
    {
        if(self.respawnTimer <= 0f) {
            GravityArrow.ApplyArrows(self, player);
        }
        orig(self, player);
    }

    private static DashCollisionResults DashBlockHook(On.Celeste.DashBlock.orig_OnDashed orig, DashBlock self, Player player, Vector2 direction)
    {
        var result = orig(self, player, direction);
        if (result == DashCollisionResults.Rebound)
        {
            GravityArrow.ApplyArrows(self, player);
        }
        return result;
    }

    private static Celeste.Player WindUpPlayerCheck(On.Celeste.BounceBlock.orig_WindUpPlayerCheck orig, Celeste.BounceBlock self)
    {
        var player = self.Scene.Tracker.GetEntity<Player>();
        if (player != null && player.Collider is not TransformCollider)
            return orig(self);
        player = self.GetPlayerOnTop();
        if (player == null || (player != null && player.Speed.Y < 0f))
        {
            player = self.GetPlayerClimbing();
        }
        return player;
    }
    private static void ControllerHook(ILContext il)
    {
        var cursor = new ILCursor(il);
        cursor.GotoNext(MoveType.After, i => i.MatchCallvirt<Solid>("HasPlayerOnTop"));
        cursor.EmitLdloc1();
        cursor.EmitDelegate(InvertPlayerOnTop);
        cursor.GotoNext(MoveType.After, i => i.MatchCallvirt<Solid>("HasPlayerClimbing"));
        cursor.EmitLdloc1();
        cursor.EmitDelegate(InvertPlayerClimbing);
    }
    private static bool InvertPlayerOnTop(bool cond, MoveBlock block)
    {
        var player = block.Scene.Tracker.GetEntity<Player>();
        if (player?.Collider is TransformCollider transformCollider)
        {
            if (transformCollider.gravity.gravity.Horizontal())
            {
                return block.HasPlayerClimbing();
            }
        }
        return cond;
    }
    private static bool InvertPlayerClimbing(bool cond, MoveBlock block)
    {
        var player = block.Scene.Tracker.GetEntity<Player>();
        if (player?.Collider is TransformCollider transformCollider)
        {
            if (transformCollider.gravity.gravity.Horizontal())
            {
                return block.HasPlayerOnTop();
            }
        }
        return cond;
    }

    private static void GetArrows(On.Monocle.Entity.orig_Awake orig, Monocle.Entity self, Monocle.Scene scene)
    {
        orig(self, scene);
        if (self is Spring || self is DashBlock || self is Bumper)
        {
            GravityArrow.GetArrows(self);
        }
    }
}
