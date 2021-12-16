using AMS.Web.Core;
using AMS.Web.Models;
using Sitecore;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Links;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMS.Web.Services
{
    public class BlogService
    {
        public IEnumerable<BlogArchive> Archives()
        {
            var blogs = List();
            if (blogs == null && !blogs.Any())
                return null;

            Item blogHomePage = GetBlogHomePage();
            if (blogHomePage == null)
                return null;

            var archives = new List<BlogArchive>();
            var years = blogs.Select(x => x.PostDate).GroupBy(x => x.Year);
            var archivePageUrl = LinkManager.GetItemUrl(blogHomePage);
            foreach (var year in years)
            {
                var firstDate = year.First();
                archives.Add(new BlogArchive
                {
                    Title = firstDate.ToString("yyyy"),
                    Url = GetArcgiveUrl(archivePageUrl, firstDate)
                });
            }

            return archives;
        }

        /// <param name="t">category/tag</param>   
        /// <param name="y">year</param>
        /// <param name="m">month</param>
        public IEnumerable<BlogPost> List(string t, int? y)
        {
            if (!string.IsNullOrWhiteSpace(t))
            {
                var category = t.Replace("-", " ").ToLower();
                return List().Where(x => x.Categories != null && x.Categories.Any(c => c.Title.ToLower() == category));
            }

            if (y.HasValue)
            {
                return List().Where(x => x.PostDate.Year == y.Value);
            }

            return null;
        }

        public IEnumerable<BlogPost> List()
        {
            Item blogHomePage = GetBlogHomePage();
            if (blogHomePage == null)
                return null;

            var blogs = new List<BlogPost>();
            var blogsItems = blogHomePage.Axes.GetDescendants().Where(x => x.TemplateID == TemplateIds.BLOG_POST_PAGE);
            foreach (var item in blogsItems)
            {
                var blog = new BlogPost();
                blog.Url = LinkManager.GetItemUrl(item);
                blog.Title = item.Fields[FieldNames.FIELD_NAME_TITLE].Value;
                blog.Content = item.Fields[FieldNames.FIELD_NAME_CONTENT].Value;

                DateField postDateField = item.Fields[FieldNames.FIELD_POSTDATE];
                if (postDateField != null)
                    blog.PostDate = postDateField.DateTime;

                MultilistField categoriesField = item.Fields[FieldNames.FIELD_BLOG_CATEGORY];
                if (categoriesField != null && categoriesField.TargetIDs != null)
                {
                    var categoriesItems = categoriesField.TargetIDs.Take(5).Select(Sitecore.Context.Database.GetItem);
                    if (categoriesItems.Any())
                    {
                        var categoryPageUrl = LinkManager.GetItemUrl(blogHomePage);
                        blog.Categories = categoriesItems.Select(x => new BlogCategory
                        {
                            Title = x.Fields[FieldNames.FIELD_VALUE].Value,
                            Url = GetCategoryUrl(categoryPageUrl, x.Fields[FieldNames.FIELD_VALUE].Value.Replace(" ", "-").ToLower())
                        });
                    }
                }

                blogs.Add(blog);
            }

            return blogs.OrderByDescending(x => x.PostDate);
        }               

        /// <param name="t">category/tag</param> 
        public KeyValuePair<string, string> GetCategoryPage(HttpRequestBase request)
        {
            var param = request.Params["t"];
            if (!string.IsNullOrWhiteSpace(param))
            {
                Item blogHomePage = GetBlogHomePage();
                if (blogHomePage == null)
                    return new KeyValuePair<string, string>();
  
                var root = Context.Database.GetItem(Items.BlogCategories);
                var category = root.Axes.GetDescendants().FirstOrDefault(x => x.Fields[FieldNames.FIELD_VALUE].Value.Replace(" ", "-").ToLower() == param);                

                var url = LinkManager.GetItemUrl(blogHomePage);
                return new KeyValuePair<string, string>(category == null ? null : category.Fields[FieldNames.FIELD_VALUE].Value, GetCategoryUrl(url, param));
            }

            return new KeyValuePair<string, string>();
        }

        public KeyValuePair<string, string> GetArchivePage(HttpRequestBase request)
        {
            int y;            
            if (int.TryParse(request.Params["y"], out y))
            {
                var date = new DateTime(y, 1, 1);
                Item blogHomePage = GetBlogHomePage();
                if (blogHomePage == null)
                    return new KeyValuePair<string, string>();

                var url = LinkManager.GetItemUrl(blogHomePage);
                return new KeyValuePair<string, string>(@date.ToString("yyyy"), GetArcgiveUrl(url, date));
            }

            return new KeyValuePair<string, string>();
        }

        public string GetPageUrl(BlogList model, int? page)
        {
            Item blogHomePage = GetBlogHomePage();
            if (blogHomePage == null)
                return null;

            var url = LinkManager.GetItemUrl(blogHomePage);
            if (!string.IsNullOrWhiteSpace(model.Category))
                url = GetCategoryUrl(url, model.Category);
            else if (model.Year > 0)
                url = GetArcgiveUrl(url, new DateTime(model.Year.Value, 1, 1));

            if (page > 0)
            {
                if (!string.IsNullOrWhiteSpace(model.Category) || model.Year > 0)
                {
                    url = string.Format("{0}/{1}", url, page);
                }
                else
                {
                    url = string.Format("{0}?page={1}", url, page);
                }
            }

            return url;
        }

        private string GetCategoryUrl(string absolutePath, string categoryName)
        {
            return string.Format("{0}/category/{1}", absolutePath.Replace(".aspx", ""), categoryName);
        }

        private string GetArcgiveUrl(string absolutePath, DateTime date)
        {
            return string.Format("{0}/{1}", absolutePath.Replace(".aspx", ""), date.Year);
        }

        private Item GetBlogHomePage()
        {
            Item blogHomePage;
            if (Context.Item.TemplateID == TemplateIds.BLOG_HOME_PAGE)
            {
                blogHomePage = Context.Item;
            }
            else
            {
                blogHomePage = Context.Item.Axes.GetAncestors().Where(x => x.TemplateID == TemplateIds.BLOG_HOME_PAGE).FirstOrDefault();
            }

            return blogHomePage;
        }

        private Item GetBlogArchivePage()
        {
            Item blogArchivePage;
            if (Context.Item.TemplateID == TemplateIds.BLOG_ARCHIVE_PAGE)
            {
                blogArchivePage = Context.Item;
            }
            else
            {
                var blogHomePage = GetBlogHomePage();
                blogArchivePage = blogHomePage.Axes.GetDescendants().Where(x => x.TemplateID == TemplateIds.BLOG_ARCHIVE_PAGE).FirstOrDefault();
            }

            return blogArchivePage;
        }

        private Item GetBlogCategoryPage()
        {
            Item blogCategoryPage;
            if (Context.Item.TemplateID == TemplateIds.BLOG_CATEGORY_PAGE)
            {
                blogCategoryPage = Context.Item;
            }
            else
            {
                var blogHomePage = GetBlogHomePage();
                blogCategoryPage = blogHomePage.Axes.GetDescendants().Where(x => x.TemplateID == TemplateIds.BLOG_CATEGORY_PAGE).FirstOrDefault();
            }

            return blogCategoryPage;
        }
    }
}