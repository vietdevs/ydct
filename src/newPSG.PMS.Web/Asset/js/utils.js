function openPopup(url,width,height){
    var newWindow = open(url, '_blank', 'toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=yes,resizable=yes,copyhistory=yes,width='+width+',height='+height+',left='+100+', top='+100+',screenX='+100+',screenY='+100+'',false);
}

function function_exists(func_name)
{
    var eval_string;
    if (window.opener)
    {
        eval_string = 'window.opener.' + func_name;
    }
    else
    {
        eval_string = 'window.' + func_name;
    }
    return eval(eval_string + ' ? true : false');
}

function openModalPopup(url,width,height) {
    if (window.showModalDialog) {
        window.showModalDialog(url,'_blank','dialogWidth:' + width + ';dialogHeight:' +height);
    }
    else {
        var newWindow = open(url, '_blank', 'toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=yes,resizable=yes,copyhistory=yes,width='+width+',height='+height+',left='+100+', top='+100+',screenX='+100+',screenY='+100+'',false);
    }
}

// Disable window
function showWindowDisabled() {
    var winDiv = document.getElementById('windowDisabled');
    winDiv.style.width = screen.width;
    winDiv.style.height = screen.height;
    winDiv.style.display = '';
    return true;
}

// Enable window
function hideWindowDisabled() {
    document.getElementById('windowDisabled').style.display = 'none';
    return true;
}
function showHide(obj, img_url, name1, name2, areaID) {
    if (obj.src.lastIndexOf(name1) == -1)
    {
        obj.src = img_url + name1;
        document.getElementById(areaID).style.display = 'none';
    }
    else
    {
        obj.src = img_url + name2;
        document.getElementById(areaID).style.display='';
    }
}

// Format number, add Commas
function addCommas(nStr) {
    nStr += '';
    x = nStr.split('.');
    x1 = x[0];
    x2 = x.length > 1 ? '.' + x[1] : '';
    var rgx = /(\d+)(\d{3})/;
    while (rgx.test(x1)) {
        x1 = x1.replace(rgx, '$1' + ',' + '$2');
    }
    return x1 + x2;
}

// Show formatted number
function showFormatNumber(obj)
{
    if(obj != null && obj.value != null)
    {
        var nStr = trimAll(obj.value);
        if(nStr != '')
        {
           Tip(addCommas(nStr), FIX, [obj, 0, 0], ABOVE, true);
        }
    }
}

// open fullScreen window
function openFullPopup(url,width,height){
    var newWindow = open(url, '_blank', 'toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=yes,resizable=yes,copyhistory=yes,width='+width+',height='+height+',left='+0+', top='+0+',screenX='+100+',screenY='+100+'',false);
}

//trungnt add 5/5/2009

/*function to enabale,disable content and all childs */
function toggleDisabled(el) {
    try
    {
        el.disabled = !el.disabled;
        if (el.childNodes && el.childNodes.length > 0) {
            for (var x = 0; x < el.childNodes.length; x++)
            {
                toggleDisabled(el.childNodes[x]);
            }
        }
    }
    catch(E){
    }
}

String.format = function( text )

{

    //check if there are two arguments in the arguments list

    if ( arguments.length <= 1 )

    {

        //if there are not 2 or more arguments there’s nothing to replace

        //just return the original text

        return text;

    }

    //decrement to move to the second argument in the array

    var tokenCount = arguments.length - 2;

    for( var token = 0; token <= tokenCount; token++ )

    {

        //iterate through the tokens and replace their placeholders from the original text in order

        text = text.replace( new RegExp( "\\{" + token + "\\}", "gi" ),

                                                arguments[ token + 1 ] );

    }

    return text;

};

function openModalPopupWindow(url,width,height)
{
    popUpObj=window.open(url,
    "ModalPopUp",
    "toolbar=no," +
    "scrollbars=yes," +
    "location=yes," +
    "statusbar=no," +
    "menubar=no," +
    "resizable=0," +
    "width=" +width +",",
    "height=" + height + ",",
    "left = 490," +
    "top=300"
    );
    popUpObj.focus();
    LoadModalDiv();
}
function LoadModalDiv()
{
    var bcgDiv = document.getElementById("divBackground");
    bcgDiv.style.display="block";
    if (bcgDiv != null)
    {
        if (document.body.clientHeight > document.body.scrollHeight)
        {
            bcgDiv.style.height = document.body.clientHeight + "px";
        }
        else
        {
            bcgDiv.style.height = document.body.scrollHeight + "px" ;
        }
        bcgDiv.style.width = "100%";
    }
}
function HideModalDiv()
 {
    var bcgDiv = document.getElementById("divBackground");
    bcgDiv.style.display="none";
 }




/**
 * Xu ly nut Check all.
 * @param tableId ID cua bang
 * @param checkName Ten cua cac checkbox
 * @param checkAll Doi tuong Check all, ta co the truyen vao this
 * @return void
 * @author HuyenNV
 */
function tctProcessCheckAll(tableId, checkName, checkAll) {
    var myTable = document.getElementById(tableId);
    var inputs = myTable.getElementsByTagName('input');
    for (var i = 0; i < inputs.length; i++) {
        if (inputs[i].name.indexOf(checkName) >= 0) {
            inputs[i].checked = checkAll.checked;
        }
    }
}

/**
 * Kiem tra xem cac check box co duoc chon hay khong.
 * @param tableId ID cua bang
 * @param checkName Ten cua cac checkbox
 * @param message Thong bao khi chua chon checkbox nao
 * @return true neu co it nhat 1 checkbox duoc chon, false neu khong co checkbox nao duoc chon
 * @author HuyenNV
 */
function tctCheckSelected(tableId, checkName, message) {
    var found = false;
    var myTable = document.getElementById(tableId);
    var inputs = myTable.getElementsByTagName('input');
    for (var i = 0; i < inputs.length; i++) {
        if ((inputs[i].name.indexOf(checkName) >= 0) && inputs[i].checked) {
            found = true;
            break;
        }
    }
    if (!found) {
        alert((message == undefined) ? "Bạn chưa chọn checkbox nào!" : message);
        return false;
    } else {
        return true;
    }
}

function changePageTitle(title){
	document.title = title;
	if(document.getElementById("myHeader") != null){
		document.getElementById("myHeader").innerHTML = title;
	}
}