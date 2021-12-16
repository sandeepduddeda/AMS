using AMS.Migration.MigrateDataService;
using Sitecore.Configuration;
using Sitecore.SecurityModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace AMS.Migration
{
    public class ServiceClient
    {
        public List<MissingPage> GetAllMissingPages()
        {
            var client = new MigrateDataSoapClient();

            var missingPages = client.GetAllMissingPages(); // missingPages - 488

            missingPages.WriteXml(@"f:\temp\MissingPages\MissingPages.xml");

            Console.WriteLine("missingPages = {0}", missingPages.Rows.Count);
          
            var resultList = new List<MissingPage>();
            var missingPagesList = missingPages.AsEnumerable().ToList();

            var regExp = new Regex(@"^[a-zA-Z0-9\*\$][a-zA-Z0-9\-._\$]*(\(\d{1,}\)){0,1}$");
            var filteredMissingPagesList = missingPagesList.Where(page => regExp.IsMatch(page["FileName"].ToString())).ToList();
            Console.WriteLine("filteredMissingPagesList = {0}", filteredMissingPagesList.Count);

            foreach (var missingPage in missingPages.AsEnumerable().ToList())
            {
                var fileName = missingPage["FileName"].ToString().Replace(".aspx", "");

                if (!string.IsNullOrEmpty(fileName))
                {
                    var page = new MissingPage();

                    page.OriginalFileName = fileName;

                    var formattedFileName = FormatName(page.OriginalFileName);
                    //var changedFileName = formattedFileName
                    //    .Replace("!","")
                    //    .Replace("'","")
                    //    .Replace(",","")
                    //    .TrimStart(' ');

                    //if(changedFileName != formattedFileName)
                    //{
                    //    using (StreamWriter file = new StreamWriter(@"f:\temp\changedNames.txt", true))
                    //    {
                    //        file.WriteLine(page.OriginalFileName);
                    //        file.WriteLine(formattedFileName);
                    //        file.WriteLine(changedFileName);
                    //        file.WriteLine("------------------");
                    //    } 
                    //}

                    page.FileName = formattedFileName;

                    page.MetaTitle = missingPage["MetaTitle"] as string;
                    page.MetaKeyword = missingPage["MetaKeyword"] as string;
                    page.MetaDescription = missingPage["MetaDescription"] as string;
                    page.Body = missingPage["body"] as string;

                    resultList.Add(page);
                }
            }
            
            var names = filteredMissingPagesList.Select(i => i["FileName"]);
            var diff = missingPagesList.Where(i => !names.Contains(i["FileName"])).Select(i => i["FileName"] as string).ToList();
            Console.WriteLine("diff = {0}", diff.Count());

            File.WriteAllLines(@"f:\temp\MissingPages\diff.txt", diff);

            return resultList;
        }


        public List<Page> ProcessPages()
        {
            var client = new MigrateDataSoapClient();

            var branches = client.GetAllBranchesDetails(); // branches - 69
            var locations = client.GetAllLocationsPages(); // locations - 3874

            branches.WriteXml(@"f:\temp\AllBranches.xml");
            locations.WriteXml(@"f:\temp\AllLocations.xml");
            Console.WriteLine("Branches = {0}", branches.Rows.Count);
            Console.WriteLine("Locations = {0}", locations.Rows.Count);

            var branchesList = branches.AsEnumerable().ToList();
            var locationsList = locations.AsEnumerable().ToList();

            var regExp = new Regex(@"^[a-zA-Z0-9\*\$][a-zA-Z0-9\-._\$]*(\(\d{1,}\)){0,1}$");
            var filteredLocationsList = locationsList.Where(l => !string.IsNullOrEmpty(l["URL"].ToString()) &&
                                                            l["URL"].ToString().Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries).All(i => regExp.IsMatch(i)));

            var resultList = new List<Page>();

            foreach (var location in filteredLocationsList)
            {
                var page = CreatePageFromLocation(location);
                var url = location["URL"].ToString();

                // city or branch
                if (location.Field<bool?>("IsViewable") == false)
                {
                    page.ItemType = ItemType.General;
                }
                else if (url.EndsWith("_moving_articles.aspx"))
                {
                    page.ItemType = ItemType.MovingArticles;
                }
                else if (url.EndsWith("/index.aspx"))
                {
                    if (location.Field<bool?>("IsBranch") ?? false)
                    {
                        page.ItemType = ItemType.Branch;

                        var branchId = location["Branch"].ToString();
                        var branch = branchesList.FirstOrDefault(b => b["ID"].ToString() == branchId);

                        ProcessBranchAddressInfo(page, branch);
                    }
                    else
                    {
                        page.ItemType = ItemType.City;
                    }
                }
                else
                {
                    // will set type to general for l1 of based on parent page
                    page.ItemType = ItemType.Undefined;
                }

                //string name;
                //if (url.EndsWith("/index.aspx"))
                //{
                //    name = url.Replace("/index.aspx", "").TrimStart(new[] { '/' });

                //    if (page.ItemType == ItemType.Branch)
                //    {
                //        var branch = branchesList.FirstOrDefault(b => b["ID"].ToString() == location["Branch"].ToString());

                //        ProcessBranchAddressInfo(page, branch);
                //    }
                //}
                //else
                //{
                //    name = url.Replace(".aspx", "").Substring(url.LastIndexOf('/') + 1);
                //}

                var processedUrl = location["URL"].ToString().Replace("index.aspx", "").Replace(".aspx", "");
                var paths = processedUrl.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries).Select(name => FormatName(name)).ToArray();
                page.ItemName = FormatName(paths[paths.Length - 1]);

                //Console.WriteLine("{0} \t {1}", page.CanonicalUrl, page.ItemName);

                //if (url.EndsWith("/index.aspx"))
                //{
                //    var name = url.Replace("/index.aspx", "").TrimStart(new[] { '/' });
                //    page.ItemName = FormatName(name);

                //    if (location.Field<bool?>("IsBranch") ?? false)
                //    {
                //        // branch
                //        page.ItemType = ItemType.Branch;

                //        var branchId = location["Branch"].ToString();
                //        var branch = branchesList.FirstOrDefault(b => b["ID"].ToString() == branchId);

                //        ProcessBranchAddressInfo(page, branch);
                //    }
                //    else
                //    {
                //        // city
                //        page.ItemType = ItemType.City;
                //    }
                //}
                //else
                //{
                //    // general page or child page
                //    page.ItemType = ItemType.Undefined;
                //    var name = url.Replace(".aspx", "").Substring(url.LastIndexOf('/') + 1);

                //    page.ItemName = FormatName(name);
                //}

                resultList.Add(page);
            }

            Console.WriteLine("Result list = {0}", resultList.Count);
            //Console.WriteLine("ItemType.Undefined = {0}", resultList.Where(i => i.ItemType == ItemType.Undefined).Count());

            //var r = resultList.Where(ii => ii.ItemType == ItemType.Undefined).ToList();
            //for (int i = 0; i < 20; i++)
            //{
            //    Console.WriteLine("ItemType.Undefined = {0}", r[i].CanonicalUrl);
            //}

            //return resultList;


            // convert to hierarchical data structure based on url
            var level1 = resultList.Where(i => i.Paths.Length == 1).ToList();
            var level2 = resultList.Where(l2 => l2.Paths.Length == 2 && level1.Select(l1 => l1.Paths[0]).Contains(l2.Paths[0])).ToList();
            Console.WriteLine("level1 = {0}", level1.Count());
            Console.WriteLine("level2 = {0}", level2.Count());

            // urls that have not been migrated
            var ids = level1.Select(l1 => l1.PageId).Concat(level2.Select(l2 => l2.PageId)).ToList();
            var ignoredLocations = locationsList.Where(dr => !ids.Contains((int)dr["ID"])).Select(dr => dr["URL"].ToString()).ToList();
            Console.WriteLine("Invalid locations = {0}", ignoredLocations.Count);
            File.WriteAllLines(@"f:\temp\invalidLocations.txt", ignoredLocations);


            var topLevel = new List<Page>();

            int counter = 0;
            int sortOrderLevel1 = 100;
            foreach (var page in level1)
            {
                page.SortOrder = sortOrderLevel1;
                sortOrderLevel1 += 100;

                //set all Undefined types of level 1 to General
                if (page.ItemType == ItemType.Undefined)
                    page.ItemType = ItemType.General;

                var children = level2.Where(l2 => l2.Paths[0] == page.Paths[0]).ToList();
                //set all Undefined types of level 2 to type of the parent
                children.Where(i => i.ItemType == ItemType.Undefined).ToList().ForEach(i => i.ItemType = page.ItemType);

                for (int i = 0; i < children.Count; i++)
                {
                    children[i].SortOrder = (i + 1) * 100;
                }

                page.Children = children;

                //Console.WriteLine("Page: {0}", page.CanonicalUrl);
                //Console.ReadLine();

                topLevel.Add(page);
                counter++;
            }

            //using (var streamWriter = new StreamWriter(@"f:\temp\lines.txt"))
            //{
            //    PrintUrls(streamWriter, topLevel);
            //}

            //File.WriteAllLines(@"f:\temp\urlsWithoutParent.txt", level2);

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<Page>));

            using (FileStream fs = new FileStream(@"f:\temp\plainPages.xml", FileMode.Create))
            {
                xmlSerializer.Serialize(fs, resultList);
            }

            using (FileStream fs = new FileStream(@"f:\temp\hierarchicalPages.xml", FileMode.Create))
            {
                xmlSerializer.Serialize(fs, topLevel);
            }


            return topLevel;
            #region Test
            //var indexUrls = locationsList.Where(dr =>
            //                //(dr.Field<bool?>("IsBranch") ?? false) == true &&
            //                dr["URL"].ToString().EndsWith("index.aspx")
            //            )
            //            .Select(dr => dr["URL"].ToString())
            //            .ToList();

            //File.WriteAllLines(@"f:\temp\indexUrls.txt", indexUrls);
            //Console.WriteLine("Index URLs = {0}", indexUrls.Count); // 66

            //var regExp = new Regex(@"^[a-zA-Z0-9\*\$][a-zA-Z0-9\s\-._\$]*(\(\d{1,}\)){0,1}$");
            //var invalidUrls = locationsList
            //            .Select(dr => dr["URL"].ToString())
            //            .Where(url => url.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries).Any(i => !regExp.IsMatch(i)))
            //            .ToList();

            //File.WriteAllLines(@"f:\temp\invalidUrls.txt", invalidUrls);
            //Console.WriteLine("Invalide URLs = {0}", invalidUrls.Count); // 66

            //var locationsWithIndexUrls = locationsList.Where(dr =>
            //    (dr.Field<bool?>("IsBranch") ?? false) == true &&
            //    dr["URL"].ToString().EndsWith("index.aspx")
            //)
            //.ToList();

            //var diff = branchesList.Where(b => !locationsWithIndexUrls.Any(l=> l["Branch"].ToString() == b["ID"].ToString()))
            //    .Select(b => b["ID"].ToString())
            //    .ToList();

            //File.WriteAllLines(@"f:\temp\diff.txt", diff);
            //Console.WriteLine("Diff URLs = {0}", diff.Count); // 66

            //var urls = locationsList.Select(dr => dr["URL"].ToString()).ToList();
            //File.WriteAllLines(@"f:\temp\Urls.txt", urls);
            //Console.WriteLine("URLs = {0}", urls.Count);

            //var duplicateKeys = locationsList.Select(dr => dr["URL"].ToString()).GroupBy(x => x)
            //            .Where(group => group.Count() > 1)
            //            .Select(group => group.Key).ToList();
            //File.WriteAllLines(@"f:\temp\duplicateKeys.txt", duplicateKeys);
            //Console.WriteLine("URLs = {0}", duplicateKeys.Count);



            //using (var file = new StreamWriter(@"f:\temp\test.txt"))
            //{
            //foreach (var row in locations.Rows)
            //{
            //    file.WriteLine(row["URL"]);
            //}
            //}

            //Console.ReadLine();
            #endregion
        }

        private static void ProcessBranchAddressInfo(Page page, DataRow branch)
        {
            page.CorporateName = branch["CorporateName"] as string;
            page.Address1 = branch["Address1"] as string;
            page.Address2 = branch["Address2"] as string;
            page.City = branch["City"] as string;
            page.State = branch["State"] as string;
            page.Zip = branch["Zip"] as string;
            page.Tel = branch["Phone"] as string;
            page.Fax = branch["Fax"] as string;

            string dot = branch.Field<Int64?>("DOT").HasValue ? (branch.Field<Int64?>("DOT")).ToString() : null;
            string mc = branch["MC"] as string;

            if (!string.IsNullOrEmpty(dot))
                page.AdditionalInfo = string.Format("U.S. DOT No.: {0}", dot);
            
            if (!string.IsNullOrEmpty(mc))
            {
                if (!string.IsNullOrEmpty(page.AdditionalInfo))
                    page.AdditionalInfo += "<br/>";

                page.AdditionalInfo += string.Format("MC: {0}", mc);
            }
        }

        private static string FormatName(string name)
        {
            //name = name.ToLowerInvariant().Replace('_', ' ');
            name = name.Replace('_', ' ');

            //TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            //name = textInfo.ToTitleCase(name);

            return name;
        }

        private static Page CreatePageFromLocation(DataRow location)
        {
            var processedUrl = location["URL"].ToString().Replace("index.aspx", "").Replace(".aspx", "");
            var paths = processedUrl.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries).Select(name => FormatName(name)).ToArray();

            var baseUri = new Uri(Config.SITE_URL);
            var pageUri = new Uri(baseUri, location["URL"].ToString().TrimStart(new[] { '/' }));

            return new Page
            {
                PageId = Convert.ToInt32(location["ID"]),
                BranchId = Convert.ToInt32(location["Branch"]),
                Paths = paths,

                ItemDisplayName = location["PageCaption"] as string,

                Title = location["Title"] as string,
                Subtitle = location["Summary"] as string,
                Content = location["Post"] as string,

                MetaTitle = location["MetaTitle"] as string,
                MetaKeyword = location["MetaKeyword"] as string,
                MetaDescription = location["MetaDescription"] as string,
                CanonicalUrl = pageUri.ToString(),
                GooglePublisherTag = location["GooglePublisherTag"] as string,

                LogoAlt = location["LogoAlt"] as string
            };
        }

        private static void PrintUrls(StreamWriter streamWriter, List<Page> structure)
        {
            foreach (var page in structure)
            {
                streamWriter.WriteLine("{0}{1}", new String('\t', page.Paths.Length - 1), string.Join("/", page.Paths));

                if (page.Children.Any())
                    PrintUrls(streamWriter, page.Children);
            }
        }

        internal void ChangeTemplates()
        {
            var client = new MigrateDataSoapClient();
            var locations = client.GetAllLocationsPages(); // locations - 3874

            //branches.WriteXml(@"f:\temp\AllBranches.xml");
            locations.WriteXml(@"f:\temp\AllLocationsForUpdate.xml");
            Console.WriteLine("Locations = {0}", locations.Rows.Count);

            var locationsList = locations.AsEnumerable().ToList();

            var regExp = new Regex(@"^[a-zA-Z0-9\*\$][a-zA-Z0-9\-._\$]*(\(\d{1,}\)){0,1}$");
            var filteredLocationsList = locationsList.Where(l => !string.IsNullOrEmpty(l["URL"].ToString()) &&
                                                            l["URL"].ToString().Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries).All(i => regExp.IsMatch(i))).ToList();

            Console.WriteLine("filteredLocationsList = {0}", filteredLocationsList.Count());

            var items = Factory.GetDatabase("master").GetItem(Config.HOME_PAGE_PATH).Axes.GetDescendants();
            Console.WriteLine("item.Axes.GetDescendants = {0}", items.Count());

            var generalTemplate = Factory.GetDatabase("master").GetTemplate(Config.TEMPLATE_ID_GENERAL_PAGE);
            var movingArticlesTemplate = Factory.GetDatabase("master").GetTemplate(Config.TEMPLATE_ID_MOVING_ARTICLES_PAGE);

            using (new SecurityDisabler())
            {
                foreach (var item in items)
                {
                    var canonicalUrl = item["Canonical URL"].Replace(Config.SITE_URL, "").TrimStart(new[] { '/' });
                    var sourceItem = filteredLocationsList.FirstOrDefault(l => l["URL"].ToString().TrimStart(new[] { '/' }) == canonicalUrl);

                    // manually created items: 404, terms...
                    if (sourceItem == null)
                        continue;

                    if (sourceItem.Field<bool?>("IsViewable") == false)
                    {
                        item.ChangeTemplate(generalTemplate);
                    }

                    if (sourceItem["URL"].ToString().EndsWith("_moving_articles.aspx"))
                    {
                        item.ChangeTemplate(movingArticlesTemplate);
                    }
                }
            }
           
        }
    }
}
