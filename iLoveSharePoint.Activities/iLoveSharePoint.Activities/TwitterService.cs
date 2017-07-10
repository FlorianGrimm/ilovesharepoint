using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace iLoveSharePoint.Activities
{
    public class TwitterService
    {
        // Methods
        public static string MakeTinyUrl(string message)
        {
            try
            {
                if (message.Length <= 12)
                {
                    return message;
                }
                if (!(message.ToLower().StartsWith("http") || message.ToLower().StartsWith("ftp")))
                {
                    message = "http://" + message;
                }
                using (StreamReader reader = new StreamReader(WebRequest.Create("http://tinyurl.com/api-create.php?url=" + message).GetResponse().GetResponseStream()))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (Exception)
            {
                return message;
            }
        }

        public static void PostTweet(string username, string password, string tweet, bool shorten)
        {
            string str = string.Empty;
            try
            {
                if (shorten)
                {
                    str = ToTinyURLS(tweet);
                }
                else
                {
                    str = tweet;
                }

                if (str.Length > 140)
                {
                    throw new Exception("Tweet length exceeds 140 characters!");
                }
                byte[] bytes = Encoding.UTF8.GetBytes("status=" + str);
                HttpWebRequest request = (HttpWebRequest) WebRequest.Create("http://twitter.com/statuses/update.xml");
                request.Method = "POST";
                request.Credentials = new NetworkCredential(username, password);
                request.ServicePoint.Expect100Continue = false;
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = bytes.Length;
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();
                WebResponse response = request.GetResponse();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        private static string ToTinyURLS(string txt)
        {
            MatchCollection matchs = new Regex(@"http://([\w+?\.\w+])+([a-zA-Z0-9\~\!\@\#\$\%\^\&amp;\*\(\)_\-\=\+\\\/\?\.\:\;\'\,]*)?", RegexOptions.IgnoreCase).Matches(txt);
            foreach (Match match in matchs)
            {
                string newValue = MakeTinyUrl(match.Value);
                txt = txt.Replace(match.Value, newValue);
            }
            return txt;
        }
    }

}






