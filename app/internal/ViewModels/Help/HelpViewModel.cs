using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Net.Http;

namespace app
{
    public class HelpViewModel : ObservableObject
    {

        private HelpContentModel _helpTextModel;

        public HelpContentModel HelpTexts => _helpTextModel;

        public HelpViewModel()
        {
            _helpTextModel = new HelpContentModel();
        }

    }
}
