namespace ServicesGateManagment.Client.Services
{
    public class AlertService
    {
        public event Action<string>? OnShow;

        public void Show(string message)
        {
            OnShow?.Invoke(message);
        }
    }
}
