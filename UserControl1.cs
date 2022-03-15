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
        int WindowX;
        int WindowY;
        int pagesPadding = 10;
        int whichPageI_editNow = 0;
        int whichPageI_editedBefore = -1;
        int onWhatPageIM_RightNow = 0;
        int ScrolledValue = 0;
        int ValueModuluPageSize = 0;
        bool AlreadySavedOnce = false;
        bool scrollingUp = false;
        bool scrollingDown = false;
        int CursurSquareSizeInPixles = 32;
        int PreviousScrollingValue = -1;
        Document pdoc = new Document(PageSize.A4, 20f, 20f, 30f, 30f);
        Panel MainScrollablePanel = new Panel();
        List<PictureBox> Stamps_pictureBoxList = new List<PictureBox>();    //  <---- STAMPS
        List<PictureBox> PDF_Pages_pictureBoxList = new List<PictureBox>();    //  <---- PDF pages
        List<List<PictureBox>> AllChildrenOfPages_ByIndex = new List<List<PictureBox>>();
        List<string> PDFsThatSupposedToMergeInto1_PDFFile = new List<string>();
        List<System.Drawing.Image> FromSinglePDF2ImagesList = new List<System.Drawing.Image>();
        List<System.Drawing.Image> DifferantStamps = new List<System.Drawing.Image>();
        List<PDFEditor.StampLine> stamplines = new List<PDFEditor.StampLine>();
        List<PictureBox> UndoList = new List<PictureBox>();
        List<PictureBox> UndoThe_UndoList = new List<PictureBox>();
        Size FPageSize = new Size(0,0);



        string OUTPUTPDF = @"";
        string INPUTPDF = @"";
        string Stamp_Path = @"";
        string SignaturePath = @"C:\Program Files (x86)\MSE\Magic xpa 4.7\Projects\PDF_Creator_UserControlOriente\Resources\Signature.bmp";

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
        ////////////////////////////////////////////
        ////// MINIMUM STAMP SIZE (3px x 3px) //////
        ////////////////////////////////////////////
        /**/ int stampWidth = 700;                ////
        /**/ int stampHeight = 400;               ////
        ////////////////////////////////////////////
        public UserControl1()
        {
            if (this.stampHeight < 3 || this.stampWidth < 3)
            {
                while (true)
                {
                    MessageBox.Show("STAMP SIZE TOO SMALL , SHOULD BE AT LEAST 3X3 (PIXELS)");
                    Thread.Sleep(1500);
                }
            }
            InitializeComponent();
            this.MainScrollablePanel.Location = new Point(0, 0);
            this.MainScrollablePanel.Width = this.Width-System.Windows.Forms.SystemInformation.VerticalScrollBarWidth;
            this.MainScrollablePanel.BackColor = System.Drawing.Color.DarkGray;
            this.Controls.Add(MainScrollablePanel);
            this.MainScrollablePanel.BringToFront();
            this.MainScrollablePanel.AutoScroll = false;
            this.MainScrollablePanel.HorizontalScroll.Enabled = false;
            this.MainScrollablePanel.HorizontalScroll.Visible = false;
            this.MainScrollablePanel.HorizontalScroll.Maximum = 0;
            this.MainScrollablePanel.AutoScroll = true;
            this.MainScrollablePanel.Scroll += (s, e) => { ScrollingHandele(); };
            this.MainScrollablePanel.MouseWheel += (s, e) => { ScrollingHandele(); };
            this.MainScrollablePanel.Anchor = (AnchorStyles.Top);
            this.MainScrollablePanel.Dock = DockStyle.Fill;
            this.Anchor = (AnchorStyles.Top);
            this.Dock = DockStyle.Fill;
        }
        private void UserControl1_Load(object sender, EventArgs e) { }
        public void PrintStatsCoords()
        {
            MessageBox.Show("this parent:      " + this.Parent.Location.ToString());
            MessageBox.Show("this Scroll:      " + this.MainScrollablePanel.Location.ToString());
            MessageBox.Show("this Page:      " + this.PDF_Pages_pictureBoxList[0].Location.ToString());
            MessageBox.Show("this Mouse:      " + MousePosition.ToString());
        }
        public void UndoAction()
        {
            if (this.UndoList.Count > 0)
            {
                this.UndoList[this.UndoList.Count - 1].Visible = false;
                this.UndoThe_UndoList.Add(this.UndoList[this.UndoList.Count - 1]);
                this.UndoList.RemoveAt(this.UndoList.Count - 1);
            }
        }
        public void ReDoAction()
        {
            if (this.UndoThe_UndoList.Count > 0)
            {
                this.UndoThe_UndoList[this.UndoThe_UndoList.Count - 1].Visible = true;
                this.UndoList.Add(this.UndoThe_UndoList[this.UndoThe_UndoList.Count - 1]);
                this.UndoThe_UndoList.RemoveAt(this.UndoThe_UndoList.Count - 1);
            }
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////   SETTERS FROM MAGICXPA TO CONTROL OF /////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////
        //~~~~~~~~~~~~~~~~~~~ STAMP SETTERS  ~~~~~~~~~~~~~~~~~~~~~~//
        public void SetStampRectangleWidth(int a)
        {
            this.stampWidth = a;
        }
        public void SetStampRectangleHeight(int a)
        {
            this.stampHeight = a;
        }
        public void SetStampLineTextTitle(string a)
        {
            this.StampLineTextTitle = a;
        }
        public void SetStampLineTitleTextSize(int a)
        {
            this.StampLineTitleTextSize = a;
        }
        public void SetStampLineTextColorTitle(string a)
        {
            this.StampLineTitleTextColor = a;
        }
        public void SetStampLineTitleTextFont(string a)
        {
            this.StampLineTitleTextFont = a;
        }
        public void SetStampLineTitleTextStyle(string a)
        {
            this.StampLineTitleTextStyle = a;
        }
        public void SetStampLineTextLine1(string a)
        {
            this.StampLineTextLine1 = a;
        }
        public void SetStampLineTextLine2(string a)
        {
            this.StampLineTextLine2 = a;
        }
        public void SetStampLineTextLine3(string a)
        {
            this.StampLineTextLine3 = a;
        }
        public void SetStampLineTextFont(string a)
        {
            this.StampLineTextFont = a;
        }
        public void SetStampLineTextStyle(string a)
        {
            this.StampLineTextStyle = a;
        }
        public void SetStampLineTextSizeV(int a)
        {
            this.StampLineTextSize = a;
        }
        public void SetStampLineTextColor(string a)
        {
            this.StampLineTextColor = a;
        }
        public void AddStampLine(string linetext, string linefont, string linefontstyle, float linefontsize, string linetextcolor)
        {
            PDFEditor.StampLine line = new PDFEditor.StampLine(linetext, linefont, linefontstyle, linefontsize, linetextcolor);
            this.stamplines.Add(line);
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
        public string GetOutputPDF_Path()
        {
            return this.OUTPUTPDF;

        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//
        /////////////////////////////////////////////////////////////////////////////////////////////////////
        public int isPictureBox_OverLappingAnother(PictureBox WholePage, PictureBox Stamp)
        {
            System.Drawing.Rectangle intersectionArea = System.Drawing.Rectangle.Intersect(WholePage.Bounds,Stamp.Bounds);
            //MessageBox.Show("Overlaping with size of  : " + intersectionArea.Size.ToString());
            return intersectionArea.Size.Height;
        }
        public void MakeStampChildOfThePage(PictureBox WholePage, PictureBox Stamp)
        {
            WholePage.Controls.Add(Stamp);
        }
        private void eraseAllObjects()
        {
            string[] filePaths = Directory.GetFiles(@"C:\Program Files (x86)\MSE\Magic xpa 4.7\Projects\PDF_Creator_UserControlOriente\PDFs\NeedToMerge\");
            foreach (string filePath in filePaths)
            {
                File.Delete(filePath);
            }
            for (int i = 0; i < this.FromSinglePDF2ImagesList.Count; i++)
            {
                this.FromSinglePDF2ImagesList.RemoveAt(i);
            }
            for (int i = 0; i < this.PDFsThatSupposedToMergeInto1_PDFFile.Count; i++)
            {
                this.PDFsThatSupposedToMergeInto1_PDFFile.RemoveAt(i);
            }
            for (int i = 0; i < this.Stamps_pictureBoxList.Count; i++)
            {
                this.Stamps_pictureBoxList[i].Controls.Clear();
                this.Stamps_pictureBoxList.RemoveAt(i);
            }
            for (int i = 0; i < this.PDF_Pages_pictureBoxList.Count; i++)
            {
                this.PDF_Pages_pictureBoxList[i].Controls.Clear();
                this.PDF_Pages_pictureBoxList.RemoveAt(i);
            }
            for (int i = 0; i < this.AllChildrenOfPages_ByIndex.Count; i++)
            {
                for (int j = 0; j < this.AllChildrenOfPages_ByIndex[i].Count; j++)
                {
                    this.AllChildrenOfPages_ByIndex[i][j].Controls.Clear();
                    this.AllChildrenOfPages_ByIndex[i].RemoveAt(j);
                }
                this.AllChildrenOfPages_ByIndex.RemoveAt(i);
            }
            this.FromSinglePDF2ImagesList = null;
            this.PDFsThatSupposedToMergeInto1_PDFFile = null;
            this.PDF_Pages_pictureBoxList = null;
            this.AllChildrenOfPages_ByIndex = null;
            PDF_Pages_pictureBoxList = new List<PictureBox>();
            AllChildrenOfPages_ByIndex = new List<List<PictureBox>>();
            PDFsThatSupposedToMergeInto1_PDFFile = new List<string>();
            FromSinglePDF2ImagesList = new List<System.Drawing.Image>();
            this.MainScrollablePanel.Controls.Clear();
        }
        public void LoadPDF(string PDFFilePath)
        {
            if (this.AlreadySavedOnce)
            {
                this.eraseAllObjects();
                this.ConverAllPDFsInsideFolderToImages(this.OUTPUTPDF);
                //MessageBox.Show("Show input " + this.OUTPUTPDF);
            }
            else
            {
                this.ConverAllPDFsInsideFolderToImages(PDFFilePath);
                this.FPageSize = this.FromSinglePDF2ImagesList[0].Size;
               // MessageBox.Show("Show input " + PDFFilePath);
            }
            for (int i = 0; i < this.FromSinglePDF2ImagesList.Count; i++)
            {
                PictureBox Page = new PictureBox();
                Page.Image = PDFEditor.Layout.ResizeImage(this.FromSinglePDF2ImagesList[i], this.FPageSize.Width, this.FPageSize.Height);
                Page.Location = new Point(0, ((i) * this.pagesPadding + (i * Page.Image.Height)));
                Page.Size = new Size(Page.Image.Width, Page.Image.Height);
                this.PDF_Pages_pictureBoxList.Add(Page);
                this.MainScrollablePanel.Controls.Add(this.PDF_Pages_pictureBoxList[i]);
                this.AllChildrenOfPages_ByIndex.Add(new List<PictureBox> { Page });
                Page.Anchor = AnchorStyles.Top;
                //MessageBox.Show("Page Size : "+Page.Size.ToString());
            }
            
            this.MainScrollablePanel.Size = this.FPageSize;
            this.BackColor = Color.DarkGray;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ReturnWhatPageIM()
        {
            MessageBox.Show("Page "+this.onWhatPageIM_RightNow.ToString()+" Scrolled :"+this.ScrolledValue.ToString()+"    Delta :"+
                (this.ScrolledValue%(this.PDF_Pages_pictureBoxList[0].Height+this.pagesPadding))
                );
        }
        public void ScrollingHandele()
        {
            for (int i = 0; i < this.PDF_Pages_pictureBoxList.Count; i++)
            {
                if ((MainScrollablePanel.VerticalScroll.Value >= i * (this.PDF_Pages_pictureBoxList[0].Height + this.pagesPadding)) &&
                     (MainScrollablePanel.VerticalScroll.Value < (i+1) * (this.PDF_Pages_pictureBoxList[0].Height + this.pagesPadding)))
                {
                    this.onWhatPageIM_RightNow = (i + 1);
                    this.ScrolledValue = MainScrollablePanel.VerticalScroll.Value;
                    this.ValueModuluPageSize = this.ScrolledValue % (this.PDF_Pages_pictureBoxList[0].Height + this.pagesPadding);
                }
            }
            if ((this.ValueModuluPageSize >= ((13.0 /28) * this.PDF_Pages_pictureBoxList[0].Height)))
            {
                for (int j = 0; this.ValueModuluPageSize > 10; j++)
                {
                    MainScrollablePanel.VerticalScroll.Value = MainScrollablePanel.VerticalScroll.Value + 10;
                    this.ScrolledValue = MainScrollablePanel.VerticalScroll.Value;
                    this.ValueModuluPageSize = this.ScrolledValue % (this.PDF_Pages_pictureBoxList[0].Height + this.pagesPadding);
                    //Thread.Sleep(1);
                }
            }
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
                });
            }
        }
        public void Save_Click()
        {
            this.AlreadySavedOnce = true;
            this.Show();
            for (int i = 0; i < this.AllChildrenOfPages_ByIndex.Count; i++)
            {
                var pgSize = new iTextSharp.text.Rectangle(this.AllChildrenOfPages_ByIndex[i][0].Width, this.AllChildrenOfPages_ByIndex[i][0].Height);
                Document PagePDF = new Document(pgSize);     
                PdfWriter pWriter = PdfWriter.GetInstance(PagePDF, new FileStream(@"C:\Program Files (x86)\MSE\Magic xpa 4.7\Projects\PDF_Creator_UserControlOriente\PDFs\NeedToMerge\" + i.ToString() + ".pdf", FileMode.OpenOrCreate));
                PagePDF.Open();

                iTextSharp.text.Image ItextImageBackground = iTextSharp.text.Image.GetInstance(this.AllChildrenOfPages_ByIndex[i][0].Image,
                                                             System.Drawing.Imaging.ImageFormat.Png);
                ItextImageBackground.SetAbsolutePosition(0, 0);
                PagePDF.Add(ItextImageBackground);
                for (int j = 1; j < this.AllChildrenOfPages_ByIndex[i].Count && (this.AllChildrenOfPages_ByIndex[i][j].Visible == true); j++)
                {
                    iTextSharp.text.Image ItextImage = iTextSharp.text.Image.GetInstance(this.AllChildrenOfPages_ByIndex[i][j].Image,
                                                         System.Drawing.Imaging.ImageFormat.Png);
                    int OutputX = this.AllChildrenOfPages_ByIndex[i][j].Location.X;
                    int OutputY = this.AllChildrenOfPages_ByIndex[i][0].Height-(2*AllChildrenOfPages_ByIndex[i][j].Location.Y)+(this.AllChildrenOfPages_ByIndex[i][j].Location.Y)-this.AllChildrenOfPages_ByIndex[i][j].Height;
                    ItextImage.SetAbsolutePosition(OutputX, OutputY);
                    PagePDF.Add(ItextImage);
                }
                PagePDF.Close();
                this.PDFsThatSupposedToMergeInto1_PDFFile.Add(@"C:\Program Files (x86)\MSE\Magic xpa 4.7\Projects\PDF_Creator_UserControlOriente\PDFs\NeedToMerge\" + i.ToString() + ".pdf");
            }
            //for (int i = 0; i < this.AllChildrenOfPages_ByIndex.Count; i++)
            //{
                //MessageBox.Show("Page " + i.ToString() + " got " + (this.AllChildrenOfPages_ByIndex[i].Count - 1).ToString() + " Stamps on it");
            //}
            this.MergeTo1PDF(PDFsThatSupposedToMergeInto1_PDFFile, this.OUTPUTPDF);
            this.LoadPDF(this.OUTPUTPDF);
        }
        public void ConverAllPDFsInsideFolderToImages(string input)
        {
            SautinSoft.PdfFocus f = new SautinSoft.PdfFocus();
            string pdfPath = input;
            f.OpenPdf(pdfPath);
            if (f.PageCount > 0)
            {
                f.ImageOptions.ImageFormat = ImageFormat.Jpeg;
                f.ImageOptions.Dpi = 95;
                f.ImageOptions.JpegQuality = 50000;
                for (int page = 1; page <= f.PageCount; page++)
                {
                    this.FromSinglePDF2ImagesList.Add(f.ToDrawingImage(page));
                }
            }
            f.ClosePdf();
            if (this.AlreadySavedOnce == false)
            {
                this.FPageSize = this.FromSinglePDF2ImagesList[0].Size;
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
                //string fileName = Stamp_Path;
                StringFormat sf = new StringFormat();
                sf.LineAlignment = StringAlignment.Center;
                this.AddStampLine(StampLineTextTitle, StampLineTitleTextFont, StampLineTitleTextStyle, StampLineTitleTextSize, StampLineTitleTextColor);
                this.AddStampLine(StampLineTextLine1, StampLineTextFont, StampLineTextStyle, StampLineTextSize, StampLineTextColor);
                this.AddStampLine(StampLineTextLine2, StampLineTextFont, StampLineTextStyle, StampLineTextSize, StampLineTextColor);
                this.AddStampLine(StampLineTextLine3, StampLineTextFont, StampLineTextStyle, StampLineTextSize, StampLineTextColor);
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
                float textHeigh = this.stamplines.Select(ds => ds.LineFontSize + interline).Sum();
                float textWidth = this.stamplines.Select(ds => (int)(ds.LineFontSize + 100)).Sum();
                float textPositionHeight = ((stamp.Height + textHeigh) / 2) - textHeigh + (textHeigh / 100);
                float textPositionWidth = ((stamp.Width + textWidth) / 2) - textWidth - (textWidth / 16);
                foreach (var item in this.stamplines)
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
                    foreach (var stampline in this.stamplines)
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
                this.DifferantStamps.Add(bitmap);
                //bitmap.Save(fileName, ImageFormat.Png);
                //bitmap.Dispose();
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
            Console.WriteLine("MouseDown");
            if (this.Dragging == true)
            {
                this.Dragging = false;
                this.Cursor = Cursors.Default;
            }
            else
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
                c.Location = new Point((int)(MousePosition.X-this.Parent.Location.X-(c.Width/2)-(Cursor.Size.Width/4))+1, 
                                       (int)(MousePosition.Y-this.Parent.Location.Y-(c.Height/2)-(Cursor.Size.Height)+this.ValueModuluPageSize)+1);
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
            System.Drawing.Image LoadingImage = this.DifferantStamps[0];
            var TestPic = new PictureBox
            {
                Name = "pictureBox",
                Size = new Size(LoadingImage.Width, LoadingImage.Height),
                Location = new Point((this.PDF_Pages_pictureBoxList[0].Width/2)-(LoadingImage.Width/2) , (this.PDF_Pages_pictureBoxList[0].Height / 2) - (LoadingImage.Height / 2)),
                /*
                                Location = new Point((int)(MousePosition.X- this.Parent.Location.X - (LoadingImage.Width/2)),
                                    (int)(MousePosition.Y- this.Parent.Location.Y - (LoadingImage.Height/2)+ this.ValueModuluPageSize)),
                 */

                BackColor = Color.White,
                Image = LoadingImage,
            };
            TestPic.MouseDown += new System.Windows.Forms.MouseEventHandler(TestPic_MouseDown);
            TestPic.MouseMove += new System.Windows.Forms.MouseEventHandler(TestPic_MouseMove);
            TestPic.MouseUp += new System.Windows.Forms.MouseEventHandler(TestPic_MouseUp);
            Stamps_pictureBoxList.Add(TestPic);
            this.Dragging = true;
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //MessageBox.Show("MousePosition = "+ MousePosition.X+"    WindowX = "+this.WindowX+"    StampWidth = "+TestPic.Width
            //    +"      PDF_Position = "+ this.PDF_Pages_pictureBoxList[0].Location.X+"     Stamp Position = "+TestPic.Location.X);

            this.UndoList.Add(TestPic);
            int overlap = 0;
            int CurrentIndex = -1;
            for (int i = 0; i < this.PDF_Pages_pictureBoxList.Count; i++)
            {
                int TempOverlap = isPictureBox_OverLappingAnother(this.PDF_Pages_pictureBoxList[i], TestPic);
                //MessageBox.Show("Page Y : " + this.PDF_Pages_pictureBoxList[i].Location.Y.ToString() + "  MouseX : " + MousePosition.X.ToString() + "  MouseY : " + MousePosition.Y.ToString());
                //MessageBox.Show("TestPicLocation : " + TestPic.Location.ToString());
                if (TempOverlap >= overlap)
                {
                    overlap = TempOverlap;
                    CurrentIndex = i;
                }
            }
            whichPageI_editNow = CurrentIndex;
            this.PDF_Pages_pictureBoxList[CurrentIndex].Controls.Add(TestPic);
            this.AllChildrenOfPages_ByIndex[CurrentIndex].Add(TestPic);
            TestPic.BringToFront();
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////    HOW ADDED PICTURES ARE GONNA REACT WHEN DIFFERENT EVENTS ACCURE    /////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////
    }
}
