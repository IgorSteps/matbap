using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace app
{
    public class ExpressionViewModel : ObservableObject
    {
        private readonly IEvaluator _evaluator;
        public string _expression;
        private string _answer;
        private RelayCommand _evalauteCmd;

        public ExpressionViewModel(IEvaluator evaluator)
        {
            _evaluator = evaluator;
            _evalauteCmd = new RelayCommand(Evaluate);
        }

        public RelayCommand EvaluateCmd => _evalauteCmd;

        public string Expression
        {
            get => _expression;
            set => SetProperty(ref _expression, value);
        }

        public string Answer
        {
            get => _answer;
            set => SetProperty(ref _answer, value);
        }

        public void Evaluate()
        {
            var result = _evaluator.Evaluate(Expression);
            if(result.HasError)
            {
                Answer = result.Error.ToString();
                return;
            }

            Answer = result.Result;
        }

    }
}
