using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Roadkill.Text
{
    public class ApplicationSettings
    {
        public string AttachmentsFolder { get; set; }
        public string AttachmentsUrlPath { get; set; }
        public string CustomTokensPath { get; set; }
        public string HtmlElementWhiteListPath { get; set; }
        public string PluginsPath { get; internal set; }
        public string PluginsBinPath { get; internal set; }
        public bool UseHtmlWhiteList { get; set; }

        public ApplicationSettings()
        {
            AttachmentsFolder = Path.Combine(Directory.GetCurrentDirectory(), "attachments");
            AttachmentsUrlPath = "/attachments/";
        }
    }
}