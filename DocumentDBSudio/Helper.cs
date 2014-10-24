//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Microsoft.Azure.DocumentDBStudio.Properties;

namespace Microsoft.Azure.DocumentDBStudio
{
    static class Constants
    {
        public readonly static string ProductVersion = "0.21";
        public readonly static string ApplicationName = "Azure DocumentDB Studio";

        /// <summary>
        ///  We can enable when there is emulator.
        /// </summary>
        public readonly static string LocalEmulatorEndpoint = "NotReleasedYet";
        public readonly static string LocalEmulatorMasterkey = "NotReleasedYet";
    }

    /// <summary>
    /// </summary>
    class Helper
    {
        static internal string FormatTextAsHtml(string text, bool encodeWhitespace)
        {
            return FormatTextAsHtml(text, encodeWhitespace, true);
        }

        static internal string FormatTextAsHtml(string text, bool encodeWhitespace, bool includeBodyTags)
        {
            // There must be a BCL function that will do this correctly...
            string html;
            html = text.Replace("&", "&amp;");
            html = html.Replace("<", "&lt;");
            html = html.Replace(">", "&gt;");
            html = html.Replace("\"", "&quot;");
            html = html.Replace("\'", "&apos;");
            if (encodeWhitespace)
            {
                html = html.Replace("\r\n", "<br>");
                html = html.Replace("\n", "<br>");
                html = html.Replace(" ", "&nbsp;");
            }
            else
            {
                html = "<pre id=preContent>" + html + "</pre>";
            }

            if (includeBodyTags)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<html><head><title>TextPrintJSON</title><style type=\"text/css\">");
                sb.Append("body, pre, table { font-size: 9pt; }");
                sb.Append("body, pre, table { font-family : Consolas, Courier New, monospace }");
                sb.Append("td { vertical-align : top } ");
                sb.Append("</style></head><body>");
                sb.Append(html);
                sb.Append("</body></html>");
                html = sb.ToString();
            }

            return html;
        }
    }

}
