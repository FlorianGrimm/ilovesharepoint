var powerGuiLauncher = null;
var powerGuiLauncherEditor = null;
var scriptElement = null;
var applyButton = null;
var needsRefresh = null;

function initializePowerGuiLauncher() {

    try {

        applyButton = document.getElementById(applyButtonElementId);
        scriptElement = document.getElementById(scriptElementId);
        scriptElementEditor = document.getElementById(scriptEditorElementId);
        needsRefresh = document.getElementById(needsRefreshElementId);

        if(needsRefresh.value=='true')
        {
            needsRefresh.value = 'false';
            document.forms[0].submit();
        }

        powerGuiLauncher = new ActiveXObject("iLoveSharePoint.PowerGuiLauncher");
        powerGuiLauncherEditor = new ActiveXObject("iLoveSharePoint.PowerGuiLauncher");
    }
    catch (ex) {
    }

    if (powerGuiLauncher != null) {

        powerGuiLauncher.OnScriptChanged = onScriptChanged;
        powerGuiLauncher.ScriptName = "PowerWebPart";
        powerGuiLauncher.Initialize();

        powerGuiLauncherEditor.OnScriptChanged = onEditorScriptChanged;
        powerGuiLauncherEditor.ScriptName = "PowerWebPartEditor";
        powerGuiLauncherEditor.Initialize();

        var launcherSpan = document.getElementById('btnPowerGuiLauncher');
        if(launcherSpan!=null)launcherSpan.style.display = '';

        var launcherSpanEditor = document.getElementById('btnPowerGuiLauncher4Editor');
        if (launcherSpanEditor != null) launcherSpanEditor.style.display = '';
    }
}

function onScriptChanged(obj, script) {
    scriptElement.value = script;
    needsRefresh.value = 'true';
    applyButton.click();
}

function onEditorScriptChanged(obj, script) {
    scriptElementEditor.value = script;
    needsRefresh.value = 'true';
    applyButton.click();
}

function startPowerGuiLaucher(elementId, applyButtonId) {
    if (powerGuiLauncher != null) {
        powerGuiLauncher.StartPowerGui(scriptElement.value);
    }
}

function startPowerGuiLaucherEditor(elementId, applyButtonId) {
    if (powerGuiLauncherEditor != null) {
        powerGuiLauncherEditor.StartPowerGui(scriptElementEditor.value);
    }
}

function disposePowerGuiLauncher() {
    if (powerGuiLauncher != null)
        powerGuiLauncher.Dispose();
        
    if (powerGuiLauncherEditor != null)
        powerGuiLauncherEditor.Dispose();
}

function pingDebugConsole(elementId) {

    var element = document.getElementById(elementId);
    $.ajax({
        type: "GET",
        cache: false,
        async: false,
        url: "/_layouts/iLoveSharePoint/PowerWebPartDebugPing.aspx",
        data: "debugUrl=" + element.value,
        success: function(data) { alert(data); }
    });
}

function setToClientIP(elementId, ip) {
    var url = "http://" + ip + ":8777";
    var element = document.getElementById(elementId);
    element.value = url;
}

_spBodyOnLoadFunctionNames.push("initializePowerGuiLauncher");
window.onunload = disposePowerGuiLauncher; 