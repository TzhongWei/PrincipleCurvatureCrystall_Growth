using Rhino.Geometry;
using Rhino.UI.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace PrincipleCurvatureCrystall_Growth
{
    public struct UVPoint
    {
        public bool IsValid { get; private set; }
        public double X;
        public double Y;
        public static UVPoint CreateByNormalised(double x, double y, Surface Container)
        {
            if (Container != null && Utl.ContainsBond(new Interval(0, 1), x) && Utl.ContainsBond(new Interval(0, 1), y))
            {
                var DomX = Container.Domain(0);
                var DomY = Container.Domain(1);
                return new UVPoint(
                    DomX.Min + DomX.Length * x,
                    DomY.Min + DomY.Length * y,
                    Container
                    );
            }
            else return UVPoint.Unset;
        }
        public static UVPoint Unset => new UVPoint(double.NaN, double.NaN, null);
        public UVPoint(double x, double y, Surface Container)
        {
            if (Container == null)
            {
                this.IsValid = false;
                this.X = double.NaN;
                this.Y = double.NaN;
            }
            var UDom = Container.Domain(0);
            var VDom = Container.Domain(1);
            if (Utl.ContainsBond(UDom, y) && Utl.ContainsBond(VDom, y))
            {
                this.X = x;
                this.Y = y;
                this.IsValid = true;
            }
            else
            {
                
                this.IsValid = false;
                this.X = double.NaN;
                this.Y = double.NaN;
            }
        }
        public Point3d GetDisplayGeometry(Surface Container) => Container.PointAt(this.X, this.Y);
    }
}
