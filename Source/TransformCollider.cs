
using Celeste;
using Celeste.Mod;
using Microsoft.Xna.Framework;
using Monocle;

public class TransformCollider : Collider
{
    public Hitbox source;
    public Hitbox hitbox;
    public Vector2 offset;
    public GravityComponent gravity;
    public override float Width
    {
        get
        {
            if (gravity.Track)
            {
                return source.Width;
            }
            else
            {
                return hitbox.Width;
            }
        }
        set { }
    }
    public override float Height
    {
        get
        {
            if (gravity.Track)
            {
                return source.Height;
            }
            else
            {
                return hitbox.Height;
            }
        }
        set { }
    }
    public override float Top
    {
        get
        {
            if (gravity.Track)
            {
                return source.Top;
            }
            else
            {
                return hitbox.Top;
            }
        }
        set { }
    }
    public override float Bottom
    {
        get
        {
            if (gravity.Track)
            {
                return source.Bottom;
            }
            else
            {
                return hitbox.Bottom;
            }
        }
        set { }
    }
    public override float Left
    {
        get
        {
            if (gravity.Track)
            {
                return source.Left;
            }
            else
            {
                return hitbox.Left;
            }
        }
        set { }
    }
    public override float Right
    {
        get
        {
            if (gravity.Track)
            {
                return source.Right;
            }
            else
            {
                return hitbox.Right;
            }
        }
        set { }
    }
    public TransformCollider(Hitbox hitbox, Hitbox source, GravityComponent gravity)
    {
        this.hitbox = hitbox;
        this.gravity = gravity;
        this.source = source;
        offset = hitbox.Position;
    }
    public override void Added(Entity entity)
    {
        base.Added(entity);
        hitbox.Entity = Entity;
    }
    public void Update()
    {
        hitbox.Position = offset;
        if (!gravity.Track) return;
        var dif = Entity.Position - gravity.origin;
        hitbox.Position += -dif + dif.Rotate(gravity.gravity);
    }
    public override Collider Clone()
    {
        return new TransformCollider((Hitbox)hitbox.Clone(), source, gravity);
    }

    public override bool Collide(Circle circle)
    {
        Update();
        return hitbox.Collide(circle);
    }
    public override bool Collide(ColliderList list)
    {
        Update();
        return hitbox.Collide(list);
    }

    public override bool Collide(Vector2 point)
    {
        Update();
        return hitbox.Collide(point);
    }

    public override bool Collide(Rectangle rect)
    {
        Update();
        return hitbox.Collide(rect);
    }

    public override bool Collide(Vector2 from, Vector2 to)
    {
        Update();
        return hitbox.Collide(from, to);
    }

    public override bool Collide(Hitbox box)
    {
        if(box.Entity is JumpThru) return false;
        if(box.Entity is SolidJumpThru jt) {
            var old = Entity.Position;
            Entity.Position = gravity.origin;
            Update();
            var outside = hitbox.AbsoluteBottom <= jt.oldBounds.Top;
            Entity.Position = old;
            Update();
            return hitbox.Collide(box) && outside;
        }
        Update();
        return hitbox.Collide(box);
    }

    public override bool Collide(Grid grid)
    {
        Update();
        return hitbox.Collide(grid);
    }
    public override void Render(Camera camera, Color color)
    {
        Draw.HollowRect(hitbox.AbsoluteX, hitbox.AbsoluteY, hitbox.Width, hitbox.Height, Color.Red);
    }
}
