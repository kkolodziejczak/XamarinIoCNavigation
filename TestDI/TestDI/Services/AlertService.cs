using TestDI.Interfaces;

namespace TestDI.Services
{
    public class AlertService : IAlertService
    {
        public string Init { get; protected set; }

        public AlertService()
        {
            Init = "Good!";
        }
    }
}
