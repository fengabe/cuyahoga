using System;
using System.Web;
using System.Web.Security;
using System.Web.Caching;

using log4net;

using Cuyahoga.Core;
using Cuyahoga.Core.Service;
using Cuyahoga.Core.Domain;
using Cuyahoga.Core.Util;
using Cuyahoga.Core.Security;

namespace Cuyahoga.Web.HttpModules
{
	/// <summary>
	/// HttpModule to extend Forms Authentication. When a user logs in, the profile is loaded and put 
	/// in the cache with a simple key containing 'USER_' + userId.
	/// TODO: move non-httpmodule methods to another class?
	/// </summary>
	public class AuthenticationModule : IHttpModule
	{
		private const string USER_CACHE_PREFIX = "User_";
		private const int AUTHENTICATION_TIMEOUT = 20;
		private static readonly ILog log = LogManager.GetLogger(typeof(AuthenticationModule));

		public AuthenticationModule()
		{
		}

		public void Init(HttpApplication context)
		{
			context.AuthenticateRequest += new EventHandler(Context_AuthenticateRequest);
		}

		public void Dispose()
		{
			// Nothing here	
		}

		/// <summary>
		/// Try to authenticate the user. If authentication succeeds, an instance of the Cuyahoga user is 
		/// cached for future usage.
		/// </summary>
		/// <param name="username"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		public bool AuthenticateUser(string username, string password, bool persistLogin)
		{
			CoreRepository cr = (CoreRepository)HttpContext.Current.Items["CoreRepository"];
			string hashedPassword = Encryption.StringToMD5Hash(password);
			try
			{
				User user = cr.GetUserByUsernameAndPassword(username, hashedPassword);
				if (user != null)
				{
					if (! user.IsActive)
					{
						log.Warn(String.Format("Inactive user {0} tried to login.", user.UserName));
						throw new AccessForbiddenException("The account is disabled.");
					}
					user.IsAuthenticated = true;
					string currentIp = HttpContext.Current.Request.UserHostAddress;
					user.LastLogin = DateTime.Now;
					user.LastIp = currentIp;
					// Save login date and IP
					cr.UpdateObject(user);
					// Create the authentication ticket
					HttpContext.Current.User = new CuyahogaPrincipal(user);
					FormsAuthenticationTicket ticket = 
						new FormsAuthenticationTicket(1, user.Name, DateTime.Now, DateTime.Now.AddMinutes(AUTHENTICATION_TIMEOUT), persistLogin, "");
					string cookiestr = FormsAuthentication.Encrypt(ticket);
					HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, cookiestr);                
					if (persistLogin)
					{
						cookie.Expires = DateTime.Now.AddYears(1);
					}
					HttpContext.Current.Response.Cookies.Add(cookie);
					// Finally cache the user
					CacheUser(HttpContext.Current, user);
					return true;
				}
				else
				{
					log.Warn(String.Format("Invalid username-password combination: {0}:{1}.", user.UserName, password));
					return false;
				}
			}
			catch (Exception ex)
			{
				log.Error(String.Format("An error occured while logging in user {0}.", username));
				throw new Exception(String.Format("Unable to log in user '{0}': " + ex.Message, username), ex);
			}
		}

		/// <summary>
		/// Log out the current user and remove the instance from the cache.
		/// </summary>
		public void Logout()
		{
			if (HttpContext.Current.User != null && HttpContext.Current.User.Identity.IsAuthenticated)
			{
				string cacheIdentifier = USER_CACHE_PREFIX + HttpContext.Current.User.Identity.Name;
				HttpContext.Current.Cache.Remove(cacheIdentifier);
				FormsAuthentication.SignOut();
			}
		}

		private void CacheUser(HttpContext context, User user)
		{
			string cacheIdentifier = USER_CACHE_PREFIX + user.Id.ToString();
			context.Cache.Insert(cacheIdentifier, user, null, DateTime.MaxValue, new TimeSpan(0, AUTHENTICATION_TIMEOUT, 0));
		}

		private void Context_AuthenticateRequest(object sender, EventArgs e)
		{
			HttpApplication app = (HttpApplication)sender;
			if (app.Context.User != null && app.Context.User.Identity.IsAuthenticated)
			{
				CoreRepository cr = (CoreRepository)HttpContext.Current.Items["CoreRepository"];
				// There is a logged-in user with a standard Forms Identity. Replace it with
				// the cached Cuyahoga identity (the User class implements IIdentity). 
				string cacheIdentifier = USER_CACHE_PREFIX + app.Context.User.Identity.Name;
				if (app.Context.Cache[cacheIdentifier] == null)
				{
					// For some reason the user is still logged-in but the cuyahoga User instance was removed 
					// from the cache (for instance when the process is recycled). Fetch a new instance and cache it.
					int userId = Int32.Parse(app.Context.User.Identity.Name);
					if (HttpContext.Current.Items["CoreRepository"] != null)
					{
						User user = (User)cr.GetObjectById(typeof(User), userId);
						user.IsAuthenticated = true;
						CacheUser(app.Context, user);
					}
				}
				User cuyahogaUser = (User)app.Context.Cache[cacheIdentifier];
				// Attach the user to the current session.
				cr.AttachObjectToCurrentSession(cuyahogaUser);
				// Set the user context for the application.
				app.Context.User = new CuyahogaPrincipal(cuyahogaUser);
			}
		}
	}
}