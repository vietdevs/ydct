(function () {
    appModule.controller('tenant.views.dashboard.trangchu', [
        '$scope', '$timeout', '$window', '$state', '$http', '$filter', 'appSession',
        function ($scope, $timeout, $window, $state, $http, $filter, appSession) {
            var vm = this;
            var initVar = () => {
                vm.ROLE_LEVEL = app.ROLE_LEVEL;
                vm.session = appSession;
                vm.roleLv = appSession.user.roleLevel;
                vm.isAdmin = (appSession.user.userName == 'admin' || vm.roleLv == vm.ROLE_LEVEL.SA);

                if (appSession.user.roleLevel == vm.ROLE_LEVEL.VAN_THU) {
                    vm.motCuaPhanCongDashBoard = abp.auth.hasPermission('Pages.ThuTucDefault.MotCuaPhanCong');
                    vm.vanThuDuyetDashBoard = abp.auth.hasPermission('Pages.ThuTucDefault.VanThuDuyet');
                }
                else if (appSession.user.roleLevel == vm.ROLE_LEVEL.LANH_DAO_CUC) {
                    vm.lanhDaoCucDashBoard = abp.auth.hasPermission('Pages.ThuTucDefault.LanhDaoCucDuyet');
                    vm.showDanhSachThuTuc = true;
                }
                else {
                    vm.showDanhSachThuTuc = true;
                }
            }
            var mainFun = () => {
                vm.gourl = function (url) {
                    $state.go(url);
                };
                vm.chonThuTuc = function () {
                    $window.location.href = '/Application#!/luachonthutuc';
                };
            };
            var init = () => {
                initVar();
                mainFun();
            };
            init();
        }
    ]);
})();