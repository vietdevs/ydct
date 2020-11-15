(function () {
    appLanhDaoCucModule.controller('common.views.layoutLanhDaoCuc', [
        '$scope', '$rootScope',
        function ($scope, $rootScope) {
            $scope.$on('$viewContentLoaded', function () {
                App.initComponents(); // init core components
            });
        }
    ]);
})();