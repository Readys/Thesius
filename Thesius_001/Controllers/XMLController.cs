using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Xml.Serialization;
using Thesius_001.Models;
using System.IO;
using System.Globalization;
using System.Xml.XPath;

namespace Thesius_001.Controllers
{
    public class XMLController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: XML
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult LoadXML()
        {
            var r = db.XMLtable.ToList();
            db.XMLtable.RemoveRange(r);
            db.SaveChanges();

            string XMLPath = System.Web.HttpContext.Current.Server.MapPath("~/Test/001.xml");

            //var ItemObjects = new List<XMLtable>();

            XDocument doc = XDocument.Load(XMLPath);
            var elements = doc.XPathSelectElements("/body").Elements("item").ToList();
            elements.ForEach(item =>
            {
                db.XMLtable.Add(new XMLtable
                {
                    FullName = item.Descendants("nsiPurchaseMethodData").Descendants("creator").First().Value,
                    Inn = Convert.ToInt64(item.Descendants("nsiPurchaseMethodData").Descendants("creator").Descendants("inn").First().Value),
                    Kpp = Convert.ToInt64(item.Descendants("nsiPurchaseMethodData").Descendants("creator").Descendants("kpp").First().Value),
                    Name = item.Descendants("nsiPurchaseMethodData").DescendantsAndSelf("name").First().Value,
                    Code = Convert.ToInt64(item.Descendants("nsiPurchaseMethodData").DescendantsAndSelf("code").First().Value),
                    Competitive = Convert.ToBoolean(item.Descendants("nsiPurchaseMethodData").DescendantsAndSelf("competitive").First().Value),
                    IsElectronic = Convert.ToBoolean(item.Descendants("nsiPurchaseMethodData").DescendantsAndSelf("isElectronic").First().Value),
                });
                db.SaveChanges();
            });

            var XMLList = db.XMLtable.ToList();

            return Json(new { XMLList }, JsonRequestBehavior.AllowGet);

            // передаем в конструктор тип класса
            //XmlSerializer formatter = new XmlSerializer(typeof(Trias), "http://zakupki.gov.ru/223fz/reference/1");

            //XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            //ns.Add("ns2", "http://zakupki.gov.ru/223fz/reference/1");

            //// получаем поток, куда будем записывать сериализованный объект
            ////using (FileStream fs = new FileStream("persons.xml", FileMode.OpenOrCreate))
            ////{
            ////    formatter.Serialize(fs, person);

            ////    Console.WriteLine("Объект сериализован");
            ////}

            //// десериализация
            //using (FileStream fs = new FileStream(XMLPath, FileMode.OpenOrCreate))
            //{
            //    Trias newPerson = (Trias)formatter.Deserialize(fs);
            //}


            //XDocument doc = XDocument.Load(XMLPath);
            ////var body = doc.Descendants();
            //XNamespace ns2 = "http://zakupki.gov.ru/223fz/reference/1";

            //List<XmlTest> XmlTest = new List<XmlTest>();

            //var body = doc.Descendants(ns2 + "body")
            //  .Descendants(ns2 + "item")
            //  .Descendants(ns2 + "nsiPurchaseMethodData")
            //  .Select(node => new
            //    {
            //        Code = node.Descendants(ns2 + "code"),
            //        Creator = node.Descendants(ns2 + "creator"),
            //        Competitive = node.Descendants(ns2 + "competitive"),
            //        IsElectronic = node.Descendants(ns2 + "isElectronic"),                 
            //  }).ToArray();

            //foreach (var item in body)
            //{
            //    var a = item;
            //    //var b = item.Creator.First().kpp;
            //}

            //string XMLPath_ =  XMLPath.Replace("ns2:", "");

            //XmlSerializer ser = new XmlSerializer(typeof(body));
            //body b;
            //using (XmlReader reader = XmlReader.Create(XMLPath_))
            //{              
            //    b = (body)ser.Deserialize(reader);
            //}


            //var xDoc = XDocument.Load(XMLPath); //or XDocument.Load(filename);XDocument.Parse(XMLPath)
            //XNamespace ns = "ns2";
            //var items = xDoc.Descendants(ns + "body")
            //                .Select(x => new
            //                {
            //                    item = x.Element(ns + "item").Descendants()
            //                                 .Select(dim => new {
            //                                     guid = dim.Attribute("guid").Value,
            //                                 })
            //                                .ToList()}).ToList();

            ////DataSet dataSet = new DataSet();
            //string XMLPath = System.Web.HttpContext.Current.Server.MapPath("~/Test/147.xml");

            //XmlDocument docXML = new XmlDocument(); // XML-документ
            //docXML.Load(XMLPath); // загрузить XML

            //XmlNamespaceManager _namespaceManager = new XmlNamespaceManager(docXML.NameTable); // инициализация пространства имён
            //_namespaceManager.AddNamespace("ns2", "body"); // указываем значение пространства имён "ns2" (данные взяли из XML)

            //string _region = docXML.DocumentElement.SelectNodes("//ns2:body/ns2:item/ns2:nsiPurchaseMethodData/ns2:createDateTime", _namespaceManager)[0].InnerText;

            //XDocument document = XDocument.Parse(xml);
            //string overdueStatus = document.Descendants("mNBFDDSLOANLOOKUPDetailType")
            //    .Where(n => (string)n.Element("DUEMONTH") == "202103")
            //    .Select(n => (string)n.Element("OVERDUESTATUS"))
            //    .FirstOrDefault();

            //string XSDPath = System.Web.HttpContext.Current.Server.MapPath("~/Test/XSD/contract.xsd");
            //dataSet.ReadXmlSchema(XSDPath);
            //dataSet.ReadXml(XMLPath, XmlReadMode.IgnoreSchema);





        }
    }
}