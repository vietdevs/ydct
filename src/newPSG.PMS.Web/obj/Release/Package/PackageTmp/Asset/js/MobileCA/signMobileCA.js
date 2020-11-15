
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
});


function isInteger(s) {
    var i;
    for (i = 0; i < s.length; i++) {
        // Check that current character is number.
        var c = s.charAt(i);
        if (((c < "0") || (c > "9")))
            return false;
    }
    // All characters are numbers.
    return true;
}
function isMobileNumber(obj) {
    var phoneNumber = obj.value;
    if (phoneNumber != null && trim(phoneNumber).length > 0) {
        var fCheck = phoneNumber.substring(0, 1);
        if (isInteger(trim(phoneNumber))) {
            var strMsg = "";
            if ((parseInt(fCheck, 10) == 0)) {
                strMsg = "Số điện thoại không được bắt đầu là số 0!";
                alert(strMsg);
                obj.value = "";
                setTimeout(function () {
                    obj.focus();
                    obj.select();
                }, 0);
            } else
                if (trim(phoneNumber).length > 11) {
                    strMsg = "Chiều dài phải nhỏ hơn 12 số!";
                    alert(strMsg);
                    obj.value = "";
                    setTimeout(function () {
                        obj.focus();
                        obj.select();
                    }, 0);
                }
        } else {
            alert("Số điện thoại phải là số!");
            obj.value = "";
            setTimeout(function () {
                obj.focus();
                obj.select();
            }, 0);
        }
    }
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

    if (jQuery("input[name=phoneNumber]").length == 0 || jQuery("input[name=phoneNumber]").val() == undefined
            || jQuery("input[name=phoneNumber]").val().trim().length == 0) {
        alert("Chưa nhập Số điện thoại");
        return false;
    }


    var phoneNumber = jQuery("input[name=phoneNumber]").val();
    var fCheck = phoneNumber.substring(0, 1);
    if (isInteger(trim(phoneNumber))) {
        if ((parseInt(fCheck, 10) == 0)) {
            alert("Số điện thoại không được bắt đầu là số 0!");
            jQuery("input[name=phoneNumber]").focus();
            return false;
        } else
            if (trim(phoneNumber).length > 12) {
                alert("Chiều dài phải nhỏ hơn bằng 12 số!");
                jQuery("input[name=phoneNumber]").focus();
                return false;
            }
        if (trim(phoneNumber).substring(0, 2) != "84") {
            alert("Số điện thoại phải có định dạng 84xxxxxxxxxx!");
            jQuery("input[name=phoneNumber]").focus();
            return false;
        }
    } else {
        alert("Số điện thoại phải là số!");
        jQuery("input[name=phoneNumber]").focus();
        return false;
    }
    return true;
}

function signMobileCA() {
    if (!validate()) {
        return;
    }
    var fileName = jQuery("#resultUpload input[name=fileName]").val();
    var phoneNumber = jQuery("input[name=phoneNumber]").val();
    showProgressing();

    areaId = "resultSignMobileCA";
    jQuery("#resultSignMobileCA").html("");
    var actionUrl = URL_WEB + 'MobileCA/SignMobileCA';
    new Ajax.Updater(areaId, actionUrl, {
        asynchronous: false,
        method: 'post',
        parameters: { phoneNumber: phoneNumber, fileName: fileName }
    });


    if (jQuery("#resultSignMobileCA input[name=statusResultSignMobileCA]").length == 0) {
        hideAllPopup();
        alert("Ký không thành công");
        return false;
    }
    if (jQuery("#resultSignMobileCA input[name=statusResultSignMobileCA]").val() != "success"
            || jQuery("#resultSignMobileCA input[name=signedFileName]").val() == undefined
            || jQuery("#resultSignMobileCA input[name=signedFileName]").val().length == 0) {
        hideAllPopup();
        if (jQuery("#resultSignMobileCA input[name=descErrorSignMobileCA]").val() != undefined
                && jQuery("#resultSignMobileCA input[name=descErrorSignMobileCA]").val().length != 0) {
            alert("Ký không thành công. " + jQuery("#resultSignMobileCA input[name=descErrorSignMobileCA]").val().trim());
        } else {
            alert("Ký không thành công");
        }
        return false;
    }

    var signedFileName = jQuery("#resultSignMobileCA input[name=signedFileName]").val();

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