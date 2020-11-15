/* 'app' MODULE DEFINITION */
var appLanhDaoCucModule = angular.module("appLanhDaoCuc", [
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
    'abp',
]);

/* ROUTE DEFINITIONS */

appLanhDaoCucModule.config([
    '$stateProvider', '$urlRouterProvider', '$qProvider',
    function ($stateProvider, $urlRouterProvider, $qProvider) {
        /****************END PAGE APPLICATION*********************/

        //$qProvider settings
        $qProvider.errorOnUnhandledRejections(false);
    }
]);