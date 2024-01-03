using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace app
{
    public class ExpressionViewModel : ObservableObject
    {
        private readonly IEvaluator _evaluator;
        public Expression _expression;
        private string _answer;
        private RelayCommand _evalauteCmd;
        private string _expressionValue;

        public ExpressionViewModel(IEvaluator evaluator)
        {
            _evaluator = evaluator;
            _evalauteCmd = new RelayCommand(Evaluate);
        }


        public RelayCommand EvaluateCmd => _evalauteCmd;

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
            var result = _evaluator.Evaluate(_expression, _expressionValue);
            if(result.HasError)
            {
                Answer = result.Error.ToString();
                return;
            }

            Answer = result.Result;
        }

    }
}
