using Godot;

namespace Game
{
    public class Global
    {
        private Node node;

        private Global() { }

        public static Global Of(Node node)
        {
            return new Global { node = node };
        }

        public Application Application
        {
            get
            {
                return node.GetNode<Application>(Application.NODE_PATH);
            }
        }

        public Navigator Navigator
        {
            get
            {
                return node.GetNode<Navigator>(Navigator.NODE_PATH);
            }
        }

        public Network.HttpClient HttpClient
        {
            get
            {
                return node.GetNode<Network.HttpClient>(Network.HttpClient.NODE_PATH);
            }
        }

        public Network.SocketClient SocketClient
        {
            get
            {
                return node.GetNode<Network.SocketClient>(Network.SocketClient.NODE_PATH);
            }
        }

        public ThreadPool ThreadPool
        {
            get
            {
                return node.GetNode<ThreadPool>(ThreadPool.NODE_PATH);
            }
        }

        public WindowController WindowController
        {
            get
            {
                return node.GetNode<WindowController>(WindowController.NODE_PATH);
            }
        }
    }

}
