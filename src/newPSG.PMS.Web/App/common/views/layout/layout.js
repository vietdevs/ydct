(function () {
    appModule.controller('common.views.layout', [
        '$scope', '$rootScope',
        function ($scope, $rootScope) {
            $scope.$on('$viewContentLoaded', function () {
                App.initComponents(); // init core components
            });
            $rootScope.$on('$stateChangeStart',
                function (event, toState, toParams, fromState, fromParams) {
                    $rootScope.tabMenu = null;
                });
        }
    ]);
})();

Date.prototype.ddMMyyyy = function () {
    var mm = this.getMonth() + 1; // getMonth() is zero-based
    var dd = this.getDate();

    return [(dd > 9 ? '' : '0') + dd,
    (mm > 9 ? '' : '0') + mm,
    this.getFullYear()
    ].join('');
};