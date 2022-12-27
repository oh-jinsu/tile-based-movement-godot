using Godot;

namespace Game
{
    public class SplashController : BaseNode
    {
        public override void _Ready()
        {
            var accessToken = Application.AuthRepository.GetAccessToken();

            if (accessToken == null)
            {
                Navigator.GoTo(Scene.Creation);
            }
            else
            {
                Navigator.GoTo(Scene.Game);
            }
        }
    }
}
