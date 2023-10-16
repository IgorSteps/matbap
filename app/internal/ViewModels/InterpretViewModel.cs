using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;

namespace app
{
    public class InterpretationViewModel : ObservableObject
    {
        /// <summary>
        /// Interpretation model reference to math engine.
        /// </summary>
        private readonly InterpretationModel _model;
        
        /// <summary>
        /// Arithemtic expression passed into GUI.
        /// </summary>
        private string _expression;
       
        /// <summary>
        /// Response to the input expression:
        ///  - Can be an answer to an equation;
        ///  - Can be an error;
        /// </summary>
        private string _response;

        private readonly RelayCommand _interpretCmd;

        public InterpretationViewModel()
        {
            _model = new InterpretationModel();
            _interpretCmd = new RelayCommand(Interpret);
        }

        /// <summary>
        ///  Informs the view if the _expression changes.
        /// </summary>
        public string Expression
        {
            get => _expression;
            set => SetProperty(ref _expression, value);
        }

        /// <summary>
        ///  Informs the view if the _response changes.
        /// </summary>
        public string Response
        {
            get => _response;
            private set => SetProperty(ref _response, value);
        }

        /// <summary>
        ///  InterpretCmd binds to a button in the view, executes the Interpret() when clicked.
        /// </summary>
        public RelayCommand InterpretCmd => _interpretCmd;

        private void Interpret()
        {
            Response = _model.Interpret(Expression);
        }
    }
}
