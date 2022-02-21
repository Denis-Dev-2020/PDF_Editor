using PDFReaderImages.Properties;
using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SautinSoft;
using iTextSharp;
//using iTextSharp.text;
using iTextSharp.text.pdf;
namespace PDFReaderImages
{
    public partial class Form1 : Form
    {
        bool Dragging = false;   //  <---- IS IM PRESSING ON AN IMAGE
        int NewImagesCount = 0;
        public Form1()
        {
            InitializeComponent();
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            SautinSoft.PdfFocus f = new SautinSoft.PdfFocus();    
            string pdfPath = "";
            string BackgroundImagePath = "";
            OpenFileDialog op = new OpenFileDialog();
            if (op.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                pdfPath = op.FileName;
            }
            string jpegDir = Path.GetDirectoryName(pdfPath);
            f.OpenPdf(pdfPath);
            if (f.PageCount > 0)
            {
                f.ImageOptions.ImageFormat = ImageFormat.Jpeg;
                f.ImageOptions.Dpi = 120;
                f.ImageOptions.JpegQuality = 95;
                for (int page = 1; page <= f.PageCount- f.PageCount+1; page++)
                {
                    string jpegPath = Path.Combine(jpegDir, String.Format("page{0}.jpg", page));
                    BackgroundImagePath = jpegPath;
                    f.ToImage(jpegPath, page);
                }
            }
            this.pictureBox2.Image = Image.FromFile(BackgroundImagePath);   //  <--- Outputs image , from PDF convert to IMG



            /////////////////////////////////   INITIALIZE BACKGROUND PDF TO BE WORKING ON   ////////////////////////////////////////////
        }




        //   CREATE NEW IMAGE INSTANCE ON YOUR MOUSE POSITION AND IF YOU MOVE MOUSE PIC WILL MOVE TOO UNTIL MOUSECLICK   //
        private void CreateNewInstance_BasedOnMousePos(String PicPath)
        {
            this.Dragging = true;
            Console.WriteLine(PicPath);
            Point Point_test = new Point(200, 200);
            var TestPic = new PictureBox
            {
                Name = "pictureBox",
                Size = new Size(180, 180),
                Location = new Point(MousePosition.X-206, MousePosition.Y-206),
                BackColor = Color.White,
                Image = Image.FromFile(PicPath)
            };
            TestPic.MouseDown += new System.Windows.Forms.MouseEventHandler(TestPic_MouseDown);
            TestPic.MouseMove += new System.Windows.Forms.MouseEventHandler(TestPic_MouseMove);
            TestPic.MouseUp += new System.Windows.Forms.MouseEventHandler(TestPic_MouseUp);
            this.Controls.Add(TestPic);
            TestPic.BringToFront();
            this.NewImagesCount = NewImagesCount + 1;
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////    HOW ADDED PICTURES ARE GONNA REACT WHEN DIFFERENT EVENTS ACCURE    ///////////
        public void TestPic_MouseDown(object sender, EventArgs e)
        {
            Console.WriteLine("down");
            if (this.Dragging == true)
            {
                this.Dragging = false;
            } else
            {
                this.Dragging = true;
            }
        }
        public void TestPic_MouseUp(object sender, EventArgs e)
        {
            Console.WriteLine("up");
        }
        public void TestPic_MouseMove(object sender, EventArgs e)
        {
            Control c = sender as Control;
            if (this.Dragging && c != null)
            {
                c.Top = MousePosition.Y-204;
                c.Left = MousePosition.X-204;
            }
        }
        //////////////////////////////////////////////////////////////////////////////////////////////
        private void button2_Click(object sender, EventArgs e)
        {
               OpenFileDialog op = new OpenFileDialog();
               if (op.ShowDialog() == System.Windows.Forms.DialogResult.OK)
               {
                CreateNewInstance_BasedOnMousePos(op.FileName);
               }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();

            using (var bmp = new Bitmap(400, 400))
            {
                pictureBox2.DrawToBitmap(bmp,new Rectangle(0,0,bmp.Width,bmp.Height));
                bmp.Save("C:\\Users\\drabotay\\OneDrive - Magicsoftware\\Desktop\\Test\\Testabc.jpg");
            }
            this.Show();
        }
    }
}