/**
 * So sanh ngay bat dau, ngay ket thuc, ngay hien tai.
 * Co nhieu form su dung kieu so sanh nay.
 * @param startDateId ID
 * @param endDateId Chieu rong cua popup
 * @return
 *   false:
 *     - Khong chon ngay bat dau
 *     - Khong chon ngay ket thuc
 *     - Ngay bat dau lon hon ngay hien tai
 *     - Ngay ket thuc lon hon ngay hien tai
 *     - Ngay bat dau lon hon ngay ket thuc
 *   true:
 *     - Cac truong hop con lai
 * @author HuyenNV
 */
function tctValidateDates(startDateId, endDateId) {
    var startDate = document.getElementById(startDateId).value;
    var endDate = document.getElementById(endDateId).value;

    if (startDate == "") {
        alert("Bạn phải chọn ngày bắt đầu!");
        document.getElementById(startDateId).focus();
        document.getElementById(startDateId).select();
    } else if (endDate == "") {
        alert("Bạn phải chọn ngày kết thúc!");
        document.getElementById(endDateId).focus();
        document.getElementById(endDateId).select();
    } else {
        var sd = new Date();
        sd.setFullYear(parseInt(startDate.substring(6, 10), 10), parseInt(startDate.substring(3, 5), 10) - 1, parseInt(startDate.substring(0, 2), 10));
        var ed = new Date();
        ed.setFullYear(parseInt(endDate.substring(6, 10), 10), parseInt(endDate.substring(3, 5), 10) - 1, parseInt(endDate.substring(0, 2), 10));
        var currentDate = new Date();

        if (currentDate < sd) {
            alert("Hãy nhập ngày bắt đầu nhỏ hơn hoặc bằng ngày hiện tại!");
            document.getElementById(startDateId).focus();
            document.getElementById(startDateId).select();
            return false;
        } else if (currentDate < ed) {
            alert("Hãy nhập ngày kết thúc nhỏ hơn hoặc bằng ngày hiện tại!");
            document.getElementById(endDateId).focus();
            document.getElementById(endDateId).select();
            return false;
        } else if (ed < sd) {
            alert("Hãy nhập ngày bắt đầu nhỏ hơn hoặc bằng ngày kết thúc!");
            document.getElementById(startDateId).focus();
            document.getElementById(startDateId).select();
            return false;
        }
    }
    return true;
}

/**
 * So sanh 2 ngay.
 * Cac ngay co dinh dang 'dd/mm/yyyy'.
 * @param date1 
 * @param date2
 * @return -1: date1 less than date2, 0: date1 equals date2, 1: date1 greater than date2
 * @author HuyenNV
 */
function tctCompareDates(date1, date2) {
    var arrayDate1 = date1.split("/");
    var arrayDate2 = date2.split("/");

    var nam1   = parseFloat(arrayDate1[2]);
    var nam2   = parseFloat(arrayDate2[2]);
    var thang1 = parseFloat(arrayDate1[1]);
    var thang2 = parseFloat(arrayDate2[1]);
    var ngay1  = parseFloat(arrayDate1[0]);
    var ngay2  = parseFloat(arrayDate2[0]);

    if (nam1 < nam2) {
        return -1;
    } else if (nam1 > nam2) {
        return 1;
    } else {
        if (thang1 < thang2) {
            return -1;
        } else if (thang1 > thang2) {
            return 1;
        } else {
            if (ngay1 < ngay2) {
                return -1;
            } else if (ngay1 > ngay2) {
                return 1;
            } else {
                return 0;
            }
        }
    }
}

/**
 * So sanh ngay nao do voi ngay hien tai ngay hien tai.
 * Ngay dua vao co dinh dang 'dd/mm/yyyy'.
 * @param date1
 * @return -1: date1 less than current date, 0: date1 equals current date, 1: date1 greater than current date
 * @author HuyenNV
 */
function tctCompareToCurrentDate(date1) {
    var currentDate = new Date();
    var d = currentDate.getDate();
    var m = currentDate.getMonth() + 1;
    var y = currentDate.getFullYear();

    var arrayDate1 = date1.split("/");
    var nam1   = parseFloat(arrayDate1[2]);
    var thang1 = parseFloat(arrayDate1[1]);
    var ngay1  = parseFloat(arrayDate1[0]);

    if (nam1 < y) {
        return -1;
    } else if (nam1 > y) {
        return 1;
    } else {
        if (thang1 < m) {
            return -1;
        } else if (thang1 > m) {
            return 1;
        } else {
            if (ngay1 < d) {
                return -1;
            } else if (ngay1 > d) {
                return 1;
            } else {
                return 0;
            }
        }
    }
}



/**
 * Ham mo popup.
 * @param url Dia chi URL cua popup
 * @param width Chieu rong cua popup
 * @param height Chieu cao cua popup
 * @param name Ten cua cua so mo
 * @return void
 * @author HuyenNV
 */
function tctOpenPopup(url, width, height, name) {
    if (name == undefined) {
        name = "_blank";
    }
    _winRef = window.open(url, name, 'toolbar=no,titlebar=no,location=no,directories=no,status=no,menubar=no,scrollbars=yes,resizable=yes,copyhistory=yes,width=' +
        width + ',height=' + height + ',left=100,top=100,screenX=100,screenY=100', false);
    _winRef.focus();
    return _winRef;
}

/**
 * Ham mo modal popup.
 * @param url Dia chi URL cua popup
 * @param pwidth Chieu rong cua popup (px)
 * @param pheight Chieu cao cua popup (px)
 * @param ptop Khoang cach cua popup so voi canh tren man hinh (px)
 * @param pleft Khoang cach cua popup so voi canh trai man hinh (px)
 * @return void
 * @author HuyenNV
 */
function tctOpenModalPopup(url, pwidth, pheight, ptop, pleft) {
    if (pwidth == undefined) {
        pwidth = 800;
    }
    if (pheight == undefined) {
        pheight = 600;
    }
    if (ptop == undefined) {
        ptop = 100;
    }
    if (pleft == undefined) {
        pleft = 100;
    }
    window.showModalDialog(url, null, "dialogWidth:" + pwidth + "px;dialogHeight:" + pheight +
        "px;dialogTop:" + ptop + "px;dialogLeft:" + pleft + "px;");
}

/**
 * Ham xu ly check box.
 * Ham ap dung voi cac check box trong bang ma co tien ich Check all.
 * Khi chung ta click vao tung checkbox thi cung phai kiem tra trang thai Check all.
 * @param tableId ID cua display:table
 * @param checkName Ten cua cac checkbox
 * @param checkAllId ID cua Check all
 * @return void
 * @author HuyenNV
 */
function tctCheckCheckAll(tableId, checkName, checkAllId) {
    var numTotal = 0;
    var numSelected = 0;
    var myTable = document.getElementById(tableId);
    var inputs = myTable.getElementsByTagName('input');
    for (var i = 0; i < inputs.length; i++) {
        if (inputs[i].name.indexOf(checkName) >= 0) {
            numTotal++;
            if (inputs[i].checked) {
                numSelected++;
            }
        }
    }
    if (numSelected == numTotal) {
        document.getElementById(checkAllId).checked = true;
    } else {
        document.getElementById(checkAllId).checked = false;
    }
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
 * Kiem tra xem mot so co phai la so tu nhien hay khong.
 * Xau dua vao chi gom cac chu so,
 * khong co khoang cach giua cac chu so,
 * khong co ky tu dac biet + - . ,
 * @param myNumber So ma ban muon kiem tra
 * @return true neu so do la so tu nhien
 * @author HuyenNV
 */
function tctIsNaturalNumber(myNumber) {
    var numberRegExp = /^\d+$/i;
    return numberRegExp.test(trim(myNumber));
}

/**
 * Kiem tra xem xau co dung dinh dang so dien thoai hay khong.
 * Xau dua vao chi gom cac chu so va dau cach,
 * bat dau bang chu so,
 * @param myNumber Xau ma ban muon kiem tra
 * @return true neu xau dung dinh dang
 * @author HuyenNV
 */
function tctIsPhoneNumber(myNumber) {
    var numberRegExp = /^\d[\d\s]*$/i;
    return numberRegExp.test(trim(myNumber));
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

/**
 * Kiem tra so la nguyen duong hay khong,cat  0 dau
 * @param obj :doi tuong can xu ly
 * @param msg : msg dua ra neu sai
 * @param noAlert : co can alert khong
 * @return true neu object nhap la string so duong, dong thoi cat 0 dau
 * @author DucHT
 */
function tctValidatePositiveNumber(obj,msg, noAlert){

    var checkString = obj.value;

    checkString = trimAll(checkString);


        if (!isNum(checkString) || parseFloat(checkString) < 0)
        {
            var strMsg = (msg != null && msg != undefined) ? msg : 'Dữ liệu phải là số nguyên dương!';
            if(noAlert == undefined) {
                alert (strMsg);
            }
            setTimeout(function(){
                obj.focus();obj.select();
            },0);
            return false;
        }
        obj.value= parseInt(obj.value,10);
    
    
    return true;
}

/**
 * Kiem tra so la nguyen duong hay khong,cat  0 dau
 * @param obj :doi tuong can xu ly
 * @return true neu object nhap la string so duong, dong thoi cat 0 dau
 * @author DucHT
 */
function tctValidateInteger(obj)

{   
    var i;
    var string= obj.value;
    alert(string);
    for (i = 0; i < string.length; i++){
            // Check that current character is number.
            var c = string.charAt(i);

            if (((c < "0") || (c > "9"))) return false;
    }

    // All characters are numbers.
    obj.value = parseInt(obj.value);
    return true;

}

/**
 * cat space trai va phai cua xau
 * @param objId :doi tuong can xu ly
 * @return xau tra ve
 * @author DucHT
 */
function tctTrimLeftRight(objId){
            var value= document.getElementById(objId).value;
            document.getElementById(objId).value=trim(value);
}

/**
 * Kiem tra va cat xau
 * @param objId: ID doi tuong can xu ly
 * @param maxlength: chieu dai toi da
 * @return true neu object nhap la string so duong, dong thoi cat 0 dau
 * @author DucHT
 */
function tctValidateText(objId, maxlength) {
    var value= document.getElementById(objId).value;
    var max = 199;
    if (maxlength != null && maxlength != undefined) {
        max = maxlength - 1;
    }
	var tmp = value.replace(/\n/g, "aa");
	var diff = tmp.length - value.length;
	if (diff < 0) {
		diff = 0;
	}
    if (tmp.length > max) {
        alert("Bạn không được nhập quá " + max + " ký tự!");
        value = value.substring(0, max - diff);
        document.getElementById(objId).value = value;
    }

    return;
}
/**
 * Kiem tra so la thap phan duong hay khong,cat  0 dau
 * @param obj :doi tuong can xu ly
 * @param msg : msg dua ra neu sai
 * @param noAlert : co can alert khong
 * @return true neu object nhap la string so duong, dong thoi cat 0 dau
 * @author DucHT
 */
function tctValidatePositiveDouble(obj , msg, noAlert)
{
    if(trimAll(obj.value)==""){
        return true;
    }
    var IsDouble = testDouble(obj);

    if (IsDouble && trimAll(obj.value) != '')
    {
        IsDouble = parseFloat(obj.value) > 0;
    }
    if(!IsDouble)
    {
        var strMsg = (msg != null && msg != undefined) ? msg : 'Dữ liệu phải là số thập phân dương!';
        if(noAlert == undefined) {
            alert (strMsg);
        }
        setTimeout(function(){
            obj.focus();obj.select();
        },0);
        return false;
    }
    obj.value = parseFloat(obj.value);
    return true;

}

/**
 * Scroll cac row trong table, phan title thi ko scroll
 * @param tableEl : doi tuong Table
 * @param tableHeight : chieu cao Table
 * @param tableWidth : chieu rong Table. Co the bo trong
 * @param widthType : 'px' hoac '%'
 * @return
 * @author AuNH
 */

function tctScrollableTable (tableEl, tableHeight, tableWidth, widthType) {
	if(tableWidth == null)
		widthType = 'px';
	this.initIEengine = function () {

		this.containerEl.style.overflowY = 'auto';
		if (this.tableEl.parentElement.clientHeight - this.tableEl.offsetHeight < 0) {
			this.tableEl.style.width = '97%';
		} else {
			this.containerEl.style.overflowY = 'hidden';
			this.tableEl.style.width = this.newWidth +widthType;
		}

		if (this.thead) {
			var trs = this.thead.getElementsByTagName('tr');
			for (x=0; x<trs.length; x++) {
				trs[x].style.position ='relative';
				trs[x].style.setExpression("top",  "this.parentElement.parentElement.parentElement.scrollTop + 'px'");
			}
		}

		if (this.tfoot) {
			var trs = this.tfoot.getElementsByTagName('tr');
			for (x=0; x<trs.length; x++) {
				trs[x].style.position ='relative';
				trs[x].style.setExpression("bottom",  "(this.parentElement.parentElement.offsetHeight - this.parentElement.parentElement.parentElement.clientHeight - this.parentElement.parentElement.parentElement.scrollTop) + 'px'");
			}
		}

		eval("window.attachEvent('onresize', function () { document.getElementById('" + this.tableEl.id + "').style.visibility = 'hidden'; document.getElementById('" + this.tableEl.id + "').style.visibility = 'visible'; } )");
	};


	this.initFFengine = function () {
		this.containerEl.style.overflow = 'hidden';
		this.tableEl.style.width = this.newWidth + widthType;

		var headHeight = (this.thead) ? this.thead.clientHeight : 0;
		var footHeight = (this.tfoot) ? this.tfoot.clientHeight : 0;
		var bodyHeight = this.tbody.clientHeight;
		var trs = this.tbody.getElementsByTagName('tr');
		if (bodyHeight >= (this.newHeight - (headHeight + footHeight))) {
			this.tbody.style.overflow = '-moz-scrollbars-vertical';
			for (x=0; x<trs.length; x++) {
				var tds = trs[x].getElementsByTagName('td');
				tds[tds.length-1].style.paddingRight += this.scrollWidth + 'px';
			}
		} else {
			this.tbody.style.overflow = '-moz-scrollbars-none';
		}

		var cellSpacing = (this.tableEl.offsetHeight - (this.tbody.clientHeight + headHeight + footHeight)) / 4;
		this.tbody.style.height = (this.newHeight - (headHeight + cellSpacing * 2) - (footHeight + cellSpacing * 2)) + 'px';

	};

	this.tableEl = tableEl;
	this.scrollWidth = 16;

	this.originalHeight = this.tableEl.clientHeight;
	this.originalWidth = this.tableEl.clientWidth;

	this.newHeight = parseInt(tableHeight);
	this.newWidth = tableWidth ? parseInt(tableWidth) : this.originalWidth;

	this.tableEl.style.height = 'auto';
	this.tableEl.removeAttribute('height');

	this.containerEl = this.tableEl.parentNode.insertBefore(document.createElement('div'), this.tableEl);
	this.containerEl.appendChild(this.tableEl);
	this.containerEl.style.height = this.newHeight + 'px';
	this.containerEl.style.width = this.newWidth + widthType;


	var thead = this.tableEl.getElementsByTagName('thead');
	this.thead = (thead[0]) ? thead[0] : null;

	var tfoot = this.tableEl.getElementsByTagName('tfoot');
	this.tfoot = (tfoot[0]) ? tfoot[0] : null;

	var tbody = this.tableEl.getElementsByTagName('tbody');
	this.tbody = (tbody[0]) ? tbody[0] : null;

	if (!this.tbody) return;

	if (document.all && document.getElementById && !window.opera) this.initIEengine();
	if (!document.all && document.getElementById && !window.opera) this.initFFengine();
}

function tctValidateDelete(objList,objArray,message) {
         if (!tctCheckSelected(objList, objArray, message)) {
             return false;
         } else {
             return confirm(' Bạn có chắc chắn muốn hủy các dữ liệu này không?  ?');
         }
     }