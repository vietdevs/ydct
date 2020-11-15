
var hSession = "";
var process = false;
var certUserBase64 = "";
var signatureBase64 = "";
var lastError = "";

var uploadObj;
jQuery(document).ready(function () {
    uploadObj = jQuery("#fileuploader").uploadFile({
        url: URL_WEB + 'File/Upload',
        fileName: "filePdf",
        //            autoSubmit: false,
        multiple: false,
        maxFileCount: 1,
        allowedTypes: "pdf",
        onSubmit: function (files) {
            //                showProgressing();
            //return false;
        },
        onSuccess: function (files, data, xhr, pd) {
            //files: list of files
            //data: response from server
            //xhr : jquer xhr object
            jQuery("#resultUpload").html(data);
            if (jQuery("#resultUpload input[name=statusResultUpload]").length == 0) {
                alert("Chưa chọn file PDF để ký");
            } else if (jQuery("#resultUpload input[name=statusResultUpload]").val() != "success"
                    || jQuery("#resultUpload input[name=fileName]").val() == undefined
                    || jQuery("#resultUpload input[name=fileName]").val().indexOf(".pdf") == -1) {
                if (jQuery("#resultUpload input[name=descErrorDownloadFile]").val() != undefined
                        && jQuery("#resultUpload input[name=descErrorDownloadFile]").val().length != 0) {
                    alert("Upload file PDF chưa thành công, vui lòng chọn lại file PDF. " + jQuery("#resultUpload input[name=descErrorDownloadFile]").val().trim());
                } else {
                    alert("Upload file PDF chưa thành công, vui lòng chọn lại file PDF");
                }
            }
            hideAllPopup();
        },
        onError: function (files, status, errMsg, pd) {
            hideAllPopup();
            jQuery("#resultUpload").html("");
            alert("Upload file PDF chưa thành công, vui lòng chọn lại file PDF");
        }
    });
    initPlugin();
});

function dec2hex(d) {
    var hD = '0123456789ABCDEF';
    var h = hD.substr(d & 15, 1);
    while (d > 15) {
        d >>= 4;
        h = hD.substr(d & 15, 1) + h;
    }
    return h;
}

function convertBase64ToHexa(stringBase64) {
    var output = base64_decode(stringBase64);
    var separator = "";
    var hexText = "";
    for (i = 0; i < output.length; i++) {
        hexText = hexText + separator + (output[i] < 16 ? "0" : "") + dec2hex(output[i]);
    }
    return hexText;
}

function base64_decode(stringBase64) {
    var keyStr = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/="
    var output = new Array();
    var chr1, chr2, chr3;
    var enc1, enc2, enc3, enc4;
    var i = 0;

    var orig_input = stringBase64;
    stringBase64 = stringBase64.replace(/[^A-Za-z0-9\+\/\=]/g, "");
    if (orig_input != stringBase64)
        alert("Warning! Characters outside Base64 range in input string ignored.");
    if (stringBase64.length % 4) {
        alert("Error: Input length is not a multiple of 4 bytes.");
        return "";
    }

    var j = 0;
    while (i < stringBase64.length) {

        enc1 = keyStr.indexOf(stringBase64.charAt(i++));
        enc2 = keyStr.indexOf(stringBase64.charAt(i++));
        enc3 = keyStr.indexOf(stringBase64.charAt(i++));
        enc4 = keyStr.indexOf(stringBase64.charAt(i++));

        chr1 = (enc1 << 2) | (enc2 >> 4);
        chr2 = ((enc2 & 15) << 4) | (enc3 >> 2);
        chr3 = ((enc3 & 3) << 6) | enc4;

        output[j++] = chr1;
        if (enc3 != 64)
            output[j++] = chr2;
        if (enc4 != 64)
            output[j++] = chr3;

    }
    return output;
}
function initPlugin() {
    var xmlhttp;
    var response = "";
    if (window.XMLHttpRequest) {// code for IE7+, Firefox, Chrome, Opera, Safari
        xmlhttp = new XMLHttpRequest();
    } else {// code for IE6, IE5
        xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");
    }
    xmlhttp.onreadystatechange = function () {
        if (xmlhttp.readyState == 4 && xmlhttp.status == 200) {
            response = xmlhttp.responseText;
            process = false;
            if (response != "") {
                hSession = response;
                return true;
            }
            if (response == "") {
                alert("Vui long cai dat SignPlugin va bam f5 de tiep tuc");
                window.open('<%=request.getContextPath()%>/SignPlugin_Installer.exe');
                return false;
            }
        }
    }
    xmlhttp.open("POST", "http://localhost:8888/getSession", true);
    xmlhttp.send();
    return true;
}

function getLastError(message) {
    lastError = "";
    var ReqLastErr;
    if (window.XMLHttpRequest) {// code for IE7+, Firefox, Chrome, Opera, Safari
        ReqLastErr = new XMLHttpRequest();
    } else {// code for IE6, IE5
        ReqLastErr = new ActiveXObject("Microsoft.XMLHTTP");
    }
    ReqLastErr.onreadystatechange = function () {
        if (ReqLastErr.readyState == 4 && ReqLastErr.status == 200) {
            process = false;
            lastError = ReqLastErr.responseText
            if (message != undefined && message != "") {
                alert(message + "Error code = " + lastError);
            } else {
                alert("Error code = " + lastError);
            }
        }
    }
    ReqLastErr.open("POST", "http://localhost:8888/getLastErr", true);
    ReqLastErr.send();
}


function signHash(hashHex) {
    signatureBase64 = "";
    if (hashHex == "") {
        alert("Lỗi hash");
        return null;
    }

    var xmlhttp;
    if (window.XMLHttpRequest) {// code for IE7+, Firefox, Chrome, Opera, Safari
        xmlhttp = new XMLHttpRequest();
    } else {// code for IE6, IE5
        xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");
    }
    xmlhttp.onreadystatechange = function () {
        if (xmlhttp.readyState == 4 && xmlhttp.status == 200) {

            signatureBase64 = xmlhttp.responseText;

            if (signatureBase64 == null || signatureBase64 == undefined || signatureBase64.trim().length == 0) {
                hideAllPopup();
                alert("Sign Hash không thành công");
                return false;
            }

            var fileName = jQuery("#resultUpload input[name=fileName]").val();
            areaId = "resultInsertSignature";
            jQuery("#resultInsertSignature").html("");
            var actionUrl = URL_WEB + 'USBToken/InsertSignatrue';
            new Ajax.Updater(areaId, actionUrl, {
                asynchronous: false,
                method: 'post',
                parameters: { signatureBase64: signatureBase64, fileName: fileName }
            });


            if (jQuery("#resultInsertSignature input[name=statusResultInsertSignature]").length == 0) {
                hideAllPopup();
                alert("Insert Signature không thành công");
                return false;
            }
            if (jQuery("#resultInsertSignature input[name=statusResultInsertSignature]").val() != "success"
                    || jQuery("#resultInsertSignature input[name=signedFileName]").val() == undefined
                    || jQuery("#resultInsertSignature input[name=signedFileName]").val().length == 0) {
                hideAllPopup();
                if (jQuery("#resultInsertSignature input[name=descErrorInsertSignature]").val() != undefined
                        && jQuery("#resultInsertSignature input[name=descErrorInsertSignature]").val().length != 0) {
                    alert("Insert Signature không thành công. " + jQuery("#resultInsertSignature input[name=descErrorInsertSignature]").val().trim());
                } else {
                    alert("Insert Signature không thành công");
                }
                return false;
            }

            var signedFileName = jQuery("#resultInsertSignature input[name=signedFileName]").val();

            //Download file
            var actionUrl = URL_WEB + 'File/Download?'
                    + "fileName=" + encodeURIComponent(signedFileName);
            jQuery.fileDownload(actionUrl)
                    .done(function () {
                        hideAllPopup();
                        alert("Download signed file success");
                    })
                    .fail(function () {
                        hideAllPopup();
                        alert("Download signed filefail");
                    });

            hideAllPopup();

        }
    }
    xmlhttp.open("POST", "http://localhost:8888/signHash", true);
    xmlhttp.setRequestHeader("Content-type", "application/x-www-form-urlencoded");
    xmlhttp.send("sessionID=" + hSession + "&HashVal=" + hashHex + "&HashOpt=0");

}


function validate() {
    if (jQuery("#resultUpload input[name=statusResultUpload]").length == 0) {
        alert("Chưa chọn file PDF để ký");
        return false;
    }
    if (jQuery("#resultUpload input[name=statusResultUpload]").val() != "success"
            || jQuery("#resultUpload input[name=fileName]").val() == undefined
            || jQuery("#resultUpload input[name=fileName]").val().indexOf(".pdf") == -1) {
        if (jQuery("#resultUpload input[name=descErrorDownloadFile]").val() != undefined
                && jQuery("#resultUpload input[name=descErrorDownloadFile]").val().length != 0) {
            alert("Upload file PDF chưa thành công, vui lòng chọn lại file PDF. " + jQuery("#resultUpload input[name=descErrorDownloadFile]").val().trim());
        } else {
            alert("Upload file PDF chưa thành công, vui lòng chọn lại file PDF");
        }
        return false;
    }

    //if (jQuery("input[name=fieldName]").length == 0 || jQuery("input[name=fieldName]").val() == undefined
    //    || jQuery("input[name=fieldName]").val().trim().length == 0) {
    //    alert("Chưa điền Field Name");
    //    return false;
    //}

    //if (jQuery("input[name=displayText]").length == 0 || jQuery("input[name=displayText]").val() == undefined
    //    || jQuery("input[name=displayText]").val().trim().length == 0) {
    //    alert("Chưa điền Display Text");
    //    return false;
    //}

    return true;
}
 