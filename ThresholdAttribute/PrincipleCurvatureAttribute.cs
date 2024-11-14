using Rhino.Geometry;
using Rhino.Render;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace PrincipleCurvatureCrystal_Growth.ThresholdAttribute
{
    public class PrincipleCurvatureAttribute : ThresholdAttributeBase
    {
        public override string Name { get => "PrincipleCurvatureAttribute";}
        public override string Description { get => "In this class the threshold can be used for crystallising points according to the principle curvature on a surface"; }
        public double ConstantDistanceWeigth = 1.0;
        public double ConstantNormalWeigth = 1.0;
        internal double MoleculeNormalDotProduct(Molecule M1, Molecule M2)
        {
            var Srf = M1.environment.Container;
            var NorM1 = M1.Location.GetNormalFromSurface(Srf);
            var NorM2 = M2.Location.GetNormalFromSurface(Srf);
            return Math.Abs(NorM1 * NorM2);
        }
        public double BondingEnergyExpression(Molecule M1, Molecule M2)
        {
            var DotVal = MoleculeNormalDotProduct(M1, M2);
            var DistVal = Utl.MoleculeDistance(M1, M2);
            return Math.Pow(DotVal, 2) * ConstantNormalWeigth / Math.Pow(DistVal, 2) * ConstantDistanceWeigth;
        }
        public override void ThresholdAction(Molecule molecule)
        {

            var Env = molecule.environment;
            //This is the lowest cost for bondingEnergy
            var BondingEnergy = Env.VibrationEnergy;
            bool IsLast = false;
            int SelectedCrystalBranch = -1;
            Crystal SelectedCrystal = null;
            Molecule SelectedMolecule = null;
            foreach(Crystal CrystalPt in Env.crystals) 
            {
                for(int i = 0; i < CrystalPt.LastNode.Length; i++)
                {
                    var Node = CrystalPt.LastNode[i];
                    var CurrentNodeBondingEnergy = BondingEnergyExpression(molecule, Node);
                    if (CurrentNodeBondingEnergy > BondingEnergy)
                    {
                        BondingEnergy = CurrentNodeBondingEnergy;
                        SelectedCrystal = CrystalPt;
                        SelectedMolecule = Node; //This molecule will be the following node with the "Node"
                        SelectedCrystalBranch = i;
                        IsLast = true;
                    }
                    if (Node.IsFixed)
                    {
                        var FormerMolecule = Node.BondedNode;
                        while (FormerMolecule.IsFixed)
                        {
                            var FormerBondingEnergy = FormerMolecule.BondingEnergy;
                            var FormerAndCurrentBondingEnergy = BondingEnergyExpression(FormerMolecule, molecule);
                            if( FormerAndCurrentBondingEnergy > FormerBondingEnergy && 
                                FormerAndCurrentBondingEnergy > BondingEnergy)
                            {
                                BondingEnergy = FormerAndCurrentBondingEnergy;
                                SelectedCrystal = CrystalPt;
                                SelectedMolecule = FormerMolecule.BondedNode; //FormerMolecule is going to be replaced by current molecule
                                SelectedCrystalBranch = i;
                                IsLast = false;
                            }
                            FormerMolecule = FormerMolecule.BondedNode;
                        }
                    }
                }
            }

            if (BondingEnergy == Env.VibrationEnergy) return;
            if(IsLast && SelectedCrystal.IsFull)
            { return; }
            else
            {
                molecule.BondingEnergy = BondingEnergy;
                molecule.IsFixed = true;
                molecule.BondedNode = SelectedMolecule;
                SelectedCrystal.LastNode[SelectedCrystalBranch] = molecule;
                //Release the original crystall path
                if(SelectedMolecule.HasNextNode)
                {
                    var OriginalFollowingMolecule = SelectedMolecule.NextNode;
                    SelectedCrystal.FixMolecule.Remove(OriginalFollowingMolecule);
                    OriginalFollowingMolecule.IsFixed = false;
                    OriginalFollowingMolecule.BondedNode = null;
                    while(OriginalFollowingMolecule.HasNextNode)
                    {
                        OriginalFollowingMolecule = SelectedMolecule.NextNode;
                        SelectedCrystal.FixMolecule.Remove(OriginalFollowingMolecule);
                        OriginalFollowingMolecule.IsFixed = false;
                        OriginalFollowingMolecule.BondedNode = null;
                    }
                }
                SelectedMolecule.NextNode = molecule;
                SelectedCrystal.FixMolecule.Add(molecule);
            }
        }
    }
}
