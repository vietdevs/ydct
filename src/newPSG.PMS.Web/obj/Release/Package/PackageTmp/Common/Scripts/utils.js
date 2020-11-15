var app = app || {};
(function () {

    app.utils = app.utils || {};

    app.utils.truncateString = function (str, maxLength, postfix) {
        if (!str || !maxLength || str.length <= maxLength) {
            return str;
        }

        if (postfix === false) {
            return str.substr(0, maxLength);
        }

        return str.substr(0, maxLength - 1) + '&#133;';
    }

    app.utils.removeCookie = function (key) {
        document.cookie = key + '=; Path=/; Expires=Thu, 01 Jan 1970 00:00:01 GMT;';
    }

    app.utils.tryParseJSON = function(jsonString) {
        try {
            var o = JSON.parse(jsonString);

            // Handle non-exception-throwing cases:
            // Neither JSON.parse(false) or JSON.parse(1234) throw errors, hence the type-checking,
            // but... JSON.parse(null) returns null, and typeof null === "object", 
            // so we must check for that, too. Thankfully, null is falsey, so this suffices:
            if (o && typeof o === "object") {
                return o;
            }
        }
        catch (e) { }

        return false;
    };
})();