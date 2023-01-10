namespace Game
{
    public class BaseNode : Godot.Node
    {
        protected Application Application
        {
            get
            {
                return GetNode<Application>(Application.NODE_PATH);
            }
        }

        protected Navigator Navigator
        {
            get
            {
                return GetNode<Navigator>(Navigator.NODE_PATH);
            }
        }

        protected Network.HttpClient HttpClient
        {
            get
            {
                return GetNode<Network.HttpClient>(Network.HttpClient.NODE_PATH);
            }
        }

        protected Network.SocketClient SocketClient
        {
            get
            {
                return GetNode<Network.SocketClient>(Network.SocketClient.NODE_PATH);
            }
        }

        protected ThreadPool ThreadPool
        {
            get
            {
                return GetNode<ThreadPool>(ThreadPool.NODE_PATH);
            }
        }

        protected WindowController WindowController
        {
            get
            {
                return GetNode<WindowController>(WindowController.NODE_PATH);
            }
        }
    }

}
