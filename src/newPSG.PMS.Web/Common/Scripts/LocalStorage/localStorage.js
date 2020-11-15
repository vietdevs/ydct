var app = app || {};
(function () {

    app.localStorage = app.localStorage || {};

    app.localStorage.setItem = function (key, value) {
        if (!localStorage) {
            return;
        }

        localStorage.setItem(key, JSON.stringify(value));
    };

    app.localStorage.getItem = function (key, callback) {
        if (!localStorage) {
            return null;
        }

        var value = localStorage.getItem(key);
        if (callback) {
            callback(value);
        } else {
            return value;
        }
    };

    //Thêm SessionStorage
    app.sessionStorage = app.sessionStorage || {};
    app.sessionStorage.set = function (key, value) {
        if (typeof value === 'object') value = JSON.stringify(value);
        sessionStorage.setItem(key, value);
    }
    app.sessionStorage.get = function (key) {
        value = sessionStorage.getItem(key);
        try {
            return JSON.parse(value);
        } catch (e) {
            return null;
        }
    }

    //Xử lý thêm LocalStorage
    app.localStorage.set = function (key, value) {
        if (typeof value === 'object') value = JSON.stringify(value);
        localStorage.setItem(key, value);
    }
    app.localStorage.get = function (key) {
        value = localStorage.getItem(key);
        try {
            return JSON.parse(value);
        } catch (e) {
            return null;
        }
    }

})();
