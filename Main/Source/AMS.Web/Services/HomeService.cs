using AMS.Web.Core;
using AMS.Web.Models;
using Newtonsoft.Json;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;

namespace AMS.Web.Services
{
    public class HomeService
    {
        public LocationsModel GetLocations()
        {
            Sitecore.Data.Database database = Sitecore.Configuration.Factory.GetDatabase("web");
            Item contextItem = database.SelectSingleItem(ItemPaths.PATH_HOME);
            return new LocationsModel
            {
                Items = contextItem.Axes.GetDescendants().Where(x => x.TemplateID.ToString() == TemplateIds.TEMPLATE_ID_BRANCH_PAGE && !string.IsNullOrEmpty(x.Fields["Corporate Name"].Value.ToString())).ToList().OrderBy(x => x.DisplayName).ToList()
            };
        }

        public ReviewsListModel GetReviews(string branch, int pageNumber, int pageSize)
        {
            var navService = new NavigationService();
            var branchName = branch == null ?
                navService.GetCurrentBranchName() ?? "National" :
                branch;
            
            var reviews = FetchReviews(branchName, pageNumber, pageSize);
            Sitecore.Diagnostics.Log.Info(string.Format("Branch Review data: Branch-{0},reviews Count: {1}" ,branch,reviews.Count), this);

            return new ReviewsListModel
            {
                BranchName = branchName,
                PageNum = pageNumber,
                PageSize = pageSize,
                Reviews = reviews
            };
        }

        private List<ReviewModel> FetchReviews(string branch, int pageNumber = 1, int pageSize = 15)
        {
            try
            {
                if (pageNumber == 1)
                {
                    var reviews = CacheService.Instance.Get("Reviews_" + branch, () =>
                    {
                        return InvokeReviewsService(branch, pageNumber, pageSize);
                    });

                    return reviews;
                }
                else
                {
                    return InvokeReviewsService(branch, pageNumber, pageSize);
                }

            }
            catch (Exception ex)
            {
                Log.Error("Error fetching Reviews for " + branch, ex, this);
            }

            return null;
        }

        private List<ReviewModel> InvokeReviewsService(string branch, int pageNumber, int pageSize)
        {
            using (var client = new WebClient())
            {
                var data = new NameValueCollection();
                data.Add("branchName", branch);
                data.Add("PageNumber", pageNumber.ToString());
                data.Add("PageSize", pageSize.ToString());

                var result = client.UploadValues(Config.ReviewsServiceAppUrl, data);

                return JsonConvert.DeserializeObject<List<ReviewModel>>(Encoding.UTF8.GetString(result));
            }
        }
    }
}