using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrincipleCurvatureCrystall_Growth.ThresholdAttribute
{
    public abstract class ThresholdAttributeBase
    {
        public abstract string Name { get;}
        public abstract string Description { get; }

        public override string ToString()
            => Name + ": " + Description;
        public abstract void ThresholdAction(Molecule molecule);
        public override bool Equals(object obj)
            => Name == ((ThresholdAttributeBase)obj).Name;
        public override int GetHashCode()
            => base.GetHashCode();
    }
}
