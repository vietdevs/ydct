(function (abp, angular) {

    if (!angular) {
        return;
    }

    abp.ng = abp.ng || {};

    abp.ng.http = {
        defaultError: {
            message: 'Có lỗi xảy ra!',
            details: 'Xin vui lòng thử lại hoặc liên hệ với CSKH.'
        },

        defaultError401: {
            message: 'Bạn không có quyền truy cập!',
            details: 'Xin vui lòng thử lại hoặc liên hệ với CSKH.'
        },

        defaultError403: {
            message: 'Bạn không có quyền truy cập!',
            details: 'Xin vui lòng thử lại hoặc liên hệ với CSKH.'
        },

        defaultError404: {
            message: 'Không tìm thấy trang bạn yêu cầu!',
            details: 'Hãy kiểm tra lại đường dẫn trang.'
        },

        logError: function (error) {
            abp.log.error(error);
        },

        showError: function (error) {
            if (error.details) {
                //code nguyen ban
                //return abp.message.error(error.details, error.message || abp.ng.http.defaultError.message);
                //code custom sang.nv
                return abp.notify.error("Có lỗi xảy ra trong quá trình xử lý. Vui lòng liên hệ quản trị viên để được hỗ trợ.");
            } else {
                //code nguyen ban
                //return abp.message.error(error.details, error.message || abp.ng.http.defaultError.message);
                //code custom sang.nv
                return abp.notify.error("Có lỗi xảy ra trong quá trình xử lý. Vui lòng liên hệ quản trị viên để được hỗ trợ.");
            }
        },

        handleTargetUrl: function (targetUrl) {
            if (!targetUrl) {
                location.href = abp.appPath;
            } else {
                location.href = targetUrl;
            }
        },

        handleNonAbpErrorResponse: function (response, defer) {
            if (response.config.abpHandleError !== false) {
                switch (response.status) {
                    case 401:
                        abp.ng.http.handleUnAuthorizedRequest(
                            abp.ng.http.showError(abp.ng.http.defaultError401),
                            abp.appPath
                        );
                        break;
                    case 403:
                        abp.ng.http.showError(abp.ajax.defaultError403);
                        break;
                    case 404:
                        abp.ng.http.showError(abp.ajax.defaultError404);
                        break;
                    default:
                        abp.ng.http.showError(abp.ng.http.defaultError);
                        break;
                }
            }

            defer.reject(response);
        },

        handleUnAuthorizedRequest: function (messagePromise, targetUrl) {
            if (messagePromise) {
                messagePromise.done(function () {
                    abp.ng.http.handleTargetUrl(targetUrl || abp.appPath);
                });
            } else {
                abp.ng.http.handleTargetUrl(targetUrl || abp.appPath);
            }
        },

        handleResponse: function (response, defer) {
            var originalData = response.data;

            if (originalData.success === true) {
                response.data = originalData.result;
                defer.resolve(response);

                if (originalData.targetUrl) {
                    abp.ng.http.handleTargetUrl(originalData.targetUrl);
                }
            } else if (originalData.success === false) {
                var messagePromise = null;

                if (originalData.error) {
                    if (response.config.abpHandleError !== false) {
                        //code nguyen ban
                        //messagePromise = abp.ng.http.showError(originalData.error);

                        //Code custom sang.nv
                        messagePromise = abp.notify.error("Có lỗi xảy ra trong quá trình xử lý. Vui lòng liên hệ quản trị viên để được hỗ trợ.");
                    }
                } else {
                    originalData.error = defaultError;
                }

                abp.ng.http.logError(originalData.error);

                response.data = originalData.error;
                defer.reject(response);

                if (response.status == 401 && response.config.abpHandleError !== false) {
                    abp.ng.http.handleUnAuthorizedRequest(messagePromise, originalData.targetUrl);
                }
            } else { //not wrapped result
                defer.resolve(response);
            }
        }
    }

    var abpModule = angular.module('abp', []);

    abpModule.config([
        '$httpProvider', function ($httpProvider) {
            $httpProvider.interceptors.push(['$q', function ($q) {

                return {

                    'request': function (config) {
                        if (config.url.indexOf('.cshtml') !== -1) {
                            config.url = abp.appPath + 'AbpAppView/Load?viewUrl=' + config.url + '&_t=' + abp.pageLoadTime.getTime();
                        }

                        return config;
                    },

                    'response': function (response) {
                        if (!response.data || !response.data.__abp) {
                            //Non ABP related return value
                            return response;
                        }

                        var defer = $q.defer();
                        abp.ng.http.handleResponse(response, defer);
                        return defer.promise;
                    },

                    'responseError': function (ngError) {
                        var defer = $q.defer();

                        if (!ngError.data || !ngError.data.__abp) {
                            abp.ng.http.handleNonAbpErrorResponse(ngError, defer);
                        } else {
                            abp.ng.http.handleResponse(ngError, defer);
                        }

                        return defer.promise;
                    }

                };
            }]);
        }
    ]);

    abp.event.on('abp.dynamicScriptsInitialized', function () {
        abp.ng.http.defaultError.message = abp.localization.abpWeb('DefaultError');
        abp.ng.http.defaultError.details = abp.localization.abpWeb('DefaultErrorDetail');
        abp.ng.http.defaultError401.message = abp.localization.abpWeb('DefaultError401');
        abp.ng.http.defaultError401.details = abp.localization.abpWeb('DefaultErrorDetail401');
        abp.ng.http.defaultError403.message = abp.localization.abpWeb('DefaultError403');
        abp.ng.http.defaultError403.details = abp.localization.abpWeb('DefaultErrorDetail403');
        abp.ng.http.defaultError404.message = abp.localization.abpWeb('DefaultError404');
        abp.ng.http.defaultError404.details = abp.localization.abpWeb('DefaultErrorDetail404');
    });

})((abp || (abp = {})), (angular || undefined));