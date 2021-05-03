using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Point = System.Windows.Point;


namespace WPFTest
{
    public class ZoomBorder : Border
    {
        private UIElement child = null;
        private Point origin;
        private Point start;
        ImageEx imageEx = null;

        private static bool _getROI = false;
        public static bool GetROI
        {
            get
            {
                return _getROI;
            }
            set
            {
                _getROI = value;
            }
        }
        private TranslateTransform GetTranslateTransform(UIElement element)
        {
            return (TranslateTransform)((TransformGroup)element.RenderTransform)
              .Children.First(tr => tr is TranslateTransform);
        }

        private ScaleTransform GetScaleTransform(UIElement element)
        {
            return (ScaleTransform)((TransformGroup)element.RenderTransform)
              .Children.First(tr => tr is ScaleTransform);
        }

        public override UIElement Child
        {
            get { return base.Child; }
            set
            {
                if (value != null && value != this.Child)
                    this.Initialize(value);
                base.Child = value;
            }
        }

        public void Initialize(UIElement element)
        {
            this.child = element;
            if (child != null)
            {
                imageEx = element as ImageEx;

                TransformGroup group = new TransformGroup();
                ScaleTransform st = new ScaleTransform();
                group.Children.Add(st);
                TranslateTransform tt = new TranslateTransform();
                group.Children.Add(tt);
                child.RenderTransform = group;
                child.RenderTransformOrigin = new Point(0.0, 0.0);

                //event border
                this.MouseWheel += child_MouseWheel;
                this.MouseLeftButtonDown += child_MouseLeftButtonDown;
                this.MouseLeftButtonUp += child_MouseLeftButtonUp;
                this.MouseMove += child_MouseMove;
                this.PreviewMouseRightButtonDown += new MouseButtonEventHandler(
                  child_PreviewMouseRightButtonDown);

                //event ImageEx
                imageEx.MouseDown += ImageEx_MouseDown;
                imageEx.MouseUp += ImageEx_MouseUp;
                imageEx.MouseMove += ImageEx_MouseMove;
            }
        }

        private void ImageEx_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_getROI)
                return;
            if (!imageEx.Drag)
                return;

            var mat = new Matrix();
            mat.RotateAt(imageEx.RectRotation, imageEx.CenterPoint.X, imageEx.CenterPoint.Y);
            mat.Translate(imageEx.OffsetRect.X, imageEx.OffsetRect.Y);
            mat.Invert();

            var point = mat.Transform(new Point(e.GetPosition(imageEx).X, e.GetPosition(imageEx).Y));

            Point offsetSize;
            Point clamped;

            switch (imageEx.DragAnchor)
            {
                case AnchorPoint.TopLeft:

                    clamped = new Point(Math.Min(imageEx.Rect.BottomRight.X - 5d, point.X),
                        Math.Min(imageEx.Rect.BottomRight.Y - 5d, point.Y));
                    offsetSize = new Point(clamped.X - imageEx.DragStart.X, clamped.Y - imageEx.DragStart.Y);
                    imageEx.Rect = new Rect(
                        imageEx.DragRect.Left + offsetSize.X,
                        imageEx.DragRect.Top + offsetSize.Y,
                        imageEx.DragRect.Width - offsetSize.X,
                        imageEx.DragRect.Height - offsetSize.Y);

                    break;

                case AnchorPoint.TopRight:
                    clamped = new Point(Math.Max(imageEx.Rect.BottomLeft.X - 5d, point.X),
                        Math.Min(imageEx.Rect.BottomLeft.Y - 5d, point.Y));
                    offsetSize = new Point(clamped.X - imageEx.DragStart.X, clamped.Y - imageEx.DragStart.Y);
                    imageEx.Rect = new Rect(
                        imageEx.DragRect.Left,
                        imageEx.DragRect.Top + offsetSize.Y,
                        imageEx.DragRect.Width + offsetSize.X,
                        imageEx.DragRect.Height - offsetSize.Y);

                    break;

                case AnchorPoint.BottomLeft:
                    clamped = new Point(Math.Min(imageEx.Rect.TopRight.X + 5d, point.X),
                        Math.Max(imageEx.Rect.TopRight.Y + 5d, point.Y));
                    offsetSize = new Point(clamped.X - imageEx.DragStart.X, clamped.Y - imageEx.DragStart.Y);
                    imageEx.Rect = new Rect(
                        imageEx.DragRect.Left + offsetSize.X,
                        imageEx.DragRect.Top,
                        imageEx.DragRect.Width - offsetSize.X,
                        imageEx.DragRect.Height + offsetSize.Y);

                    break;

                case AnchorPoint.BottomRight:
                    clamped = new Point(Math.Max(imageEx.Rect.TopLeft.X + 5d, point.X),
                        Math.Max(imageEx.Rect.TopLeft.Y + 5d, point.Y));
                    offsetSize = new Point(clamped.X - imageEx.DragStart.X, clamped.Y - imageEx.DragStart.Y);
                    imageEx.Rect = new Rect(
                        imageEx.DragRect.Left,
                        imageEx.DragRect.Top,
                        imageEx.DragRect.Width + offsetSize.X,
                        imageEx.DragRect.Height + offsetSize.Y);

                    break;

                case AnchorPoint.Rotation:
                    //var vecX = (point.X);
                    //var vecY = (-point.Y);

                    var vecX = (point.X - imageEx.CenterPoint.X);
                    var vecY = (imageEx.CenterPoint.Y - point.Y);

                    var len = Math.Sqrt(vecX * vecX + vecY * vecY);

                    var normX = vecX / len;
                    var normY = vecY / len;

                    //In rectangles's space, 
                    //compute dot product between, 
                    //Up and mouse-position vector
                    var dotProduct = (0 * normX + 1 * normY);
                    var angle = Math.Acos(dotProduct);

                    if (vecX < 0)
                        angle = -angle;

                    // Add (delta-radians) to rotation as degrees
                    imageEx.RectRotation += (float)((180 / Math.PI) * angle);
                    if (imageEx.RectRotation > 360 || imageEx.RectRotation < -360)
                        imageEx.RectRotation = 0;

                    break;

                case AnchorPoint.Center:
                    //move this in screen-space
                    imageEx.OffsetRect = new Point(e.GetPosition(imageEx).X - imageEx.DragStartOffset.X,
                        e.GetPosition(imageEx).Y - imageEx.DragStartOffset.Y);
                    
                    break;
            }

            imageEx.InvalidateVisual();
        }

        private void ImageEx_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!_getROI)
                return;
            imageEx.Drag = false;
        }

        private void ImageEx_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!_getROI)
                return;

            // Compute a Screen to Rectangle transform 

            var mat = new Matrix();
            mat.RotateAt(imageEx.RectRotation, imageEx.CenterPoint.X, imageEx.CenterPoint.Y);
            mat.Translate(imageEx.OffsetRect.X, imageEx.OffsetRect.Y);
            mat.Invert();

            // Mouse point in Rectangle's space. 
            var point = mat.Transform(new Point(e.GetPosition(imageEx).X, e.GetPosition(imageEx).Y));

            var rect = imageEx.Rect;
            var rectTopLeft = new Rect(imageEx.Rect.Left - 5f, imageEx.Rect.Top - 5f, 10f, 10f);
            var rectTopRight = new Rect(imageEx.Rect.Left + imageEx.Rect.Width - 5f, imageEx.Rect.Top - 5f, 10f, 10f);
            var rectBottomLeft = new Rect(imageEx.Rect.Left - 5f, imageEx.Rect.Top + imageEx.Rect.Height - 5f, 10f, 10f);
            var rectBottomRight = new Rect(imageEx.Rect.Left + imageEx.Rect.Width - 5f, imageEx.Rect.Top + imageEx.Rect.Height - 5f, 10f, 10f);
            //var rectRotate = new Rect(imageEx.Rect.Left + imageEx.Rect.Width / 2, imageEx.Rect.Top - 20f, 10f, 10f);
            var ellipse = new EllipseGeometry(new Point(imageEx.Rect.Left + imageEx.Rect.Width / 2, imageEx.Rect.Top - 30), 7d, 7d);

            if (!imageEx.Drag)
            {
                //We're in Rectangle space now, so its simple box-point intersection test
                if (rectTopLeft.Contains(point))
                {

                    imageEx.Drag = true;
                    imageEx.DragStart = new Point(point.X, point.Y);
                    imageEx.DragAnchor = AnchorPoint.TopLeft;
                    imageEx.DragRect = new Rect(imageEx.Rect.Left, imageEx.Rect.Top, imageEx.Rect.Width, imageEx.Rect.Height);
                }
                else if (rectTopRight.Contains(point))
                {

                    imageEx.Drag = true;
                    imageEx.DragStart = new Point(point.X, point.Y);
                    imageEx.DragAnchor = AnchorPoint.TopRight;
                    imageEx.DragRect = new Rect(imageEx.Rect.Left, imageEx.Rect.Top, imageEx.Rect.Width, imageEx.Rect.Height);
                }
                else if (rectBottomLeft.Contains(point))
                {

                    imageEx.Drag = true;
                    imageEx.DragStart = new Point(point.X, point.Y);
                    imageEx.DragAnchor = AnchorPoint.BottomLeft;
                    imageEx.DragRect = new Rect(imageEx.Rect.Left, imageEx.Rect.Top, imageEx.Rect.Width, imageEx.Rect.Height);
                }
                else if (rectBottomRight.Contains(point))
                {

                    imageEx.Drag = true;
                    imageEx.DragStart = new Point(point.X, point.Y);
                    imageEx.DragAnchor = AnchorPoint.BottomRight;
                    imageEx.DragRect = new Rect(imageEx.Rect.Left, imageEx.Rect.Top, imageEx.Rect.Width, imageEx.Rect.Height);
                }
                else if (ellipse.FillContains(point))
                {

                    imageEx.Drag = true;
                    imageEx.DragStart = new Point(point.X, point.Y);
                    imageEx.DragAnchor = AnchorPoint.Rotation;
                    imageEx.DragRect = new Rect(imageEx.Rect.Left, imageEx.Rect.Top, imageEx.Rect.Width, imageEx.Rect.Height);
                }
                else if (rect.Contains(point))
                {
                    imageEx.Drag = true;
                    //imageEx.DragStart = new Point(point.X, point.Y);
                    imageEx.DragAnchor = AnchorPoint.Center;
                    imageEx.DragRect = new Rect(imageEx.Rect.Left, imageEx.Rect.Top, imageEx.Rect.Width, imageEx.Rect.Height);
                    imageEx.DragStartOffset = new Point(e.GetPosition(imageEx).X - imageEx.OffsetRect.X, e.GetPosition(imageEx).Y - imageEx.OffsetRect.Y);
                }
            }

        }

        public void Reset()
        {
            if (child != null)
            {
                // reset zoom
                var st = GetScaleTransform(child);
                st.ScaleX = 1.0;
                st.ScaleY = 1.0;

                // reset pan
                var tt = GetTranslateTransform(child);
                tt.X = 0.0;
                tt.Y = 0.0;
            }
        }

        #region Child Events

        private void child_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (child != null)
            {
                var st = GetScaleTransform(child);
                var tt = GetTranslateTransform(child);

                double zoom = e.Delta > 0 ? .2 : -.2;
                if (!(e.Delta > 0) && (st.ScaleX < .4 || st.ScaleY < .4))
                    return;

                Point relative = e.GetPosition(child);
                double absoluteX;
                double absoluteY;

                absoluteX = relative.X * st.ScaleX + tt.X;
                absoluteY = relative.Y * st.ScaleY + tt.Y;

                st.ScaleX += zoom;
                st.ScaleY += zoom;

                tt.X = absoluteX - relative.X * st.ScaleX;
                tt.Y = absoluteY - relative.Y * st.ScaleY;
            }
        }

        private void child_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!_getROI)
            {
                if (child != null)
                {
                    var tt = GetTranslateTransform(child);
                    start = e.GetPosition(this);
                    origin = new Point(tt.X, tt.Y);
                    this.Cursor = Cursors.Hand;
                    child.CaptureMouse();
                }
            }

        }

        private void child_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!_getROI)
            {
                if (child != null)
                {
                    child.ReleaseMouseCapture();
                    this.Cursor = Cursors.Arrow;
                }
            }

        }

        void child_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Reset();
        }

        private void child_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_getROI)
            {
                if (child != null)
                {
                    if (child.IsMouseCaptured)
                    {
                        var tt = GetTranslateTransform(child);
                        Vector v = start - e.GetPosition(this);
                        tt.X = origin.X - v.X;
                        tt.Y = origin.Y - v.Y;
                    }
                }
            }

        }
        #endregion
    }

}
