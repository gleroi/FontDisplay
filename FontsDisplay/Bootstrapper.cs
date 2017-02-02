using System.Dynamic;
using System.Windows;
using Caliburn.Micro;

namespace FontsDisplay
{
    public class Bootstrapper : BootstrapperBase
    {
        public Bootstrapper()
        {
            this.Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            dynamic settings = new ExpandoObject();
            settings.Width = 1280;
            settings.Height = 768;
            settings.SizeToContent = SizeToContent.Manual;
            this.DisplayRootViewFor<FontViewModel>(settings);
        }
    }
}