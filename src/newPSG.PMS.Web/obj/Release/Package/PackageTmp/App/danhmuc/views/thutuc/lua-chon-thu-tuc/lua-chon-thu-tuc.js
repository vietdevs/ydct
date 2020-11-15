(function () {
    appModule.controller('danhmuc.thutuc.luachonthutuc', [
        '$rootScope', '$scope', '$uibModal', 'appSession', '$timeout', '$filter', '$state', 'abp.services.app.thuTuc',
        function ($rootScope, $scope, $uibModal, appSession, $timeout, $filter, $state, thuTucService) {
            var vm = this;
            $rootScope.currentThuTuc = null;
            $rootScope.currentMaThuTuc = null;
            vm.filterText = '';
            vm.twdp = 1;
            function loadThuTuc() {
                let _req = {
                    ID: 0,
                    Filter: null,
                    MaxResultCount: 200,
                    SkipCount: 0,
                }

                abp.services.app.thuTuc.getLuaChonThuTuc(_req).then(function (data) {
                    console.log(data, 'dsThuTuc');
                
                    $timeout(function () {
                        angular.forEach(data, function (it) {
                 
                            vm.menu = abp.nav.menus.MainMenu;
                            angular.forEach(vm.menu.items, function (lv1) {
                   
                                if (lv1.displayName == app.keyThuTuc) {
                                    angular.forEach(lv1.items, function (lv2) {
                                        if (app.isNullOrEmpty(lv2.url)) {
                                            var getIdDisplay = lv2.displayName.split('-')
                                            if (lv2.displayName.indexOf(it.thuTucIdEnum) > -1 && Number(getIdDisplay[1]) == it.thuTucIdEnum) {
                                                it.thaotacthutuc = lv2.items;
                                            }
                                        }
                                    });
                                }
                            });
                        });
                        vm.danhsachthutuc = data;
                        console.log(vm.danhsachthutuc,'wqeqwe')
                        vm.allthutuc = data;
                        vm.twdpthutuc = data;
                    });
                });
            }

            vm.filterthutuc = function () {
                let _filter = vm.filterText.replace(/  /g, ' ').trim().toLowerCase();

                vm.danhsachthutuc = $filter('filter')(vm.twdpthutuc, function (item) {
                    return (_filter == '' || item.maThuTuc.toLowerCase().indexOf(_filter) > -1
                        || item.tenKhongDau.toLowerCase().indexOf(_filter) > -1
                        || item.tenThuTuc.toLowerCase().indexOf(_filter) > -1);
                });
            };

            vm.gourl = function (url) {
                //$state.go(url);
                var _url = $state.href(url, {});
                window.open(_url, '_self');
            };

            var init = function () {
                loadThuTuc();
            }
            init();

            vm.favorites = function (item) {
                thuTucService.favorites(item.id).then(function (res) {
                    console.log(res);

                    if (res.data.isError == false) {
                        abp.notify.success('Cập nhật thành công.');
                        if (item.css == 'font-yellow-crusta') {
                            item.css = 'font-default';
                        }
                        else item.css = 'font-yellow-crusta';
                    }
                    else {
                        abp.notify.error('Có lỗi xảy ra.');
                    }
                });
            }
        }
    ]);
})()