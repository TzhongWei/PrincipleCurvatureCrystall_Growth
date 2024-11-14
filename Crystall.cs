using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrincipleCurvatureCrystall_Growth
{
    public class Crystall
    {
        public Molecule CentralCrystall;
        public int CrystallSize; //The total number of the molecules
        public Crystall(Molecule CentralCrystall, int Size = 40)
        {
            this.CentralCrystall = CentralCrystall;
            this.CrystallSize = Size;
            this.Branch = new Molecule[4];
        }
        public Molecule[] Branch;
        public List<LineCurve> GetPath(Surface Container)
        {
            List<LineCurve> Lines = new List<LineCurve>();
            for(int i = 0; i < Branch.Length; i++) 
            {
                var CurrentNode = Branch[i];
                Lines.Add(CurrentNode.BondingPath(Container));
                while(CurrentNode.HasNextNode)
                {
                    CurrentNode = CurrentNode.NextNode;
                    Lines.Add(CurrentNode.BondingPath(Container));
                }
            }
            return Lines;
        }
    }
}
