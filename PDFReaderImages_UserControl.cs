#region ||~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Title ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~||
/*
                             .----------------.  .----------------.  .----------------. 
                            | .--------------. || .--------------. || .--------------. |
                            | |   ______     | || |  ________    | || |  _________   | |
                            | |  |_   __ \   | || | |_   ___ `.  | || | |_   ___  |  | |
                            | |    | |__) |  | || |   | |   `. \ | || |   | |_  \_|  | |
                            | |    |  ___/   | || |   | |    | | | || |   |  _|      | |
                            | |   _| |_      | || |  _| |___.' / | || |  _| |_       | |
                            | |  |_____|     | || | |________.'  | || | |_____|      | |
                            | |              | || |              | || |              | |
                            | '--------------' || '--------------' || '--------------' |
                             '----------------'  '----------------'  '----------------' 
 .----------------.  .----------------.  .----------------.  .----------------.  .----------------.  .----------------. 
| .--------------. || .--------------. || .--------------. || .--------------. || .--------------. || .--------------. |
| |  _________   | || |  ________    | || |     _____    | || |  _________   | || |     ____     | || |  _______     | |
| | |_   ___  |  | || | |_   ___ `.  | || |    |_   _|   | || | |  _   _  |  | || |   .'    `.   | || | |_   __ \    | |
| |   | |_  \_|  | || |   | |   `. \ | || |      | |     | || | |_/ | | \_|  | || |  /  .--.  \  | || |   | |__) |   | |
| |   |  _|  _   | || |   | |    | | | || |      | |     | || |     | |      | || |  | |    | |  | || |   |  __ /    | |
| |  _| |___/ |  | || |  _| |___.' / | || |     _| |_    | || |    _| |_     | || |  \  `--'  /  | || |  _| |  \ \_  | |
| | |_________|  | || | |________.'  | || |    |_____|   | || |   |_____|    | || |   `.____.'   | || | |____| |___| | |
| |              | || |              | || |              | || |              | || |              | || |              | |
| '--------------' || '--------------' || '--------------' || '--------------' || '--------------' || '--------------' |
 '----------------'  '----------------'  '----------------'  '----------------'  '----------------'  '----------------' 
                      .----------------.  .----------------.  .----------------.  .----------------. 
                     | .--------------. || .--------------. || .--------------. || .--------------. |
                     | |    _____     | || |     ____     | || |    _____     | || |    _____     | |
                     | |   / ___ `.   | || |   .'    '.   | || |   / ___ `.   | || |   / ___ `.   | |
                     | |  |_/___) |   | || |  |  .--.  |  | || |  |_/___) |   | || |  |_/___) |   | |
                     | |   .'____.'   | || |  | |    | |  | || |   .'____.'   | || |   .'____.'   | |
                     | |  / /____     | || |  |  `--'  |  | || |  / /____     | || |  / /____     | |
                     | |  |_______|   | || |   '.____.'   | || |  |_______|   | || |  |_______|   | |
                     | |              | || |              | || |              | || |              | |
                     | '--------------' || '--------------' || '--------------' || '--------------' |
                      '----------------'  '----------------'  '----------------'  '----------------' 
<Company:
    __  ___            _         _____       ______                         
   /  |/  /___ _____ _(_)____   / ___/____  / __/ /__      ______ _________ 
  / /|_/ / __ `/ __ `/ / ___/   \__ \/ __ \/ /_/ __/ | /| / / __ `/ ___/ _ \
 / /  / / /_/ / /_/ / / /__    ___/ / /_/ / __/ /_ | |/ |/ / /_/ / /  /  __/
/_/  /_/\__,_/\__, /_/\___/   /____/\____/_/  \__/ |__/|__/\__,_/_/   \___/ 
             /____/                                                         >
<GITHUB:
       __           _                __               ___   ____ ___   ____ 
  ____/ /__  ____  (_)____      ____/ /__ _   __     |__ \ / __ \__ \ / __ \
 / __  / _ \/ __ \/ / ___/_____/ __  / _ \ | / /_______/ // / / /_/ // / / /
/ /_/ /  __/ / / / (__  )_____/ /_/ /  __/ |/ /_____/ __// /_/ / __// /_/ / 
\__,_/\___/_/ /_/_/____/      \__,_/\___/|___/     /____/\____/____/\____/  >
 */
#endregion ||~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~||
#region || ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ PDF-Editor 2022 ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~||
#region || ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ REQUIRED IMPORTS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~||
using System;
using System.IO;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using iTextSharp.text;
using iTextSharp.text.pdf;
#endregion ||~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~||
namespace PDFReaderImages_UserControl
{
    public partial class UserControl1 : UserControl
    {
        #region ||~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ IMPORTANT GLOBAL VARIABLES ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~||
        Document pdoc = new Document(PageSize.A4, 20f, 20f, 30f, 30f);
        Panel MainScrollablePanel = new Panel();
        Size FPageSize = new Size(0, 0);
        bool Dragging = false;   //  <---- AM I PRESSING ON AN IMAGE
        int WindowX = 0;
        int WindowY = 0;
        int UserControlX = 0;
        int UserControlY = 0;
        int pagesPadding = 10;
        int tempPadding = -1;
        int onWhatPageIWas_Before = 0;
        int onWhatPageIM_RightNow = 1;
        int onWhatPixelIWas_Before = 0;
        int onWhatPixelIM_RightNow = 1;
        bool ScrollingUp = false;
        bool ScrollingDown = true;
        bool MicroScrollingUp = false;
        bool MicroScrollingDown = true;
        int ScrolledValue = 0;
        int ValueModuluPageSize = 0;
        bool AlreadySavedOnce = false;
        bool eraseAllRedo = true;
        int PDF_ConvertQuality = 125;
        System.Drawing.Drawing2D.GraphicsPath mousePath = new System.Drawing.Drawing2D.GraphicsPath();
        string OUTPUTPDF = @"";
        string SignaturePath = @"";
        string PDFs_NeedToMerge_Path = @"";
        #region ||~~~~~ MINIMUM STAMP SIZE (10px x 10px) ~~~~||
        int stampWidth = 200;
        int stampHeight = 150;
        #endregion ||~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~||
        #endregion ||~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~||
        #region ||~~~~~~~~~~~~~~~~~~~~~~~~~~ GLOBAL LISTS THAT HELPS US TRACE THINGS ~~~~~~~~~~~~~~~~~~~~~~~~~~~||
        List<PictureBox> Stamps_pictureBoxList = new List<PictureBox>();
        List<PictureBox> PDF_Pages_pictureBoxList = new List<PictureBox>();
        List<List<PictureBox>> AllChildrenOfPages_ByIndex = new List<List<PictureBox>>();
        List<string> PDFsThatSupposedToMergeInto1_PDFFile = new List<string>();
        List<System.Drawing.Image> FromSinglePDF2ImagesList = new List<System.Drawing.Image>();
        List<System.Drawing.Image> DifferantStamps = new List<System.Drawing.Image>();
        List<PDFEditor.StampLine> stamplines = new List<PDFEditor.StampLine>();
        List<PictureBox> UndoList = new List<PictureBox>();
        List<PictureBox> RedoList = new List<PictureBox>();
        List<List<PictureBox>> UndoList_WithMultActions = new List<List<PictureBox>>();
        List<List<PictureBox>> RedoList_WithMultActions = new List<List<PictureBox>>();
        List<PictureBox> UndoList_WithMultActions_WaitingList = new List<PictureBox>();
        #endregion ||~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~||
        public UserControl1()
        {
            #region ||~~~~~~~~~~~~~ PANEL INITIALIZATION AND STAMP SIZE CHECK ~~~~~~~~~~~~~~||
            if (this.stampHeight < 10 || this.stampWidth < 10)
            {
                while (true)
                {
                    MessageBox.Show("STAMP SIZE TOO SMALL , SHOULD BE AT LEAST 10x10 (PIXELS)");
                    Thread.Sleep(1500);
                }
            }
            InitializeComponent();
            this.MainScrollablePanel.Width = this.Width - System.Windows.Forms.SystemInformation.VerticalScrollBarWidth;
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
            DoubleBuffered = true;
            #endregion ||~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~||
        }
        #region ||~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ SETTERS TO CONTROL FROM M-XPA ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~||
        #region ||~~~~~~~~~~~~~~~~~ STAMP SETTERS ~~~~~~~~~~~~~~~~~~~||
        public void SetStampRectangleWidth(int a)
        {
            this.stampWidth = a;
        }
        public void SetStampRectangleHeight(int a)
        {
            this.stampHeight = a;
        }
        public void SetSignature_Path(string path)
        {
            this.SignaturePath = path;
        }
        public void AddStampLine(string linetext, string linefont, string linefontstyle, float linefontsize, string linetextcolor)
        {
            PDFEditor.StampLine line = new PDFEditor.StampLine(linetext, linefont, linefontstyle, linefontsize, linetextcolor);
            this.stamplines.Add(line);
        }
        #endregion ||~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~||
        #region ||~~~~~~~~~~~~~~~~~ BASIC CONFIG ~~~~~~~~~~~~~~~~~~~||
        public void SetOutputPDF_Path(string path)
        {
            this.OUTPUTPDF = path;
        }
        public void SetPDFs_NeedToMergePath(string path)
        {
            this.PDFs_NeedToMerge_Path = path;
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
        public void SetPDF_ConvertQuality(int x)
        {
            this.PDF_ConvertQuality = x;
        }
        #endregion ||~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~||
        #endregion ||~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~||
        #region ||~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ INITIALIZE ONCE ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~||
        private void UserControl1_Load(object sender, EventArgs e) { }
        public void ScrollingHandele()
        {
            this.DraggingScrollFix();
            for (int i = 0; i < this.PDF_Pages_pictureBoxList.Count; i++)
            {
                if ((MainScrollablePanel.VerticalScroll.Value >= i * (this.PDF_Pages_pictureBoxList[0].Height + this.pagesPadding) - (this.Height / 2)) &&
                     (MainScrollablePanel.VerticalScroll.Value < (i + 1) * (this.PDF_Pages_pictureBoxList[0].Height + this.pagesPadding) - (this.Height / 2)))
                {
                    if (this.onWhatPageIM_RightNow != this.onWhatPageIWas_Before)
                    {
                        new ToolTip().Show("Page " + this.onWhatPageIM_RightNow, this,
                                            (int)(this.MainScrollablePanel.Width / 2),
                                            (int)(this.MainScrollablePanel.Location.Y) + 20,
                                            600);
                    }
                    this.onWhatPageIWas_Before = this.onWhatPageIM_RightNow;
                    this.onWhatPageIM_RightNow = (i + 1);
                    this.ScrolledValue = MainScrollablePanel.VerticalScroll.Value;
                    this.ValueModuluPageSize = this.ScrolledValue % (int)((this.PDF_Pages_pictureBoxList[0].Height / 2) + this.pagesPadding);
                    if (this.onWhatPageIWas_Before > this.onWhatPageIM_RightNow)
                    {
                        this.ScrollingDown = false;
                        this.ScrollingUp = true;
                    }
                    else if (this.onWhatPageIWas_Before < this.onWhatPageIM_RightNow)
                    {
                        this.ScrollingDown = true;
                        this.ScrollingUp = false;
                    }
                }
            }
        }
        public void DraggingScrollFix()
        {
            int ScrollingSensetivity = 110; // Higher number is more sensetive , upper limit is around 115 (wheel scrolling size)
            if (this.Dragging)
            {
                this.onWhatPixelIWas_Before = this.onWhatPixelIM_RightNow;
                this.onWhatPixelIM_RightNow = this.MainScrollablePanel.VerticalScroll.Value;
                if (this.onWhatPixelIWas_Before > this.onWhatPixelIM_RightNow)
                {
                    this.MicroScrollingDown = false;
                    this.MicroScrollingUp = true;
                }
                else if (this.onWhatPixelIWas_Before < this.onWhatPixelIM_RightNow)
                {
                    this.MicroScrollingDown = true;
                    this.MicroScrollingUp = false;
                }
                if (this.MicroScrollingDown == true && this.MicroScrollingUp == false)
                {
                    this.MainScrollablePanel.VerticalScroll.Value = this.MainScrollablePanel.VerticalScroll.Value - ScrollingSensetivity;
                }
                else if (this.MicroScrollingDown == false && this.MicroScrollingUp == true && this.MainScrollablePanel.VerticalScroll.Value > 0)
                {
                    this.MainScrollablePanel.VerticalScroll.Value = this.MainScrollablePanel.VerticalScroll.Value + ScrollingSensetivity;
                }
            }
            DoubleBuffered = true;
            this.MainScrollablePanel.Refresh();
        }
        public int FindLargestRowIn_stamplines()
        {
            int max = 0;
            for (int i = 0; i < this.stamplines.Count; i++)
            {
                if (this.stamplines[i].LineText.Length > max)
                {
                    max = this.stamplines[i].LineText.Length;
                }
            }
            return max;
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
                stamp.SignatureFilePath = SignaturePath;
                stamp.TextLines = new List<PDFEditor.StampLine>();
                StringFormat sf = new StringFormat();
                sf.LineAlignment = StringAlignment.Center;
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
                int FontWidth = 9;
                int FontHeight = 9;
                float textPositionHeight = ((int)(stampHeight / 2.5)) - (textHeigh / 2);
                List<RectangleF> rectangles = new List<RectangleF>() { new RectangleF(0, 0, stamp.Width, stamp.Height) };
                List<RectangleF> rectanglesShadow = new List<RectangleF>() { new RectangleF(-3, 3, stamp.Width - 3, stamp.Height + 3) };
                foreach (var item in this.stamplines)
                {
                    rectangles.Add(new RectangleF(
                        new PointF(0, textPositionHeight),
                        new SizeF(rectangles[0].Width, item.LineFontSize)));
                    rectanglesShadow.Add(new RectangleF(
                        new PointF(0, textPositionHeight),
                        new SizeF(rectanglesShadow[0].Width, item.LineFontSize + 3)));

                    textPositionHeight += item.LineFontSize + interline;
                }
                using (Graphics graphic = Graphics.FromImage(bitmap))
                {
                    Bitmap SignatureImage = new Bitmap(stamp.SignatureFilePath);
                    if ((SignatureImage.Height < (stampHeight / 2)) && (SignatureImage.Width < (stamp.Width / 2)))
                    {
                        stamp.SignatureLocationX = (int)((this.stampWidth / 2) - (SignatureImage.Width / 2));
                        stamp.SignatureLocationY = stamp.Height - SignatureImage.Height;
                        graphic.DrawImage(SignatureImage, new Point(stamp.SignatureLocationX, stamp.SignatureLocationY));
                    }
                    else
                    {
                        double tempW = SignatureImage.Width;
                        double tempH = SignatureImage.Height;
                        double tempRatio = tempW / tempH;
                        while (tempW > (stamp.Width / 2) || (tempH > (stamp.Height / 2)))
                        {
                            tempW = tempW - 1;
                            tempH = tempW / tempRatio;
                        }
                        SignatureImage = PDFEditor.Layout.ResizeImage(SignatureImage, (int)tempW, (int)(tempH));
                        stamp.SignatureLocationX = (int)((this.stampWidth / 2) - (SignatureImage.Width / 2));
                        stamp.SignatureLocationY = stamp.Height - SignatureImage.Height;
                        graphic.DrawImage(SignatureImage, new Point(stamp.SignatureLocationX, stamp.SignatureLocationY));
                    }
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
                            graphic.DrawString(stampline.LineText, font, new SolidBrush(Color.Black), rectanglesShadow[rectArea], format);
                            graphic.DrawString(stampline.LineText, font, stampline.LineTextColor, rectangles[rectArea], format);
                        }
                    }

                }
                this.DifferantStamps.Add(bitmap);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            ///////~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~///////
        }
        #endregion ||~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~||
        #region ||~~~~~~~~~~~~~~~~~~~~~~~~~~ FUNCTIONS THAT CALCULATE IN REAL-TIME ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~||
        public int ReturnOnWhatPage_isThisPictureBox(PictureBox a)
        {
            for (int i = 0; i < this.AllChildrenOfPages_ByIndex.Count; i++)
            {
                for (int j = 0; j < this.AllChildrenOfPages_ByIndex[i].Count; j++)
                {
                    if (a == this.AllChildrenOfPages_ByIndex[i][j])
                    {
                        return i;
                    }
                }
            }
            return -1;
        }
        public int isPictureBox_OverLappingAnother(PictureBox WholePage, PictureBox Stamp)
        {
            System.Drawing.Rectangle intersectionArea = System.Drawing.Rectangle.Intersect(WholePage.Bounds, Stamp.Bounds);
            return intersectionArea.Size.Height;
        }
        public void UpdateComponentPosition(double x, double y)
        {
            this.UserControlX = (int)(x * 1.25);
            this.UserControlY = (int)(y * 1.6);
        }
        public void MakeStampChildOfThePage(PictureBox WholePage, PictureBox Stamp)
        {
            WholePage.Controls.Add(Stamp);
        }
        private void eraseAllObjects()
        {
            string[] filePaths = Directory.GetFiles(this.PDFs_NeedToMerge_Path);
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
            for (int i = 0; i < this.UndoList.Count; i++)
            {
                this.UndoList.RemoveAt(i);
            }
            for (int i = 0; i < this.RedoList.Count; i++)
            {
                this.RedoList.RemoveAt(i);
            }
            for (int i = 0; i < this.UndoList_WithMultActions.Count; i++)
            {
                for (int j = 0; j < this.UndoList_WithMultActions[i].Count; j++)
                {
                    this.UndoList_WithMultActions[i][j].Controls.Clear();
                    this.UndoList_WithMultActions[i].RemoveAt(j);
                }
                this.UndoList_WithMultActions.RemoveAt(i);
            }
            for (int i = 0; i < this.RedoList_WithMultActions.Count; i++)
            {
                for (int j = 0; j < this.RedoList_WithMultActions[i].Count; j++)
                {
                    this.RedoList_WithMultActions[i][j].Controls.Clear();
                    this.RedoList_WithMultActions[i].RemoveAt(j);
                }
                this.RedoList_WithMultActions.RemoveAt(i);
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
            this.PDF_Pages_pictureBoxList = new List<PictureBox>();
            this.AllChildrenOfPages_ByIndex = new List<List<PictureBox>>();
            this.PDFsThatSupposedToMergeInto1_PDFFile = new List<string>();
            this.FromSinglePDF2ImagesList = new List<System.Drawing.Image>();
            this.UndoList_WithMultActions = new List<List<PictureBox>>();
            this.RedoList_WithMultActions = new List<List<PictureBox>>();
            this.MainScrollablePanel.Controls.Clear();
        }
        #endregion ||~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~||
        #region ||~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ PDF FILE HANDLING ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~||
        public void ConverAllPDFsInsideFolderToImages(string input)
        {
            SautinSoft.PdfFocus f = new SautinSoft.PdfFocus();
            string pdfPath = input;
            f.OpenPdf(pdfPath);
            if (f.PageCount > 0)
            {
                f.ImageOptions.ImageFormat = ImageFormat.Jpeg;
                f.ImageOptions.Dpi = this.PDF_ConvertQuality;
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
            int ScrollBarWidth = System.Windows.Forms.SystemInformation.VerticalScrollBarWidth;
            int ScrollBarHeight = System.Windows.Forms.SystemInformation.HorizontalScrollBarHeight;
            int TitleBarHight = System.Windows.Forms.SystemInformation.CaptionHeight;
            if (this.AlreadySavedOnce == false)
            {
                this.MainScrollablePanel.Size = new System.Drawing.Size(this.Size.Width, this.Size.Height);
            }
        }
        public void LoadPDF(string PDFFilePath)
        {
            if (this.AlreadySavedOnce)
            {
                this.eraseAllObjects();
                this.ConverAllPDFsInsideFolderToImages(this.OUTPUTPDF);
            }
            else
            {
                this.ConverAllPDFsInsideFolderToImages(PDFFilePath);
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
            }
            this.BackColor = Color.DarkGray;
        }
        #endregion ||~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~||
        #region ||~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ PDF FILE SAVING ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~||
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
                PdfWriter pWriter = PdfWriter.GetInstance(PagePDF, new FileStream(this.PDFs_NeedToMerge_Path + i.ToString() + ".pdf", FileMode.OpenOrCreate));
                iTextSharp.text.Image ItextImageBackground = iTextSharp.text.Image.GetInstance(this.AllChildrenOfPages_ByIndex[i][0].Image,
                                                             System.Drawing.Imaging.ImageFormat.Png);
                ItextImageBackground.SetAbsolutePosition(0, 0);
                PagePDF.Open();
                PagePDF.Add(ItextImageBackground);
                for (int j = 1; j < this.AllChildrenOfPages_ByIndex[i].Count; j++)
                {
                    if (this.AllChildrenOfPages_ByIndex[i][j].Visible)
                    {
                        iTextSharp.text.Image ItextImage = iTextSharp.text.Image.GetInstance(this.AllChildrenOfPages_ByIndex[i][j].Image,
                                                         System.Drawing.Imaging.ImageFormat.Png);
                        int OutputX = this.AllChildrenOfPages_ByIndex[i][j].Location.X;
                        int OutputY = this.AllChildrenOfPages_ByIndex[i][0].Height - (2 * AllChildrenOfPages_ByIndex[i][j].Location.Y) + (this.AllChildrenOfPages_ByIndex[i][j].Location.Y) - this.AllChildrenOfPages_ByIndex[i][j].Height;
                        ItextImage.SetAbsolutePosition(OutputX, OutputY);
                        PagePDF.Add(ItextImage);
                    }

                }
                PagePDF.Close();
                this.PDFsThatSupposedToMergeInto1_PDFFile.Add(this.PDFs_NeedToMerge_Path + i.ToString() + ".pdf");
            }
            this.MergeTo1PDF(PDFsThatSupposedToMergeInto1_PDFFile, this.OUTPUTPDF);
            this.LoadPDF(this.OUTPUTPDF);
        }
        #endregion ||~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~||
        #region ||~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ FUNCTIONAL METHODS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~||
        public void UndoAction()
        {
            this.eraseAllRedo = false;
            /*if (this.UndoList.Count > 0)
            {
                this.UndoList[this.UndoList.Count - 1].Visible = false;
                this.RedoList.Add(this.UndoList[this.UndoList.Count - 1]);
                this.UndoList.RemoveAt(this.UndoList.Count - 1);

            }*/
            

            if (this.UndoList_WithMultActions_WaitingList.Count > 0)
            {
                int searchingIndex = -1;
                for (int i = 0; i < this.UndoList_WithMultActions.Count; i++)
                {
                    if (((PictureBox)this.UndoList_WithMultActions_WaitingList[this.UndoList_WithMultActions_WaitingList.Count - 1]) == this.UndoList_WithMultActions[i][0] && this.UndoList_WithMultActions[i].Count > 0)
                    {
                        searchingIndex = i;

                    }
                }
                if (searchingIndex > -1)
                {
                    this.UndoList_WithMultActions[searchingIndex][0].Location = new Point(
                                                                                this.UndoList_WithMultActions[searchingIndex][this.UndoList_WithMultActions[searchingIndex].Count - 2].Location.X,
                                                                                this.UndoList_WithMultActions[searchingIndex][this.UndoList_WithMultActions[searchingIndex].Count - 2].Location.Y);
                    this.UndoList_WithMultActions[searchingIndex].RemoveAt(this.UndoList_WithMultActions[searchingIndex].Count - 1);
                    this.UndoList_WithMultActions_WaitingList.RemoveAt(this.UndoList_WithMultActions_WaitingList.Count - 1);
                    Invalidate();
                }
                else
                {
                    this.UndoList_WithMultActions_WaitingList[this.UndoList_WithMultActions_WaitingList.Count - 1].Visible = false;
                    this.RedoList.Add(this.UndoList_WithMultActions_WaitingList[this.UndoList_WithMultActions_WaitingList.Count - 1]);
                    this.UndoList_WithMultActions_WaitingList.RemoveAt(this.UndoList_WithMultActions_WaitingList.Count - 1);
                }

            }



        }
        public void RedoAction()
        {
            if ((this.RedoList.Count > 0) && (this.eraseAllRedo != true))
            {
                this.RedoList[this.RedoList.Count - 1].Visible = true;
                this.UndoList.Add(this.RedoList[this.RedoList.Count - 1]);
                this.RedoList.RemoveAt(this.RedoList.Count - 1);
            }
            else
            {
                for (int i = 0; i < this.RedoList.Count; i++)
                {
                    this.RedoList.RemoveAt(i);
                    this.RedoList = null;
                    this.RedoList = new List<PictureBox>();
                }
            }
        }
        public void PrintStatsCoords()
        {
            /*
            MessageBox.Show("this parent:      " + this.Parent.Location.ToString());
            MessageBox.Show("this window:      [" + this.WindowX.ToString() + "," + this.WindowY.ToString() + "]");
            MessageBox.Show("this parent size:      " + this.Parent.Size.ToString());
            MessageBox.Show("this Scroll Y:    " + this.MainScrollablePanel.VerticalScroll.Value.ToString());
            MessageBox.Show("this Panel Location:    " + this.MainScrollablePanel.Location.ToString());
            MessageBox.Show("this Panel Size:    " + this.MainScrollablePanel.Size.ToString());
            MessageBox.Show("this Location:    " + this.Location.ToString());
            MessageBox.Show("this Size:    " + this.Size.ToString());
            MessageBox.Show("this Page Y:      " + this.PDF_Pages_pictureBoxList[this.onWhatPageIM_RightNow - 1].Location.Y.ToString());
            MessageBox.Show("this Mouse:       " + MousePosition.ToString());
            MessageBox.Show("UserControl Location [" + this.UserControlX.ToString() + "," + this.UserControlY.ToString() + "]");
            */
            for (int i = 0; i < this.UndoList_WithMultActions[2].Count; i++)
            {
                MessageBox.Show(this.UndoList_WithMultActions[2][i].Location.ToString());
            }

        }
        #endregion ||~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~||
        #region ||~~~~~~~~~~~~~~ HOW ADDED PICTURES ARE GONNA REACT WHEN DIFFERENT EVENTS ACCURE ~~~~~~~~~~~~~~~||
        public void TestPic_MouseDown(object sender, EventArgs e)
        {
            int searchingIndex = -1;
            for (int i = 0; i < this.UndoList_WithMultActions.Count; i++)
            {
                if (((PictureBox)sender)== this.UndoList_WithMultActions[i][0])
                {
                    searchingIndex = i;
                }
            }
            Console.WriteLine("MouseDown");
            if (this.Dragging == true)
            {
                this.Dragging = false;
                this.Cursor = Cursors.Default;
                var Temp4UndoRedoPictureBox = new PictureBox
                {
                    Name = "pictureBox",
                    Size = new Size(((PictureBox)sender).Width, ((PictureBox)sender).Height),
                    Location = new Point(((PictureBox)sender).Location.X, ((PictureBox)sender).Location.Y),
                    BackColor = Color.Transparent,
                    Image = ((PictureBox)sender).Image,
                };
                this.UndoList_WithMultActions[searchingIndex].Add(Temp4UndoRedoPictureBox);
                this.UndoList_WithMultActions_WaitingList.Add(Temp4UndoRedoPictureBox);
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
            Console.WriteLine("MouseUp" + "   " + this.pagesPadding);
            ((PictureBox)sender).BorderStyle = BorderStyle.None;
        }
        public void TestPic_MouseMove(object sender, EventArgs e)
        {
            this.tempPadding = this.pagesPadding;
            this.pagesPadding = 0;
            int temp = 1;
            int zero = 0;
            int One = 1;
            if (this.PDF_Pages_pictureBoxList[this.onWhatPageIM_RightNow - 1].Location.Y >= 0)
            {
                temp = this.PDF_Pages_pictureBoxList[this.onWhatPageIM_RightNow - 1].Location.Y + (this.pagesPadding * (this.onWhatPageIM_RightNow - 1));
                zero = 0;
            }
            else if (this.PDF_Pages_pictureBoxList[this.onWhatPageIM_RightNow - 1].Location.Y < 0)
            {
                temp = this.PDF_Pages_pictureBoxList[this.onWhatPageIM_RightNow - 1].Location.Y;
                zero = 0;
            }
            else
            {
                temp = 0;
                One = -1;
                zero = 1;
            }
            int MagicNumber = 80;
            int WindowBorder = 1;
            int WindowTopBar = System.Windows.Forms.SystemInformation.CaptionHeight;
            Control c = sender as Control;
            if ((this.Dragging) && (c != null) && (1 + this.ReturnOnWhatPage_isThisPictureBox(((PictureBox)sender)) == this.onWhatPageIM_RightNow))
            {
                c.Location = new Point((int)(MousePosition.X - this.Parent.Location.X - (c.Width / 2) - (Cursor.Size.Width / 4)) + 1 - this.WindowX - this.Location.X,
                                       (int)(MousePosition.Y - this.Parent.Location.Y - (c.Height / 2) - (Cursor.Size.Height)) + 1 - (One * (temp))+70 + (this.ValueModuluPageSize * zero) - this.WindowY - this.Location.Y- MagicNumber);

                ((PictureBox)sender).BorderStyle = BorderStyle.FixedSingle;
                this.Cursor = Cursors.Cross;
            }

            this.pagesPadding = this.tempPadding;
            this.tempPadding = -1;
            this.MainScrollablePanel.Invalidate();

            if (this.Dragging)
            {
                System.Drawing.Rectangle CursorBound = new System.Drawing.Rectangle(this.WindowX + this.Location.X + (c.Width / 2),
                                                                                    this.WindowY + this.Location.Y + (c.Height) + WindowTopBar,
                                                                                    this.MainScrollablePanel.Width  - c.Width,
                                                                                    this.MainScrollablePanel.Height -c.Height);
                Cursor.Clip = CursorBound;
            }
            else
            {
                Cursor.Clip = new System.Drawing.Rectangle(-2,-2,-1,-1);
            }
        }
        #endregion ||~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~||
        #region ||~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ CREATE STAMP AT MOUSE LOCATION ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~||
        public void ApprovedStamp_Click(int WinX, int WinY)
        {
            int overlap = 0;
            int CurrentIndex = -1;
            this.eraseAllRedo = true;
            this.Dragging = true;
            this.UpdateWindowPosition(WinX, WinY);
            #region ||~~~~~~~ CREATE NEW IMAGE INSTANCE ON YOUR PAGE CENTER POSITION AND IF YOU MOVE MOUSE PIC WILL MOVE TOO UNTIL MOUSECLICK ~~~~~~~||
            System.Drawing.Image LoadingImage = this.DifferantStamps[0];

            var TestPic = new PictureBox
            {
                Name = "pictureBox",
                Size = new Size(LoadingImage.Width, LoadingImage.Height),
                Location = new Point((this.PDF_Pages_pictureBoxList[0].Width / 2) - (LoadingImage.Width / 2),
                                     100+this.ValueModuluPageSize),
                BackColor = Color.Transparent,
                Image = LoadingImage,
            };
            TestPic.MouseDown += new System.Windows.Forms.MouseEventHandler(TestPic_MouseDown);
            TestPic.MouseMove += new System.Windows.Forms.MouseEventHandler(TestPic_MouseMove);
            TestPic.MouseUp += new System.Windows.Forms.MouseEventHandler(TestPic_MouseUp);
            TestPic.Visible = true;
            Stamps_pictureBoxList.Add(TestPic);
            #endregion ||~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~||
            this.UndoList.Add(TestPic);

            this.UndoList_WithMultActions.Add(new List<PictureBox> { TestPic });
            this.UndoList_WithMultActions_WaitingList.Add(TestPic);
            for (int i = 0; i < this.PDF_Pages_pictureBoxList.Count; i++)
            {
                int TempOverlap = isPictureBox_OverLappingAnother(this.PDF_Pages_pictureBoxList[i], TestPic);
                if (TempOverlap >= overlap)
                {
                    overlap = TempOverlap;
                    CurrentIndex = i;
                }
            }
            this.PDF_Pages_pictureBoxList[CurrentIndex].Controls.Add(TestPic);
            this.AllChildrenOfPages_ByIndex[CurrentIndex].Add(TestPic);
            TestPic.BringToFront();
            Cursor.Position = new Point(TestPic.Location.X + (TestPic.Width/2)+this.WindowX+this.Location.X, TestPic.Location.Y + (TestPic.Height)+this.WindowY+this.Location.Y-this.ValueModuluPageSize);
            new ToolTip().Show("Added to Page " + (CurrentIndex+1).ToString(), this,
                                            (int)(this.MainScrollablePanel.Width / 2),
                                            (int)(this.MainScrollablePanel.Location.Y) + 20,
                                            600);
        }
        #endregion ||~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~||
    }
}
#endregion ||~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~||
