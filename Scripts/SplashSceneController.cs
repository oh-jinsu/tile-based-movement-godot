using Godot;

namespace Game
{   
    public class SplashSceneController : BaseNode
    {
        public override void _Ready()
        {
            SocketClient.Subscribe(OnPacketReceived);

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
            ThreadPool.Spawn(() => {
                if (!SocketClient.Connect()) {
                    WindowController.PopupDialog("서버를 연결할 수 없습니다.");

                    return;
                }

                var token = Application.AuthRepository.GetAccessToken();

                var hello = new Network.Outgoing.Hello
                {
                    token = token,
                };

                SocketClient.Write(hello.Serialize());
            });
        }

        private void OnPacketReceived(Network.Incoming.Packet packet) {
            if (packet is Network.Incoming.Hello hello) {
                var arguments = new GameSceneArguments {
                    hello = hello,
                };

                Navigator.GoToGameScene(arguments);
            }
        }

        private void Reset()
        {
            Application.AuthRepository.DeleteAccessToken();
        }

        public override void _ExitTree()
        {
            SocketClient.Unsubscribe(OnPacketReceived);
        }
    }
}
