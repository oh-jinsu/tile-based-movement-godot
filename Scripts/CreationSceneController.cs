using Godot;

namespace Game
{
    using static Constant;

    public class CreationSceneController : Node
    {
        private LineEdit lineEdit;

        private Button button;

        private MutableObservable<string> username = new()
        {
            Value = ""
        };

        private MutableObservable<bool> pending = new()
        {
            Value = false
        };

        public override void _Ready()
        {
            InitializeLineEdit();

            InitializeButton();

            username.Subscribe(OnUsernameChanged);

            pending.Subscribe(OnPendingChanged);
        }

        private void InitializeLineEdit()
        {
            lineEdit = GetNode<LineEdit>("VBoxContainer/Control/VBoxContainer/LineEdit");

            lineEdit.Connect("text_changed", this, nameof(OnEditUsernameChanged));
        }

        private void InitializeButton()
        {
            button = GetNode<Button>("VBoxContainer/Control/VBoxContainer/Button");

            button.Connect("pressed", this, nameof(OnButtonPressed));

            button.Disabled = true;
        }

        private void OnEditUsernameChanged(string value)
        {
            username.Value = value;
        }

        private void OnUsernameChanged(string value)
        {
            button.Disabled = !IsValidUsername(value);
        }

        private static bool IsValidUsername(string value)
        {
            var regex = new System.Text.RegularExpressions.Regex(@"^(?=.*[a-zA-Z가-힣])[a-zA-Z가-힣]{2,8}$");

            if (!regex.IsMatch(value))
            {
                return false;
            }

            return true;
        }

        private void OnButtonPressed()
        {
            pending.Value = true;

            Global.Of(this).ThreadPool.Spawn(SaveAuth);
        }

        private async void SaveAuth()
        {
            var request = new Model.SaveAuthRequest
            {
                username = username.Value
            };

            var (ok, response) = await Global.Of(this).HttpClient.PostAsync(ApiUri("auth"), request);

            if (!ok)
            {
                var exception = Deserializer.FromJson<Model.Exception>(response);

                OnSaveAuthFailureResponse(exception);

                return;
            }

            var model = Deserializer.FromJson<Model.SaveAuthResponse>(response);

            OnSaveAuthSuccessResponse(model);
        }

        private void OnSaveAuthSuccessResponse(Model.SaveAuthResponse response)
        {
            Global.Of(this).Application.AuthRepository.SaveAccessToken(response.token);

            Global.Of(this).Navigator.GoToSplashScene();
        }

        private void OnSaveAuthFailureResponse(Model.Exception exception)
        {
            Global.Of(this).WindowController.PopupDialog(exception.message);

            pending.Value = false;
        }

        private void OnPendingChanged(bool pending)
        {
            if (pending)
            {
                button.Text = "잠시만 기다려 주세요...";

                button.Disabled = true;

                lineEdit.Editable = false;
            }
            else
            {
                button.Text = "게임 시작";

                button.Disabled = false;

                lineEdit.Editable = true;
            }
        }

        public override void _ExitTree()
        {
            lineEdit.Disconnect("text_changed", this, nameof(OnEditUsernameChanged));

            button.Disconnect("pressed", this, nameof(OnButtonPressed));

            pending.Unsubscribe(OnPendingChanged);

            username.Unsubscribe(OnUsernameChanged);
        }
    }

}
