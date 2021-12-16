using AMS.Migration.MigrateDataService;
using Sitecore.Configuration;
using Sitecore.Data.Fields;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace AMS.Migration
{
    class Program
    {
        static void Main(string[] args)
        {
            DirectoryInfo di = new DirectoryInfo(@"f:\temp\PayPerClick\");

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }

            var pages = new MigrateDataSoapClient().GetAllPayPerClickCombinations();

            pages.WriteXml(@"f:\temp\PayPerClick\combs.xml");

            /*
            DirectoryInfo di = new DirectoryInfo(@"f:\temp\MissingPages\");

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }

            var pages = new ServiceClient().GetAllMissingPages();
            var importer = new SitecoreImporter();
            importer.ImportMissingPages(pages);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<MissingPage>));

            using (FileStream fs = new FileStream(@"f:\temp\MissingPages\PagesWithGuids.xml", FileMode.Create))
            {
                xmlSerializer.Serialize(fs, pages);
            }
             */
            //return;
            //new SitecoreImporter().DeleteImportedPages();
            //return;

            //var pages = new ServiceClient().ProcessPages();

            //var importer = new SitecoreImporter();
            //importer.ProcessPages(pages);

            //XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<Page>));

            //using (FileStream fs = new FileStream(@"f:\temp\pagesWithGuids.xml", FileMode.Create))
            //{
            //    xmlSerializer.Serialize(fs, pages);
            //}

            //List<Page> deserializedPages = null;
            //using (FileStream fs = new FileStream(@"f:\temp\pagesWithGuids.xml", FileMode.Open))
            //{
            //    deserializedPages = (List<Page>)xmlSerializer.Deserialize(fs);
            //}
            //new SitecoreImporter().TestItemNames(pages);
            //var item = Factory.GetDatabase("master").GetItem("{D3B9658B-66AA-4A05-9726-4E42AAB6AC24}");
            //MultilistField relatedLocationsField = item.Fields["Related Locations"];
            //var items = relatedLocationsField.GetItems();
            //Console.WriteLine("relatedLocationsitems = {0}", items.Length);

            //importer.SetRelatedLocations(deserializedPages);
            //importer.SetRelatedLocations(pages);


            //new ServiceClient().ChangeTemplates();

            Console.WriteLine("Done");
            //Console.ReadLine();
        }
    }
}
