using System.Windows;
using System.Windows.Media;

namespace WPFTest
{
    public static class MatrixExtension
    {
        public static Point TransformPoint(this Matrix @this, Point point)
        {
            var points = new[] { point };

            @this.Transform(points);

            return points[0];
        }
    }
}
