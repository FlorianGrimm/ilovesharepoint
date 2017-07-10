using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.SharePoint;
using System.Web;
using System.Management.Automation.Runspaces;
using System.Management.Automation;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security;
using System.Reflection;
using Microsoft.SharePoint.Administration;

namespace iLoveSharePoint.EventReceivers
{
    public class ConfigurePowerEventReceiver : Page
    {
        protected TextBox scriptBox;
        protected TextBox sequenceNumber;
        protected Button saveButton;
        protected Button cancelButton;

        protected SPWeb web;
        protected SPList list;
        protected SPFeature feature;
        protected SPFeatureProperty scriptProperty;
        protected SPFeatureProperty sequenceProperty;

        protected PowerEventType eventType;
        protected string eventDefinitionType;

        protected string propNameScript;
        protected string propNameSequence;
        protected string targetName;
        protected string redirectUrl;

        protected override void OnInit(EventArgs e)
        {
            if(SPFarm.Local.CurrentUserIsAdministrator()==false)
                throw new SecurityException("Access Denied! Current user is not a farm administrator.");

            base.OnInit(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            eventType = (PowerEventType)Enum.Parse(typeof(PowerEventType),Request["Type"]);

            web = SPContext.Current.Web;
            list = SPContext.Current.List;
            feature = web.Features[PowerEventReceiversConstants.FeatureId];

            switch (eventType)
            {
                case PowerEventType.Item:
                    propNameScript = PowerEventReceiversConstants.PowerItemEventReceiverPropNamePrefixScript + list.RootFolder.Url;
                    propNameSequence = PowerEventReceiversConstants.PowerListEventReceiverPropNamePrefixSequence + list.RootFolder.Url;
                    eventDefinitionType = typeof(PowerItemEventReceiver).FullName;
                    targetName = list.Title;
                    redirectUrl = list.ParentWeb.Url + "/_layouts/listedit.aspx?List=" + HttpUtility.UrlEncode(list.ID.ToString());
                    break;
                case PowerEventType.List:
                    propNameScript = PowerEventReceiversConstants.PowerListEventReceiverPropNamePrefixScript + list.RootFolder.Url;
                    propNameSequence = PowerEventReceiversConstants.PowerListEventReceiverPropNamePrefixSequence + list.RootFolder.Url;
                    eventDefinitionType = typeof(PowerListEventReceiver).FullName;
                    targetName = list.Title;
                    redirectUrl = list.ParentWeb.Url + "/_layouts/listedit.aspx?List=" + HttpUtility.UrlEncode(list.ID.ToString());
                    break;
                case PowerEventType.Web:
                    propNameScript = PowerEventReceiversConstants.PowerWebEventReceiverPropNamePrefixScript;
                    propNameSequence = PowerEventReceiversConstants.PowerWebEventReceiverPropNamePrefixSequence + web.ID.ToString();
                    eventDefinitionType = typeof(PowerWebEventReceiver).FullName;
                    targetName = web.Title;
                    redirectUrl = web.Url + "/_layouts/settings.aspx";
                    break;
                default:
                    throw new Exception("Unknown event type!");
            }     
            
            scriptProperty = feature.Properties[propNameScript];
            sequenceProperty = feature.Properties[propNameSequence];

            if (web.CurrentUser.IsSiteAdmin == false)
                throw new SecurityException();

            if (IsPostBack == false)
            {
                if (scriptProperty != null)
                    scriptBox.Text = scriptProperty.Value;
                else
                {
                    switch (eventType)
                    {
                        case PowerEventType.Item:
                            scriptBox.Text = PowerEventReceiversConstants.PowerItemEventReceiverScriptTemplate;
                            break;
                        case PowerEventType.List:
                            scriptBox.Text = PowerEventReceiversConstants.PowerListEventReceiverScriptTemplate;
                            break;
                        case PowerEventType.Web:
                            scriptBox.Text = PowerEventReceiversConstants.PowerWebEventReceiverScriptTemplate;
                            break;

                        default:
                            throw new Exception("Unknown event type!");
                    }
                    
                }

                if (sequenceProperty != null)
                    sequenceNumber.Text = sequenceProperty.Value;
            }

            saveButton.Click += new EventHandler(saveButton_Click);
            cancelButton.Click += new EventHandler(cancelButton_Click);
        }

        void cancelButton_Click(object sender, EventArgs e)
        {
            Response.Redirect(redirectUrl, true);
        }

        void saveButton_Click(object sender, EventArgs e)
        {
            if (SPFarm.Local.CurrentUserIsAdministrator() == false)
                throw new SecurityException("Access Denied! Current user is not a farm administrator.");

            if (scriptProperty == null)
            {
                scriptProperty = new SPFeatureProperty(propNameScript, scriptBox.Text);
                feature.Properties.Add(scriptProperty);
            }
            else
            {
                scriptProperty.Value = scriptBox.Text;
            }

            if (sequenceProperty == null)
            {
                sequenceProperty = new SPFeatureProperty(propNameSequence, sequenceNumber.Text);
                feature.Properties.Add(sequenceProperty);
            }
            else
            {
                sequenceProperty.Value = sequenceNumber.Text;  
            }

            feature.Properties.Update();

            //clean power event receivers
            List<SPEventReceiverDefinition> receiversToDelete = new List<SPEventReceiverDefinition>();

            SPEventReceiverDefinitionCollection receivers = null;
            if (eventType == PowerEventType.Item || eventType == PowerEventType.List)
            {
                receivers = list.EventReceivers;
            }
            else
            {
                receivers = web.EventReceivers;
            }
            
            foreach (SPEventReceiverDefinition receiver in receivers)
            {
                if (receiver.Class == typeof(PowerItemEventReceiver).FullName)
                {
                    receiversToDelete.Add(receiver);
                }
            }

            foreach (SPEventReceiverDefinition receiver in receiversToDelete)
            {
                receiver.Delete();
            }

            if (!String.IsNullOrEmpty(sequenceNumber.Text))
            {
                Runspace runspace = null;
                try
                {
                    runspace = RunspaceFactory.CreateRunspace();
                    runspace.Open();
                    Pipeline pipe = runspace.CreatePipeline(scriptBox.Text);
                    pipe.Invoke();

                    pipe = runspace.CreatePipeline("get-childitem function:\\");
                    Collection<PSObject> results = pipe.Invoke();

                    string[] receiverTypes = Enum.GetNames(typeof(SPEventReceiverType));

                    foreach (PSObject obj in results)
                    {
                        FunctionInfo func = (FunctionInfo)obj.BaseObject;

                        if (receiverTypes.Contains(func.Name))
                        {
                            SPEventReceiverDefinition eventReceiverDef = null;
                            if (eventType == PowerEventType.Web)
                            {
                               eventReceiverDef = web.EventReceivers.Add();
                            }
                            else
                            {
                                eventReceiverDef = list.EventReceivers.Add();
                            }
                                
                            eventReceiverDef.Assembly = Assembly.GetExecutingAssembly().FullName;
                            eventReceiverDef.Class = eventDefinitionType;
                            eventReceiverDef.Type = (SPEventReceiverType)Enum.Parse(typeof(SPEventReceiverType), func.Name);
                            eventReceiverDef.SequenceNumber = int.Parse(sequenceNumber.Text);
                            eventReceiverDef.Update();
                        }

                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (runspace != null && runspace.RunspaceStateInfo.State != RunspaceState.Closed)
                    {
                        runspace.Close();
                        runspace = null;
                    }
                }
            }

            Response.Redirect(redirectUrl, true);
        }
    }
}
