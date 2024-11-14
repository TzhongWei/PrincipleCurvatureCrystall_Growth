﻿using Rhino.Geometry;
using Rhino.UI.Theme;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;

namespace PrincipleCurvatureCrystal_Growth
{
    //Each molecule has two hands only.
    public class Molecule
    {
        //The environment
        public Environment environment;
        //Represented the molecule has bondedNode
        public bool IsFixed;
        //Represented the molecule data is set up
        public bool IsValid;
        //The bondingEnergy with bondedNode and this Node
        public double BondingEnergy;
        //The Location of molecule in a container
        public UVPoint Location { get; private set; }
        //The first bonded molecule
        public Molecule BondedNode;
        //The second bonded molecule
        public Molecule NextNode;
        //A function to make the bonding between this molecule and other
        public delegate void ThresholdSetting(Molecule molecule);
        public Dictionary<string, ThresholdSetting> Threshold { get; private set; }
        public static Molecule Unset => new Molecule();
        private Molecule() 
        {
            this.IsValid = false;
            this.IsFixed = false;
            this.BondedNode = null;
            this.NextNode = null;
            this.Location = UVPoint.Unset;
            this.Threshold = null;
        }
        public static Molecule CreateByUV(Environment environment, Point3d UVPt, Dictionary<string, ThresholdSetting> Threshold)
        {
            var Mole = Molecule.CreateMolecule(environment, Threshold);
            Mole.Location = UVPoint.CreateByNormalised(UVPt.X, UVPt.Y, environment.Container);
            return Mole;
        }
        private Molecule(Environment environment, Dictionary<string, ThresholdSetting> Threshold)
        {
            this.IsValid = true;
            this.environment = environment;
            this.Threshold = Threshold;
            this.IsFixed = false;
            this.Location = Utl.RandomPoint(environment.Container);
            this.BondedNode = Molecule.Unset;
            this.NextNode = Molecule.Unset;
            BondingEnergy = -1;
        }
        public void Execute()
        {
            foreach (var item in Threshold)
            {
                item.Value(this);
            }
        }
        public static Molecule CreateMolecule(Environment environment, Dictionary<string, ThresholdSetting> Threshold)
            => new Molecule(environment, Threshold);
        public Point3d GetDisplayGeometry(Surface Container) => this.Location.GetDisplayGeometry(Container);
        public LineCurve BondingPath(Surface Container) =>
            IsFixed ? new LineCurve(
                this.Location.GetDisplayGeometry(Container),
                this.BondedNode.Location.GetDisplayGeometry(Container)
                ) : null;
        public bool HasNextNode => this.NextNode.IsValid;
    }

}