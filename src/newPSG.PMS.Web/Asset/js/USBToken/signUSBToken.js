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
function initPlugin() {
    if (!(VtPlugin.initPlugin()) || VtPlugin.getVersion() != '1.1.0.0') {
        alert('Cần cài đặt Viettel-CA Signer Plugin');
        window.open(URL_WEB + "/Asset/plugin/ViettelCASigner.msi");
        return false;
    }
    return true;
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
 