using System;
using System.Net;

namespace ClientLibrary.Utils
{
    public static class ServerState
    {
        public static bool isAlive(string host, ushort port)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Uri.UriSchemeHttp + Uri.SchemeDelimiter + host + ":" + port);
            HttpWebResponse response;

            bool result = false;

            try
            {
                HttpStatusCode code = (response = (HttpWebResponse)request.GetResponse()).StatusCode;
                response.Close();

                result = true;
            }
            catch
            {
                result = false;
            }

            return result;
        }
    }
}
