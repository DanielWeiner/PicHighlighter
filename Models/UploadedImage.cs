using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Hosting;

namespace PictureThing.Models
{
    public class UploadedImage
    {
        public HttpPostedFileBase PostedFile { get; set; }
        public Bitmap Image { get; set; }
        public int[][][] Colors { get; set; }
        public string[][] CssClassAssignments { get; set; }
        public Dictionary<string, string> CssColors { get; set; }
        public char[][] Chars { get; set; }
        public bool TranspText { get; set; }
        public string Browser { get; set; }

        public UploadedImage(HttpPostedFileBase postedFile, string text, int winWidth, double opacity, bool transpText, string browser)
        {
            Browser = browser;
            CssColors = new Dictionary<string,string>();
            if (text.Length == 0)
            {
                text = "Ingle ha flibble ho slop-woggle!! Zip roo zungle flang blobbity flubbity wheezer tangtongle? Zipping quibblegobble. Boo cringle crangle da wow oodle shnuzzle dee wheezer. Zunk loo flunging tangleblop. Flob ha zongle! Blung zip yada wacko? Twiddle hum crongle boo flung-shnuzzle!! \"Nip flung duh?\" blip zingflub. Nip shnazzy bloobity bleebing duh meep doo zippity zungleflap. Hum tingle zip dong shruberific wooglezong, yip dobba bling zap dangely hizzleshnuzzle shnaz. Yip da blop flop do-da jingely bling ingleshnozzle? Yip boo wobbledabba loo hum razz, boo bleebity hum woggle loo. Blongity dabbaingle. Flooble wacko ha bling loo gobble? Blung da zangity goblinrazz. Hum zap flungoodle nip duh zing, ho blappity ha flobble ha. Kanoodle bang da tingle dee bling? \"Loo shnuzzle doo?\" zingle Chaka Khan. Blapdoo-blippity-ingle! Ha boo shnuzzledingle bam roo weeble, hum flungity da shnuzzle hum. Zong nip goblin loo gobble-dilznoofus!! Doo zonkity dabbafunk! Dang yap crongle woggle? Zip blo Mr. Burns...funky flupping boofraggle. Bam yip hum Marge fliptangle. Zang flip? Wubble da yada boo whack shnazzle hum flub? \"Zap dilznoofus boo?\" bizzle Mr. Burns. Hizzle doo zingle! Dilznoofus duh wiggle wow? Whack boo izzle flooble? Dizzle ding shnizzledobba, \"zowee doo yip fling,\" zap Kenny zoom abracadabra ho tangle-woogle...flip yip boo! Da flibble donglejingle. Boo shrubbery shnozzle jangleshnozzle nip da zap \"sloppy flop-slop\". Chef ha Mr. Slave ha rakity flakity bizzle-zangle. Da hum Mr. Burns floobleslap! Yap nippy gobblezongle! Hum bam rizzletizzle dee zip flobble, doo sloppy yip dabba roo. Zap zap dazzle dong shruberific crangely tingle woggleblop? Nip boo wigglewobble loo roo flooble, zip flipping zip abracadabra zip. Cringle bleep? Woogle weeble yip ting roo bing? Dong hum bla! Leia bam blob roo dilznoofus blipping hizzleblo. Da blap Kyle...dongely bleeping razzblup. Oodle plop bing boo shnoz flob flong doo dilznoofus. Woogle blup? \"Roo tangle yip?\" crungle bleebzing. Roo bluppity zappity blingity dee dingle ha blippity wibbletongle. Cake ha ingle! Bloobing crungledizzle. \"Zip doof hum?\" blip Chef. Blo fraggle zonkwheezer, \"wiggle doo zip bloo,\" dee Bart whack noodle loo woggle-noodle...zoom boo nip! Oodle bam blap loo flee slap roo ding? Zangity zungledilznoofus. Blee ha wheezer-tingle. Flub blip wiggle yip zung shrubbery ling da flip. Shrubbery yap blopping dilznoofuswoogle. Zip blangity flong blab flop zap shnazzle. Blap yip wacko da jingle shnoz zap blang? Zip blongity tizzleflang! Nip bing crunglehizzy. ";
            }
            TranspText = transpText;
            var horizScale = 8;
            var vertScale = 14;
            this.PostedFile = postedFile;
            this.Image = new Bitmap(postedFile.InputStream);
            var ratio = (double)winWidth / Image.Width;
            this.Image = new Bitmap(Image, winWidth, (int)(ratio * Image.Height));
            this.Image = Image.Clone(new Rectangle(0,0,Image.Width - (Image.Width%horizScale), Image.Height-(Image.Height%vertScale)), Image.PixelFormat);
            var width = (int)Image.Width/horizScale;
            var height = (int)Image.Height/vertScale;

            BitmapData srcData = Image.LockBits(new Rectangle(0, 0, Image.Width, Image.Height), ImageLockMode.ReadOnly, Image.PixelFormat);
            var stride = srcData.Stride;

            Colors = new int[height][][];
            CssClassAssignments = new string[height][];

            unsafe
            {
                byte* p = (byte*)(void*) srcData.Scan0;
                for (int row = 0; row < height; row++)
                {
                    Colors[row] = new int[width][];
                    CssClassAssignments[row] = new string[width];
                    for (int col = 0; col < width; col++)
                    {
                        Colors[row][col] = new int[3];
                        for (int color = 0; color < 3; color++)
                        {
                            Colors[row][col][color] = 0;
                        }
                        for (int subRow = row * vertScale; subRow < (row + 1) * vertScale; subRow++)
                        {
                            for (int subCol = col * horizScale; subCol < (col + 1) * horizScale; subCol++)
                            {
                                for (int color = 0; color < 3; color++)
                                {
                                    int index = (subRow * stride) + (subCol*4) + color;
                                    Colors[row][col][color] += p[index];
                                }
                            }
                        }
                        for (int color = 0; color < 3; color++)
                        {
                            Colors[row][col][color] /= (horizScale*vertScale) ;
                        }
                        string rgba = "rgba(" + Colors[row][col][2] + "," + Colors[row][col][1] + "," + Colors[row][col][0] + "," + opacity + ")";
                        if (!CssColors.ContainsKey(rgba)) 
                        {
                            CssColors.Add(rgba, "hl-" + CssColors.Count);
                        }
                        CssClassAssignments[row][col] = CssColors[rgba];
                    }
                }
            }
            Image.UnlockBits(srcData);

            text = text.Replace("\r\n", "\u00A0").Replace("\n", "\u00A0").Replace("\r", "\u00A0").Replace("\t", "\u00A0\u00A0\u00A0\u00A0").Replace(" ", "\u00A0");
            if (text.Length >= height * width)
            {
                text = text.Substring(0, height * width);
            }
            else
            {
                StringBuilder sb = new StringBuilder(text);
                while(sb.Length < width*height) {
                    sb.Append(sb.ToString());
                }
                text = sb.ToString(0, width * height);
            }
            Chars = new char[height][];
            for (int i = 0; i < height; i++)
            {
                Chars[i] = text.ToCharArray(i * width, width);
            }

            using (FileStream fs = new FileStream(HostingEnvironment.ApplicationPhysicalPath + "/Content/pictureStyle.css", FileMode.Create)) 
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(@"* {
    margin: 0;
    padding:0;
    font-family:""Courier New"", Courier, monospace;
    font-size:14px;
    line-height:14px;
    white-space:nowrap;
}
");
                if (TranspText)
                {
                    sb.AppendLine("span::selection {color: transparent;}");
                }
                foreach (KeyValuePair<string, string> color in CssColors)
                {
                    sb.Append(".").Append(color.Value).Append("::").Append((Browser == "firefox" ? "-moz-" : "")).Append("selection { background: ").Append(color.Key).AppendLine("; }");
                }
                string fileString = sb.ToString();
                StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
                sw.Write(fileString);
                sw.Flush();
                sw.Close();
                fs.Close();
            }
        }
    }
}