using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FacebookApi.Models
{
    public interface IFacebookService {
        Task<Account> GetAccountAsync(string accessToken);
        Task<List<FacebookFriend>> GetFacebookFriendsAsync(string accessToken);
        Task PostOnWallAsync(string accessToken, string message);
    }

    public class FacebookService : IFacebookService {
        private readonly IFacebookClient _facebookClient;

        public FacebookService(IFacebookClient facebookClient) {
            _facebookClient = facebookClient;
        }

        public async Task<Account> GetAccountAsync(string accessToken) {
            var result = await _facebookClient.GetAsync<dynamic>(
                accessToken, "me", "fields=id,name,email,first_name,last_name,age_range,birthday,gender,locale");

            if (result == null) {
                return new Account();
            }

            var account = new Account {
                Id = result.id,
                Email = result.email,
                Name = result.name,
                UserName = result.username,
                FirstName = result.first_name,
                LastName = result.last_name,
                Locale = result.locale
            };

            return account;
        }

        public async Task<List<FacebookFriend>> GetFacebookFriendsAsync(string accessToken) {
            var result = await _facebookClient.GetAsync<dynamic>(
                accessToken, "me/friends", "");

            if (result == null) {
                return new List<FacebookFriend>();
            }

            string data = result["data"].ToString();
            var facebookFriends = JsonConvert.DeserializeObject<List<FacebookApi.Models.FacebookFriend>>(data);

            return facebookFriends;
        }

        public async Task PostOnWallAsync(string accessToken, string message)
            => await _facebookClient.PostAsync(accessToken, "me/feed", new { message });
    }
}
