// need load the moment.js to use these filters.

(function () {
    appModule.filter('momentFormat', function () {
        return function (date, formatStr) {
            if (!date) {
                return '-';
            }
            return moment(date).format(formatStr);
        };
    }).filter('fromNow', function () {
        return function (date) {
            return moment(date).fromNow();
        };
    }).filter("catchuoifilter", function () {
        return function (str) {
            try {
                if (str.length > 50) {
                    let chuoicat = str.slice(0, 50);
                    var vitri = chuoicat.lastIndexOf(" ");
                    if (vitri != -1) {
                        chuoicat = chuoicat.slice(0, vitri);
                    }
                    chuoicat = chuoicat.concat(" ...");
                    return chuoicat;
                } else {
                    return str;
                }
            } catch (e) {
                return str;
            }
        };
    });
})();