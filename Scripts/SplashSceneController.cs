using Godot;

namespace Game
{   
    using Network;

    public class SplashSceneController : Node
    {
        private Application application;

        private SocketClient socketClient;

        public override void _Ready()
        {
            application = Global.Of(this).Application;

            socketClient = Global.Of(this).SocketClient;

            socketClient.Subscribe(OnPacketReceived);

            var accessToken = application.AuthRepository.GetAccessToken();

            if (accessToken == null)
            {
                Global.Of(this).Navigator.GoToCreationScene();
            }
            else
            {
                StartGame();
            }
        }

        private void StartGame()
        {
            Global.Of(this).ThreadPool.Spawn(() => {
                if (!socketClient.Connect()) {
                    Global.Of(this).WindowController.PopupDialog("서버를 연결할 수 없습니다.");

                    return;
                }

                var token = application.AuthRepository.GetAccessToken();

                var hello = new Network.Outgoing.Hello
                {
                    token = token,
                };

                socketClient.Write(hello.Serialize());
            });
        }

        private void OnPacketReceived(Network.Incoming.Packet packet) {
            if (packet is Network.Incoming.Hello hello) {
                var arguments = new GameSceneArguments {
                    hello = hello,
                };

                Global.Of(this).Navigator.GoToGameScene(arguments);
            }
        }

        private void Reset()
        {
            application.AuthRepository.DeleteAccessToken();
        }

        public override void _ExitTree()
        {
            socketClient.Unsubscribe(OnPacketReceived);
        }
    }
}
