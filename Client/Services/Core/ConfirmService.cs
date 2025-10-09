namespace ServicesGateManagment.Client.Services.Core
{
    public class ConfirmService
    {
        public event Func<string, string, Task<bool>> OnConfirm;

        public async Task<bool> Confirm(string title, string message)
        {
            if (OnConfirm != null)
            {
                return await OnConfirm.Invoke(title, message);
            }
            return false;
        }
    }

}
