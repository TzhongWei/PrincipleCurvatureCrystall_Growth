using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrincipleCurvatureCrystall_Growth.ThresholdAttribute;

namespace PrincipleCurvatureCrystall_Growth
{
    public class Environment
    {
        public Surface Container { get; private set; }
        public double VibrationEnergy { get; private set; }
        public Molecule[] Molecules;
        public Environment(
            Surface Container, 
            double InitialEnergy, 
            int MoleculesCount,
            List<ThresholdAttributeBase> Actions 
            ) 
        {
            this.VibrationEnergy = InitialEnergy;
            this.Molecules = new Molecule[MoleculesCount];
            var ActionDic = new Dictionary<string, Molecule.ThresholdSetting>();
            for (int i = 0; i < Actions.Count; i++)
                ActionDic.Add(Actions[i].Name, Actions[i].ThresholdAction);
            for(int i = 0; i < MoleculesCount; i++) 
            {
                this.Molecules[i] = Molecule.CreateMolecule(this, ActionDic);
            }
        }
        public void SetStartCrystalPoints()
        {

        }
        public bool IterationOfEnergy()
        {
            //if vibrationEnergy is zero representing the environment freezed, not things can move.
            if (VibrationEnergy == 0) return true;
            return false;
        }
        public IEnumerable<Point3d> GetDisplayGeometry()
        {
            foreach (var point in Molecules)
            {
                yield return point.GetDisplayGeometry(this.Container);
            }
        }
    }
}
