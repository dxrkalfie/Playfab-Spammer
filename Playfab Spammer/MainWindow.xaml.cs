using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PlayFabSpammerWPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (e.ButtonState == MouseButtonState.Pressed)
                this.DragMove();
        }

        private async void StartPlayFabAction(object sender, RoutedEventArgs e)
        {
            string titleId = TitleIdTextBox.Text.Trim();

            if (!int.TryParse(NumberOfUsersTextBox.Text.Trim(), out int numberOfUsers) || numberOfUsers <= 0)
            {
                AppendLog("Please enter a valid number of users.");
                return;
            }

            if (string.IsNullOrEmpty(titleId))
            {
                AppendLog("Title ID is required.");
                return;
            }

            PlayFabSettings.staticSettings.TitleId = titleId;

            StartActionButton.IsEnabled = false;
            AppendLog($"Creating {numberOfUsers} test users...");

            try
            {
                for (int i = 0; i < numberOfUsers; i++)
                {
                    string randomName = "Dxrk_FuckYou" + Guid.NewGuid().ToString("N").Substring(0, 8);
                    var request = new LoginWithCustomIDRequest
                    {
                        CustomId = randomName,
                        CreateAccount = true,
                        InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
                        {
                            GetUserAccountInfo = true
                        }
                    };

                    var result = await PlayFabClientAPI.LoginWithCustomIDAsync(request);

                    if (result.Error != null)
                        AppendLog($"[Error] {result.Error.GenerateErrorReport()}");
                    else
                        AppendLog($"[Success] Created user: {randomName}");

                    await Task.Delay(2000);
                }

                AppendLog("Finished creating test users.");
            }
            catch (Exception ex)
            {
                AppendLog($"Exception: {ex.Message}");
            }
            finally
            {
                StartActionButton.IsEnabled = true;
            }
        }

        private void AppendLog(string message)
        {
            LogTextBox.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}\n");
            LogTextBox.ScrollToEnd();
        }
    }
}
