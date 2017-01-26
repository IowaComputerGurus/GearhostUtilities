using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace IowaComputerGurus.Utility.GearHost.Services
{
    public interface IHttpClientService
    {
        TRes MakeHttpGetRequest<TRes>(string requestUri);

        TRes MakeHttpGetRequest<TRes>(string requestUri, string acceptString,
            List<KeyValuePair<string, string>> requestHeaders);

        TRes MakeHttpPostRequest<TReq, TRes>(string requestUri, TReq postData);

        TRes MakeHttpPostRequest<TReq, TRes>(string requestUri, string acceptString,
            List<KeyValuePair<string, string>> requestHeaders, TReq postData);

        List<KeyValuePair<string, string>> MakeHttpPostRequestReturnHeaders<TReq>(string requestUri, string acceptString,
            List<KeyValuePair<string, string>> requestHeaders, TReq postData, int successStatusCode);

        TRes MakeHttpPostRequestUrlEncoded<TRes>(string requestUri, List<KeyValuePair<string, string>> data);

        TRes MakeHttpPostRequestUrlEncoded<TRes>(string requestUri, string acceptString,
            List<KeyValuePair<string, string>> requestHeaders, List<KeyValuePair<string, string>> data);
    }

    public class HttpClientService : IHttpClientService
    {
        public TRes MakeHttpGetRequest<TRes>(string requestUri)
        {
            return MakeHttpGetRequest<TRes>(requestUri, string.Empty, new List<KeyValuePair<string, string>>());
        }

        public TRes MakeHttpGetRequest<TRes>(string requestUri, string acceptString,
            List<KeyValuePair<string, string>> requestHeaders)
        {
            //Set the request URI
            var request = (HttpWebRequest) WebRequest.Create(requestUri);
            request.Method = "GET";
            request.ProtocolVersion = HttpVersion.Version10;

            //Add headers to the request as needed
            foreach (var header in requestHeaders)
            {
                request.Headers.Add(header.Key, header.Value);
            }

            //Setup accept value if needed
            if (!string.IsNullOrEmpty(acceptString))
                request.Accept = acceptString;

            //Prepare our response object
            TRes result;

            //Setup timer
            var timer = new Stopwatch();
            timer.Start();

            //Try to make the request, use a generic handler to capture and report errors
            try
            {
                using (var response = (HttpWebResponse) request.GetResponse())
                {
                    using (var responseSR = new StreamReader(response.GetResponseStream()))
                    {
                        var resultDetail = responseSR.ReadToEnd();
                        result = JsonConvert.DeserializeObject<TRes>(resultDetail);
                    }
                    response.Close();
                }
            }
            catch (WebException ex)
            {
                //Process as a known error, IF this fails, let that exception rise
                var responseText = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                throw new ApplicationException(responseText);
            }
            finally
            {
                timer.Stop();
            }

            return result;
        }

        public TRes MakeHttpPostRequest<TReq, TRes>(string requestUri, TReq postData)
        {
            return MakeHttpPostRequest<TReq, TRes>(requestUri, string.Empty, new List<KeyValuePair<string, string>>(),
                postData);
        }

        public TRes MakeHttpPostRequest<TReq, TRes>(string requestUri, string acceptString,
            List<KeyValuePair<string, string>> requestHeaders, TReq postData)
        {
            //Set the request URI & Set the log entry details
            var request = (HttpWebRequest) WebRequest.Create(requestUri);
            var postString = JsonConvert.SerializeObject(postData);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.ProtocolVersion = HttpVersion.Version10;

            //Add headers to the request as needed
            foreach (var header in requestHeaders)
            {
                request.Headers.Add(header.Key, header.Value);
            }

            //Setup accept value if needed
            if (!string.IsNullOrEmpty(acceptString))
                request.Accept = acceptString;

            //Write our post data
            var dataToSendBytes = Encoding.UTF8.GetBytes(postString);
            request.ContentLength = dataToSendBytes.LongLength;
            using (var postStream = request.GetRequestStream())
            {
                postStream.Write(dataToSendBytes, 0, dataToSendBytes.Length);
            }

            //Prepare our response object
            TRes result;

            //Setup timer
            var timer = new Stopwatch();
            timer.Start();

            //Try to make the request, use a generic handler to capture and report errors
            try
            {
                using (var response = (HttpWebResponse) request.GetResponse())
                {
                    using (var responseSR = new StreamReader(response.GetResponseStream()))
                    {
                        var resultDetail = responseSR.ReadToEnd();
                        result = JsonConvert.DeserializeObject<TRes>(resultDetail);
                    }
                    response.Close();
                }
            }
            catch (WebException ex)
            {
                //Process as a known error, IF this fails, let that exception rise
                var responseText = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                throw new ApplicationException(responseText);
            }
            finally
            {
                timer.Stop();
            }

            return result;
        }

        public List<KeyValuePair<string, string>> MakeHttpPostRequestReturnHeaders<TReq>(string requestUri,
            string acceptString, List<KeyValuePair<string, string>> requestHeaders,
            TReq postData, int successStatusCode)
        {
            //Set the request URI & Set the log entry details
            var request = (HttpWebRequest) WebRequest.Create(requestUri);
            var postString = JsonConvert.SerializeObject(postData);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.ProtocolVersion = HttpVersion.Version10;

            //Add headers to the request as needed
            foreach (var header in requestHeaders)
            {
                request.Headers.Add(header.Key, header.Value);
            }

            //Setup accept value if needed
            if (!string.IsNullOrEmpty(acceptString))
                request.Accept = acceptString;

            //Write our post data
            var dataToSendBytes = Encoding.UTF8.GetBytes(postString);
            request.ContentLength = dataToSendBytes.LongLength;
            using (var postStream = request.GetRequestStream())
            {
                postStream.Write(dataToSendBytes, 0, dataToSendBytes.Length);
            }

            //Setup timer
            var timer = new Stopwatch();
            timer.Start();
            var returnCollection = new List<KeyValuePair<string, string>>();

            //Try to make the request, use a generic handler to capture and report errors
            try
            {
                using (var response = (HttpWebResponse) request.GetResponse())
                {
                    //If it isn't our code throw an error
                    if ((int) response.StatusCode != successStatusCode)
                        throw new WebException(new StreamReader(response.GetResponseStream()).ReadToEnd());

                    //Else enumerate headers
                    returnCollection.AddRange(
                        response.Headers.AllKeys.Select(
                            key => new KeyValuePair<string, string>(key, response.Headers[key])));

                    response.Close();
                }
            }
            catch (WebException ex)
            {
                if ((int) ex.Status == successStatusCode)
                {
                    returnCollection.AddRange(
                        ex.Response.Headers.AllKeys.Select(
                            key => new KeyValuePair<string, string>(key, ex.Response.Headers[key])));
                }
                else
                {
                    //Process as a known error, IF this fails, let that exception rise
                    var responseText = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                    throw new ApplicationException(responseText);
                }
            }
            finally
            {
                timer.Stop();
            }

            return returnCollection;
        }

        public TRes MakeHttpPostRequestUrlEncoded<TRes>(string requestUri, List<KeyValuePair<string, string>> data)
        {
            return MakeHttpPostRequestUrlEncoded<TRes>(requestUri, string.Empty,
                new List<KeyValuePair<string, string>>(), data);
        }

        public TRes MakeHttpPostRequestUrlEncoded<TRes>(string requestUri, string acceptString,
            List<KeyValuePair<string, string>> requestHeaders,
            List<KeyValuePair<string, string>> data)
        {
            //Set the request URI & Set the log entry details
            var request = (HttpWebRequest) WebRequest.Create(requestUri);
            var postFormat = FormatUrlEncoded(data);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ProtocolVersion = HttpVersion.Version10;

            //Add headers to the request as needed
            foreach (var header in requestHeaders)
            {
                request.Headers.Add(header.Key, header.Value);
            }

            //Setup accept value if needed
            if (!string.IsNullOrEmpty(acceptString))
                request.Accept = acceptString;

            //Write our post data
            var dataToSendBytes = Encoding.UTF8.GetBytes(postFormat);
            request.ContentLength = dataToSendBytes.LongLength;
            using (var postStream = request.GetRequestStream())
            {
                postStream.Write(dataToSendBytes, 0, dataToSendBytes.Length);
            }

            //Prepare our response object
            TRes result;

            //Setup timer
            var timer = new Stopwatch();
            timer.Start();

            //Try to make the request, use a generic handler to capture and report errors
            try
            {
                using (var response = (HttpWebResponse) request.GetResponse())
                {
                    using (var responseSR = new StreamReader(response.GetResponseStream()))
                    {
                        var resultDetail = responseSR.ReadToEnd();
                        result = JsonConvert.DeserializeObject<TRes>(resultDetail);
                    }
                    response.Close();
                }
            }
            catch (WebException ex)
            {
                //Process as a known error, IF this fails, let that exception rise
                var responseText = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                throw new ApplicationException(responseText);
            }
            finally
            {
                timer.Stop();
            }

            return result;
        }

        private string FormatUrlEncoded(List<KeyValuePair<string, string>> data)
        {
            var strings = new List<string>();
            foreach (var item in data)
            {
                strings.Add($"{item.Key}={item.Value}");
            }
            return string.Join("&", strings);
        }
    }
}