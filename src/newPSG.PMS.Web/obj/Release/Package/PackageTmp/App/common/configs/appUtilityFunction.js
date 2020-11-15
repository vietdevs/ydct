var app = app || {};

Date.prototype.valid = function () {
    return isFinite(this);
};

app.validatorForm = function (form) {
    $(form).formValidation({
        framework: 'bootstrap',
        icon: {
        },
        excluded: ':disabled',
        fields: {
        },
    })
        .off('success.form.fv')
        .on('success.form.fv', function (e) {
            var $form = $(e.target),
                fv = $(e.target).data('formValidation');
            fv.defaultSubmit();
        })
        .on('err.field.fv', function (e, data) {
            if (data.fv.getSubmitButton()) {
                data.fv.disableSubmitButtons(false);
            }
        })
        .on('success.field.fv', function (e, data) {
            if (data.fv.getSubmitButton()) {
                data.fv.disableSubmitButtons(false);
            }
        }).on('err.validator.fv', function (e, data) {
            // $(e.target)    --> The field element
            // data.fv        --> The FormValidation instance
            // data.field     --> The field name
            // data.element   --> The field element
            // data.validator --> The current validator name

            data.element
                .data('fv.messages')
                // Hide all the messages
                .find('.help-block[data-fv-for="' + data.field + '"]').hide()
                // Show only message associated with current validator
                .filter('[data-fv-validator="' + data.validator + '"]').show();
        });
};

app.isNullOrEmpty = function (o) {
    var f = false;
    if (typeof (o) === 'undefined') f = true;
    if (o == null) f = true;
    o = String(o);
    o = o.replace(/ /g, '');
    if (o == '') f = true;
    return f;
}

app.sortTable = function (lstData, sortingObj, keyname, isNeedReverse) {
    sortingObj.oldSortKey = angular.copy(sortingObj.sortKey);
    sortingObj.sortKey = keyname;   //set the sortKey to the param passed
    if (isNeedReverse) {
        if (sortingObj.oldSortKey === sortingObj.sortKey) {
            sortingObj.reverse = !sortingObj.reverse;
        }
    }

    lstData = $filter('orderBy')(lstData, sortingObj.sortKey, sortingObj.reverse);
    return lstData;
}
app.validDate = function (value) {
    var formatDate = 'dd/MM/yyyy';
    if (value !== null && value !== "") {
        var sDate = kendo.parseDate(value, formatDate);
        var d = new Date(sDate);
        if (!d.valid() || sDate === null) {
            return false;
        }
        return true;
    } else
        return false;
};

app.removeDau = function (alias) {
    var str = alias;
    str = str.toLowerCase();
    str = str.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, "a");
    str = str.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, "e");
    str = str.replace(/ì|í|ị|ỉ|ĩ/g, "i");
    str = str.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, "o");
    str = str.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u");
    str = str.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y");
    str = str.replace(/đ/g, "d");
    str = str.replace(/!|@|%|\^|\*|\(|\)|\+|\=|\<|\>|\?|\/|,|\.|\:|\;|\'|\"|\&|\#|\[|\]|~|\$|_|`|-|{|}|\||\\/g, " ");
    str = str.replace(/ + /g, " ");
    str = str.trim();
    return str;
}
app.locdau = function (str) {
    str = str.toLowerCase();
    str = str.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, "a");
    str = str.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, "e");
    str = str.replace(/ì|í|ị|ỉ|ĩ/g, "i");
    str = str.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, "o");
    str = str.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u");
    str = str.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y");
    str = str.replace(/đ/g, "d");
    str = str.replace(/–|!|@|\$|%|\^|\*|\(|\)|\+|\=|\<|\>|\?|\/|,|\.|\:|\'| |\"|\&|\#|\[|\]|~/g, "-");
    str = str.replace(/-+-/g, "-");
    str = str.replace(/^\-+|\-+$/g, "");
    return str;
};

app.excelReport = function (tableId, excelFileName) {
    var tab_text = "<table border='2px'><tr bgcolor='#ffffff'>";
    var textRange; var j = 0;
    var tab = document.getElementById(tableId); // id of table

    for (j = 0; j < tab.rows.length; j++) {
        tab_text = tab_text + tab.rows[j].innerHTML + "</tr>";
        //tab_text=tab_text+"</tr>";
    }

    tab_text = tab_text + "</table>";
    tab_text = tab_text.replace(/<A[^>]*>|<\/A>/g, "");//remove if u want links in your table
    tab_text = tab_text.replace(/<img[^>]*>/gi, ""); // remove if u want images in your table
    tab_text = tab_text.replace(/<input[^>]*>|<\/input>/gi, ""); // reomves input params

    var ua = window.navigator.userAgent;
    var msie = ua.indexOf("MSIE ");

    if (msie > 0 || !!navigator.userAgent.match(/Trident.*rv\:11\./))      // If Internet Explorer
    {
        txtArea1.document.open("txt/html", "replace");
        txtArea1.document.write(tab_text);
        txtArea1.document.close();
        txtArea1.focus();
        sa = txtArea1.document.execCommand("SaveAs", true, excelFileName + ".xls");
    }
    else                 //other browser not tested on IE 11
        var sa = 'data:application/vnd.ms-excel,' + encodeURIComponent(tab_text);
    var link = document.createElement('a');
    link.download = excelFileName + ".xls";
    link.href = sa;
    link.click();
    return (sa);
};

app.formatBreakLineTextareaToHTML = function (textareaString) {
    //Hàm này dùng để format các ký tự xuống dòng trong textarea thành xuống dòng để bind dưới dạng HTML
    if (textareaString) {
        textareaString = textareaString.replace('\n', '<br/>');
    }
    return textareaString;
};

app.lowerFirstLetter = function (string) {
    if (string) {
        return string.charAt(0).toLowerCase() + string.slice(1);
    }
    return string;
};

app.formatNumber = function (nStr) {
    if (nStr) {
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

    return '';
};

app.formatDate = function (date, strFormart) {
    if (date) {
        var d = new Date(date),
            month = '' + (d.getMonth() + 1),
            day = '' + d.getDate(),
            year = d.getFullYear();

        if (month.length < 2) month = '0' + month;
        if (day.length < 2) day = '0' + day;

        return [day, month, year].join('-');
    }
    return date;
};

app.checkValidateForm = function (id) {
    var a = $(id).find(".custom-error-validate");
    var _check = ($(id).find(".custom-error-validate").length) || $(id).find(".help-block").is(":visible");
    $(id).find(".custom-error-validate").show();
    return !_check;
}

app.validateEmail = (email) => {
    var re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    return re.test(String(email).toLowerCase());
}

app.validateSdt = (sdt) => {
    var re = /^[+]*[(]{0,1}[0-9]{1,3}[)]{0,1}[-\s\./0-9]*$/g;
    return re.test(String(sdt).toLowerCase());
}

app.validateNumber = (num) => {
    var re = /^[0-9]{1,10}$/;
    return re.test(String(num).toLowerCase());
}

app.getStrThuMucHoSo = function () {
    var d = new Date(),
        year = d.getFullYear(),
        month = '' + (d.getMonth() + 1),
        day = '' + d.getDate(),
        hour = '' + d.getHours(),
        minute = '' + d.getMinutes(),
        second = '' + d.getSeconds();

    if (month.length < 2) month = '0' + month;
    if (day.length < 2) day = '0' + day;
    if (hour.length < 2) hour = '0' + hour;
    if (minute.length < 2) minute = '0' + minute;
    if (second.length < 2) second = '0' + second;

    var _strThuMucHoSo = 'HOSO_' + year + month + day + '_' + hour + minute + second;

    return _strThuMucHoSo;
};