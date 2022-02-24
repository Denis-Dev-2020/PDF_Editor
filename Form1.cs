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
using System.Diagnostics;
namespace PDFReaderImages
{
    public partial class Form1 : Form
    {
        Graphics g;
        int x123 = -1;
        int y123 = -1;
        Pen pen;
        bool Dragging = false;   //  <---- AM I PRESSING ON AN IMAGE
        int NewImagesCount = 0;
        bool PenMoving = false;
        bool AmIdrawing = false;
        int CursurSquareSizeInPixles = 32;
        Document pdoc = new Document(PageSize.A4, 20f, 20f, 30f, 30f);
        List<PictureBox> pictureBoxList = new List<PictureBox>();
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
            System.Drawing.Image SDFjksdfk3 = PDFEditor.Layout.ResizeImage(System.Drawing.Image.FromFile(BackgroundImagePath),595,842); //  <-- Resizing to actual A4 Output size
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
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /////////////////////////////////   INITIALIZE BACKGROUND PDF TO BE WORKING ON   ////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            PdfWriter pWriter = PdfWriter.GetInstance(pdoc, new FileStream("C:\\Users\\drabotay\\OneDrive - Magicsoftware\\Desktop\\Test\\OutputPDF.pdf", FileMode.Create));
            this.pdoc.Open();
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            ///
        }
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
            
            Control c = sender as Control;
            if (this.Dragging && c != null)
            {
                c.Location = new Point((int)(MousePosition.X-(c.Width/2)- this.Location.X)- this.CursurSquareSizeInPixles, (int)(MousePosition.Y-(c.Height/2) - this.Location.Y)- this.CursurSquareSizeInPixles);
                ((PictureBox)sender).BorderStyle = BorderStyle.FixedSingle;
                ((PictureBox)sender).BackColor = Color.White; // <---- may want to change this back to transparent late 50%
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
                //CreateNewInstance_BasedOnMousePos(op.FileName);

                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //   CREATE NEW IMAGE INSTANCE ON YOUR MOUSE POSITION AND IF YOU MOVE MOUSE PIC WILL MOVE TOO UNTIL MOUSECLICK   //
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                if (!this.AmIdrawing)
                {
                    this.NewImagesCount = NewImagesCount + 1;
                    System.Drawing.Image LoadingImage = System.Drawing.Image.FromFile(op.FileName);
                    var TestPic = new PictureBox
                    {
                        Name = "pictureBox",
                        Size = new Size(LoadingImage.Width, LoadingImage.Height),
                        Location = new Point(MousePosition.X - (this.Location.X + this.pictureBox2.Location.X), MousePosition.Y - (this.Location.Y + this.pictureBox2.Location.Y)),
                        BackColor = Color.White,
                        Image = LoadingImage,
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
                else
                {
                    Console.WriteLine("PLEASE FINISH SIGNING..");
                }
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
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
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////   SIGNING BUTTON (PEN FOR DRAWING), CLICK AGAIN TO DISABLE   ///////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Signature_MouseUp(object sender, EventArgs e)
        {
            this.PenMoving = false;
            this.x123 = -1;
            this.y123 = -1;
        }
        public void Signature_MouseDown(object sender, EventArgs e)
        {
            Console.WriteLine("Signing");
            this.pen.Color = Color.Black;
            this.pen.Width = 10;
            this.pen.Brush = new SolidBrush(Color.Black);
            this.pen.DashCap = new DashCap();
            this.PenMoving = true;
            this.x123 = MousePosition.X-(this.Location.X+pictureBox2.Location.X);
            this.y123 = MousePosition.Y-(this.Location.Y + pictureBox2.Location.Y + this.CursurSquareSizeInPixles);
        }
        public void Signature_MouseMove(object sender, EventArgs e)
        {
            if (this.PenMoving && this.x123!=-1 && this.y123!=-1 && this.AmIdrawing)
            {
                g.DrawLine(pen,new Point(this.x123,this.y123),
                    new Point(MousePosition.X- (this.Location.X + pictureBox2.Location.X),
                              MousePosition.Y- (this.Location.Y + pictureBox2.Location.Y + this.CursurSquareSizeInPixles)));
                this.x123 = MousePosition.X- (this.Location.X + pictureBox2.Location.X);
                this.y123 = MousePosition.Y- (this.Location.Y + pictureBox2.Location.Y + this.CursurSquareSizeInPixles);
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            PictureBox Signature = new PictureBox();
            Signature.Location = new Point(0, 0);
            Signature.Size = new Size(this.pictureBox2.Size.Width, this.pictureBox2.Size.Height);
            Signature.BackColor = Color.Transparent;
            if (this.AmIdrawing==false)
            {
                this.AmIdrawing = true;
                
                this.pictureBox2.Controls.Add(Signature);
                Signature.BringToFront();
                g = Signature.CreateGraphics();
                pen = new Pen(Color.Black,5);
                Signature.MouseDown += new System.Windows.Forms.MouseEventHandler(Signature_MouseDown);
                Signature.MouseUp += new System.Windows.Forms.MouseEventHandler(Signature_MouseUp);
                Signature.MouseMove += new System.Windows.Forms.MouseEventHandler(Signature_MouseMove);
                Console.WriteLine("Creating Panel At location"+ Signature.Location+"    Size "+ Signature.Size);
                this.Cursor = Cursors.UpArrow;
            }
            else
            {
                this.AmIdrawing = false;
                this.Cursor = Cursors.Default;
            }
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////   STAMP GENERATOR BUTTON   /////////////////////////////////////////////////////
        private void button4_Click(object sender, EventArgs e)
        {
            PDFEditor.Stamps.RectangleStamp stamp = new PDFEditor.Stamps.RectangleStamp();
            try
            {
                stamp.FrameColor = Color.Green;
                stamp.FrameLineWidth = 5;
                stamp.FillColor = Color.Blue;
                stamp.Width = 230;
                stamp.Height = 170;
                stamp.TextLines = new List<PDFEditor.StampLine>();
                stamp.SignatureFilePath = @"c:\temp\Signature.bmp";
                stamp.SignatureLocationX = 15;
                stamp.SignatureLocationY = 50;
                string fileName = @"c:\temp\stamp.png";
                StringFormat sf = new StringFormat();
                sf.LineAlignment = StringAlignment.Center;
                List<PDFEditor.StampLine> stamplines = new List<PDFEditor.StampLine>();
                PDFEditor.StampLine line1 = new PDFEditor.StampLine("מאושר", "Arial", FontStyle.Bold, 40f, Brushes.Green, sf);
                PDFEditor.StampLine line2 = new PDFEditor.StampLine("מספר רישיון - 748361", "Segoe UI", FontStyle.Regular, 15f, Brushes.Black, sf);
                PDFEditor.StampLine line3 = new PDFEditor.StampLine("רח' אנה פרנק 12 , בת ים", "Segoe UI", FontStyle.Regular, 15f, Brushes.Black, sf);
                PDFEditor.StampLine line4 = new PDFEditor.StampLine("שירותי הנדסה ואדריכלות", "Segoe UI", FontStyle.Regular, 15f, Brushes.Black, sf);
                stamplines.Add(line1);
                stamplines.Add(line2);
                stamplines.Add(line3);
                stamplines.Add(line4);
                List<RectangleF> rectangles = new List<RectangleF>() {
                        new RectangleF(5.8188976377953F, 5.48031496062993F, 170.59055118110234F, 170.6456692913386F)
                        };
                StringFormat format = new StringFormat()
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center,
                    FormatFlags = StringFormatFlags.NoClip,
                    Trimming = StringTrimming.EllipsisWord
                };
                Bitmap bitmap = new Bitmap(Convert.ToInt32(stamp.Width + 1), Convert.ToInt32(stamp.Height + 1), System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                float interline = 5.0f;
                float textHeigh = stamplines.Select(ds => ds.LineFontSize + interline).Sum();
                float textWidth = stamplines.Select(ds => (int)(ds.LineFontSize + line4.LineText.Length)).Sum();
                float textPositionHeight = ((stamp.Height + textHeigh) / 2) - textHeigh + (textHeigh / line4.LineText.Length);
                float textPositionWidth = ((stamp.Width + textWidth) / 2) - textWidth - (textWidth / 16);
                foreach (var item in stamplines)
                {
                    rectangles.Add(new RectangleF(
                        new PointF(textPositionWidth, textPositionHeight),
                        new SizeF(rectangles[0].Width, item.LineFontSize)));
                    textPositionHeight += item.LineFontSize + interline;
                }
                using (Graphics graphic = Graphics.FromImage(bitmap))
                {
                    Pen FramePen = new Pen(stamp.FrameColor, stamp.FrameLineWidth);
                    System.Drawing.Rectangle destination = new System.Drawing.Rectangle(0, 0, stamp.Width, stamp.Height);
                    graphic.DrawRectangle(FramePen, destination);
                    int rectArea = 0;
                    foreach (var stampline in stamplines)
                    {
                        rectArea += 1;
                        using (System.Drawing.Font font = new System.Drawing.Font(stampline.LineFontFamily, 
                                            stampline.LineFontSize, stampline.LineFontStyle, GraphicsUnit.Pixel))
                        {
                            graphic.DrawString(stampline.LineText, font, stampline.LineTextColor, rectangles[rectArea], format);
                        }
                    }
                    Bitmap SignatureImage = new Bitmap(stamp.SignatureFilePath);
                    graphic.DrawImage(SignatureImage, new Point(stamp.SignatureLocationX, stamp.SignatureLocationY));
                }
                bitmap.Save(fileName, ImageFormat.Png);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    }
}
