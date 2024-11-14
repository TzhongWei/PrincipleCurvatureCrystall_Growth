using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PrincipalCurvatureCrystal_Growth
{
    internal static class Utl
    {
        public static readonly Random Rand = new Random();
        internal static double MoleculeDistance(Molecule M1, Molecule M2)
        {
            var Srf = M1.environment.Container;
            var Pt1 = M1.Location.GetDisplayGeometry(Srf);
            var Pt2 = M2.Location.GetDisplayGeometry(Srf);
            return Pt1.DistanceTo(Pt2);
        }
        internal static UVPoint RandomPoint(Surface Container)
        {
            double x = 0, y = 0;
            for(int i  = 0; i < 2; i++)
            {
                //var Rand = new Random(DateTime.Now.Millisecond);
                x = Rand.NextDouble();
                y = Rand.NextDouble();
            }
            return UVPoint.CreateByNormalised(x, y, Container);
        }
        internal static IEnumerable<UVPoint> RandomPoints(Surface Container, int Count)
        { 
            for(int i = 0; i < Count; i++) 
            {
                yield return RandomPoint(Container);
            }
        }
        internal static bool ContainsBond(Interval Interval, double t)
    => Interval.Min < t && Interval.Max > t;
    }
}
