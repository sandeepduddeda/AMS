using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace AMS.Web.Core
{
    public class SitecoreErrorData
    {
        private const string DefaultErrorMessage = "An error occurred while rendering this view";

        private const string SitecoreErrorPrefix = "AMSERR";

        private UInt32 NumericId { get; set; }
        public string ID { get; private set; }

        public Exception SourceException { get; private set; }

        public string Message { get; private set; }

        private SitecoreErrorData()
        {
            NumericId = GenerateUniqueNumber();
            ID = SitecoreErrorPrefix + NumericId.ToString("X");
            Message = string.Empty;
        }

        public SitecoreErrorData(Exception sourceException)
            : this()
        {
            if (sourceException == null) throw new ArgumentNullException("sourceException");

            SourceException = sourceException;

            GenerateErrorMessage();
        }

        public HtmlString HtmlString()
        {
            var div = new TagBuilder("div");
            div.Attributes.Add("class", "alert alert-danger");
            div.Attributes.Add("role", "alert");

            var pMessage = new TagBuilder("p");
            pMessage.SetInnerText(Message);

            var pInfo = new TagBuilder("p");
            pInfo.SetInnerText(string.Format("Please search <strong>{0}</strong> in Logs for details", ID));

            div.InnerHtml += pMessage;
            div.InnerHtml += pInfo;
            return new HtmlString(div.ToString());
        }

        private void GenerateErrorMessage()
        {
            var exlist = new List<Exception>();
            var errMsgChain = new StringBuilder();

            string message = DefaultErrorMessage; // default

            if (SourceException != null)
            {
                // Unwind the nested exceptions using the InnerException property.
                Exception curex = SourceException;

                while (curex != null)
                {
                    exlist.Add(curex);
                    curex = curex.InnerException;
                }

                // Build full error message by reverting the error list.
                for (int i = exlist.Count - 1; i >= 0; i--)
                {
                    curex = exlist[i];

                    errMsgChain.AppendFormat("{0}: {1}", curex.GetType().Name, curex.Message);

                    if (i > 0) errMsgChain.Append(" ---> ");
                }

                message = errMsgChain.ToString(); // override
            }

            Message = string.Format("{0}: {1}", ID, message);
        }

        private static UInt32 GenerateUniqueNumber()
        {
            byte[] buffer = Guid.NewGuid().ToByteArray();

            UInt32 uNumber = 0;
            for (int i = 0; i < 4; i++)
            {
                uNumber ^= BitConverter.ToUInt32(buffer, i * 4);
            }

            return uNumber;
        }
    }
}