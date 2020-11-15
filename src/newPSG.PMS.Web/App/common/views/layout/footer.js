(function () {
    appModule.controller('common.views.layout.footer', [
        '$rootScope', 'appSession',
        function ($scope, appSession) {
            var vm = this;
            vm.tenant = appSession.tenant;
            $scope.$on('$includeContentLoaded', function () {
                Layout.initFooter(); // init footer
            });

            vm.getProductNameWithEdition = function () {
                var productName = 'Cục Quản lý Dược - Bộ Y tế';
                if (appSession.tenant && appSession.tenant.editionDisplayName) {
                    productName = productName + ' ' + appSession.tenant.editionDisplayName;
                }

                return productName;
            }
        }
    ]);
})();