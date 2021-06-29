using System.ComponentModel;
using CodeMetrics.Calculators.Contracts;

namespace CodeMetrics.Calculators
{
    public class Complexity : IComplexity, INotifyPropertyChanged
    {
        private int value;
        public int Value
        {
            get { return value; }
            set
            {
                this.value = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(Value)));
            }
        }

        public Complexity(int value)
        {
            Value = value;
        }

        public event PropertyChangedEventHandler PropertyChanged = (sender, args) => { };

        public override string ToString()
        {
            return $"Complexity:{value}";
        }
    }
}