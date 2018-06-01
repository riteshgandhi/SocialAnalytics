using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FacebookApi.Models;

namespace FacebookApi.Controllers
{
    public class FBController : Controller
    {
        [HttpGet("fbapi/profile")]
        public IActionResult GetProfile(string token) {
            var facebookClient = new FacebookClient();
            var facebookService = new FacebookService(facebookClient);
            var getAccountTask = facebookService.GetAccountAsync(token);
            Task.WaitAll(getAccountTask);
            var account = getAccountTask.Result;
            return Ok(account);
        }

        [HttpGet("fbapi/friends")]
        public IActionResult GetFriends(string token) {
            var facebookClient = new FacebookClient();
            var facebookService = new FacebookService(facebookClient);
            var getFacebookFriendsTask = facebookService.GetFacebookFriendsAsync(token);
            Task.WaitAll(getFacebookFriendsTask);
            var facebookFriends = getFacebookFriendsTask.Result;
            return Ok(facebookFriends);
        }
    }
}