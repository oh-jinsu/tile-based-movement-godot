namespace Game
{
    public class SplashSceneController : BaseNode
    {
        public override void _Ready()
        {
            var accessToken = Application.AuthRepository.GetAccessToken();

            if (accessToken == null)
            {
                Navigator.GoToCreationScene();
            }
            else
            {
                StartGame();
            }
        }

        private void StartGame()
        {
            if (SocketClient.Connect())
            {
                WindowController.PopupDialog("서버를 연결할 수 없습니다.");

                return;
            }

            var token = Application.AuthRepository.GetAccessToken();

            var hello = new Packet.Hello
            {
                token = token,
            };

            var data = hello.Serialize();

            SocketClient.Write(data);

            // Navigator.GoToGameScene(new GameSceneArguments());
        }

        private void Reset()
        {
            Application.AuthRepository.DeleteAccessToken();
        }
    }
}
