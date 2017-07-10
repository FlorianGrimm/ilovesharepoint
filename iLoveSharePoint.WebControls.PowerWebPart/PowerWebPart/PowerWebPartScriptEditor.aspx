<%@ Page Language="C#"  %>
<html>
<head>
    <title>PowerWebPart Script Editor</title>
</head>
<body>
    <form runat="server">
        <table border="0" width="100%" >
        <tr>
            <td><textarea id="scriptTextBox" name="scriptTextBox" cols="30" rows="28" style="width:100%;" runat="server"></textarea></td>
        </tr>
        <tr>
           <td><input type="button" id="saveButton" name="saveButton"  value="Okay" onclick="javascript:save()"/> 
                <input type="button" id="closeButton" name="cancelButton" onclick="javascript:window.close();" value="Cancel" />     
           </td>       
        </tr>
        </table>
    </form>
    <script language="javascript" type="text/javascript">
        var parentTargetElementId = getQueryString("elementId");
        var parentFormName =getQueryString("formName");
    
        function save()
        {
            var targetElement = opener.document.getElementById(parentTargetElementId);
            targetElement.value = document.getElementById('scriptTextBox').value;            
            opener.document.forms[parentFormName].MSOTlPn_Button.value='apply';
            opener.document.forms[parentFormName].submit();  
            window.close();
        }
        
        function getQueryString(ji) {
            hu = window.location.search.substring(1);
            gy = hu.split("&");
            for (i=0;i<gy.length;i++) {
                ft = gy[i].split("=");
                if (ft[0] == ji) {
                return ft[1];
                }
            }
        }
        
         document.getElementById('scriptTextBox').value = opener.document.getElementById(getQueryString("elementId")).value;
   
    </script>
</body>
</html>

