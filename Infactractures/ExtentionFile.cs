﻿using System;
using System.IO;
using System.Web.Configuration;

namespace ssc.consulting.switchboard.Infactractures
{
    public class ExtentionFile
    {
        
        public static bool CheckExtention(string file)
        {
            string listextension = WebConfigurationManager.AppSettings["ExtentionFile"].ToString();
            string extensionfile = Path.GetExtension(file);
            if (extensionfile != null)
            {
                if (listextension.Contains(extensionfile))
                    return true;      
            }
            return false;
        }

        public static string GetExtentionString(string file)
        {
            string result = String.Empty;
            string listextension = WebConfigurationManager.AppSettings["ExtentionFile"].ToString();
            string extensionfile = Path.GetExtension(file);
            if (extensionfile != null)
            {
                if (listextension.Contains(extensionfile))
                    result = extensionfile.Replace(".", "");    
            }
            return result;
        }

        public static bool CheckExtentionFileImage(string file)
        {
            string listextension = WebConfigurationManager.AppSettings["typeimage"].ToString();
            string extensionfile = Path.GetExtension(file);
            if (extensionfile != null)
            {
                if (listextension.Contains(extensionfile))
                    return true;
            }
            return false;
        }

        public static string GetExtentionFileImageString(string file)
        {
            string result = String.Empty;
            string listextension = WebConfigurationManager.AppSettings["typeimage"].ToString();
            string extensionfile = Path.GetExtension(file);
            if (extensionfile != null)
            {
                if (listextension.Contains(extensionfile))
                    result = extensionfile.Replace(".", "");    
            }
            return result;
        }
    }
}