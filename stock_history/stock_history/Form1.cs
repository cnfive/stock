using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Net;

using System.IO.Compression;
using System.Web;

using System.IO;

using HtmlAgilityPack;

using System.Text.RegularExpressions;

using System.Threading; 

namespace stock_history
{
    public partial class Form1 : Form
    {
        private int count=0;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Thread oThread2 = new Thread(new ThreadStart(get_stock_history));
                oThread2.Start(); // 程序运行的是Alpha.Beta()方法 
            this.timer1.Interval = 1;//设置每隔千分之一秒钟执行一次
            this.timer1.Enabled = true;
            this.timer1.Start();

        }
    
    private void get_stock_history()
    {
            string strCon = "Data Source=(local);database=lucene;uid=sa;pwd=qq901;";
            DataClasses1DataContext db;
            db = new DataClasses1DataContext(strCon);

            string code = textBox1.Text.Trim();

            for(int year=2006;year<2017;year++)
            {
                for (int jidu = 1; jidu < 5;jidu++ )
                {
                    string url = "http://money.finance.sina.com.cn/corp/go.php/vMS_MarketHistory/stockid/" + code + ".phtml?year=" + year.ToString() + "&jidu="+jidu.ToString();
                    string htmlbody = GetPageData(url);
                    //  Response.Write(htmlbody);
                    //获取数据表格
                    HtmlAgilityPack.HtmlDocument htmldoc = new HtmlAgilityPack.HtmlDocument();

                    htmldoc.LoadHtml(htmlbody);

                    HtmlNode rootNode = htmldoc.DocumentNode;

                    string s_xpath = "//table[@id='FundHoldSharesTable']";

                    string table = "";
                    if (rootNode.SelectSingleNode(s_xpath) != null)
                    {

                        table = rootNode.SelectSingleNode(s_xpath).InnerHtml;

                        //     Response.Write(table);

                        //获取股票名称
                        string xpath_name = "//thead[1]/tr[1]/th[1]";


                        //获取行元素

                        htmldoc.LoadHtml(table);

                        rootNode = htmldoc.DocumentNode;

                        string name = "";


                        string stock_name = "";

                        if (rootNode.SelectSingleNode(xpath_name) != null)
                        {

                            name = rootNode.SelectSingleNode(xpath_name).InnerText;
                            name = name.Replace("年季度历史交易", "");
                            //llll



                            stock_name = name;
                        }


                        //获取商户节点
                        string xpath_tr = "//tr";
                        //   HtmlNode rootNode9 = htmldoc5.DocumentNode;
                        HtmlNodeCollection trNodeList = rootNode.SelectNodes(xpath_tr);

                        List<string> tr_l = new List<string>();
                        foreach (HtmlNode d in trNodeList)
                        {


                            tr_l.Add(d.InnerHtml);

                            //    Response.Write(d.InnerHtml);
                        }
                        foreach (string p in tr_l)
                        {
                            stock_trade_history stock = new stock_trade_history();
                            stock.stock_name = stock_name;

                            htmldoc.LoadHtml(p);
                            rootNode = htmldoc.DocumentNode;

                            s_xpath = "//td[1]/div[1]/a[1]";
                            string riqi = "";
                            if (rootNode.SelectSingleNode(s_xpath) != null)
                            {

                                riqi = rootNode.SelectSingleNode(s_xpath).InnerText;

                                //llll

                                //   Response.Write(riqi);
                                stock.date = riqi;

                            }
                            s_xpath = "//td[2]/div[1]";
                            string kaipanjia = "";

                            if (rootNode.SelectSingleNode(s_xpath) != null)
                            {

                                kaipanjia = rootNode.SelectSingleNode(s_xpath).InnerText;

                                //llll

                                //  Response.Write(kaipanjia);

                                stock.kaipanjia = kaipanjia;
                            }
                            s_xpath = "//td[3]/div[1]";
                            string zuigaojia = "";

                            if (rootNode.SelectSingleNode(s_xpath) != null)
                            {

                                zuigaojia = rootNode.SelectSingleNode(s_xpath).InnerText;

                                //llll

                                //  Response.Write(zuigaojia);

                                stock.zuigaojia = zuigaojia;
                            }

                            s_xpath = "//td[4]/div[1]";
                            string soupanjia = "";

                            if (rootNode.SelectSingleNode(s_xpath) != null)
                            {

                                soupanjia = rootNode.SelectSingleNode(s_xpath).InnerText;

                                //llll

                                //    Response.Write(soupanjia);


                                stock.soupanjia = soupanjia;
                            }
                            s_xpath = "//td[5]/div[1]";
                            string zuidijia = "";

                            if (rootNode.SelectSingleNode(s_xpath) != null)
                            {

                                zuidijia = rootNode.SelectSingleNode(s_xpath).InnerText;

                                //llll

                                //  Response.Write(zuidijia);

                                stock.zuidijia = zuidijia;
                            }

                            s_xpath = "//td[6]/div[1]";
                            string jiaoyiliang = "";

                            if (rootNode.SelectSingleNode(s_xpath) != null)
                            {

                                jiaoyiliang = rootNode.SelectSingleNode(s_xpath).InnerText;

                                //llll

                                //  Response.Write(jiaoyiliang);
                                stock.jiaoyiliang = jiaoyiliang;
                            }

                            s_xpath = "//td[7]/div[1]";
                            string jiaoyijinge = "";

                            if (rootNode.SelectSingleNode(s_xpath) != null)
                            {

                                jiaoyijinge = rootNode.SelectSingleNode(s_xpath).InnerText;

                                //llll

                                //    Response.Write(jiaoyijinge);
                                stock.jiaoyijine = jiaoyijinge;
                            }
                            //提交数据
                            if (riqi != "")
                            {
                                db.stock_trade_history.InsertOnSubmit(stock);

                                ///提交插入操作
                                db.SubmitChanges();
                                count++;
                            }
                        }




                    }

                }

            }
            MessageBox.Show("获取结束！");

        }

        private void button2_Click(object sender, EventArgs e)
        {
            string strCon = "Data Source=(local);database=lucene;uid=sa;pwd=qq901;";
            DataClasses1DataContext db;
            db = new DataClasses1DataContext(strCon);
            string url = "http://quote.eastmoney.com/stocklist.html";
            string htmlbody = GetPageData(url);

            //获取 链接地址和标题

            string pattern5 = @"<a[\s\S]*?>[\s\S]*?</a>";
            string pattern11 = @"(?<=href=[\u0022\u0027]?)(?!javascript)((?<Protocol>\w+):/)?[\/]?(?<Domain>[\w.]+\/?)((?![\u0022\u0027])\S)*";


            ///定义获取标题的正则表达式
            string pattern = @"(?<=<title\s*[^>]*>).*(?=<\/title>)";

            string title = @"(?<=<a\s*[^>]*>)[\s\S]*?(?=<\/a>)";

            string pattern_dd = @"<dd[\s\S]*?>[\s\S]*?</dd>";
            htmlbody = htmlbody.Replace("<a name=\"sh\"/>","");
             htmlbody = htmlbody.Replace("<a name=\"sz\"/>","");
               MatchCollection matches_url_title = Regex.Matches(htmlbody, pattern5, RegexOptions.IgnoreCase);

               foreach (Match m_u_t in matches_url_title)
               {
                   //寻找带下一页文字的链接全部
                   if (m_u_t.Value.IndexOf("(") > -1)
                   {

                       MatchCollection matches_title = Regex.Matches(m_u_t.Value, title, RegexOptions.IgnoreCase);
                       foreach(Match t in matches_title)
                       {

                           stock_id_name id_name = new stock_id_name();

                      //     MessageBox.Show(t.Value);
                           id_name.stock_name = t.Value;
                           string strTest =t.Value;
                           int startIndex = strTest.LastIndexOf("(") + 1;
                         int length = strTest.Length - strTest.IndexOf(")");


                           id_name.stock_id=strTest.Substring(startIndex, strTest.Length - length - startIndex);
                           db.stock_id_name.InsertOnSubmit(id_name);

                           ///提交插入操作
                           db.SubmitChanges();
                       }

                   }

               }
               MessageBox.Show("ok!");

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }


        private string GetPageData(string url)
        {
            StringBuilder s = new StringBuilder(102400);
            WebClient wr = new WebClient();
            wr.Headers[HttpRequestHeader.AcceptEncoding] = "gzip, deflate";
            //   wr.Encoding = System.Text.Encoding.GetEncoding("utf-8");


            //使用代理服务器

            //   WebProxy proxyobject = new WebProxy("http://97.87.24.113:80", true);
            //    wr.Proxy=proxyobject;

            //模拟浏览器


            wr.Headers.Add("Accept", "image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, */*");
            wr.Headers.Add("Accept-Language", "zh-CN");
            wr.Headers.Add("User-Agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)");

            //    wr.Headers.Add("User-Agent", "EtaoSpider");
            //  byte[] buffer = wr.DownloadData(url);
            byte[] buffer;
        // 检查下载异常
        t1: try
            {

                buffer = wr.DownloadData(url);

            }

            catch (Exception ex)
            {


                // MessageBox.Show(ex.Message);

                //  Thread.Sleep(5000);

                //     Delay_Second(1);

                goto t1;



            }



            GZipStream g = new GZipStream((Stream)(new MemoryStream(buffer)), CompressionMode.Decompress);
            byte[] d = new byte[20480];
            int l = g.Read(d, 0, 20480);
            while (l > 0)
            {
                s.Append(Encoding.Default.GetString(d, 0, l));
                l = g.Read(d, 0, 20480);
            }
            //  Console.Write(s.ToString() + "\n\n\n" + s.Length);
            Random d2 = new Random(DateTime.Now.Second);
            int suiji;
            suiji = d2.Next(5, 10);

            //Thread.Sleep(suiji);
            //    Delay_Second(suiji);

            return (s.ToString());
        }

        private void Delay_Second(int second)
        {
            DateTime now = DateTime.Now;
            while (now.AddSeconds(second) > DateTime.Now)
            {
                //程序等待中，等待指定的时间...
                //    Application.DoEvents();     //释放CPU占用，以便CPU可以执行其它操作，不导致程序死掉-----根据实际使用。





            }
            //执行后面的程序


        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label2.Text = "共获取" + count + "条股票交易信息！";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Thread oThread3 = new Thread(new ThreadStart(create_txt));
            oThread3.Start(); // 程序运行的是Alpha.Beta()方法 
            this.timer1.Interval = 1;//设置每隔千分之一秒钟执行一次
            this.timer1.Enabled = true;
            this.timer1.Start();

        }

        private void create_txt()

    {
            ///申明字符编码,命名空间System.Text
            Encoding encoding = Encoding.GetEncoding("UTF-8");
            string directory = "bbsmax";
            string fileName = "stock.txt";
            string templateUrl = "Templates/t.htm";
            StreamReader sr = null;
            ///StreamWriter实现一个 TextWriter，使其以一种特定的编码向流中写入字符，命名空间Syste.IO。
            StreamWriter sw = null;
            string strCon = "Data Source=(local);database=lucene;uid=sa;pwd=qq901;";
            DataClasses1DataContext db;
            db = new DataClasses1DataContext(strCon);

            //清理未完成的站点链接，重新计算
            IEnumerable<stock_trade_history> matches = from stock in db.stock_trade_history
                                                           where stock.id > 0
                                                           select stock;
            if (matches.Count() > 0)
            {
                foreach(stock_trade_history stock in matches)
                {

                    // 创建文本文件

                    try
                    {
                      //  string fileName2 = "fenlu" + j + ".aspx";

                        ///保存新创建的HTML静态文件
                   //     sw = new StreamWriter( fileName, false, encoding);
                       // if (sw == null) return false;
                //        sw.Write(stock.kaipanjia);
                        ///清理缓存区
                   //     sw.Flush();

                        string txt = stock.stock_name.Trim()+"  "+stock.kaipanjia+"  "+stock.zuigaojia+"  "+stock.soupanjia+"  "+stock.zuidijia+"  "+stock.jiaoyiliang+"  "+stock.jiaoyijine+"  "+stock.date.Trim();

                        if(!File.Exists("stock.txt"))
                        {
                            sw = File.CreateText("stock.txt");
                       //不换行
                            //   sw.Write("");
                            sw.WriteLine(txt);
                            count++;


                        }
                        else
                        {
                            //如果文件存在就 添加一些内容

                            sw = File.AppendText("stock.txt");
                            sw.WriteLine(txt);
                            count++;



                        }
                    }
                    catch (Exception ex)
                    {
                       // HttpContext.Current.Response.Write(ex.Message);
                       // HttpContext.Current.Response.End();
                    }
                    finally
                    {
                        sw.Close();
                        sw.Dispose();
                    }

                }

            }
            MessageBox.Show("数据集生成完成！");


        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {

        }


    }
}
