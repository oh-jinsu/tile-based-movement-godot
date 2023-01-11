using Godot;

namespace Game
{
    public class FollowingCamera : Camera
    {
        [Export]
        private int dinstance = 15;

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

            var degree = Mathf.Pi * RotationDegrees.x / 180;

            var y = -1 * dinstance * Mathf.Sin(degree);

            var x = dinstance * Mathf.Cos(degree);

            GlobalTranslation = Following.GlobalTranslation + new Vector3(0f, y, x);
        }
    }
}
