<%@ Page Language="C#" MasterPageFile="~/_layouts/pickerdialog.master"  %>
<asp:Content ID="Content1" contentplaceholderid="PlaceHolderDialogHeaderPageTitle" runat="server">
		Simple Script Editor
</asp:Content>
<asp:Content ID="Content5" contentplaceholderid="PlaceHolderSiteName" runat="server">
		iLove SharePoint
</asp:Content>
<asp:Content ID="Content3" contentplaceholderid="PlaceHolderDialogTitleInTitleArea" runat="server">
		Simple Script Editor
</asp:Content>
<asp:Content ID="Content6" contentplaceholderid="PlaceHolderHelpLink" runat="server">
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderId="PlaceHolderAdditionalPageHead" runat="server">
<script language="javascript" type="text/javascript" src="/_layouts/iLoveSharePoint/jquery-1.4.2.min.js" ></script>
<script language="javascript" type="text/javascript">

    var targetElement = null;
    var textBox = null;
    var applyButton = null;

    _spBodyOnLoadFunctionNames.push("init");

    function init() {

        $("input[id*='btnOk']").click(save);
        targetElement = opener.document.getElementById(getQueryString("elementId"));
        textBox = document.getElementById("textBox");
        applyButton = opener.document.getElementById(getQueryString("applyButtonId"));
        textBox.value = targetElement.value;
    }

    function save() {
        targetElement.value = textBox.value;
        applyButton.click();
        window.close();
    }

    function getQueryString(queryString) {
        hu = window.location.search.substring(1);
        gy = hu.split("&");
        for (i = 0; i < gy.length; i++) {
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
                            event.returnValue = false;
                        }
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
</asp:Content>

<asp:Content ID="Content4" contentplaceholderid="PlaceHolderDialogBodySection" runat="server">
	 <table border="0" width="100%" >
        <tr>
            <td>
                <textarea id="textBox" name="textBox" cols="30" rows="25" style="width:100%;" onkeydown="allowTabCharacter()"></textarea>
            </td>
        </tr>
      </table>
</asp:Content>
