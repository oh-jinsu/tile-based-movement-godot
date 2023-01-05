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

        protected HttpClient HttpClient
        {
            get
            {
                return GetNode<HttpClient>(HttpClient.NODE_PATH);
            }
        }

        protected SocketClient SocketClient
        {
            get
            {
                return GetNode<SocketClient>(SocketClient.NODE_PATH);
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
