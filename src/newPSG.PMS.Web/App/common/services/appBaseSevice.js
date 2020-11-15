(function () {
    appModule.factory('baseService', ['$log',
        function ($log) {
            var guestInfo = {};
            var isCompleteModalProgressBar = false;

            function ValidatorForm(form) {
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

            function isNullOrEmpty(val) {
                if (val != null && val != undefined && val != "") {
                    return false;
                } else
                    return true;
            };

            function validDate(value) {
                var formatDate = 'dd/MM/yyyy';
                if (value != null && value != "") {
                    var sDate = kendo.parseDate(value, formatDate);
                    var d = new Date(sDate);
                    if (!d.valid() || sDate == null) {
                        return false;
                    }
                    return true;
                } else
                    return false;
            };

            function canculateAgeByDOB(dateString) {
                var today = new Date();
                var birthDate = new Date(dateString);
                var age = today.getFullYear() - birthDate.getFullYear();
                var m = today.getMonth() - birthDate.getMonth();
                if (m < 0 || (m === 0 && today.getDate() < birthDate.getDate())) {
                    age--;
                }
                if (age == 0) {
                    age = 1;
                }
                return age;
            };

            function gridCommonOptions() {
                return {
                    columns: [],
                    pageable: {
                        refresh: true,
                        pageSizes: false,
                        messages: {
                            empty: app.localize('Không có dữ liệu'),
                            display: app.localize('Hiện thị {2} bản ghi')
                        }
                    },
                    dataSource: {
                    }
                };
            }

            function locdau(str) {
                str = str.toLowerCase();
                str = str.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, "a");
                str = str.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, "e");
                str = str.replace(/ì|í|ị|ỉ|ĩ/g, "i");
                str = str.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, "o");
                str = str.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u");
                str = str.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y");
                str = str.replace(/đ/g, "d");
                str = str.replace(/!|@|\$|%|\^|\*|\(|\)|\+|\=|\<|\>|\?|\/|,|\.|\:|\'| |\"|\&|\#|\[|\]|~/g, "-");
                str = str.replace(/-+-/g, "-");
                str = str.replace(/^\-+|\-+$/g, "");
                return str;
            }

            function excelReport(tableId, excelFileName) {
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
            }

            function formatBreakLineTextareaToHTML(textareaString) {
                //Hàm này dùng để format các ký tự xuống dòng trong textarea thành xuống dòng để bind dưới dạng HTML
                if (textareaString) {
                    textareaString = textareaString.replace('\n', '<br/>');
                }
                return textareaString;
            }
            function lowerFirstLetter(string) {
                if (string) {
                    return string.charAt(0).toLowerCase() + string.slice(1);
                }
                return string;
            }
            return {
                ValidatorForm: ValidatorForm,
                isNullOrEmpty: isNullOrEmpty,
                canculateAgeByDOB: canculateAgeByDOB,
                gridCommonOptions: gridCommonOptions,
                locdau: locdau,
                validDate: validDate,
                excelReport: excelReport,
                formatBreakLineTextareaToHTML: formatBreakLineTextareaToHTML,
                lowerFirstLetter: lowerFirstLetter
            };
        }
    ]);
})();
Date.prototype.valid = function () {
    return isFinite(this);
}