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
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Drawing.Drawing2D;
namespace PDFReaderImages
{
    public partial class Form1 : Form
    {
        bool Dragging = false;   //  <---- AM I PRESSING ON AN IMAGE
        int NewImagesCount = 0;
        Document pdoc = new Document(PageSize.A4, 20f, 20f, 30f, 30f);
        List<PictureBox> pictureBoxList = new List<PictureBox>();
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////   RESIZING IMAGES FUNCTION   ///////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static Bitmap ResizeImage(System.Drawing.Image image, int width, int height)
        {
            var destRect = new System.Drawing.Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);
            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);
            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }
            return destImage;
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public Form1()
        {
            InitializeComponent();
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /////////////////////   LOAD PDF FROM PC THEN CONVERTS IT TO JPG THEN ADDS IT TO PICTUREBOX2   ////////////////////////
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            
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
                for (int page = 1; page <= f.PageCount - f.PageCount + 1; page++)
                {
                    string jpegPath = Path.Combine(jpegDir, String.Format("page{0}.jpg", page));
                    BackgroundImagePath = jpegPath;
                    f.ToImage(jpegPath, page);
                }
            }
            System.Drawing.Image SDFjksdfk3 = ResizeImage(System.Drawing.Image.FromFile(BackgroundImagePath),595,842); //  <-- Resizing to actual A4 Output size
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //////////////////   REMOVE WATERMARK SIGNATURE OF TRIAL VERSION OF PDF TO JPG CONVERTER   ////////////////////
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
            var tempBMP = (Bitmap)SDFjksdfk3;
            for (int i = 0; i < 290; i++)
            {
                for (int j = 0; j < 65 ; j++)
                {
                    tempBMP.SetPixel(i,j,Color.White);
                }
            }
            SDFjksdfk3 = tempBMP;
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
            this.pictureBox2.Image = SDFjksdfk3;   //  <--- Outputs image , from PDF convert to IMG
            this.pictureBoxList.Add(this.pictureBox2);
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /*                BLANK A4 TO PICTUREBOX2
            System.Drawing.Image LoadingImage = System.Drawing.Image.FromFile("C:\\Users\\drabotay\\source\\repos\\PDFDragDemo\\PDFDragDemo\\PDFs\\Blank_A4.png");
            this.pictureBox2.Width = LoadingImage.Width;
            this.pictureBox2.Height = LoadingImage.Height;
            this.pictureBox2.Image = LoadingImage;
            this.Height = this.pictureBox2.Height + this.pictureBox2.Location.Y + 50;
            this.Width = (int)((this.pictureBox2.Width + this.pictureBox2.Location.X + 30)*1.5);*/
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /////////////////////////////////   INITIALIZE BACKGROUND PDF TO BE WORKING ON   ////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            PdfWriter pWriter = PdfWriter.GetInstance(pdoc, new FileStream("C:\\Users\\drabotay\\OneDrive - Magicsoftware\\Desktop\\Test\\OutputPDF.pdf", FileMode.Create));
            this.pdoc.Open();
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //   CREATE NEW IMAGE INSTANCE ON YOUR MOUSE POSITION AND IF YOU MOVE MOUSE PIC WILL MOVE TOO UNTIL MOUSECLICK   //
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void CreateNewInstance_BasedOnMousePos(String PicPath)
        {
            this.NewImagesCount = NewImagesCount + 1;
            System.Drawing.Image LoadingImage = System.Drawing.Image.FromFile(PicPath);
            var TestPic = new PictureBox
            {
                Name = "pictureBox",
                Size = new Size(LoadingImage.Width,LoadingImage.Height),
                Location = new Point(MousePosition.X-(this.Location.X+this.pictureBox2.Location.X),MousePosition.Y-(this.Location.Y+this.pictureBox2.Location.Y)),
                BackColor = Color.White,
                Image = LoadingImage
            };
            TestPic.MouseDown += new System.Windows.Forms.MouseEventHandler(TestPic_MouseDown);
            TestPic.MouseMove += new System.Windows.Forms.MouseEventHandler(TestPic_MouseMove);
            TestPic.MouseUp += new System.Windows.Forms.MouseEventHandler(TestPic_MouseUp);
            this.Controls.Add(TestPic);
            TestPic.BringToFront();
            this.NewImagesCount = NewImagesCount + 1;
            pictureBoxList.Add(TestPic);
            this.Dragging = true;
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////    HOW ADDED PICTURES ARE GONNA REACT WHEN DIFFERENT EVENTS ACCURE    /////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////
        public void TestPic_MouseDown(object sender, EventArgs e)
        {
            Console.WriteLine("MouseDown");
            if (this.Dragging == true)
            {
                this.Dragging = false;
                this.Cursor = Cursors.Default;
            } else
            {
                this.Dragging = true;
                this.Cursor = Cursors.Cross;
            }
            ((PictureBox)sender).BorderStyle = BorderStyle.None;
        }
        public void TestPic_MouseUp(object sender, EventArgs e)
        {
            Console.WriteLine("MouseUp");
            ((PictureBox)sender).BorderStyle = BorderStyle.None;
        }
        public void TestPic_MouseMove(object sender, EventArgs e)
        {
            int CursurSquareSizeInPixles = 32;
            Control c = sender as Control;
            if (this.Dragging && c != null)
            {
                c.Location = new Point((int)(MousePosition.X-(c.Width/2)- this.Location.X)- CursurSquareSizeInPixles, (int)(MousePosition.Y-(c.Height/2) - this.Location.Y)- CursurSquareSizeInPixles);
                ((PictureBox)sender).BorderStyle = BorderStyle.FixedSingle;
                ((PictureBox)sender).BackColor = Color.Red;
                this.Cursor = Cursors.Cross;
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////
        ////   IMAGES BUTTON CLICKED TO SELECT IMAGE FROM PC AND PASS IT TO FUNCTION ////
        /////////////////////////////////////////////////////////////////////////////////
        private void button2_Click(object sender, EventArgs e)
        {
               OpenFileDialog op = new OpenFileDialog();
               if (op.ShowDialog() == System.Windows.Forms.DialogResult.OK)
               {
                CreateNewInstance_BasedOnMousePos(op.FileName);
               }
        }
        /////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////   SAVE TO PDF BUTTON - ITERATES OVER ALL PICTUREBOXES EXCEPT 1 AND ADDING THEM TO PDF THEN CLOSES PDF //////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void button1_Click(object sender, EventArgs e)
        {
            this.Show();
            iTextSharp.text.Image ItextImageBackground = iTextSharp.text.Image.GetInstance(this.pictureBoxList[0].Image, 
                System.Drawing.Imaging.ImageFormat.Png);
            ItextImageBackground.SetAbsolutePosition(0, 0);
            pdoc.Add(ItextImageBackground);
            for (int i = 1; i < this.pictureBoxList.Count; i++)
            {
                iTextSharp.text.Image ItextImage = iTextSharp.text.Image.GetInstance(this.pictureBoxList[i].Image,
                    System.Drawing.Imaging.ImageFormat.Png);
                int OutputX = this.pictureBoxList[i].Location.X-this.pictureBox2.Location.X;
                int OutputY = this.pictureBox2.Height-this.pictureBoxList[i].Location.Y-this.pictureBoxList[i].Height+this.pictureBox2.Location.Y;
                ItextImage.SetAbsolutePosition(OutputX, OutputY);
                pdoc.Add(ItextImage);
            }
            this.pdoc.Close();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            Console.WriteLine(this.Location);
            Console.WriteLine(this.pictureBox2.Location);
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    }
}
