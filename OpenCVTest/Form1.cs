using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Emgu.CV.UI;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace OpenCVTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
                        var file = @"C:\Users\Kallan\Dropbox\Camera Uploads\2014-06-13 18.16.58.jpg";
            var ul = new PointF(270,262);
            var ur = new PointF(2984,439);
            var dl = new PointF(33,4047);
            var dr = new PointF(2981,4087);
            var blah = CameraCalibration.GetPerspectiveTransform(new PointF[] { ul, ur, dr, dl }, new PointF[] { new PointF(0, 0), new PointF(3000, 0), new PointF(3000, 4000), new PointF(0, 4000) });

            //var image = new Image<Emgu.CV.Gray, double>(Bitmap.FromFile(file));

            var control = new ImageBox();
            control.Top = 0;
            control.Left = 0;
            control.Width = Width;
            control.Dock = DockStyle.Fill;
            control.Height = Height;
            //control.Image = (IImage)Image.FromFile("C:\\Users\\Kallan\\Pictures\\2012-11-18\\SS.png");
            //Emgu.CV.Image<
            control.FunctionalMode = ImageBox.FunctionalModeOption.Everything;
            Controls.Add(control);

            control.MouseUp += new MouseEventHandler(control_MouseUp);

            //var matrix = new Emgu.CV.MatND<double>();
            

            //Matrix<(<(<'TDepth>)>)>
        }

        private IList<PointF> _points = new List<PointF>(); //ul, ur, dr, dl
        void control_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != System.Windows.Forms.MouseButtons.Left) return;

            var control = (ImageBox)sender;
            _points.Add(new PointF(GetX(e.X), GetY(e.Y)));
            if (_points.Count < 4)
            {
                return;
            }


            var width = (float)(Math.Sqrt(_points[1].X * _points[1].X - _points[0].X * _points[0].X) + Math.Sqrt(_points[2].X * _points[2].X - _points[3].X * _points[3].X)) / 2f;
            var height = (float)(Math.Sqrt(_points[3].Y * _points[3].Y - _points[0].Y * _points[0].Y) + Math.Sqrt(_points[2].Y * _points[2].Y - _points[1].Y * _points[1].Y)) / 2f;

            var blah = CameraCalibration.GetPerspectiveTransform(_points.ToArray<PointF>(), new PointF[] { new PointF(0, 0), new PointF(width, 0), new PointF(width, height), new PointF(0, height) });

            var image = control.Image as Image<Bgr, byte>;
            var newImage = image.WarpPerspective(blah, Emgu.CV.CvEnum.INTER.CV_INTER_NN, Emgu.CV.CvEnum.WARP.CV_WARP_FILL_OUTLIERS, new Bgr(0, 0, 0));

            var intWidth = (int)width;
            var intHeight = (int)height;
            control.Image = newImage.GetSubRect(new Rectangle(0, 0, intWidth, intHeight));

            _points.Clear();
        }

        private float GetX(float controlX)
        {
            var control = (ImageBox)Controls[0];

            var zoomscale = control.ZoomScale;
            if (zoomscale == 0) return 0;

            return Math.Min(controlX / (float)zoomscale, control.Image.Bitmap.Width);
        }

        private float GetY(float controlY)
        {
            var control = (ImageBox)Controls[0];

            var zoomscale = control.ZoomScale;
            if (zoomscale == 0) return 0;

            return Math.Min(controlY / (float)zoomscale, control.Image.Bitmap.Height);
        }


    }
}
