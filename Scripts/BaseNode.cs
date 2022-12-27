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
    }

}
