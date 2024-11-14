using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrincipleCurvatureCrystal_Growth
{
    public class Crystal
    {
        public bool IsStopGrowth;
        public Molecule CentralCrystal;
        public int CrystalSize; //The total number of the molecules
        public Crystal(Molecule CentralCrystal, int Size = 40)
        {
            this.CentralCrystal = CentralCrystal;
            this.CrystalSize = Size;
            this.LastNode = new Molecule[] { CentralCrystal, CentralCrystal, CentralCrystal, CentralCrystal };
            this.IsStopGrowth = false;
        }
        public Molecule[] LastNode;
        
        public List<LineCurve> GetPath(Surface Container)
        {
            List<LineCurve> Lines = new List<LineCurve>();
            for(int i = 0; i < LastNode.Length; i++) 
            {
                var CurrentNode = LastNode[i];
                Lines.Add(CurrentNode.BondingPath(Container));
                while(CurrentNode.IsFixed)
                {
                    CurrentNode = CurrentNode.BondedNode;
                    Lines.Add(CurrentNode.BondingPath(Container));
                }
            }
            return Lines;
        }
    }
}
