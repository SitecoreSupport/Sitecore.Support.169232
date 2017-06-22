using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using TweetSharp;

namespace Sitecore.Support.Social.Twitter.Networks.Providers
{
  internal static class TwitterServiceHelper
  {
    internal static Sitecore.Support.Social.TwitterUser GetExtendedUserProfile(this TwitterService twitterService)
    {
      var method = typeof(TwitterService).GetMethod("WithHammock", BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(String), typeof(object[]) }, null)
          .MakeGenericMethod(typeof(Sitecore.Support.Social.TwitterUser));

      return method.Invoke(twitterService, new object[] { "account/verify_credentials", new object[] { ".json", "?include_entities=", false, "&include_email=true&skip_status=", true } }) as Sitecore.Support.Social.TwitterUser;

    }
  }
}