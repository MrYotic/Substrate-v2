namespace Substrate_v2.Math;
public partial class Math
{
    public struct Vec3d
    {
        public Vec3d(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
    }
}
