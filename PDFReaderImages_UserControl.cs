using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SautinSoft;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Drawing.Drawing2D;
using System.Diagnostics;
namespace PDFReaderImages_UserControl
{
    public partial class UserControl1 : UserControl
    {

        Graphics g;
        bool Dragging = false;   //  <---- AM I PRESSING ON AN IMAGE
        bool DidIPressedSaveAlready = false;
        int NewImagesCount = 0;
        int HowManyPages = 1;
        bool AmIdrawing = false;
        int CursurSquareSizeInPixles = 32;
        Document pdoc = new Document(PageSize.A4, 20f, 20f, 30f, 30f);
        List<PictureBox> Stamps_pictureBoxList = new List<PictureBox>();    //  <---- STAMPS
        List<PictureBox> PDF_Pages_pictureBoxList = new List<PictureBox>();    //  <---- PDF pages
        List<List<PictureBox>> AllChildrenOfPages_ByIndex = new List<List<PictureBox>>();
        Panel MainScrollablePanel = new Panel();
        int WindowX;
        int WindowY;
        List<string> ImageFilesFromPDF = new List<string>();
        List<string> PDFsThatSupposedToMergeInto1_PDFFile = new List<string>();
        int pagesPadding = 100;
        int pageHeigth = 1200;
        int whichPageI_editNow = 0;
        int whichPageI_editedBefore = -1;

        string OUTPUTPDF = @"";
        string INPUTPDF = @"";
        string Stamp_Path = @"";
        string SignaturePath = @"";

        string StampLineTextTitle = "";
        string StampLineTitleTextColor = "";
        string StampLineTitleTextFont = "";
        string StampLineTitleTextStyle = "";
        int StampLineTitleTextSize = 20;

        string StampLineTextLine1 = "";
        string StampLineTextLine2 = "";
        string StampLineTextLine3 = "";
        string StampLineTextColor = "";
        string StampLineTextFont = "";
        string StampLineTextStyle = "";
        int StampLineTextSize = 10;

        int stampWidth = 230;
        int stampHeight = 170;


        public UserControl1()
        {
            InitializeComponent();
            this.MainScrollablePanel.Location = new Point(0, 0);
            this.MainScrollablePanel.Size = new Size(1700, 800);
            this.MainScrollablePanel.BackColor = System.Drawing.Color.DarkGray;
            this.Controls.Add(MainScrollablePanel);
            this.MainScrollablePanel.BringToFront();
            this.MainScrollablePanel.AutoScroll = false;
            this.MainScrollablePanel.HorizontalScroll.Enabled = false;
            this.MainScrollablePanel.HorizontalScroll.Visible = false;
            this.MainScrollablePanel.HorizontalScroll.Maximum = 0;
            this.MainScrollablePanel.AutoScroll = true;
            this.MainScrollablePanel.Scroll += new System.Windows.Forms.ScrollEventHandler(ScrollingPanelEvent);
        }
        private void UserControl1_Load(object sender, EventArgs e) { }

        public void ScrollingPanelEvent(Object sender, EventArgs e)
        {
            //MessageBox.Show("Scrolled");
            int ScrolledValue = this.MainScrollablePanel.HorizontalScroll.Value;

        }

        /////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////   SETTERS FROM MAGICXPA TO CONTROL OF /////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////
        //~~~~~~~~~~~~~~~~~~~ STAMP SETTERS  ~~~~~~~~~~~~~~~~~~~~~~//
        public void ChangeStampRectangleWidth(int a)
        {
            this.stampWidth = a;
        }
        public void ChangeStampRectangleHeight(int a)
        {
            this.stampHeight = a;
        }
        public void ChangeStampLineTextTitle(string a)
        {
            this.StampLineTextTitle = a;
        }
        public void ChangeStampLineTitleTextSize(int a)
        {
            this.StampLineTitleTextSize = a;
        }
        public void ChangeStampLineTextColorTitle(string a)
        {
            this.StampLineTitleTextColor = a;
        }
        public void ChangeStampLineTitleTextFont(string a)
        {
            this.StampLineTitleTextFont = a;
        }
        public void ChangeStampLineTitleTextStyle(string a)
        {
            this.StampLineTitleTextStyle = a;
        }
        public void ChangeStampLineTextLine1(string a)
        {
            this.StampLineTextLine1 = a;
        }
        public void ChangeStampLineTextLine2(string a)
        {
            this.StampLineTextLine2 = a;
        }
        public void ChangeStampLineTextLine3(string a)
        {
            this.StampLineTextLine3 = a;
        }
        public void ChangeStampLineTextFont(string a)
        {
            this.StampLineTextFont = a;
        }
        public void ChangeStampLineTextStyle(string a)
        {
            this.StampLineTextStyle = a;
        }
        public void ChangeStampLineTextSizeV(int a)
        {
            this.StampLineTextSize = a;
        }
        public void ChangeStampLineTextColor(string a)
        {
            this.StampLineTextColor = a;
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//
        //~~~~~~~~~~~~~~~~~~~~~ BASIC CONFIG ~~~~~~~~~~~~~~~~~~~~~~~~//
        public void ChangeOutputPDF_Path(string path)
        {
            this.OUTPUTPDF = path;
        }
        public void ChangeInputPDF_Path(string path)
        {
            this.INPUTPDF = path;
        }
        public void ChangeStamp_Path(string path)
        {
            this.Stamp_Path = path;
        }
        public void ChangeSignature_Path(string path)
        {
            this.SignaturePath = path;
        }
        public void UpdateWindowPosition(int x, int y)
        {
            this.WindowX = (int)(x * 1.25);
            this.WindowY = (int)(y * 1.6);
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//
        /////////////////////////////////////////////////////////////////////////////////////////////////
        public bool isPictureBox_OverLappingAnother(PictureBox WholePage, PictureBox Stamp)
        {
            return WholePage.Bounds.IntersectsWith(Stamp.Bounds);
        }
        public void MakeStampChildOfThePage(PictureBox WholePage, PictureBox Stamp)
        {
            WholePage.Controls.Add(Stamp);
        }
        public void OpenPDFDocument()
        {
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /////////////////////////////////   INITIALIZE BACKGROUND PDF TO BE WORKING ON   ////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            PdfWriter pWriter = PdfWriter.GetInstance(pdoc, new FileStream(OUTPUTPDF, FileMode.Create));
            this.pdoc.Open();
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //this.pictureBox2.BringToFront();
        }

        public void LoadPDF()
        {
            this.BackColor = Color.DarkGray;
            //string BackgroundImagePath = @"C:\Program Files (x86)\MSE\Magic xpa 4.7\Projects\PDF_Creator_UserControlOriente\PDFsImagesFormat\page1.jpg";
            //System.Drawing.Image SDFjksdfk3 = PDFEditor.Layout.ResizeImage(System.Drawing.Image.FromFile(BackgroundImagePath), 595, 842); //  <-- Resizing to actual A4 Output size
            for (int i = 0; i < this.ImageFilesFromPDF.Count; i++)
            {
                PictureBox Page = new PictureBox();
                Page.Image = System.Drawing.Image.FromFile(this.ImageFilesFromPDF[i]);
                Page.Location = new Point(100, ((i + 1) * this.pagesPadding + (i * Page.Image.Height)));
                Page.Size = new Size(Page.Image.Width, Page.Image.Height);
                this.PDF_Pages_pictureBoxList.Add(Page);
                this.MainScrollablePanel.Controls.Add(this.PDF_Pages_pictureBoxList[i]);
                this.AllChildrenOfPages_ByIndex.Add(new List<PictureBox> { Page });
            }
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        }
        public int ReturnScrolledValue()
        {
            //Point x = this.PDF_Pages_pictureBoxList[0].PointToScreen;
            return 4;
        }
        public void MergeTo1PDF(List<String> InFiles, String OutFile)
        {
            using (FileStream stream = new FileStream(OutFile, FileMode.Create))
            using (Document doc = new Document())
            using (PdfCopy pdf = new PdfCopy(doc, stream))
            {
                doc.Open();

                PdfReader reader = null;
                PdfImportedPage page = null;

                //fixed typo
                InFiles.ForEach(file =>
                {
                    reader = new PdfReader(file);

                    for (int i = 0; i < reader.NumberOfPages; i++)
                    {
                        page = pdf.GetImportedPage(reader, i + 1);
                        pdf.AddPage(page);
                    }

                    pdf.FreeReader(reader);
                    reader.Close();
                    //File.Delete(file);
                });
            }
        }
        public void Save_Click()
        {
            this.Show();
            for (int i = 0; i < this.AllChildrenOfPages_ByIndex.Count; i++)
            {
                var pgSize = new iTextSharp.text.Rectangle(this.AllChildrenOfPages_ByIndex[i][0].Width, this.AllChildrenOfPages_ByIndex[i][0].Height);
                Document PagePDF = new Document(pgSize);
                PdfWriter pWriter = PdfWriter.GetInstance(PagePDF, new FileStream(@"C:\Program Files (x86)\MSE\Magic xpa 4.7\Projects\PDF_Creator_UserControlOriente\PDFs\NeedToMerge\" + i.ToString() + ".pdf", FileMode.Create));
                PagePDF.Open();
                iTextSharp.text.Image ItextImageBackground = iTextSharp.text.Image.GetInstance(this.AllChildrenOfPages_ByIndex[i][0].Image,
                                                             System.Drawing.Imaging.ImageFormat.Png);
                ItextImageBackground.SetAbsolutePosition(0, 0);
                PagePDF.Add(ItextImageBackground);
                for (int j = 1; j < this.AllChildrenOfPages_ByIndex[i].Count; j++)
                {
                    iTextSharp.text.Image ItextImage = iTextSharp.text.Image.GetInstance(this.AllChildrenOfPages_ByIndex[i][j].Image,
                                                         System.Drawing.Imaging.ImageFormat.Png);
                    int OutputX = this.AllChildrenOfPages_ByIndex[i][j].Location.X;
                    int OutputY = this.CursurSquareSizeInPixles+this.AllChildrenOfPages_ByIndex[i][0].Height-(2*AllChildrenOfPages_ByIndex[i][j].Location.Y)- (2*this.AllChildrenOfPages_ByIndex[i][j].Height)+(j* this.AllChildrenOfPages_ByIndex[i][j].Height);
                    ItextImage.SetAbsolutePosition(OutputX, OutputY);
                    PagePDF.Add(ItextImage);
                }
                PagePDF.Close();
                this.PDFsThatSupposedToMergeInto1_PDFFile.Add(@"C:\Program Files (x86)\MSE\Magic xpa 4.7\Projects\PDF_Creator_UserControlOriente\PDFs\NeedToMerge\" + i.ToString() + ".pdf");
            }
            for (int i = 0; i < this.AllChildrenOfPages_ByIndex.Count; i++)
            {
                MessageBox.Show("Page " + i.ToString() + " got " + (this.AllChildrenOfPages_ByIndex[i].Count - 1).ToString() + " Stamps on it");
            }
            this.MergeTo1PDF(PDFsThatSupposedToMergeInto1_PDFFile, @"C:\Program Files (x86)\MSE\Magic xpa 4.7\Projects\PDF_Creator_UserControlOriente\PDFs\OUTPUT.pdf");
            //this.LoadPDF();
        }
        public void ConverAllPDFsInsideFolderToImages()
        {
            SautinSoft.PdfFocus f = new SautinSoft.PdfFocus();
            string pdfPath = @"C:\Program Files (x86)\MSE\Magic xpa 4.7\Projects\PDF_Creator_UserControlOriente\PDFs\pdf-sample2.pdf";
            string BackgroundImagePath = "";
            string jpegDir = @"C:\Program Files (x86)\MSE\Magic xpa 4.7\Projects\PDF_Creator_UserControlOriente\PDFsImagesFormat\";
            f.OpenPdf(pdfPath);
            if (f.PageCount > 0)
            {
                f.ImageOptions.ImageFormat = ImageFormat.Jpeg;
                f.ImageOptions.Dpi = 200;
                f.ImageOptions.JpegQuality = 95;
                for (int page = 1; page <= f.PageCount; page++)
                {
                    string jpegPath = Path.Combine(jpegDir, String.Format("page{0}.jpg", page));
                    f.ToImage(jpegPath, page);
                    this.ImageFilesFromPDF.Add(jpegPath);
                }
            }
        }
        public void GenerateStamp()
        {
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Generate Stamp ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//
            PDFEditor.Stamps.RectangleStamp stamp = new PDFEditor.Stamps.RectangleStamp();
            try
            {
                stamp.FrameColor = Color.Green;
                stamp.FrameLineWidth = 5;
                stamp.FillColor = Color.Blue;
                stamp.Width = this.stampWidth;
                stamp.Height = this.stampHeight;
                stamp.TextLines = new List<PDFEditor.StampLine>();
                stamp.SignatureFilePath = SignaturePath;
                stamp.SignatureLocationX = 15;
                stamp.SignatureLocationY = 50;
                string fileName = Stamp_Path;
                StringFormat sf = new StringFormat();
                sf.LineAlignment = StringAlignment.Center;
                List<PDFEditor.StampLine> stamplines = new List<PDFEditor.StampLine>();
                PDFEditor.StampLine line1 = new PDFEditor.StampLine(StampLineTextTitle, StampLineTitleTextFont, StampLineTitleTextStyle, StampLineTitleTextSize, StampLineTitleTextColor);
                PDFEditor.StampLine line2 = new PDFEditor.StampLine(StampLineTextLine1, StampLineTextFont, StampLineTextStyle, StampLineTextSize, StampLineTextColor);
                PDFEditor.StampLine line3 = new PDFEditor.StampLine(StampLineTextLine2, StampLineTextFont, StampLineTextStyle, StampLineTextSize, StampLineTextColor);
                PDFEditor.StampLine line4 = new PDFEditor.StampLine(StampLineTextLine3, StampLineTextFont, StampLineTextStyle, StampLineTextSize, StampLineTextColor);
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
                bitmap.Dispose();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            ///////~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~///////
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////    HOW ADDED PICTURES ARE GONNA REACT WHEN DIFFERENT EVENTS ACCURE    /////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////
        public void TestPic_MouseDown(object sender, EventArgs e)
        {
            bool AlreadyExists = false;
            for (int i = 0; i < this.AllChildrenOfPages_ByIndex.Count; i++)
            {
                for (int j = 1; j < this.AllChildrenOfPages_ByIndex[i].Count; j++)
                {
                    if (((PictureBox)sender) == this.AllChildrenOfPages_ByIndex[i][j])
                    {
                        AlreadyExists = true;
                    }
                }
            }
            Console.WriteLine("MouseDown");
            if (this.Dragging == true)
            {
                this.Dragging = false;
                this.Cursor = Cursors.Default;
                for (int i = 0; i < this.PDF_Pages_pictureBoxList.Count; i++)
                {
                    if (isPictureBox_OverLappingAnother(this.PDF_Pages_pictureBoxList[i], ((PictureBox)sender))&& AlreadyExists==false)
                    {
                        this.PDF_Pages_pictureBoxList[i].Controls.Add(((PictureBox)sender));
                        ((PictureBox)sender).BringToFront();
                        this.AllChildrenOfPages_ByIndex[i].Add(((PictureBox)sender));
                    }
                }
            }
            else
            {
                this.Dragging = true;
                this.Cursor = Cursors.Cross;
                for (int i = 0; i < this.AllChildrenOfPages_ByIndex.Count; i++)
                {
                    for (int j = 1; j < this.AllChildrenOfPages_ByIndex[i].Count; j++)
                    {
                        if (((PictureBox)sender) == this.AllChildrenOfPages_ByIndex[i][j])
                        {
                            this.AllChildrenOfPages_ByIndex[i].RemoveAt(j);
                        }
                    }
                }
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
                c.Location = new Point((int)(MousePosition.X - (c.Width / 2) - this.Location.X) - this.CursurSquareSizeInPixles - this.WindowX, (int)(MousePosition.Y - (c.Height / 2) - this.Location.Y) - this.CursurSquareSizeInPixles - this.WindowY - 50);
                ((PictureBox)sender).BorderStyle = BorderStyle.FixedSingle;
                ((PictureBox)sender).BackColor = Color.White; // <---- may want to change this back to transparent late 50%
                this.Cursor = Cursors.Cross;
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////
        ////   IMAGES BUTTON CLICKED TO SELECT IMAGE FROM PC AND PASS IT TO FUNCTION ////
        /////////////////////////////////////////////////////////////////////////////////
        public void ApprovedStamp_Click(int WinX, int WinY)
        {
            this.UpdateWindowPosition(WinX, WinY);
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //   CREATE NEW IMAGE INSTANCE ON YOUR MOUSE POSITION AND IF YOU MOVE MOUSE PIC WILL MOVE TOO UNTIL MOUSECLICK   //
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            System.Drawing.Image LoadingImage = System.Drawing.Image.FromFile(Stamp_Path);
            var TestPic = new PictureBox
            {
                Name = "pictureBox",
                Size = new Size(LoadingImage.Width, LoadingImage.Height),
                Location = new Point((int)(MousePosition.X - (LoadingImage.Width / 2) - this.Location.X) - this.CursurSquareSizeInPixles - this.WindowX,
                                (int)(MousePosition.Y - (LoadingImage.Height / 2) - this.Location.Y) - this.CursurSquareSizeInPixles - this.WindowY - 50),
                BackColor = Color.White,
                Image = LoadingImage,
            };
            TestPic.MouseDown += new System.Windows.Forms.MouseEventHandler(TestPic_MouseDown);
            TestPic.MouseMove += new System.Windows.Forms.MouseEventHandler(TestPic_MouseMove);
            TestPic.MouseUp += new System.Windows.Forms.MouseEventHandler(TestPic_MouseUp);
            Stamps_pictureBoxList.Add(TestPic);
            this.Dragging = true;
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            List<PictureBox> templist = new List<PictureBox>();
            for (int i = 0; i < this.PDF_Pages_pictureBoxList.Count; i++)
            {
                if (isPictureBox_OverLappingAnother(this.PDF_Pages_pictureBoxList[i], TestPic))
                {
                    whichPageI_editNow = i;
                    this.PDF_Pages_pictureBoxList[i].Controls.Add(TestPic);
                    TestPic.BringToFront();
                    this.AllChildrenOfPages_ByIndex[i].Add(TestPic);

                    if (this.whichPageI_editNow < this.whichPageI_editedBefore)
                    {
                        templist = this.AllChildrenOfPages_ByIndex[i];
                        this.AllChildrenOfPages_ByIndex.RemoveAt(i);
                        this.AllChildrenOfPages_ByIndex.Add(templist);
                    }



                }
            }


        }
        /////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////    HOW ADDED PICTURES ARE GONNA REACT WHEN DIFFERENT EVENTS ACCURE    /////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////
    }
}
