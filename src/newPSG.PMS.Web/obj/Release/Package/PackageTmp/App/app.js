/* 'app' MODULE DEFINITION */
var appModule = angular.module("app", [
    "ui.router",
    "ui.bootstrap",
    "kendo.directives",
    'ui.utils',
    "ui.jq",
    'ui.grid',
    'ui.grid.pagination',
    "oc.lazyLoad",
    "ngSanitize",
    'angularFileUpload',
    'daterangepicker',
    'angularMoment',
    'frapontillo.bootstrap-switch',
    'angular-content-editable',
    'pdfjsViewer',
    'angular-linq',
    'ui.utils.masks',
    'angularjs-dropdown-multiselect',
    'abp',
    'summernote',
    'ngMaterial',
    'ngMessages',
    'ngCkeditor'
]);

/* LAZY LOAD CONFIG */

/* This application does not define any lazy-load yet but you can use $ocLazyLoad to define and lazy-load js/css files.
 * This code configures $ocLazyLoad plug-in for this application.
 * See it's documents for more information: https://github.com/ocombe/ocLazyLoad
 */
appModule.config(['$ocLazyLoadProvider', function ($ocLazyLoadProvider) {
    $ocLazyLoadProvider.config({
        cssFilesInsertBefore: 'ng_load_plugins_before', // load the css files before a LINK element with this ID.
        debug: false,
        events: true,
        modules: []
    });
}]);

/* THEME SETTINGS */
App.setAssetsPath(abp.appPath + 'metronic/assets/');
appModule.factory('settings', ['$rootScope', function ($rootScope) {
    var settings = {
        layout: {
            pageSidebarClosed: false, // sidebar menu state
            pageContentthite: true, // set page content layout
            pageBodySolid: false, // solid body color state
            pageAutoScrollOnLoad: 1000 // auto scroll to top on page load
        },
        layoutImgPath: App.getAssetsPath() + 'admin/layout4/img/',
        layoutCssPath: App.getAssetsPath() + 'admin/layout4/css/',
        assetsPath: abp.appPath + 'metronic/assets',
        globalPath: abp.appPath + 'metronic/assets/global',
        layoutPath: abp.appPath + 'metronic/assets/layouts/layout4'
    };

    $rootScope.settings = settings;

    return settings;
}]);

/* ROUTE DEFINITIONS */

appModule.config([
    '$stateProvider', '$urlRouterProvider', '$qProvider',
    function ($stateProvider, $urlRouterProvider, $qProvider) {
        /*** He Thong ***/
        {
            //Default route (overrided below if user has permission)
            $urlRouterProvider.otherwise("/welcome");

            //Welcome page
            $stateProvider.state('welcome', {
                url: '/welcome',
                templateUrl: '~/App/common/views/welcome/index.cshtml'
            });

            //COMMON routes

            if (abp.auth.hasPermission('Pages.Administration.Roles')) {
                $stateProvider.state('roles', {
                    url: '/roles',
                    templateUrl: '~/App/common/views/roles/index.cshtml'
                });
            }

            if (abp.auth.hasPermission('Pages.Administration.Users')) {
                $stateProvider.state('users', {
                    url: '/users?filterText',
                    templateUrl: '~/App/common/views/users/index.cshtml'
                });
            }

            if (abp.auth.hasPermission('Pages.Administration.Languages')) {
                $stateProvider.state('languages', {
                    url: '/languages',
                    templateUrl: '~/App/common/views/languages/index.cshtml'
                });

                if (abp.auth.hasPermission('Pages.Administration.Languages.ChangeTexts')) {
                    $stateProvider.state('languageTexts', {
                        url: '/languages/texts/:languageName?sourceName&baseLanguageName&targetValueFilter&filterText',
                        templateUrl: '~/App/common/views/languages/texts.cshtml'
                    });
                }
            }

            if (abp.auth.hasPermission('Pages.Administration.AuditLogs')) {
                $stateProvider.state('auditLogs', {
                    url: '/auditLogs',
                    templateUrl: '~/App/common/views/auditLogs/index.cshtml'
                });
            }

            if (abp.auth.hasPermission('Pages.Administration.OrganizationUnits')) {
                $stateProvider.state('organizationUnits', {
                    url: '/organizationUnits',
                    templateUrl: '~/App/common/views/organizationUnits/index.cshtml'
                });
            }

            $stateProvider.state('notifications', {
                url: '/notifications',
                templateUrl: '~/App/common/views/notifications/index.cshtml'
            });

            //HOST routes

            $stateProvider.state('host', {
                'abstract': true,
                url: '/host',
                template: '<div ui-view class="fade-in-up"></div>'
            });

            if (abp.auth.hasPermission('Pages.Tenants')) {
                $urlRouterProvider.otherwise("/host/tenants"); //Entrance page for the host
                $stateProvider.state('host.tenants', {
                    url: '/tenants?filterText',
                    templateUrl: '~/App/host/views/tenants/index.cshtml'
                });
            }

            if (abp.auth.hasPermission('Pages.Editions')) {
                $stateProvider.state('host.editions', {
                    url: '/editions',
                    templateUrl: '~/App/host/views/editions/index.cshtml'
                });
            }

            if (abp.auth.hasPermission('Pages.Administration.Host.Maintenance')) {
                $stateProvider.state('host.maintenance', {
                    url: '/maintenance',
                    templateUrl: '~/App/host/views/maintenance/index.cshtml'
                });
            }

            if (abp.auth.hasPermission('Pages.Administration.Host.Settings')) {
                $stateProvider.state('host.settings', {
                    url: '/settings',
                    templateUrl: '~/App/host/views/settings/index.cshtml'
                });
            }

            //TENANT routes

            $stateProvider.state('tenant', {
                'abstract': true,
                url: '/tenant',
                template: '<div ui-view class="fade-in-up"></div>'
            });

            if (abp.auth.hasPermission('Pages.Tenant.Dashboard')) {
                $urlRouterProvider.otherwise("/tenant/dashboard"); //Entrance page for a tenant
                $stateProvider.state('tenant.dashboard', {
                    url: '/dashboard',
                    templateUrl: '~/App/tenant/views/dashboard/index.cshtml'
                });
            }

            if (abp.auth.hasPermission('Pages.Administration.Tenant.Settings')) {
                $stateProvider.state('tenant.settings', {
                    url: '/settings',
                    templateUrl: '~/App/tenant/views/settings/index.cshtml'
                });
            }
        }

        //$qProvider settings
        $qProvider.errorOnUnhandledRejections(false);
    }
]);

appModule.run(["$rootScope", "settings", "$state", 'i18nService', '$uibModalStack', function ($rootScope, settings, $state, i18nService, $uibModalStack) {
    $rootScope.$state = $state;
    $rootScope.$settings = settings;

    //$rootScope.$on('$stateChangeStart',
    //function (event, toState, toParams, fromState, fromParams) {
    //    if (toState.external) {
    //        event.preventDefault();
    //        location.href = toState.url;
    //        $uibModalStack.dismissAll();
    //    }
    //});

    $rootScope.$on('$stateChangeSuccess', function () {
        $uibModalStack.dismissAll();
    });

    //Set Ui-Grid language
    if (i18nService.get(abp.localization.currentCulture.name)) {
        i18nService.setCurrentLang(abp.localization.currentCulture.name);
    } else {
        i18nService.setCurrentLang("en");
    }

    $rootScope.safeApply = function (fn) {
        var phase = this.$root.$$phase;
        if (phase === '$apply' || phase === '$digest') {
            if (fn && (typeof (fn) === 'function')) {
                fn();
            }
        } else {
            this.$apply(fn);
        }
    };
}]);

appModule.filter('breakFilter', function () {
    return function (text) {
        if (text !== undefined) return text.replace(/\n/g, '<br />');
    };
});

/* Js custom */
var onSelectKeyElei = function (key) {
    var focusedItem = sessionStorage.getItem('focusedItem');
    if (focusedItem) {
        var item = $('#' + focusedItem);
        if (item.length > 0) {
            item.val(item.val() + key);
            item.trigger('input');
            item.focus();
        }
    }
}

var onFocusElei = function (el) {
    var _id = $(el).attr("id");
    sessionStorage.setItem('focusedItem', _id);
}