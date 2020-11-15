
function hideAllPopup() {
    jQuery(".popup-alert").hide();
}

/*
 Hiện thị thông báo
 */
function showProgressing(message) {
    hideAllPopup();
    if (message != undefined) {
        jQuery("#progressing-div .loading-text").html(message);
    } else {
        jQuery("#progressing-div .loading-text").html("Hệ thống đang xử lý");
    }

    var marginTop =  window.pageYOffset - (jQuery("#progressing-div .loading-div").height()/2 ) + jQuery(window).height()/2 - 200;
    jQuery("#progressing-div .loading-div").css({"margin-top": marginTop + "px"})
    jQuery("#progressing-div").show();
}

/*
 Ẩn thông báo
 */
function hideProgressing() {
    jQuery("#progressing-div").hide();
}

/*
 Hiện thị thông báo lỗi
 */
function showMessage(message) {
    hideAllPopup();
    jQuery("#message-div .alert-text .message-content-div").html(message);
    var marginTop =  window.pageYOffset - (jQuery("#message-div .alert-div").height()/2 ) + jQuery(window).height()/2 - 100;
    jQuery("#message-div .alert-div").css({"margin-top": marginTop + "px"})
    jQuery("#message-div").show();
}

/*
 Ẩn thông báo lỗi
 */
function hideMessage() {
    jQuery("#message-div").hide();
}

function showErrorMessage(messageDivId, messageError1, messageError2) {
    if (messageDivId == null || messageDivId == undefined || messageDivId == "") {
        return;
    }
    if (messageError1 == undefined) {
        messageError1 = "";
    }
    if (messageError2 == undefined) {
        messageError2 = "";
    }
    if (messageError2 != "") {
        if (messageError1 != "") {
            messageError1 += ";" + messageError2;
        } else {
            messageError1 = messageError2;
        }
    }
    var messageArray = messageError1.split(";");
    //        var messageError1 = messageArray.join("<br/>");
    var messageError1 = messageArray[0];
    if (messageError1 != "") {
        jQuery("#" + messageDivId).show();
        document.getElementById(messageDivId).innerHTML = messageError1;
    } else {
        jQuery("#" + messageDivId).hide();
    }
}