﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrincipleCurvatureCrystall_Growth.ThresholdAttribute
{
    public class PrincipleCurvatureAttribute : ThresholdAttributeBase
    {
        public override string Name { get => "PrincipleCurvatureAttribute";}
        public override string Description { get => "In this class the threshold can be used for crystallising points according to the principle curvature on a surface"; }

        public override void ThresholdAction(Molecule molecule)
        {
            var Bonding = molecule.BondedNode;
            var CurrentLocation = molecule.Location;
            if(Bonding.HasNextNode) //If the bounding contains the Nextnode, if has to compete for the bonding
            {

            }
            else //current molecule can be the candidate to the crystall
            {

            }
        }
    }
}
