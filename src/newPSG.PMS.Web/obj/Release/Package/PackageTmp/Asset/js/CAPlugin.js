/* 
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
var CAPlugin = {
    token: "",
    cert: "",
    initPlugin: function () {
        var url = "http://localhost:8888/GetToken";
        this.token = sd.connector.get(url);
        if (this.token == null || this.token == "") {
            var rs = confirm("Chưa chạy plugin hoặc chưa cài đặt plugin tại máy người dùng, bạn có muốn tải plugin về không");
            if (rs) {
                if (navigator.userAgent.indexOf("WOW64") != -1 ||
                    navigator.userAgent.indexOf("Win64") != -1) {
                    window.open('Temp/Downloads/TokenServices.zip');
                } else {
                    window.open('Temp/Downloads/TokenServices.zip');
                    //window.open('share/js/ca/setup_32bit.exe');
                }
            }
            return false;
        } else {
            return true;
        }
    },
    getCert: function () {
        var url = "http://localhost:8888/GetCert";
        var param = { token: this.token };
        this.cert = sd.connector.get(url, param);
        return this.cert;
    },
    signHash: function (base64Hash, serialNumber) {
        var url = "http://localhost:8888/SignHash";
        var param = { token: this.token, data: base64Hash, serialNumber: serialNumber };
        this.cert = sd.connector.get(url, param);
        return this.cert;
    },
    signXml: function (xml, serialNumber) {
        var url = "http://localhost:8888/SignXml";
        var param = { token: this.token, data: xml, serialNumber: serialNumber };
        this.cert = sd.connector.get(url, param);
        return this.cert;
    },
    encrypt: function (data) {
        var url = "http://localhost:8888/Encrypt";
        var param = { token: this.token, data: data };
        this.cert = sd.connector.get(url, param);
        return this.cert;

    },
    decrypt: function (data) {
        var url = "http://localhost:8888/Decrypt";
        var param = { token: this.token, data: data };
        this.cert = sd.connector.get(url, param);
        return this.cert;
    }
};
/**
 * 
 * @param {type} base64Hash
 * @param {type} certSerial
 * @returns {@exp;@call;plugin@call;signHash}
 */




function initPlugin() {
    if (CAPlugin != undefined) {
        if (CAPlugin.initPlugin()) {
            return true;
        } else {
            alert("Khởi tạo Session không thành công");
        }
    } else {
        alert("Không tồn tại đối tượng CAPlugin");
    }
    return false;
}
var certUserBase64 = '';
function signUSB(fileName, loaichuky, numberpage) {
    console.log(certUserBase64, 'signUSB----------------');
    if (loaichuky == undefined || loaichuky == null) {
        loaichuky = 0;
    }
    if (numberpage == undefined || numberpage == null) {
        numberpage = 1;
    }
    var fileky = "";
    if (CAPlugin == undefined) {
        abp.notify.error("Không tồn tại đối tượng CAPlugin");
        return;
    }
    if (CAPlugin.token == "") {
        if (!initPlugin()) {
            certUserBase64 = '';
            return "";
        }
    }

    //Bước 1. Lấy CTS mà người dùng chọn
    console.log(certUserBase64, 'certUserBase64----------------');
    if (certUserBase64 == null || certUserBase64 == "" || certUserBase64 == "E04") {
        certUserBase64 = CAPlugin.getCert();
    }
    //  var certUserBase64 = CAPlugin.getCert();
    if (certUserBase64 == "") {
        abp.notify.error("Chọn Chứng thư số không thành công");
        return "";
    }

    //Bước 2. Đẩy CTS này lên Server để tạo Hash 

    //var areaId = "resultHashFile";
    //jQuery("#resultHashFile").html("");
    var actionUrl = '/USBToken/Hash';
    $.ajax({
        url: actionUrl,
        type: 'POST',
        async: false,

        data: {
            certUserBase64: certUserBase64,
            fileName: fileName,
            LoaiChuKy: loaichuky,
            numberPage: numberpage
        },
        success: function (kq) {
            data = kq.result;
            if ((data.statusResultHashFile + "").length == 0) {

                abp.notify.error("Tạo Hash không thành công");
                return "";
            }
            if (data.statusResultHashFile != "success"
                || data.serialNumber == undefined
                || data.hashBase64 == undefined
                || (data.serialNumber.length + "") == 0
                || data.hashBase64.length == 0) {

                if (data.descErrorHashFile != undefined
                    && data.descErrorHashFile.length != 0) {
                    abp.notify.error("Tạo Hash không thành công. " + data.descErrorHashFile.trim());
                } else {
                    abp.notify.error("Tạo Hash không thành công");
                }
                return "";
            }


            var hashBase64 = data.hashBase64;

            if (hashBase64 == "") {
                abp.notify.error("Tạo Hash không thành công");
                return "";
            }

            fileky = signHash(fileName, hashBase64, data.serialNumber);// trả về file ký 
        }
    });

    return fileky;

}

function signHash(fileName, hashBase64, serialNumber) {
    if (CAPlugin == undefined) {
        abp.notify.error("Không tồn tại đối tượng CAPlugin");
        return;
    }
    if (CAPlugin.token == "") {
        if (!initPlugin()) {
            certUserBase64 = '';
            return "";
        }
    }

    var signedFileName = "";
    //Bước 3. Sau khi nhận mã Hash từ Server, gọi Plugin ký mã Hash này để thu được Chữ ký Signature
    var signatureBase64 = CAPlugin.signHash(hashBase64, serialNumber);
    //        var signatureBase64 = CAPlugin.signHash(hash, 2); //SHA256

    if (signatureBase64 == null || signatureBase64 == undefined || signatureBase64.trim().length == 0) {

        abp.notify.error("Sign Hash không thành công");
        return "";
    }

    //Bước 4. Đẩy Chữ ký Signature lên Server để Import chữ ký này vào file kết quả.  
    var actionUrl = '/USBToken/InsertSignatrue';
    $.ajax({
        url: actionUrl,
        type: 'POST',
        async: false,
        data: {
            signatureBase64: signatureBase64,
            fileName: fileName
        },

        success: function (kq) {
            data = kq.result;
            if (data.statusResultInsertSignature.length == 0) {

                abp.notify.error("Insert Signature không thành công");
                return "";
            }

            if (data.statusResultInsertSignature != "success"
                || data.signedFileName == undefined
                || data.signedFileName.length == 0) {

                if (data.descErrorInsertSignature != undefined
                    && data.descErrorInsertSignature.length != 0) {
                    abp.notify.error("Insert Signature không thành công. " + data.descErrorInsertSignature.trim());
                } else {
                    abp.notify.error("Insert Signature không thành công");
                }
                return "";
            }
            signedFileName = data.signedFileName;

        }

    });
    return signedFileName;
}