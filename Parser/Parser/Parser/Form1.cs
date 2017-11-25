using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Parser
{
    public partial  class Form1 : Form
    {
        string pathArtdrink;
        string pathFozzy;
        string pathNovus;
        string pathRozetka;

        private List<string> rozetkaBufferPrices;
        string value;
        HtmlWeb web;
        HtmlNode Price;
        HtmlAgilityPack.HtmlDocument document;

        //Names
        Products p;
        DataTable dt;
        StringBuilder log;

        //Prices
        List<string> artdrinkPrices;
        List<string> novusPrices;
        List<string> fozzyPrices;
        List<string> metroPrices;
        List<string> auchanPrices;
        List<string> rozetkaPricesFull;



        public Form1()
        {
            pathArtdrink = System.AppDomain.CurrentDomain.BaseDirectory + "artdrink.txt";
            pathFozzy = System.AppDomain.CurrentDomain.BaseDirectory + "fozzy.txt";
            pathNovus = System.AppDomain.CurrentDomain.BaseDirectory + "novus.txt";
            pathRozetka = System.AppDomain.CurrentDomain.BaseDirectory + "rozetka.txt";

            //Prices
            artdrinkPrices = new List<string>();
            novusPrices = new List<string>();
            fozzyPrices = new List<string>();
            metroPrices = new List<string>();
            auchanPrices = new List<string>();
            rozetkaPricesFull = new List<string>();
            p = new Products();
            log = new StringBuilder();
            dt = new DataTable();
            web =  new HtmlWeb();
            rozetkaBufferPrices = new List<string>();
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dt.Columns.Add("Товар", typeof(string));
            dt.Columns.Add("artdrink.com.ua", typeof(string));
            dt.Columns.Add("fozzy.zakaz.ua", typeof(string));
            dt.Columns.Add("novus.zakaz.ua", typeof(string));
            dt.Columns.Add("rozetka.com.ua", typeof(string));
            dataGridView1.RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            if (!File.Exists(pathArtdrink))
            {
                File.Create(pathArtdrink).Dispose();
            }
            if (!File.Exists(pathFozzy))
            {
                File.Create(pathFozzy).Dispose();
            }
            if (!File.Exists(pathNovus))
            {
                File.Create(pathNovus).Dispose();
            }
            if (!File.Exists(pathRozetka))
            {
                File.Create(pathRozetka).Dispose();
            }

            // Open the stream and read it back.
            using (StreamReader sr = File.OpenText(pathArtdrink))
            {
                string s = "";
                while ((s = sr.ReadLine()) != null)
                {
                    artdrinkPrices.Add(s);
                }
            }
            // Open the stream and read it back.
            using (StreamReader sr = File.OpenText(pathFozzy))
            {
                string s = "";
                while ((s = sr.ReadLine()) != null)
                {
                    fozzyPrices.Add(s);
                }
            }
            // Open the stream and read it back.
            using (StreamReader sr = File.OpenText(pathNovus))
            {
                string s = "";
                while ((s = sr.ReadLine()) != null)
                {
                    novusPrices.Add(s);
                }
            }
            // Open the stream and read it back.
            using (StreamReader sr = File.OpenText(pathRozetka))
            {
                string s = "";
                while ((s = sr.ReadLine()) != null)
                {
                    rozetkaPricesFull.Add(s);
                }
            }

            if (artdrinkPrices.ToArray().Length == 281 && fozzyPrices.ToArray().Length == 281 && novusPrices.ToArray().Length == 281 && rozetkaPricesFull.ToArray().Length == 281)
            {
                for (int i = 0; i < 281; i++)
                {
                    DataRow row = dt.NewRow();
                    row[0] = p.productsNamesArray[i];
                    row[1] = artdrinkPrices[i];
                    row[2] = fozzyPrices[i];
                    row[3] = novusPrices[i];
                    row[4] = rozetkaPricesFull[i];

                    dt.Rows.Add(row);
                }
                
                this.dataGridView1.DataSource = dt;
                dataGridView1.Columns[0].Width = 300;
            }
            

        }
    
        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url.AbsolutePath != (sender as WebBrowser).Url.AbsolutePath)
                return;

            var document = new HtmlAgilityPack.HtmlDocument();
            document.Load(new StringReader(webBrowser1.Document.Body.InnerHtml));

            var script = document.DocumentNode.Descendants().Where(n => n.Name == "script").ElementAt(21);
            var pattern = @"price:\s*(.*?)\s*,";
            var result = Regex.Match(script.InnerText, pattern).Groups[1].Value;

            if (result != null)
            {
                value = result;
            }
            else
            {
                value = "Нема на складі";
            }
            
            rozetkaBufferPrices.Add(value);
        }

        private void Calculate(int i)
        {
            double pow = Math.Pow(i, i);
        }

        public void parseArtdrink(IProgress<int> progress)
        {
            //Artdrink Shop
            Artdrink artdrink = new Artdrink();

            for (int i = 0; i < artdrink.artdrinkArray.Length; i++)
            {
                Calculate(i);
                if (artdrink.artdrinkArray[i] != "0")
                {
                    document = web.Load(artdrink.artdrinkArray[i]);
                    Price = document.DocumentNode.SelectSingleNode("//div[@class='price']//span[@id='formated_special'] | //div[@class='price']//span[@id='formated_price']");
                    if (Price != null)
                    {
                        artdrinkPrices.Add(Price.InnerText);
                    }
                    else
                    {
                        artdrinkPrices.Add("0");
                    }
                }
                else
                {
                    artdrinkPrices.Add("0");
                }

                if (progress != null)
                    progress.Report((i + 1) * 25 / 281);
               
            }
         

        }

        public void parseNovus(IProgress<int> progress)
        {
            //Novus Shop

            Novus novus = new Novus();

            for (int i = 0; i < novus.novusArray.Length; i++)
            {
                Calculate(i);
                if (novus.novusArray[i] != "0")
                {
                    document = web.Load(novus.novusArray[i]);
                    Price = document.DocumentNode.SelectSingleNode("//*[@class='grivna price']");
                    if (Price != null)
                    {
                        novusPrices.Add(Price.InnerText);
                    }
                    else
                    {
                        novusPrices.Add("Товару нема на складі");
                    }
                }
                else
                {
                    novusPrices.Add("0");
                }

                if (progress != null)
                    progress.Report(25+(i + 1) * 25 / 281);
            }
        }


        public void parseFozzy(IProgress<int> progress)
        {
            //Fozzy Shop

            HtmlNode nodeGr;
            HtmlNode nodeKop;

            Fozzy fozzy = new Fozzy();

            for (int i = 0; i < fozzy.fozzyArray.Length; i++)
            {
                Calculate(i);
                if (fozzy.fozzyArray[i] != "0")
                {
                    document = web.Load(fozzy.fozzyArray[i]);
                    nodeGr = document.DocumentNode.SelectSingleNode("*//body//span[@id='one-item-price']//span[@class='grivna price']");
                    nodeKop = document.DocumentNode.SelectSingleNode("*//body//span[@id='one-item-price']//span[@class='kopeiki']");

                    if (nodeGr != null)
                    {
                        fozzyPrices.Add(nodeGr.InnerText + " грн. " + nodeKop.InnerText + " коп.");
                    }
                    else
                    {
                        fozzyPrices.Add("-");
                    }

                }
                else
                {
                    fozzyPrices.Add("0");
                }
                if (progress != null)
                    progress.Report(50+(i + 1) * 25 / 281);

            }
        }

        public void parseRozetka(IProgress<int> progress)
        {
            //Rozetka Shop
            Rozetka rozetka = new Rozetka();

            webBrowser1.AllowNavigation = true;
            webBrowser1.ScriptErrorsSuppressed = true;
            webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_DocumentCompleted);

            
            for (int i = 0; i < rozetka.rozetkaArray.Length; i++)
            {
                Calculate(i);
                if (rozetka.rozetkaArray[i] != "0")
                {
                    webBrowser1.Navigate(rozetka.rozetkaArray[i]);

                }
                else
                {
                    rozetkaBufferPrices.Add("0");
                    rozetkaBufferPrices.Add("0");
                }
                while (webBrowser1.ReadyState != WebBrowserReadyState.Complete) { Application.DoEvents(); }
                if (progress != null)
                    progress.Report(75+(i + 1) * 25 / 281);
            }


            int index = 0;

            foreach (var item in rozetkaBufferPrices)
            {
                if (index % 2 != 1)
                {
                    rozetkaPricesFull.Add(item);
                }
                index++;
            }


        }

        private async void button1_Click(object sender, EventArgs e)
        {
            this.label3.Visible = true;
            this.button1.Enabled = false;

            label1.Text = "";
            log.Clear();
            progressBar1.Maximum = 100;
            progressBar1.Step = 1;
         
            var progress = new Progress<int>(v =>
            {
                // This lambda is executed in context of UI thread,
                // so it can safely update form controls
                progressBar1.Value = v;
            });

            artdrinkPrices.Clear();
            log.Append("Parsing Artdrink...\n");

            label1.Text = log.ToString();
            // Run operation in another thread
            await Task.Run(() => parseArtdrink(progress));
            log.Append("Artdrink parsed..." + "\n");
            label1.Text = log.ToString();

            novusPrices.Clear();
            log.Append("Parsing Novus...\n");
            label1.Text = log.ToString();
            await Task.Run(() => parseNovus(progress));
            log.Append("Novus parsed..." + "\n");
            label1.Text = log.ToString();

            fozzyPrices.Clear();
            log.Append("Parsing Fozzy...\n");
            label1.Text = log.ToString();
            // Run operation in another thread
            await Task.Run(() => parseFozzy(progress));
            log.Append("Fozzy parsed..." + "\n");
            label1.Text = log.ToString();


            rozetkaPricesFull.Clear();
            log.Append("Parsing Rozetka...\n");
            label1.Text = log.ToString();
            // Run operation in another thread
            parseRozetka(progress);
            log.Append("Rozetka parsed..." + "\n");
            label1.Text = log.ToString();

            label3.Visible = false;
            // TODO: Do something after all calculations
            for (int i = 0; i < 281; i++)
            {
                DataRow row = dt.NewRow();
                row[0] = p.productsNamesArray[i];
                row[1] = artdrinkPrices[i];
                row[2] = fozzyPrices[i];
                row[3] = novusPrices[i];
                row[4] = rozetkaPricesFull[i];

                dt.Rows.Add(row);
            }
            this.dataGridView1.DataSource = dt;

            File.WriteAllText(pathArtdrink, String.Empty);
            using (StreamWriter writer = new StreamWriter(pathArtdrink))
            {
                foreach (string s in artdrinkPrices)
                {
                    writer.WriteLine(s);
                }
            }
            artdrinkPrices.Clear();

            File.WriteAllText(pathFozzy, String.Empty);
            using (StreamWriter writer = new StreamWriter(pathFozzy))
            {
                foreach (string s in fozzyPrices)
                {
                    writer.WriteLine(s);
                }
            }
            fozzyPrices.Clear();

            File.WriteAllText(pathNovus, String.Empty);
            using (StreamWriter writer = new StreamWriter(pathNovus))
            {
                foreach (string s in novusPrices)
                {
                    writer.WriteLine(s);
                }
            }
            novusPrices.Clear();
            File.WriteAllText(pathRozetka, String.Empty);
            using (StreamWriter writer = new StreamWriter(pathRozetka))
            {
                foreach (string s in rozetkaPricesFull)
                {
                    writer.WriteLine(s);
                }
            }
            rozetkaPricesFull.Clear();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            (dataGridView1.DataSource as DataTable).DefaultView.RowFilter = string.Format("Товар LIKE '{0}%' OR Товар LIKE '% {0}%'", textBox1.Text);
        }
    }
}
