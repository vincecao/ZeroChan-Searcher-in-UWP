using System;
using System.Collections;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ZC
{
    static class Zerochan_tools
    {
        public static async Task<StringBuilder> getLinkContant(String url)
        {
            var httpClient = new HttpClient();
            StringBuilder strHTML = new StringBuilder(await httpClient.GetStringAsync(new Uri(url)));
            return strHTML;
        }

        public static async Task<String> getLinkPath(String url)
        {
            System.Net.WebRequest req = System.Net.WebRequest.Create(url);

            System.Net.WebResponse rep = await req.GetResponseAsync();
            return rep.ResponseUri.ToString();
        }

        public static int getTimesNumber(StringBuilder a, String key, ArrayList List)
        {
            int per = -2, count = 0; //let per not be 0 and -1 at the beginning
            for (int i = 0; i < a.Length; i++)
            {
                if (IndexOf(a, key, i, false) != -1)
                {
                    if (IndexOf(a, key, i, false) != per)
                    {
                        per = IndexOf(a, key, i, false);
                        List.Add(per); //get location
                        count++;
                    }
                }
            }
            return count;
        }

        public static int IndexOf(this StringBuilder sb, string value, int startIndex, bool ignoreCase)
        {
            int index;
            int length = value.Length;
            int maxSearchLength = (sb.Length - length) + 1;

            if (ignoreCase)
            {
                for (int i = startIndex; i < maxSearchLength; ++i)
                {
                    if (Char.ToLower(sb[i]) == Char.ToLower(value[0]))
                    {
                        index = 1;
                        while ((index < length) && (Char.ToLower(sb[i + index]) == Char.ToLower(value[index])))
                            ++index;

                        if (index == length)
                            return i;
                    }
                }

                return -1;
            }

            for (int i = startIndex; i < maxSearchLength; ++i)
            {
                if (sb[i] == value[0])
                {
                    index = 1;
                    while ((index < length) && (sb[i + index] == value[index]))
                        ++index;

                    if (index == length)
                        return i;
                }
            }

            return -1;
        }

        public static StringBuilder trimStr(StringBuilder input, String startStr, String endStr)
        {
            var index = IndexOf(input, startStr, 0, false) + startStr.Length;
            var end = IndexOf(input, endStr, 0, false);
            var length = end - index + 1;

            return SubString(input, index, length);
        }

        public static StringBuilder SubString(StringBuilder input, int index, int length)
        {
            StringBuilder subString = new StringBuilder();
            if (index + length - 1 >= input.Length || index < 0)
            {
                throw new ArgumentOutOfRangeException("Index out of range!");
            }
            int endIndex = index + length;
            for (int i = index; i < endIndex; i++)
            {
                subString.Append(input[i]);
            }
            return subString;
        }

        //get html from the search word and suffix
        public static async Task<String> getCorrectUrl(String Eng_title)
        {
            return await getLinkPath("http://www.zerochan.net/search?q=" + Eng_title);
        }

        public static async Task<StringBuilder> setKeywords(String CorrectUrl, String Suffix)
        {
            String fullUrl = CorrectUrl + Suffix;
            System.Diagnostics.Debug.WriteLine(fullUrl);
            StringBuilder input = await getLinkContant(fullUrl);
            return input;
        }

        public static ArrayList getZcPicArr(StringBuilder html)
        {
            ArrayList srcPos = new ArrayList();
            ArrayList PicURLs = new ArrayList();
            ArrayList ZcPicArr = new ArrayList();


            getTimesNumber(html, "http://s3.zerochan.net/", srcPos);

            for (int i = 0; i < srcPos.Count; i++)
            {
                var index = Convert.ToInt32(srcPos[i]) + 23;
                var length = 78;
                var SecondHtml = SubString(html, index, length);
                index = 0;
                length = IndexOf(SecondHtml, "\"", 0, false);
                PicURLs.Add(SubString(SecondHtml, index, length));
            }

            for (int i = 0; i < PicURLs.Count; i++)
            {
                var a = "http://s3.zerochan.net/" + PicURLs[i].ToString();
                ZcPicArr.Add(new Zerochan_Picture(a.ToString()));
            }

            return ZcPicArr;
        }

        public static ArrayList getFullPicURLs(ArrayList PicURLs)
        {
            ArrayList arr = PicURLs;
            for (int i = 0; i < PicURLs.Count; i++)
            {
                arr[i] = PicURLs[i].ToString().Replace("240", "full").Replace("s3", "static");
            }
            return arr;
        }

        public static async Task<ArrayList> GetPages(int index, int number, string searchContant, string quality, string sort)
        {
            StringBuilder html;
            ArrayList URLlist = new ArrayList();

            if (searchContant == null)
            {
                for (int i = 1; i < number + 1; i++)
                {
                    int n = index + i;
                    //html = await setKeywords("http://www.zerochan.net/" + searchContant.Replace(" ", "+"), quality + sort + "&p=" + index + i);
                    html = await setKeywords("http://www.zerochan.net/", "?p=" + n);
                    ArrayList newlist = getZcPicArr(html);

                    for (int j = 0; j < newlist.Count; j++)
                    {
                        URLlist.Add(newlist[j]);
                    }
                }
            }
            else
            {
                for (int i = 1; i < number + 1; i++)
                {
                    int n = index + i;
                    html = await setKeywords("http://www.zerochan.net/" + searchContant.Replace(" ", "+"), quality + sort + "&p=" + n);
                    //html = await setKeywords("http://www.zerochan.net/", "?p=" + index + i);
                    ArrayList newlist = getZcPicArr(html);

                    for (int j = 0; j < newlist.Count; j++)
                    {
                        URLlist.Add(newlist[j]);
                    }
                }
            }

            
            
            return URLlist;
        }

        //input Single Picture's url get 
        public async static Task<ArrayList> getPicTags(String url)
        {
            ArrayList PicTags = new ArrayList();
            ArrayList PicTagsStart = new ArrayList();
            ArrayList PicTagsEnd = new ArrayList();

            StringBuilder html = await getLinkContant(url);

            html = trimStr(html, "<ul id=\"tags\">", "<h2>Share</h2>");

            getTimesNumber(html, "<li><a href=\"/", PicTagsStart);
            getTimesNumber(html, "</a> ", PicTagsEnd);

            for (int i = 0; i < PicTagsStart.Count; i++)
            {
                var index = Convert.ToInt32(PicTagsStart[i]);
                var length = Convert.ToInt32(PicTagsEnd[i]) + 5 - index;

                PicTags.Add(striphtml(SubString(html, index, length).ToString()));
                System.Diagnostics.Debug.WriteLine(striphtml(SubString(html, index, length).ToString()));
            }

            for (int i = 0; i < PicTags.Count; i++)
            {
                PicTags[i] = PicTags[i].ToString();
            }
            return PicTags;
        }

        //return tags' url and which is the key for textBox
        public static String returnTagUrl(String Tags)
        {
            return "http://www.zerochan.net/" + Tags.Replace(" ", "+").Replace("(", "%28").Replace(")", "%29");
        }

        public static string striphtml(string strhtml)
        {
            string stroutput = strhtml;
            Regex regex = new Regex(@"<[^>]+>|</[^>]+>");
            stroutput = regex.Replace(stroutput, "");
            return stroutput;
        }
    }
}
