
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Ninject;
using Sitecore.Diagnostics;
using Sitecore.Social.Infrastructure;
using Sitecore.Social.Infrastructure.Logging;
using Sitecore.Social.NetworkProviders;
using Sitecore.Social.NetworkProviders.Interfaces;
using Sitecore.Social.NetworkProviders.NetworkFields;
using TweetSharp;

using FieldInfo = Sitecore.Social.NetworkProviders.NetworkFields.FieldInfo;

namespace Sitecore.Support.Social.Twitter.Networks.Providers
{
  public class TwitterProvider : Sitecore.Social.Twitter.Networks.Providers.TwitterProvider, IGetAccountInfo
  {
    public TwitterProvider(Application application) : base(application)
    {
      this.twitterService = typeof(Sitecore.Social.Twitter.Networks.Providers.TwitterProvider).GetField("twitterService", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(this) as TwitterService;
    }

    AccountBasicData IGetAccountInfo.GetAccountBasicData(Account account)
    {
      Sitecore.Support.Social.TwitterUser twitterUserProfile = this.GetTwitterUserProfile(account);
      string str = twitterUserProfile?.Id.ToString(CultureInfo.InvariantCulture);
      string screenName = twitterUserProfile?.ScreenName;
      return new AccountBasicData
      {
        Account = account,
        Id = str,
        FullName = screenName,
        Email = twitterUserProfile?.Email
      };


    }

    private Sitecore.Support.Social.TwitterUser GetTwitterUserProfile(Account account)
    {
      this.twitterService.AuthenticateWith(account.AccessToken, account.AccessTokenSecret);
      return this.twitterService.GetExtendedUserProfile();
    }

    string IGetAccountInfo.GetAccountId(Account account)
    {
      return this.GetTwitterUserProfile(account).Id.ToString(CultureInfo.InvariantCulture);
    }

    IEnumerable<Field> IGetAccountInfo.GetAccountInfo(Account account, IEnumerable<FieldInfo> acceptedFields)
    {
      Assert.IsNotNull(acceptedFields, "AcceptedFields collection must be filled");
      var userProfile = this.GetTwitterUserProfile(account);
      if (userProfile != null)
      {
        var result = new List<Field>();

        foreach (var acceptedField in acceptedFields.Where(acceptedField => !string.IsNullOrEmpty(acceptedField.OriginalKey)))
        {
          PropertyInfo propertyInfo = userProfile.GetType().GetProperty(acceptedField.OriginalKey);

          object value = null;

          if (propertyInfo == null)
          {
            ExecutingContext.Current.IoC.Get<ILogManager>().LogMessage(
            string.Format(CultureInfo.CurrentCulture, "There is no field \"{0}\" in a Sitecore.Support.Social.TwitterUser object", acceptedField.OriginalKey),
            LogLevel.Warn,
            this);

            continue;
          }

          if ((value = propertyInfo.GetValue(userProfile, null)) == null)
          {
            continue;
          }

          result.Add(new Field
          {
            Name = this.GetFieldSitecoreKey(acceptedField),
            Value = value is DateTime ? ((DateTime)value).ToString(CultureInfo.InvariantCulture) : value.ToString()
          });
        }

        return result;
      }

      return null;
    }

    string IGetAccountInfo.GetDisplayName(Account account)
    {
      return this.GetTwitterUserProfile(account).ScreenName;
    }

    private readonly TwitterService twitterService;
  }
}