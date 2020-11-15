(function () {
    appModule.controller('common.views.layout.sidebar', [
        '$rootScope', '$scope', '$state', '$timeout',
        function ($rootScope, $scope, $state, $timeout) {
            var vm = this;
            vm.menu = abp.nav.menus.MainMenu;
            console.log(vm.menu);
            var selectThutuc = function () {
                angular.forEach(vm.menu.items, function (lv1) {
                    if (lv1.displayName == app.keyThuTuc) {
                        angular.forEach(lv1.items, function (lv2) {
                            if (app.isNullOrEmpty(lv2.url) && lv2.items[0]) {
                                lv2.isHide = true;
                                let _check = angular.copy($rootScope.currentThuTuc);
                                let _url = lv2.items[0].url;
                                if (_check < 9) _check = '0' + _check;
                                if ($rootScope.currentThuTuc == 0 && lv2.displayName.indexOf("Default") > -1) {
                                    lv2.isHide = false;
                                }
                                else if (_url.indexOf(_check) > -1) {
                                    lv2.isHide = false;
                                }
                            }
                        });
                    }
                });
            }
            selectThutuc();
            $rootScope.$watch('currentThuTuc', function () {
                $timeout(function () {
                    selectThutuc();
                });
            });

            $scope.$on('$includeContentLoaded', function () {
                setTimeout(function () {
                    Layout.initSidebar($state); // init sidebar
                    $(window).trigger('resize');
                }, 0);
            });
        }
    ]);
})();