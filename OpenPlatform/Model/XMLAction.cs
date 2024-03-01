using MVCPlayWithMe.General;
using System;
using System.IO;
using System.Xml.Linq;

namespace MVCPlayWithMe.OpenPlatform.Model
{
    public class XMLAction
    {
        public XDocument xDoc;
        public string pathXML;

        public XMLAction()
        {
            xDoc = null;
            pathXML = string.Empty;
        }

        public XMLAction( string path)
        {
            pathXML = path;
            InitializeXDoc();
        }

        public Boolean Save()
        {
            try
            {
                xDoc.Save(pathXML, SaveOptions.None);
                return true;
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.Message);
            }
            return false;
        }

        public void InitializeXDoc()
        {
            string fileName = Path.GetFileNameWithoutExtension(pathXML);
            try
            {
                xDoc = XDocument.Load(pathXML);
            }
            catch (Exception e)
            {
                xDoc = null;
                MyLogger.GetInstance().Fatal("Không đọc được " + pathXML + ". " + e.Message);
                throw new Exception("Không đọc được " + fileName + ". " + e.Message);
            }
        }
    }
}
