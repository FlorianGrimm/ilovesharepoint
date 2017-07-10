using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Microsoft.SharePoint.UserCode;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Navigation;

namespace ILoveSharePoint.Sandbox.WorklowActions
{
    public class CreateSiteAction
    {
        public Hashtable CreateSite(SPUserCodeWorkflowContext context, string url, string title, string description, string template, 
            int language, bool uniquePerm, bool inheritNav, bool topNav, bool quickNav)
        {
            Hashtable result = new Hashtable();

            SPSite currentSite = null;
            SPWeb currentWeb = null;
            SPWeb newWeb = null;

            try
            {
                currentSite = new SPSite(context.CurrentWebUrl);
                currentWeb = currentSite.OpenWeb();

                if (language == Int32.MinValue || language == -1)
                {
                    language = currentWeb.Locale.LCID;
                }

                if (String.IsNullOrEmpty(template))
                {
                    template = "STS#0";
                }

                var spTemplate = currentWeb.GetAvailableWebTemplates((uint)language).OfType<SPWebTemplate>().Where(t => t.Name == template).FirstOrDefault();
                if (spTemplate == null)
                {
                    spTemplate = currentWeb.GetAvailableWebTemplates((uint)language).OfType<SPWebTemplate>().Where(t => t.Title == template).FirstOrDefault();
                }


                newWeb = currentWeb.Webs.Add(url, title, description, (uint)language, spTemplate, uniquePerm, false);
                newWeb.Navigation.UseShared = inheritNav;
                newWeb.Update();

                if (topNav)
                {
                    currentWeb.Navigation.TopNavigationBar.AddAsLast(new SPNavigationNode(title, newWeb.Url, false));
                }

                if (quickNav)
                {
                    currentWeb.Navigation.AddToQuickLaunch(new SPNavigationNode(title, newWeb.Url, false), SPQuickLaunchHeading.Sites);
                }               

                result["output"] = newWeb.Url;
                result["error"] = String.Empty;

            }
            catch (Exception ex)
            {
                result["output"] = String.Empty;
                result["error"] = ex.Message;
            }
            finally
            {
                if (newWeb != null) newWeb.Dispose();
                if (currentWeb != null) currentWeb.Dispose();
                if (currentSite != null) currentSite.Dispose();
            }

            return result;
        }

    }
}
