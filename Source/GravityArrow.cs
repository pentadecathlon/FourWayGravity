using Celeste;
using Celeste.Mod;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

[CustomEntity("GravityHelperGV/GravityArrow")]
public class GravityArrow : Entity {
	Gravity gravity;
	Vector2 offset;
	public GravityArrow(EntityData data, Vector2 offset) {
		Position = data.Position + offset;
		gravity = data.Attr("direction", "down") switch {
			"left" => Gravity.Left,
			"down" => Gravity.Down,
			"right" => Gravity.Right,
			"up" => Gravity.Up,
			_ => Gravity.Down
		};
		Collider = new Hitbox(14f, 14f, -7f, -7f);
	}
	public override void Awake(Scene scene) {
		foreach(Entity entity in scene) {
			if(CollideCheck(entity)) {
				if(entity is not GravityArrow) {
					var comp = new GravityArrowComponent(gravity, Position - entity.Position);
					entity.Add(comp);
					break;
				}
			}	
		}
		RemoveSelf();
	}
	public static void ApplyArrows(Entity arrows, Entity gravity) {
		var prevGravity = Gravity.Down;
		var gravityComp = gravity.Components.Get<GravityComponent>();
		if(gravityComp != null) {
			prevGravity = gravityComp.gravity;
		}
		foreach(var arrow in arrows.Components.GetAll<GravityArrowComponent>()) {
			if(arrow.gravity != prevGravity) {
				GravityComponent.Set(gravity, arrow.gravity);
				return;
			}
		}
	}
}
public class GravityArrowComponent : Component
{
	public Gravity gravity;
	Vector2 offset;
    public GravityArrowComponent(Gravity gravity, Vector2 offset) : base(false, true)
    {
		this.gravity = gravity;
		this.offset = offset;
    }
	public override void Added(Entity entity) {
		var img = new Image(GFX.Game["objects/GravityHelperGV/gravityArrow"]);
		img.CenterOrigin();
		img.Position = offset;
		img.Rotation = gravity.Angle() + 3.141f;
		img.Color = gravity switch {
			Gravity.Left => new Color(0f, 1f, 0f, 1f),
			Gravity.Right => new Color(1f, 1f, 0f, 1f),
			Gravity.Up => new Color(1f, 0f, 0f, 1f),
			_ => new Color(0f, 0f, 1f, 1f)
		};
		entity.Add(img);
	}
}