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
        private AnchorPoint _dragAnchor = AnchorPoint.None;
        private Rect _rect;
        private Rect _rectTransform;
        private Point _centerPoint;
        private Point _offsetRect;
        private Single _rectRotation;

        //private Single _dragRot;

        public ImageEx()
        {
            _rect = new Rect(new Point(50, 50), new Size(80, 60));
            _rectTransform = _rect;
            _offsetRect = new Point(0, 0);
            _rectRotation = 0;
            _centerPoint = new Point(_rect.Left + _rect.Width / 2, _rect.Top + _rect.Height / 2);
        }

        public Point CenterPoint
        {
            get { return _centerPoint; }
            set { _centerPoint = value; }
        }

        public bool Drag
        {
            get { return _drag; }
            set { _drag = value; }
        }

        public Size DragSize
        {
            get { return _dragSize; }
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
        public Rect RectTransform
        {
            get { return _rectTransform; }
            set { _rectTransform = value; }
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
            if (ZoomBorder.GetROI)
            {
                string puttext = "Angle: " + _rectRotation + "\n" + "Offset X: " + _offsetRect.X
                    + " , " + "Offset Y: " + _offsetRect.Y + "\n"
                    + "Width: " + _rect.Width
                    + " , " + "Height: " + _rect.Height;
                FormattedText formattedText = new FormattedText(puttext, new System.Globalization.CultureInfo(10),
                FlowDirection.LeftToRight, new Typeface("Angle Degree"), 12, Brushes.DarkRed, 3.25);
                dc.DrawText(formattedText, new Point(10, 10));

                var mat = new Matrix();
                mat.RotateAt(_rectRotation, _centerPoint.X, _centerPoint.Y);
                mat.Translate(_offsetRect.X, _offsetRect.Y);

                MatrixTransform matrixTransform = new MatrixTransform(mat);
                dc.PushTransform(matrixTransform);

                dc.PushOpacity(0.6);

                // All out gizmo rectangles are defined in Rectangle Space
                var rectTopLeft = new Rect(_rect.Left - 5f, _rect.Top - 5f, 10f, 10f);
                var rectTopRight = new Rect(_rect.Left + _rect.Width - 5f, _rect.Top - 5f, 10f, 10f);
                var rectBottomLeft = new Rect(_rect.Left - 5f, _rect.Top + _rect.Height - 5f, 10f, 10f);
                var rectBottomRight = new Rect(_rect.Left + _rect.Width - 5f, _rect.Top + _rect.Height - 5f, 10f, 10f);
                var rectCenter = new Rect(_rect.Left + _rect.Width / 2 - 5f, _rect.Top + _rect.Height / 2 - 5f, 10f, 10f);

                //3 point draw line and ellipse
                Point pointLine1 = new Point(_rect.Left + _rect.Width / 2, _rect.Top);
                Point pointLine2 = new Point(_rect.Left + _rect.Width / 2, _rect.Top - 30);
                Point pointCenterEllipse = new Point(_rect.Left + _rect.Width / 2, _rect.Top - 30);

                dc.DrawRectangle(Brushes.White, new Pen(Brushes.Black, 1.5), _rect);
                dc.DrawRectangle(Brushes.WhiteSmoke, new Pen(Brushes.Blue, 1), rectTopLeft);
                dc.DrawRectangle(Brushes.WhiteSmoke, new Pen(Brushes.Blue, 1), rectTopRight);
                dc.DrawRectangle(Brushes.WhiteSmoke, new Pen(Brushes.Blue, 1), rectBottomLeft);
                dc.DrawRectangle(Brushes.WhiteSmoke, new Pen(Brushes.Blue, 1), rectBottomRight);
                dc.DrawRectangle(Brushes.WhiteSmoke, new Pen(Brushes.Black, 1), rectCenter);

                dc.DrawLine(new Pen(Brushes.Red, 1), new Point(_centerPoint.X - 15d,
                    _centerPoint.Y), new Point(_centerPoint.X + 15d, _centerPoint.Y));
                dc.DrawLine(new Pen(Brushes.Red, 1), new Point(_centerPoint.X,
                    _centerPoint.Y - 15d), new Point(_centerPoint.X, _centerPoint.Y + 15d));
                dc.DrawEllipse(Brushes.Red, null, _centerPoint, 2d, 2d);

                //draw line rotate
                dc.DrawLine(new Pen(Brushes.Black, 1.5), pointLine1, pointLine2);

                //draw ellipse rotate
                dc.DrawEllipse(Brushes.Blue, new Pen(Brushes.Black, 1.5), pointCenterEllipse, 7d, 7d);

                //calculate center point
                _centerPoint = new Point(_rect.Left + _rect.Width / 2, _rect.Top + _rect.Height / 2);

                dc.Pop();

            }
        }
    }
}
