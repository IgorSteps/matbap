using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace app
{
    public class ExpressionViewModel : ObservableObject
    {
        private readonly IEvaluator _evaluator;
        private string _answer;
        private RelayCommand _evalauteCmd, _differentiateCmd;
        private string _expressionValue;

        public ExpressionViewModel(IEvaluator evaluator)
        {
            _evaluator = evaluator;
            _evalauteCmd = new RelayCommand(Evaluate);
            _differentiateCmd = new RelayCommand(Differentiate);
        }

        public RelayCommand EvaluateCmd => _evalauteCmd;

        public RelayCommand DifferentiateCmd => _differentiateCmd;

        public string Expression
        {
            get => _expressionValue;
            set => SetProperty(ref _expressionValue, value);
        }

        public string Answer
        {
            get => _answer;
            set => SetProperty(ref _answer, value);
        }

        public void Evaluate()
        {
            var result = _evaluator.Evaluate(_expressionValue);
            if(result.HasError)
            {
                Answer = result.Error.ToString();
                return;
            }

            Answer = result.Result;
        }

        public void Differentiate()
        {
            var result = _evaluator.Differentiate(_expressionValue);
            if (result.HasError)
            {
                Answer = result.Error.ToString();
                return;
            }

            Answer = result.Result;
        }

    }
}
