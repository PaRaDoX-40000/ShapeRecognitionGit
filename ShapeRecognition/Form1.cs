using Emgu;
using Emgu.Util;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System.Drawing.Imaging;
using System.Drawing;


namespace ShapeRecognition
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            SerSize();
        }

        private class ArrayPoints 
        {
            private int index = 0;
            private Point[] points;

            public ArrayPoints(int size)
            {
                if (size <= 0)
                {
                    size= 2;                 
                }
                points = new Point[size];
            }
            public void SetPoint(int x, int y)
            {
                if(index >= points.Length)
                {
                    index = 0;
                }
                points[index] = new Point(x, y);
                index++;
            }
            public void ReserPoints()
            {
                index= 0;
            }
            public int GetCountPoints()
            {
                return index;
            }
            public Point[] GetPoints()
            {
                return points;
            }
           
        }

        private Image<Bgr, byte> image2 = null;

        private bool isMouse = false;
        private ArrayPoints arrayPoints = new ArrayPoints(2);

        Bitmap map = new Bitmap(100,100);
        Graphics graphics;

        Pen pen = new Pen(Color.Black,3f); 

        private void SerSize()
        {
            Rectangle rectangle = Screen.PrimaryScreen.Bounds;
            map = new Bitmap(rectangle.Width, rectangle.Height);
            graphics = Graphics.FromImage(map);

            pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
            pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;

        }
        private void pictureBox2_MouseDown(object sender, MouseEventArgs e)
        {
            isMouse = true;
        }

        private void pictureBox2_MouseUp(object sender, MouseEventArgs e)
        {
            isMouse = false;
            arrayPoints.ReserPoints();
        }

        private void pictureBox2_MouseMove(object sender, MouseEventArgs e)
        {
            if(isMouse==false) { return; }
            arrayPoints.SetPoint(e.X, e.Y);
            if(arrayPoints.GetCountPoints() >=2) 
            {
                graphics.DrawLines(pen, arrayPoints.GetPoints());
                pictureBox2.Image = map;
                arrayPoints.SetPoint(e.X, e.Y);

            }
        }

        private void CleanButton_Click(object sender, EventArgs e)
        {
            graphics.Clear(pictureBox2.BackColor);
            pictureBox2.Image = map;
        }

        private void CheckButton_Click(object sender, EventArgs e)
        {
            
            Bitmap bitmapImage = new Bitmap(pictureBox2.Image);

            Image<Bgr, byte> outputImage = bitmapImage.ToImage<Bgr, byte>();

            Image<Gray, byte> grayImage = outputImage.SmoothGaussian(5).Convert<Gray, byte>().ThresholdBinaryInv(new Gray(230), new Gray(255));

            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            Mat hierarchy = new Mat();

            CvInvoke.FindContours(grayImage, contours, hierarchy, Emgu.CV.CvEnum.RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);
            if (contours.Size != 3)
            {
                label1.Text = "Не правильно";
                return;
            }

            for(int i = 0; i > contours.Size; i++)
            {
                double perimeter = CvInvoke.ArcLength(contours[i], true);
                VectorOfPoint approximation = new VectorOfPoint();

                CvInvoke.ApproxPolyDP(contours[i], approximation, 0.4 * perimeter, true);
                CvInvoke.DrawContours(outputImage, contours, i, new MCvScalar(0, 0, 255), 2);
              
                if (approximation.Size < 5)
                {
                    label1.Text = "Не правильно";
                    return;
                }
                Rectangle rect = CvInvoke.BoundingRectangle(contours[i]);
                if(rect.Height> rect.Width)
                {
                    label1.Text = "Не правильно";
                    return;
                }
                label1.Text = "Правильно";
            }
        }

        
    }
}