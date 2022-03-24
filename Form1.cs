using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestPDFUserControl
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            userControl11.SetOutputPDF_Path(@"C:\Program Files (x86)\MSE\Magic xpa 4.7\Projects\PDF_Creator_UserControlOriente\PDFs\pdf-output.pdf");
            userControl11.LoadPDF(@"C:\Program Files (x86)\MSE\Magic xpa 4.7\Projects\PDF_Creator_UserControlOriente\PDFs\pdf-sample2.pdf");
            userControl11.SetSignature_Path(@"C:\Program Files (x86)\MSE\Magic xpa 4.7\Projects\PDF_Creator_UserControlOriente\Resources\Signature4.png");
            userControl11.SetPDFs_NeedToMergePath(@"C:\Program Files (x86)\MSE\Magic xpa 4.7\Projects\PDF_Creator_UserControlOriente\PDFs\NeedToMerge\");
            userControl11.AddStampLine("מאושר", "Arial", "Bold", 40, "Green");
            userControl11.AddStampLine("אדר' אליסיה רובנשטיין", "Arial", "Bold", 18, "LightGreen");
            userControl11.AddStampLine("סגנית מהנדס העיר", "Arial", "Bold", 18, "LightGreen");
            userControl11.AddStampLine("ומנהלת מח' רישוי בניה", "Arial", "Bold", 18, "LightGreen");
            userControl11.AddStampLine("אדר' אליסיה רובנשטיין", "Arial", "Bold", 18, "Blue");
            userControl11.AddStampLine("סגנית מהנדס העיר", "Arial", "Bold", 18, "Blue");
            userControl11.AddStampLine("ומנהלת מח' רישוי בניה", "Arial", "Bold", 18, "Blue");
            userControl11.AddStampLine("אדר' אליסיה רובנשטיין", "Arial", "Bold", 18, "Blue");
            userControl11.AddStampLine("סגנית מהנדס העיר", "Arial", "Bold", 18, "Yellow");
            userControl11.AddStampLine("ומנהלת מח' רישוי בניה", "Arial", "Bold", 18, "Yellow");
            userControl11.AddStampLine("אדר' אליסיה רובנשטיין", "Arial", "Bold", 18, "LightGreen");
            userControl11.AddStampLine("סגנית מהנדס העיר", "Arial", "Bold", 18, "LightGreen");
            userControl11.AddStampLine("ומנהלת מח' רישוי בניה", "Arial", "Bold", 18, "LightGreen");
            userControl11.GenerateStamp();
            //userControl11.Location = new Point(0, 800);
            
        }
        private void button1_Click(object sender, EventArgs e)
        {
            userControl11.ApprovedStamp_Click(this.Location.X,this.Location.Y);
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            userControl11.Save_Click();
        }
        private void Form1_Move(object sender, System.EventArgs e)
        {
            //userControl11.UpdateWindowPosition(this.Location.X,this.Location.Y);
            //MessageBox.Show(this.Location.ToString());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            userControl11.PrintStatsCoords();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            userControl11.UndoAction();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            userControl11.RedoAction();
        }
    }



}
