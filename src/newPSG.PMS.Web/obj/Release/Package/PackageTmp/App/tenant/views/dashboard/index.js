(function () {
    appModule.controller('tenant.views.dashboard.index', [
        '$rootScope', '$scope', '$timeout', '$window', '$state', '$http', '$filter', 'appSession',
        function ($rootScope, $scope, $timeout, $window, $state, $http, $filter, appSession) {
            var vm = this;
            var initTab = () => {
                var tabs = [
                    {
                        title: `<i class="fa fa-home" aria-hidden="true"></i> Trang chủ`,
                        content: '<div app.directives.dirpages.trangchu></div>',
                        menulabel: 'trangchu',
                    },
                ],
                    selected = null,
                    previous = null;
                $scope.tabs = tabs;
                $scope.selectedIndex = 0;
                $scope.$watch('selectedIndex', function (current, old) {
                    previous = selected;
                    selected = tabs[current];
                });
                $scope.addTab = function (_tabMenu) {
                    $window.scrollTo(0, 0);
                    _check = -1;
                    angular.forEach(tabs, function (it, idx) {
                        if (it.menulabel == _tabMenu.menulabel) {
                            _check = idx;
                            return;
                        }
                    });
                    if (_check > -1) {
                        vm.tabCurrent = _check;
                        $scope.selectedIndex = _check;
                        abp.ui.clearBusy();
                        return;
                    }
                    tabs.splice(1, 0, _tabMenu);
                    vm.tabCurrent = 1;
                    $rootScope.currentThuTuc = tabs[1].currentThuTuc;
                    $rootScope.currentMaThuTuc = tabs[1].currentMaThuTuc;
                };
                $scope.removeTab = function (tab) {
                    var index = tabs.indexOf(tab);
                    tabs.splice(index, 1);
                    vm.tabCurrent = 0;
                    $scope.selectedIndex = 0;
                    $rootScope.currentThuTuc = tabs[0].currentThuTuc;
                    $rootScope.currentMaThuTuc = tabs[0].currentMaThuTuc;
                };
                vm.tabCurrent = 0;
                vm.changeTabMenu = (idx) => {
                    vm.tabCurrent = idx;
                    $rootScope.currentThuTuc = tabs[idx].currentThuTuc;
                    $rootScope.currentMaThuTuc = tabs[idx].currentMaThuTuc;
                };
            }

            var initWatch = () => {
                $rootScope.$watch("tabMenu", function () {
                    if (!app.isNullOrEmpty($rootScope.tabMenu)) {
                        _tab = angular.copy($rootScope.tabMenu);
                        $scope.addTab(_tab);
                    }
                });
            }
            var init = () => {
                initTab();
                initWatch();
            };
            init();
        }
    ]);
})();