var VtPluginSocket = {
    hSession: "",
    initPlugin: function () {
        var LibList_MACOS = "viettel-ca_v5.dylib;viettel-ca_v4.dylib";
        var LibList_WIN = "viettel-ca_v5.dll;viettel-ca_v4.dll;viettel-ca_v2.dll";
        var functionName = "getSession";
        var paramNameArray = ['liblist'];
        var paramValueArray = [LibList_WIN + ";" + LibList_MACOS];
        var response = this.ajaxFunction(functionName, paramNameArray, paramValueArray);
        if (response != "") {
            this.hSession = response;
            return true;
        }
        return false;
    },
    getCert: function () {
        var certUserBase64 = "";
        if (this.hSession == undefined || this.hSession == null || this.hSession == "") {
            return certUserBase64;
        }
        var functionName = "getCertificate";
        var paramNameArray = ['sessionID'];
        var paramValueArray = [this.hSession];
        var response = this.ajaxFunction(functionName, paramNameArray, paramValueArray);
        if (response != "") {
            certUserBase64 = response;
        }
        return certUserBase64;
    },
    signHash: function (base64Hash, hashOpt) {
        var signatureBase64 = "";
        if (this.hSession == undefined || this.hSession == null || this.hSession == ""
                || base64Hash == undefined || base64Hash == null || base64Hash == "") {
            return signatureBase64;
        }
        if (hashOpt == undefined || (hashOpt != 0 && hashOpt != 1 && hashOpt != 2)) {
            hashOpt = 0; //Default là SHA1
        }
        var functionName = "signHash";
        var paramNameArray = ['sessionID', 'HashVal', 'HashOpt'];
        //        var paramValueArray = [this.hSession, this.convertBase64ToHexa(base64Hash), 0]; //HashOpt: loai ma hash (0: SHA-1; 1:MD5; 2:SHA256)
        var paramValueArray = [this.hSession, this.convertBase64ToHexa(base64Hash), hashOpt]; //HashOpt: loai ma hash (0: SHA-1; 1:MD5; 2:SHA256)
        var response = this.ajaxFunction(functionName, paramNameArray, paramValueArray);
        if (response != "") {
            signatureBase64 = response;
        }
        return signatureBase64;
    },
    getLastErrorCode: function () {
        var errorCode = "";
        var functionName = "getLastErr";
        var response = this.ajaxFunction(functionName);
        if (response != "") {
            errorCode = response;
        }
        return errorCode;
    },
    ajaxFunction: function (functionName, paramNameArray, paramValueArray) {
        //validate
        var response = "";
        if (functionName == undefined || functionName == null || functionName.trim() == "") {
            return response;
        }
        var xmlhttp;
        if (window.XMLHttpRequest) {
            // code for IE7+, Firefox, Chrome, Opera, Safari
            xmlhttp = new XMLHttpRequest();
        } else {
            // code for IE6, IE5
            xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");
        }
        xmlhttp.onreadystatechange = function () {
            if (xmlhttp.readyState == 4 && xmlhttp.status == 200) {
                response = xmlhttp.responseText;
            }
        }
        var method = "POST";
        var url = "http://127.0.0.1:14007/" + functionName;
        if (location.protocol == 'https:') {
            url = "https://127.0.0.1:14407/" + functionName;
        }
        var async = false; //Gui yeu cau chay dong bo
        xmlhttp.open(method, url, async);
        xmlhttp.setRequestHeader("Content-type", "application/x-www-form-urlencoded");
        if (paramNameArray == undefined || paramNameArray == null || !Array.isArray(paramNameArray) || paramNameArray.length == 0
                || paramValueArray == undefined || paramValueArray == null || !Array.isArray(paramValueArray) || paramValueArray.length == 0 ||
                paramNameArray.length != paramValueArray.length) {
            try {
                xmlhttp.send();
            } catch (err) {
                return response;
            }
        } else {
            var data = "";
            for (var i = 0; i < paramNameArray.length; i++) {
                //data += encodeURIComponent(paramNameArray[i]) + "=" + encodeURIComponent(paramValueArray[i]) + "&";
                data += paramNameArray[i] + "=" + paramValueArray[i] + "&";
            }
            if (data.length > 0) {
                data = data.substring(0, data.length - 1); //Loai bo ky tu & cuoi cung
            }
            xmlhttp.send(data);
        }
        return response;
    },
    dec2hex: function (d) {
        var hD = '0123456789ABCDEF';
        var h = hD.substr(d & 15, 1);
        while (d > 15) {
            d >>= 4;
            h = hD.substr(d & 15, 1) + h;
        }
        return h;
    },
    base64_decode: function (stringBase64) {
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
    },
    convertBase64ToHexa: function (stringBase64) {
        var output = this.base64_decode(stringBase64);
        var separator = "";
        var hexText = "";
        for (i = 0; i < output.length; i++) {
            hexText = hexText + separator + (output[i] < 16 ? "0" : "") + this.dec2hex(output[i]);
        }
        return hexText;
    },
    ERROR_CODE: {
        100100: 'Lỗi chọn CTS',
        100101: 'Lỗi Plugin',
        100102: 'CTS không hợp lệ',
        100103: 'Session không hợp lệ',
        100104: 'CTS hết hạn',
        100200: 'Dữ liệu lỗi',
        100201: 'Không tìm thấy CTS',
        100202: 'CTS không hợp lệ',
        100203: 'Lỗi xảy ra trong quá trình ký',
        100204: 'Tràn bộ nhớ',
        100205: 'Session không hợp lệ',
        100300: 'Chữ ký không đúng định dạng',
        100301: 'Lỗi phân tích CTS',
        100302: 'Chữ ký không hợp lệ',
        100303: 'Session không hợp lệ',
    }
};