using Sitecore.Configuration;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Pipelines;
using Sitecore.SecurityModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;


namespace AMS.Migration
{
    public class SitecoreImporter
    {
        private int globalCounter = 0;

        public void ImportMissingPages(List<MissingPage> pages)
        {
            int sortOrder = 70000;
            // heat the Sitecore up
            Pipeline.Start("initialize", new PipelineArgs(), true);

            // get database
            var db = Factory.GetDatabase("master");

            // get home item
            var item = db.GetItem(Config.HOME_PAGE_PATH);

            using (new SecurityDisabler())
            {
                var template = item.Database.GetTemplate(Config.TEMPLATE_ID_GENERAL_PAGE);

                foreach (var page in pages)
                {
                    var itemName = ItemUtil.ProposeValidItemName(page.FileName);

                    if (itemName != page.FileName)
                    {
                        using (var file = new StreamWriter(@"f:\temp\MissingPages\NameIssuesForRedirects.txt", true)) // append
                        {
                            file.WriteLine(page.OriginalFileName);
                            file.WriteLine(itemName);
                            file.WriteLine("-------------");
                        }
                    }

                    var existingItem = item.Children.FirstOrDefault(c => string.Equals(c.Name, itemName, StringComparison.InvariantCultureIgnoreCase));
                    if (existingItem != null)
                    {
                        Console.WriteLine("!SKIPPED!");
                        using (var file = new StreamWriter(@"f:\temp\MissingPages\SkippedItems.txt", true)) // append
                        {
                            file.WriteLine(itemName);
                            file.WriteLine(existingItem.Paths.Path);
                            file.WriteLine("-------------");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Adding {0}...", itemName);
                        var addedItem = item.Add(itemName, template);

                        page.ItemGuid = addedItem.ID.ToString();

                        using (new EditContext(addedItem))
                        {
                            addedItem.Fields[Sitecore.FieldIDs.Sortorder].Value = sortOrder.ToString();

                            if (!string.IsNullOrEmpty(page.Body))
                                addedItem["Content"] = page.Body;

                            if (!string.IsNullOrEmpty(page.MetaTitle))
                                addedItem["Meta Title"] = page.MetaTitle;

                            if (!string.IsNullOrEmpty(page.MetaDescription))
                                addedItem["Meta Description"] = page.MetaDescription;

                            if (!string.IsNullOrEmpty(page.MetaKeyword))
                                addedItem["Meta Keywords"] = page.MetaKeyword;
                        }
                        sortOrder += 100;
                        globalCounter++;
                        Console.WriteLine("#{0}\t{1}", globalCounter, addedItem.Paths.Path);
                    }
                }
            }
        }

        public void ProcessPages(List<Page> pages)
        {
            // heat the Sitecore up
            Pipeline.Start("initialize", new PipelineArgs(), true);

            // get database
            var db = Factory.GetDatabase("master");

            // get home item
            var item = db.GetItem(Config.HOME_PAGE_PATH);

            using (new SecurityDisabler())
            {
                CreatePages(item, pages, true);
            }
        }

        private void CreatePages(Item rootItem, List<Page> pages, bool isRootLevel)
        {
            //var sortOrder = 100;
            foreach (var page in pages)
            {
                TemplateItem template = null;
                switch (page.ItemType)
                {
                    case ItemType.Branch:
                        template = rootItem.Database.GetTemplate(Config.TEMPLATE_ID_BRANCH_PAGE);
                        break;
                    case ItemType.City:
                        template = rootItem.Database.GetTemplate(Config.TEMPLATE_ID_CITY_PAGE);
                        break;
                    case ItemType.General:
                        template = rootItem.Database.GetTemplate(Config.TEMPLATE_ID_GENERAL_PAGE);
                        break;
                    case ItemType.MovingArticles:
                        template = rootItem.Database.GetTemplate(Config.TEMPLATE_ID_MOVING_ARTICLES_PAGE);
                        break;
                    default:
                        break;
                }

                var itemName = ItemUtil.ProposeValidItemName(page.ItemName);

                if (itemName != page.ItemName)
                {
                    using (var file = new StreamWriter(@"f:\temp\nameIssuesForRedirects.txt", true)) // append
                    {
                        file.WriteLine(page.CanonicalUrl);
                    }
                }

                var addedItem = rootItem.Add(itemName, template);

                page.ItemGuid = addedItem.ID.ToString();

                using (new EditContext(addedItem))
                {
                    addedItem.Fields[Sitecore.FieldIDs.Sortorder].Value = page.SortOrder.ToString();

                    SetCommonFields(addedItem, page);

                    if (page.ItemType == ItemType.Branch)
                        SetAddress(addedItem, page);

                    if (!isRootLevel && (page.ItemType == ItemType.Branch || page.ItemType == ItemType.City))
                        ClearBannerFields(addedItem);
                }
                globalCounter++;

                Console.WriteLine("#{0}\t{1}", globalCounter, addedItem.Paths.Path);

                if (page.Children.Any())
                    CreatePages(addedItem, page.Children, false);
            }
        }

        private void ClearBannerFields(Item item)
        {
            item["Phone"] = string.Empty;
            item["Banner Image"] = string.Empty;
            item["Banner Title"] = string.Empty;
            item["Banner Subtitle"] = string.Empty;
        }

        private void SetCommonFields(Item item, Page page)
        {
            if (!string.IsNullOrEmpty(page.ItemDisplayName))
                item.Appearance.DisplayName = page.ItemDisplayName;

            //if (!string.IsNullOrEmpty(page.Title))
                item["Title"] = page.Title;

            //if (!string.IsNullOrEmpty(page.Subtitle))
                item["Subtitle"] = page.Subtitle;

            //if (!string.IsNullOrEmpty(page.Content))
                item["Content"] = page.Content;

            //if (!string.IsNullOrEmpty(page.MetaTitle))
                item["Meta Title"] = page.MetaTitle;

            //if (!string.IsNullOrEmpty(page.MetaDescription))
                item["Meta Description"] = page.MetaDescription;

            //if (!string.IsNullOrEmpty(page.MetaKeyword))
                item["Meta Keywords"] = page.MetaKeyword;

            //if (!string.IsNullOrEmpty(page.CanonicalUrl))
                item["Canonical URL"] = page.CanonicalUrl;

            //if (!string.IsNullOrEmpty(page.GooglePublisherTag))
                item["Google Publisher Tag"] = page.GooglePublisherTag; 
        }

        private void SetAddress(Item item, Page page)
        {
            //if (!string.IsNullOrEmpty(page.CorporateName))
                item["Corporate Name"] = page.CorporateName;

            //if (!string.IsNullOrEmpty(page.Address1))
                item["Street"] = page.Address1;

            //if (!string.IsNullOrEmpty(page.Address2))
                item["Street 2"] = page.Address2;

            //if (!string.IsNullOrEmpty(page.City))
                item["City"] = page.City;

            //if (!string.IsNullOrEmpty(page.State))
                item["State"] = page.State;

            //if (!string.IsNullOrEmpty(page.State))
                item["Zip"] = page.Zip;

            //if (!string.IsNullOrEmpty(page.Tel))
                item["Tel"] = page.Tel;

            //if (!string.IsNullOrEmpty(page.Fax))
                item["Fax"] = page.Fax;

            //if (!string.IsNullOrEmpty(page.AdditionalInfo))
                item["Additional Location Info"] = page.AdditionalInfo;
        }

        internal void TestItemNames(List<Page> pages)
        {
            foreach (var page in pages)
            {
                if (!ItemUtil.IsItemNameValid(page.ItemName))
                    Console.WriteLine("'{0}'\tPageId = {1}", page.ItemName, page.PageId, page.CanonicalUrl);
                
                if (page.Children.Any())
                    TestItemNames(page.Children);
            }
        }

        internal void SetRelatedLocations(List<Page> pages)
        {
            //var items = Factory.GetDatabase("master").GetItem(Config.HOME_PAGE_PATH).Axes.GetDescendants();
            var items = Factory.GetDatabase("master").GetItem(Config.HOME_PAGE_PATH).Children.ToList();
            //Console.WriteLine("item.Axes.GetDescendants = {0}", items.Count());

            //var list = ConvertToPlainList(pages);
            //Console.WriteLine(list.Count);
            var counter = 0;
            using (new SecurityDisabler())
            {
                foreach (var item in items)
                {
                    var current = pages.FirstOrDefault(i => i.ItemGuid == item.ID.ToString());
                    if (current != null)
                    {
                        var guidsToAdd = pages.Where(i => i.BranchId == current.BranchId && 
                                                    (i.ItemType == ItemType.Branch || i.ItemType == ItemType.City))
                                               .Select(i => i.ItemGuid)
                                               .ToList();

                        if (guidsToAdd.Any())
                        {
                            using (new EditContext(item))
                            {
                                MultilistField relatedLocationsField = item.Fields["Related Locations"];

                                foreach (var guid in guidsToAdd)
                                {
                                    if (!relatedLocationsField.Contains(guid))
                                    {
                                        relatedLocationsField.Add(guid);
                                    }
                                }
                            }
                        }
                    }

                    counter++;

                    Console.WriteLine("#{0}\t{1}", counter, item.Paths.Path);
                }
            }
        }

        //private static List<Page> ConvertToPlainList(List<Page> pages)
        //{ 
        //    var plainList = new List<Page>();
        //    Action<List<Page>> getItems = null;

        //    getItems = list =>
        //    {
        //        foreach (var item in list)
        //        {
        //            plainList.Add(item);

        //            if (!item.Children.Any()) 
        //                continue;

        //            getItems(item.Children);
        //        }
        //    };

        //    getItems(pages);

        //    return plainList;
        //}

        internal void DeleteImportedPages()
        {
            var homeItem = Factory.GetDatabase("master").GetItem(Config.HOME_PAGE_PATH);
            //var items = homeItem.Axes.GetDescendants().Where(i => i.Fields["Canonical URL"] != null && !string.IsNullOrEmpty(i["Canonical URL"])).ToList();
            var items = homeItem.Children.Where(i => i.Fields["Canonical URL"] != null && !string.IsNullOrEmpty(i["Canonical URL"])).ToList();
            Console.WriteLine(items.Count());
            int counter = 0;
            using (new SecurityDisabler())
            {
                foreach (Item l1 in items)
                {
                    foreach (Item l2 in l1.Children.Where(i => i.Fields["Canonical URL"] != null && !string.IsNullOrEmpty(i["Canonical URL"])).ToList())
                    {
                        try
                        {
                            counter++;
                            l2.Delete();
                        }
                        catch
                        {
                        }
                    }

                    try
                    {
                        counter++;
                        l1.Delete();
                    }
                    catch
                    {
                    }
                    
                    Console.WriteLine(counter);
                }
            }

            

            //XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<string>));

            //using (FileStream fs = new FileStream(@"f:\temp\ttt.xml", FileMode.Create))
            //{
            //    xmlSerializer.Serialize(fs, items);
            //}

            
        }
    }
}
