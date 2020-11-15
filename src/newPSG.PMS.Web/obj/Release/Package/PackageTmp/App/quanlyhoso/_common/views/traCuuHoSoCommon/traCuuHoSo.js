(function () {
    appModule.controller('quanlyhoso.thutuccommon.views.tracuuhoso', [
        '$scope', '$sce', '$stateParams', '$window', '$rootScope', '$uibModal', '$interval', '$location', '$filter',
        function ($scope, $sce, $stateParams, $window, $rootScope, $uibModal, $interval, $location, $filter) {
            var vm = this;

            var initVar = () => {
                vm.form = 'tra_cuu_ho_so';
                vm.formId = 10;
                vm.filter = {
                    formId: vm.formId,
                    formCase: 0, //0:TAT_CA, 1:CHUA_PHAN_CONG, 2:DA_PHAN_CONG, 3:DA_PHAN_CONG_TU_DONG
                    formCase2: 0,
                    page: 1,
                    pageSize: 10,

                    keyword: null,
                    ngayGuiTu: null,
                    ngayGuiToi: null,
                    loaiHoSoId: null,
                    tinhId: null,

                    //app-session
                    doanhNghiepId: null,
                    phongBanId: null
                };
                vm.thuTucIdEnum = app.sessionStorage.get("prm_thuTucIdEnum");
            }

            var init = () => {
                initVar();
            }
            init();
        }
    ]);
})();