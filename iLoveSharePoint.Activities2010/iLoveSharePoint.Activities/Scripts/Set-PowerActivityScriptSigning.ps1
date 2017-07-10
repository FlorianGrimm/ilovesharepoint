param($signingRequired=$(throw '-signingRequired is mandatory'))

[void][System.Reflection.Assembly]::LoadWithPartialName('ILoveSharePoint.Workflow.Activities')

[ILoveSharePoint.Workflow.Activities.Helper]::SetPowerActivityScriptSingningRequired($signingRequired)

"PowerActivity Script Signing is now set to $signingRequired"  | out-host