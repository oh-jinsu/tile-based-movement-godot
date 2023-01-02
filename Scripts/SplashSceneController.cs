namespace Game
{
    public class SplashSceneController : BaseNode
    {
        public override void _Ready()
        {
            Reset();

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

        private void Reset()
        {
            Application.AuthRepository.DeleteAccessToken();
        }
    }
}
