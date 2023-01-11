using Godot;

namespace Game
{
    public class FollowingCamera : Camera
    {
        [Export]
        private int distance = 15;

        public Spatial Following { private get; set; }

        public void SetFollowing(Spatial node)
        {
            Following = node;
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            if (Following == null)
            {
                return;
            }

            var sign = Mathf.Sign(RotationDegrees.y);

            var degree = Mathf.Pi * RotationDegrees.x / 180;

            var y = -1 * distance * Mathf.Sin(degree) * 30 / Fov;

            var z = sign * distance * Mathf.Cos(degree) * 30 / Fov;

            GlobalTranslation = Following.GlobalTranslation + new Vector3(0f, y, z);
        }
    }
}
