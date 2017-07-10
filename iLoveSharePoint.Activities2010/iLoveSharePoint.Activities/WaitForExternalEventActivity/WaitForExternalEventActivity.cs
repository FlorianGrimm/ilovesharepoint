using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Workflow.Activities;
using System.Workflow.ComponentModel;

namespace ILoveSharePoint.Workflow.Activities
{
    public class WaitForExternalEventActivity : SequenceActivity
    {
        private CallExternalMethodActivity initializeCorrelation;
        private HandleExternalEventActivity handleExternalEvent;
        public static System.Workflow.ComponentModel.DependencyProperty CorrelationIdentifierProperty = DependencyProperty.Register("CorrelationIdentifier", typeof(System.String), typeof(ILoveSharePoint.Workflow.Activities.WaitForExternalEventActivity));
        public static System.Workflow.ComponentModel.DependencyProperty ExternalEventArgsProperty = DependencyProperty.Register("ExternalEventArgs", typeof(ILoveSharePoint.Workflow.Activities.ExternalEventArgs), typeof(ILoveSharePoint.Workflow.Activities.WaitForExternalEventActivity));
        public static System.Workflow.ComponentModel.DependencyProperty ExternalDataProperty = DependencyProperty.Register("ExternalData", typeof(String), typeof(ILoveSharePoint.Workflow.Activities.WaitForExternalEventActivity));

        public WaitForExternalEventActivity()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.CanModifyActivities = true;
            System.Workflow.Runtime.CorrelationToken correlationtoken1 = new System.Workflow.Runtime.CorrelationToken();
            System.Workflow.ComponentModel.ActivityBind activitybind1 = new System.Workflow.ComponentModel.ActivityBind();
            System.Workflow.ComponentModel.WorkflowParameterBinding workflowparameterbinding1 = new System.Workflow.ComponentModel.WorkflowParameterBinding();
            System.Workflow.Runtime.CorrelationToken correlationtoken2 = new System.Workflow.Runtime.CorrelationToken();
            System.Workflow.ComponentModel.ActivityBind activitybind2 = new System.Workflow.ComponentModel.ActivityBind();
            System.Workflow.ComponentModel.WorkflowParameterBinding workflowparameterbinding2 = new System.Workflow.ComponentModel.WorkflowParameterBinding();
            this.handleExternalEvent = new System.Workflow.Activities.HandleExternalEventActivity();
            this.initializeCorrelation = new System.Workflow.Activities.CallExternalMethodActivity();
            // 
            // handleExternalEvent
            // 
            correlationtoken1.Name = "token";
            correlationtoken1.OwnerActivityName = "WaitForExternalEventActivity";
            this.handleExternalEvent.CorrelationToken = correlationtoken1;
            this.handleExternalEvent.EventName = "OnExternalEvent";
            this.handleExternalEvent.InterfaceType = typeof(ILoveSharePoint.Workflow.Activities.IWaitForExternalEventService);
            this.handleExternalEvent.Name = "handleExternalEvent";
            activitybind1.Name = "WaitForExternalEventActivity";
            activitybind1.Path = "ExternalEventArgs";
            workflowparameterbinding1.ParameterName = "e";
            workflowparameterbinding1.SetBinding(System.Workflow.ComponentModel.WorkflowParameterBinding.ValueProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind1)));
            this.handleExternalEvent.ParameterBindings.Add(workflowparameterbinding1);
            this.handleExternalEvent.Invoked += new System.EventHandler<System.Workflow.Activities.ExternalDataEventArgs>(this.Invoked);
            this.handleExternalEvent.InterfaceType = typeof(ILoveSharePoint.Workflow.Activities.IWaitForExternalEventService);
            // 
            // initializeCorrelation
            // 
            correlationtoken2.Name = "token";
            correlationtoken2.OwnerActivityName = "WaitForExternalEventActivity";
            this.initializeCorrelation.CorrelationToken = correlationtoken2;
            this.initializeCorrelation.InterfaceType = typeof(ILoveSharePoint.Workflow.Activities.IWaitForExternalEventService);
            this.initializeCorrelation.MethodName = "Initialize";
            this.initializeCorrelation.Name = "initializeCorrelation";
            activitybind2.Name = "WaitForExternalEventActivity";
            activitybind2.Path = "CorrelationIdentifier";
            workflowparameterbinding2.ParameterName = "correlationToken";
            workflowparameterbinding2.SetBinding(System.Workflow.ComponentModel.WorkflowParameterBinding.ValueProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind2)));
            this.initializeCorrelation.ParameterBindings.Add(workflowparameterbinding2);
            this.initializeCorrelation.InterfaceType = typeof(ILoveSharePoint.Workflow.Activities.IWaitForExternalEventService);
            // 
            // WaitForExternalEventActivity
            // 
            this.Activities.Add(this.initializeCorrelation);
            this.Activities.Add(this.handleExternalEvent);
            this.Name = "WaitForExternalEventActivity";
            this.CanModifyActivities = false;

        }

        [System.ComponentModel.DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
        [System.ComponentModel.BrowsableAttribute(true)]
        [System.ComponentModel.CategoryAttribute("Parameters")]
        public string CorrelationIdentifier
        {
            get
            {
                return ((string)(base.GetValue(ILoveSharePoint.Workflow.Activities.WaitForExternalEventActivity.CorrelationIdentifierProperty)));
            }
            set
            {
                base.SetValue(ILoveSharePoint.Workflow.Activities.WaitForExternalEventActivity.CorrelationIdentifierProperty, value);
            }
        }

        [System.ComponentModel.DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
        [System.ComponentModel.BrowsableAttribute(true)]
        [System.ComponentModel.CategoryAttribute("Parameters")]
        public ExternalEventArgs ExternalEventArgs
        {
            get
            {
                return ((ILoveSharePoint.Workflow.Activities.ExternalEventArgs)(base.GetValue(ILoveSharePoint.Workflow.Activities.WaitForExternalEventActivity.ExternalEventArgsProperty)));
            }
            set
            {
                base.SetValue(ILoveSharePoint.Workflow.Activities.WaitForExternalEventActivity.ExternalEventArgsProperty, value);
            }
        }

        [System.ComponentModel.DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
        [System.ComponentModel.BrowsableAttribute(true)]
        [System.ComponentModel.CategoryAttribute("Parameters")]
        public string ExternalData
        {
            get
            {
                return ((string)(base.GetValue(ILoveSharePoint.Workflow.Activities.WaitForExternalEventActivity.ExternalDataProperty)));
            }
            set
            {
                base.SetValue(ILoveSharePoint.Workflow.Activities.WaitForExternalEventActivity.ExternalDataProperty, value);
            }
        }

        private void Invoked(object sender, ExternalDataEventArgs e)
        {
            ExternalData = ExternalEventArgs.Data;
        }

    }
}
