using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrincipleCurvatureCrystal_Growth.ThresholdAttribute;

namespace PrincipleCurvatureCrystal_Growth
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
        public HashSet<Crystal> crystals { get; private set; } = null;
        public void SetStartCrystalPoints(int Count, int size = 40)
        {
            if (this.crystals.Count > 0 || this.crystals == null)
                this.crystals = new HashSet<Crystal>();
            //Randomly Pick the count of the crystal point
            for(int i = 0; i < Count; i++)
            {
                var Rand = new Random(DateTime.Now.Millisecond);
                var Item = this.Molecules[Rand.Next(0, this.Molecules.Length)];
                if(!crystals.Add(new Crystal(Item, size)))
                {
                    i--;
                }
            }
        }
        public void SetStartCrystalPoints(IEnumerable<Point3d> UVPoints, int size = 40)
        {
            if(this.crystals.Count > 0 || this.crystals == null)
                this.crystals = new HashSet<Crystal>();
            var Threshold = Molecules[0].Threshold;
            for (int i = 0; i < UVPoints.Count(); i++)
            {
                this.crystals.Add(
                    new Crystal(
                        Molecule.CreateByUV(
                            this, UVPoints.ToList()[i], 
                            Threshold),
                        size)
                    );
            }
        }
        public bool Run(double VibrationDeclineSpeed)
        {
            //if vibrationEnergy is zero representing the environment freezed, not things can move.
            if (VibrationEnergy <= 0) return false;
            if(this.crystals.Count == 0) return false;
            foreach (var molecule in this.Molecules)
                molecule.Execute();
            //Renew the list of molecules
            for (int i = 0; i < Molecules.Length; i++)
            {
                this.Molecules[i] = Molecule.CreateMolecule(this, ActionDic);
            }
            return true;
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
