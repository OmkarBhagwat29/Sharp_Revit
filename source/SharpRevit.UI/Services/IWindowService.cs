
namespace SharpRevit.UI.Services
{
    public interface IWindowService
    {
        event EventHandler WindowOpened;
        void RaiseWindowOpened();
    }
}
