using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPFTest
{
    public class ImageEx : Image
    {
        private bool _drag;
        private Size _dragSize;
        private Point _dragStart;
        private Point _dragStartOffset;
        private Rect _dragRect;
        private AnchorPoint _dragAnchor;
        private Rect _rect;
        private Point _offsetRect;
        private Single _rectRotation;
        //private Single _dragRot;

        public ImageEx()
        {
            _rect = new Rect(new Point(50, 50), new Size(100, 50));
            _offsetRect = new Point(0, 0);
            _rectRotation = 0;
        }
        public bool Drag
        {
            get { return _drag; }
            set { _drag = value; }
        }

        public Size DragSize
        {
            get { return _dragSize;}
            set { _dragSize = value; }
        }
        public Point DragStart
        {
            get { return _dragStart; }
            set { _dragStart = value; }
        }
        public Point DragStartOffset
        {
            get { return _dragStartOffset; }
            set { _dragStartOffset = value; }
        }
        public Rect DragRect
        {
            get { return _dragRect; }
            set { _dragRect = value; }
        }
        public AnchorPoint DragAnchor
        {
            get { return _dragAnchor; }
            set { _dragAnchor = value; }
        }
        public Rect Rect
        {
            get { return _rect; }
            set { _rect = value; }
        }
        public Point OffsetRect
        {
            get { return _offsetRect; }
            set { _offsetRect = value; }
        }
        public Single RectRotation
        {
            get { return _rectRotation; }
            set { _rectRotation = value; }
        }
        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            if(ZoomBorder.GetROI)
            {
                // Move Graphics handler to Rectangle's space
                var mat = new Matrix();
                mat.Translate(_offsetRect.X, _offsetRect.Y);
                mat.Rotate(_rectRotation);

                MatrixTransform matrixTransform = new MatrixTransform(mat);
                dc.PushTransform(matrixTransform);

                // All out gizmo rectangles are defined in Rectangle Space
                var rectTopLeft = new Rect(_rect.Left - 5f, _rect.Top - 5f, 10f, 10f);
                var rectTopRight = new Rect(_rect.Left + _rect.Width - 5f, _rect.Top - 5f, 10f, 10f);
                var rectBottomLeft = new Rect(_rect.Left - 5f, _rect.Top + _rect.Height - 5f, 10f, 10f);
                var rectBottomRight = new Rect(_rect.Left + _rect.Width - 5f, _rect.Top + _rect.Height - 5f, 10f, 10f);

                var rectRotate = new Rect(_rect.Left + _rect.Width / 2, _rect.Top - 20f, 10f, 10f);
                var rectCenter = new Rect(_rect.Left + _rect.Width / 2 - 5f, _rect.Top + _rect.Height / 2 - 5f, 10f, 10f);

                //3 point draw line and ellipse
                Point pointLine1 = new Point(_rect.Left + _rect.Width / 2, _rect.Top);
                Point pointLine2 = new Point(_rect.Left + _rect.Width / 2, _rect.Top - 30);
                Point pointCenterEllipse = new Point(_rect.Left + _rect.Width / 2, _rect.Top - 30);

                dc.DrawRectangle(null, new Pen(Brushes.Black, 2), _rect);
                dc.DrawRectangle(null, new Pen(Brushes.Blue, 2), rectTopLeft);
                dc.DrawRectangle(null, new Pen(Brushes.Blue, 2), rectTopRight);
                dc.DrawRectangle(null, new Pen(Brushes.Blue, 2), rectBottomLeft);
                dc.DrawRectangle(null, new Pen(Brushes.Blue, 2), rectBottomRight);
                //dc.DrawRectangle(null, new Pen(Brushes.Blue, 2), rectRotate);

                dc.DrawLine(new Pen(Brushes.Black, 2), pointLine1, pointLine2);
                dc.DrawEllipse(Brushes.Blue, new Pen(Brushes.Black, 2), pointCenterEllipse, 7d, 7d);

                dc.DrawRectangle(null, new Pen(Brushes.Blue, 2), rectCenter);
                
                dc.Pop();
            }
        }
    }
}
