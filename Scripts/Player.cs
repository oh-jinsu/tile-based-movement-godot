using Godot;

namespace Game {
    public class Player : Spatial {
        private AnimationPlayer animationPlayer;

        public override void _Ready()
        {
            animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

            var animation = animationPlayer.GetAnimation("idle");

            animation.Loop = true;

            animationPlayer.Play("idle");
        }
    }
}