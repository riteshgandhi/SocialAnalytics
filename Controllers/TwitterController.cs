using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tweetinvi;
using Tweetinvi.Parameters;
using Tweetinvi.Models;

namespace TwitterAPI.Controllers
{
    public class TwitterController : Controller
    {
        [HttpGet("twtapi/simpleSearch")]
        public IActionResult SimpleSearch([FromHeader] string consumerKey, [FromHeader] string consumerSecret,
            [FromHeader] string userAccessToken, [FromHeader] string userAccessSecret,string keyword) {

            List<ITweet> foundTweets = new List<ITweet>();

            if (authenticateUser(consumerKey, consumerSecret, userAccessToken, userAccessSecret)) {
                var matchingTweets = Search.SearchTweets(keyword);
                foundTweets = matchingTweets.ToList();
            }
            return Ok(foundTweets);
        }

        [HttpGet("twtapi/search")]
        public IActionResult search([FromHeader] string consumerKey, [FromHeader] string consumerSecret,
            [FromHeader] string userAccessToken, [FromHeader] string userAccessSecret, 
            string keyword, string latitude, string longitude, string radius) {

            List<ITweet> foundTweets = new List<ITweet>();

            if (authenticateUser(consumerKey, consumerSecret, userAccessToken, userAccessSecret)) {
                
                //other options
                //SearchTweetsParameters sparams = new SearchTweetsParameters(keyword) {
                //    GeoCode = new GeoCode(Double.Parse(latitude), 
                //        Double.Parse(longitude), 
                //        Double.Parse(radius), 
                //        DistanceMeasure.Miles),
                //    Lang = LanguageFilter.English,
                //    SearchType = SearchResultType.Popular,
                //    MaximumNumberOfResults = 100,
                //    Until = new DateTime(2015, 06, 02),
                //    SinceId = 399616835892781056,
                //    MaxId = 405001488843284480,
                //    Filters = TweetSearchFilters.Images
                //};

                GeoCode geoCode = new GeoCode(Double.Parse(latitude), Double.Parse(longitude),
                        Double.Parse(radius), DistanceMeasure.Miles);

                SearchTweetsParameters sparams = new SearchTweetsParameters(keyword) {
                    GeoCode = geoCode,
                    SearchType = SearchResultType.Mixed
                };

                var matchingTweets = Search.SearchController.SearchTweets(sparams);
                foundTweets = matchingTweets.ToList();
            }
            return Ok(foundTweets);
        }

        [HttpGet("twtapi/searchUser")]
        public IActionResult searchUser([FromHeader] string consumerKey, [FromHeader] string consumerSecret,
            [FromHeader] string userAccessToken, [FromHeader] string userAccessSecret,
            string keyword) {

            List<IUser> users = new List<IUser>();

            if (authenticateUser(consumerKey, consumerSecret, userAccessToken, userAccessSecret)) {
                var matchingRecords = Search.SearchController.SearchUsers(keyword);
                users = matchingRecords.ToList();
            }
            return Ok(users);
        }

        [HttpGet("twtapi/searchTimeline")]
        public IActionResult searchTimeline([FromHeader] string consumerKey, [FromHeader] string consumerSecret,
            [FromHeader] string userAccessToken, [FromHeader] string userAccessSecret,
            string userScreenName) {

            List<ITweet> userTweets = new List<ITweet>();

            if (authenticateUser(consumerKey, consumerSecret, userAccessToken, userAccessSecret)) {
                var matchingRecords = Timeline.TimelineController.GetUserTimeline(userScreenName);
                userTweets = matchingRecords.ToList();
            }
            return Ok(userTweets);
        }

        [HttpGet("twtapi/searchTimelineByDate")]
        public IActionResult searchTimelineForDateRange([FromHeader] string consumerKey, [FromHeader] string consumerSecret,
            [FromHeader] string userAccessToken, [FromHeader] string userAccessSecret,
            string userScreenName, string startDate, string endDate) {

            List<ITweet> userTweets = new List<ITweet>();

            if (authenticateUser(consumerKey, consumerSecret, userAccessToken, userAccessSecret)) {
                DateTime tweetStartDate = DateTime.Parse(startDate);
                DateTime tweetEndDate = DateTime.Parse(endDate).AddDays(1);

                var matchingRecords = Timeline.TimelineController.GetUserTimeline(userScreenName);
                if (matchingRecords.Any()) {
                    userTweets = matchingRecords.ToList().
                        Where(t => tweetStartDate <= t.CreatedAt && tweetEndDate >= t.CreatedAt).
                        Select(t => t).ToList();
                }
            }
            return Ok(userTweets);
        }

        private bool authenticateUser(string consumerKey, string consumerSecret,
            string userAccessToken, string userAccessSecret) {
            Auth.SetUserCredentials(consumerKey, consumerSecret, userAccessToken, userAccessSecret);
            return true;
        }
    }
}