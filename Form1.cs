using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.IO.Ports;
using Microsoft.VisualBasic;
using System.Windows.Forms.DataVisualization.Charting;
using System.IO;
using System.Globalization;



namespace SoftSensConf
{
    public partial class Form1 : Form
    {
        List<int> analogReading = new List<int>();
        List<DateTime> timeStamp = new List<DateTime>();

        List<float> analogReading1 = new List<float>();
        List<DateTime> timeStamp1 = new List<DateTime>();




        private char viewConf;

        string Status;

        

        public Form1()
        {
            InitializeComponent();
            comboBox1.Items.AddRange(SerialPort.GetPortNames());
            comboBox1.Text = "--Select--";
            string[] bitRate = new string[] { "1200", "2400", "4800", "9600", "19200", "38400", "57600", "115200" };
            comboBox2.Items.AddRange(bitRate);
            comboBox2.SelectedIndex = comboBox2.Items.IndexOf("9600");

            serialPort1.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

             timer1.Interval = 5000;
             timer1.Tick += new EventHandler(timer1_Tick);
             timer2.Interval = 5000;
             timer2.Tick += new EventHandler(timer2_Tick);

    
    }

        // Combox1
        private void comboBox1_MouseClick(object sender, MouseEventArgs e)
        {
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(SerialPort.GetPortNames());
        }

        // consct med mikrokontroller
        private void button1_Click(object sender, EventArgs e)
        {
            serialPort1.PortName = comboBox1.Text;
            serialPort1.Open();

            if (serialPort1.IsOpen)
            {
                textBox1.AppendText("Connection Established is successful");
             
            }
            else
            {
                MessageBox.Show("Porten er ikke åpen!");
            }

        }

        // disconect med mikrokontroller
        private void button2_Click(object sender, EventArgs e)
        {
            serialPort1.Close();
            MessageBox.Show("Frakoblet!");
        }


        void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {

            int iVba;
            float iVab;
            var RecivedData = ((SerialPort)sender).ReadLine(); 
            string[] separateParts = RecivedData.Split(';');


            if (RecivedData.Contains("readconf"))
            {

                NameValueTextBox.Text = separateParts[1];
                LowRValueTextBox.Text = separateParts[2];
                UpperRValueTextBox.Text = separateParts[3];
                ALowValueTextBox.Text = separateParts[4];
                AHighValueTextBox.Text = separateParts[5];

            }
            else if (RecivedData.Contains("readraw"))
            {

                textBox2.AppendText(separateParts[1] + "\r\n");

                if (int.TryParse(separateParts[1], out iVba))
                {
                    analogReading.Add(iVba);
                    timeStamp.Add(DateTime.Now);
                    chart1.Series["Raw"].Points.DataBindXY(timeStamp, analogReading);
                    chart1.Invalidate();



                }
                else
                {
                    MessageBox.Show("Gikk ikke");
                }

                
            }
            else if (RecivedData.Contains("readscaled"))
            {
                textBox2.AppendText(separateParts[1] + "\r\n");
                
                iVab = float.Parse(separateParts[1], CultureInfo.InvariantCulture.NumberFormat);
                if (iVab != 1000)
                {
                    analogReading1.Add(iVab);
                    timeStamp1.Add(DateTime.Now);
                    chart1.Series["Scaled"].Points.DataBindXY(timeStamp1, analogReading1);
                    chart1.Invalidate();
                }
                

            }
            else if (RecivedData == " ")
            {
                // en melding 
            }
            else
            {
                MessageBox.Show(RecivedData);
            }




            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
           
            serialPort1.WriteLine("readscaled");

        }
        private void timer2_Tick(object sender, EventArgs e)
        {

            serialPort1.WriteLine("readraw");

        }

     

        // Read sensor informasjon
        private void button3_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                if(radioButton2.Checked == true)
                {
                    textBox2.ReadOnly = true;
                    timer1.Start();
                }
                else if (radioButton1.Checked == true)
                {
                    textBox2.ReadOnly = true;
                    timer2.Start();
                }
                else
                {
                    MessageBox.Show("check off one of the radoboxes");
                }
            }
            else 
            {
                MessageBox.Show("Porten er ikke åpen!");
            }
            }

        // stop a lese sensor informajson
        private void button4_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                timer1.Stop();
                timer2.Stop();

            }
            else
            {
                MessageBox.Show("Porten er ikke åpen!");
            }

        
        }
        private void button9_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        { //fiks opp til slutt

            if (serialPort1.IsOpen) 
            {
                if ((textBox22.Text == String.Empty) || (textBox4.Text == String.Empty) || (textBox5.Text == String.Empty)
                    || (textBox6.Text == String.Empty) || (textBox7.Text == String.Empty))
                {
                    MessageBox.Show("One or more of the textboxes are emty. can not save");
                }
                else
                {
                    string fileName = string.Empty;
                    saveFileDialog1.InitialDirectory = "C:\\tmp";
                    saveFileDialog1.Filter = ("ssc files | *.ssc");
                    saveFileDialog1.FilterIndex = 2;
                    saveFileDialog1.FileName = "";

                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        fileName = saveFileDialog1.FileName;
                        StreamWriter outputFile = new StreamWriter(@fileName);
                        outputFile.Write(NameValueTextBox.Text + ";" + LowRValueTextBox.Text + ";" + UpperRValueTextBox.Text + ";" + ALowValueTextBox.Text + ";" + AHighValueTextBox.Text);
                        outputFile.Close();

                    }

                }
            }
            else
            {
                MessageBox.Show("prøv igjen, noe feil");
            }
        }

        private void button6_Click(object sender, EventArgs e)
        { // fiks opp til slutt

            foreach (string line in File.ReadLines(@"C:\tmp\Text2.tmp"))
            {
                textBox1.AppendText(line);
            }
            

        }



        private void button8_Click(object sender, EventArgs e)
        {

            if (serialPort1.IsOpen)
            {
                serialPort1.WriteLine("readconf");
            }
            else
            {
                MessageBox.Show("Porten er ikke åpen!");
            }
          
        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void button10_Click(object sender, EventArgs e)

        {
            
            string n = textBox22.Text;
            string L = textBox4.Text;
            string U = textBox5.Text;
            string Al = textBox6.Text;
            string Ah = textBox7.Text;

            if ((n == String.Empty) || (L == String.Empty) || (U == String.Empty) || (Al == String.Empty) || (Ah == String.Empty))
            {
                MessageBox.Show("One or more of the textboxes are emty. can not save");  
            }

            string passord = Interaction.InputBox("Enter password", "Enter password");
            

            string NewPar = n + ";" + L + ";" + U + ";" + Al + ";" + Ah;
            serialPort1.WriteLine("writeconf>" + passord + ">" + NewPar);
            

           

        }

       
    }
}
