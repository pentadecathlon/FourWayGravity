using Celeste;
using Microsoft.Xna.Framework;
using Monocle;

public class SolidJumpThru : Solid {
	JumpThru source;
    public Rectangle oldBounds;
	public SolidJumpThru(JumpThru source) : base(source.Position, source.Width, source.Height, source.Safe) {
		this.source = source;
        oldBounds = Collider.Bounds;
	}
	public override void Update() {
		Collider.Width = source.Width;
		Collider.Height = source.Height;
        oldBounds = Collider.Bounds;
        MoveToX(source.Left);
        MoveToY(source.Top);
        oldBounds = Collider.Bounds;
	}
}

