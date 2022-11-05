using MonogameBasicHelper.Attributes;
using MonogameBasicHelper.Concrete.Converters;
using MonogameBasicHelper.Events;
using MonogameBasicHelper.MVVM;
using System;
using System.Runtime.CompilerServices;

namespace MonogameBasicHelper.Test
{
    public class TestViewModel : IViewModel, IPropertyChanged
    {
        private double _D;
        private double _alpha;
        private double _beta;
        private double _A;

        private double _minorAxis;
        private double _majorAxis;

        private double _phi;
        private double _mu;
        private double _omega;

        private double _N;
        private double _L;
        private double _P;
        private double _W1;
        private double _W2;

        private double _numRevolutions;
        private double _thetaMin;

        [TextBoxElement(GroupName = "Params1")]
        public double D { get => _D; set
            {
                _D = value;
                Console.WriteLine(_D);
            }
        }

        [TextBoxElement(GroupName = "Params1")]
        public double Alpha { get => _alpha; set => _alpha = value; }

        [TextBoxElement(GroupName = "Params1")]
        public double Beta { get => _beta; set => _beta = value; }

        [TextBoxElement(GroupName = "Params1")]
        public double A { get => _A; set => _A = value; }


        [TextBoxElement(GroupName = "Params2")]
        public double MinorAxis
        {
            get => _minorAxis;

            set
            {
                _minorAxis = value;
                OnChanged();
            }
        }

        [TextBoxElement(GroupName = "Params2")]
        public double MajorAxis { get => _majorAxis; set => _majorAxis = value; }


        [TextBoxElement(GroupName = "Params3")]
        public double Phi { get => _phi; set => _phi = value; }
        [TextBoxElement(GroupName = "Params3")]
        public double Mu { get => _mu; set => _mu = value; }
        [TextBoxElement(GroupName = "Params3")]
        public double Omega { get => _omega; set => _omega = value; }

        [TrackBarElement(
            ValueToTextConverterType = typeof(DegreesTextConverter),
            ValueConverterType = typeof(DegreeToRadians),
            GroupName = "Params4", 
            Min = 0, 
            Max = 180, 
            Start = 5)]
        public double N
        {
            get => _N; set
            {
                _N = value;
                OnChanged();
            }
        }
     
        [TextBoxElement(GroupName = "Params4")]
        public double L { get => _L; set => _L = value; }

        [TrackBarElement(ValueToTextConverterType = typeof(DegreesTextConverter), GroupName = "Params4", Min = 0, Max = 100, Start = 5)]
        public double P { get => _P; 
            
            set => _P = value; }

        [TrackBarElement(ValueToTextConverterType = typeof(DegreesTextConverter), GroupName = "Params4", Min = 0, Max = 100, Start = 5 )]
        public double W1 { get => _W1; set => _W1 = value; }
     
        [TrackBarElement(ValueToTextConverterType = typeof(DegreesTextConverter), GroupName = "Params4", Min = 0, Max = 100, Start = 5)]
        public double W2 { get => _W2; set => _W2 = value; }

        [TextBoxElement(GroupName = "Params5")]
        public double NumRevolutions { get => _numRevolutions; set => _numRevolutions = value; }
        [TextBoxElement(GroupName = "Params5")]
        public double ThetaMin { get => _thetaMin; set => _thetaMin = value; }

        public event Action<string> PropertyChanged;

        private void OnChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(propertyName);
        }
    }
}
