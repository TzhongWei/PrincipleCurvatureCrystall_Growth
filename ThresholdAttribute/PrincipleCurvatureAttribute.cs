using Rhino.Geometry;
using Rhino.Render;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace PrincipalCurvatureCrystal_Growth.ThresholdAttribute
{
    public class PrincipleCurvatureAttribute : ThresholdAttributeBase
    {
        public override string Name { get => "PrincipleCurvatureAttribute";}
        public override string Description { get => "In this class the threshold can be used for crystallising points according to the principle curvature on a surface"; }
        public double ConstantDistanceWeigth = 0.1;
        public double ConstantNormalWeigth = 100.0;
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
            return Math.Pow(DotVal, 2) * 100000 / Math.Pow(DistVal, 2) * ConstantDistanceWeigth;
        }
        public override void ThresholdAction(Molecule molecule)
        {

            var Env = molecule.environment;
            //This is the lowest cost for bondingEnergy
            var BestBondingEnergy = Env.VibrationEnergy;
            Crystal SelectedCrystal = null;
            Molecule SelectedMolecule = null;
            bool insertAtLastNode = false;
            int SelectedCrystalBranch = -1;
            
            foreach(Crystal CrystalPt in Env.crystals) 
            {
                for(int i = 0; i < CrystalPt.LastNode.Length; i++)
                {
                    var Node = CrystalPt.LastNode[i];

                    //Compute bonding energy with the last node
                    var BondingEnergy = BondingEnergyExpression(molecule, Node);
                    
                    if (BondingEnergy > BestBondingEnergy)
                    {
                        BestBondingEnergy = BondingEnergy;
                        SelectedCrystal = CrystalPt;
                        SelectedMolecule = Node; //This molecule will be the following node with the "Node"
                        SelectedCrystalBranch = i;
                        insertAtLastNode = true;
                    }

                    //Traverse the bonded nodes to find a better bonding opportunity
                    var FormerMolecule = Node.BondedNode;
                    while (FormerMolecule != null && FormerMolecule.IsFixed)
                    {
                        var FormerAndCurrentBondingEnergy = BondingEnergyExpression(molecule, FormerMolecule);

                        if (FormerAndCurrentBondingEnergy > BestBondingEnergy)
                        {
                            BestBondingEnergy = FormerAndCurrentBondingEnergy;
                            SelectedCrystal = CrystalPt;
                            SelectedMolecule = FormerMolecule.BondedNode;
                            SelectedCrystalBranch = i;
                            insertAtLastNode = false;
                        }

                        FormerMolecule = FormerMolecule.BondedNode;
                        break;
                    }
                }
            }

            // If no suitable bonding energy found, skip this molecule
            if (BestBondingEnergy <= Env.VibrationEnergy) return;

            if (insertAtLastNode)
            {
                // If the crystal is full, skip the insertion
                if (SelectedCrystal.IsFull)
                    return; // The crystal is full; cannot add more molecules to this leg
                else
                {
                    // Insert the molecule at the last node
                    molecule.BondingEnergy = BestBondingEnergy;
                    molecule.IsFixed = true;
                    molecule.BondedNode = SelectedMolecule;
                    SelectedMolecule.NextNode = molecule;
                    SelectedCrystal.LastNode[SelectedCrystalBranch] = molecule;
                    SelectedCrystal.FixMolecule.Add(molecule);
                }
            }
            else
            {
                // Insert the molecule and replace the original weaker node
                molecule.BondingEnergy = BestBondingEnergy;
                molecule.IsFixed = true;
                molecule.BondedNode = SelectedMolecule;
                SelectedMolecule.NextNode = molecule;
                SelectedCrystal.FixMolecule.Add(molecule);

                // Clean the original node path
                if (SelectedMolecule.HasNextNode)
                {
                    var OriginalFollowingMolecule = SelectedMolecule.NextNode;
                    SelectedMolecule.NextNode = null;

                    while (OriginalFollowingMolecule != null)
                    {
                        var Next = OriginalFollowingMolecule.NextNode;
                        SelectedCrystal.FixMolecule.Remove(OriginalFollowingMolecule);
                        OriginalFollowingMolecule.IsFixed = false;
                        OriginalFollowingMolecule.BondedNode = null;
                        OriginalFollowingMolecule.NextNode = null;
                        OriginalFollowingMolecule = Next;
                    }
                }
            }
        }
    }
}
