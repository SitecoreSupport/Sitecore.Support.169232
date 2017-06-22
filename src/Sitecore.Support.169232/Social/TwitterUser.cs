using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using Newtonsoft.Json;

namespace Sitecore.Support.Social
{
  [Serializable, JsonObject(MemberSerialization.OptIn), DataContract, DebuggerDisplay("{ScreenName}")]
  public class TwitterUser : TweetSharp.TwitterUser
  {
    private string _email;

    [DataMember]
    public virtual string Email
    {
      get
      {
        return
          this._email;
      }
      set
      {
        if (this._email != value)
        {
          this._email = value;
          this.OnPropertyChanged("Email");
        }
      }
    }
  }
}