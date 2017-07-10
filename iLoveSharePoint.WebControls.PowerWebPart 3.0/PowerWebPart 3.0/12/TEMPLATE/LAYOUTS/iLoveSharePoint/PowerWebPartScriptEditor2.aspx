<%@ Page Language="C#"  %>
<html>
<head>
    <title>PowerWebPart Script Editor</title>
</head>
<body>
    <form runat="server">
        <table border="0" width="100%" >
        <tr>
            <td><textarea id="scriptTextBox" name="scriptTextBox" cols="30" rows="28" style="width:100%;" runat="server" onkeydown="allowTabCharacter()"></textarea></td>
        </tr>
        <tr>
           <td><input type="button" id="saveButton" name="saveButton"  value="Apply" onclick="javascript:save()"/> 
                <input type="button" id="closeButton" name="cancelButton" onclick="javascript:window.close();" value="Close" />     
           </td>       
        </tr>
        </table>
    </form>
    <script language="javascript" type="text/javascript">
        var parentTargetElementId = getQueryString("elementId");
        var applyButtonId = getQueryString("applyButtonId");
        var textBox = document.getElementById('scriptTextBox');
        textBox.value = opener.document.getElementById(getQueryString("elementId")).value;
    
        function save(){
            var targetElement = opener.document.getElementById(parentTargetElementId);
            targetElement.value = textBox.value;            
            var applyBtn = opener.document.getElementById(applyButtonId);
            applyBtn.click();
            window.close();
        }
        
        function getQueryString(queryString) {
            hu = window.location.search.substring(1);
            gy = hu.split("&");
            for (i=0;i<gy.length;i++) {
                ft = gy[i].split("=");
                if (ft[0] == queryString) {
                return ft[1];
                }
            }
        }
        
        function allowTabCharacter() {   
            if (event != null) {      
                if (event.srcElement) {         
                    if (event.srcElement.value) {            
                        if (event.keyCode == 9) {  
                            // tab character               
                            if (document.selection != null) {                  
                                document.selection.createRange().text = '\t';                  
                                event.returnValue = false;               }               
                            else {                  
                                event.srcElement.value += '\t';                  
                            return false;               
                            }           
                        }          
                    }      
                }   
            
            }
        }   
        
    </script>
</body>
</html>

