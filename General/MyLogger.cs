using Lazop.Api;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MVCPlayWithMe.General
{
    public class MyLogger
    {
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Get logger.
        /// </summary>
        /// <returns>The instance.</returns>
        public static log4net.ILog GetInstance()
        {
            return Logger;
        }

        /// <summary>
        /// Ghi log request HTTP
        /// </summary>
        /// <param name="client"></param>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="level"></param>
        public static void RestLog(RestClient client, RestRequest request, IRestResponse response, log4net.Core.Level level)
        {
            string mess = string.Empty;
            StringBuilder sb = new StringBuilder();
            if (client != null)
            {
                sb.Append(client.BuildUri(request).ToString());
                sb.Append('\n');
            }

            if (request != null)
            {
                foreach (Parameter e in request.Parameters)
                {
                    sb.Append(e.ToString());
                    sb.Append('\n');
                }
                sb.Append('\n');
            }

            if (response != null)
            {
                sb.Append(response.StatusCode.ToString());
                sb.Append('\n');
                sb.Append(response.Content);
                sb.Append('\n');
                sb.Append("ErrorException: " + response.ErrorException);
                sb.Append('\n');
                sb.Append("ErrorMessage: " + response.ErrorMessage);
                sb.Append('\n');
            }

            mess = sb.ToString();

            if (level == log4net.Core.Level.Info)
                Logger.Info(mess);
            else if (level == log4net.Core.Level.Debug)
                Logger.Debug(mess);
            else if (level == log4net.Core.Level.Warn)
                Logger.Warn(mess);
            else if (level == log4net.Core.Level.Error)
                Logger.Error(mess);
            else if (level == log4net.Core.Level.Fatal)
                Logger.Fatal(mess);
        }

        public static void InfoRestLog(RestClient client, RestRequest request, IRestResponse response)
        {
            RestLog(client, request, response, log4net.Core.Level.Info);
        }

        public static void DebugRestLog(RestClient client, RestRequest request, IRestResponse response)
        {
            RestLog(client, request, response, log4net.Core.Level.Debug);
        }

        public static void WarnRestLog(RestClient client, RestRequest request, IRestResponse response)
        {
            RestLog(client, request, response, log4net.Core.Level.Warn);
        }

        public static void ErrorRestLog(RestClient client, RestRequest request, IRestResponse response)
        {
            RestLog(client, request, response, log4net.Core.Level.Error);
        }

        public static void FatalRestLog(RestClient client, RestRequest request, IRestResponse response)
        {
            RestLog(client, request, response, log4net.Core.Level.Fatal);
        }

        public static void LazadaRestLog(LazopRequest request, LazopResponse response)
        {
            Logger.Info(request.reqUrl);
            Logger.Info(response.IsError());
            Logger.Info(response.Body);
        }
    }
}