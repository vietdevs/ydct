(function () {
    appModule.controller('quanlyhoso.thutuc99.views.lanhdaoboduyet.index', [
        '$rootScope', '$sce',
        'abp.services.app.xuLyHoSoLanhDaoBo99',
        'appSession', 'quanlyhoso.thutuc99.services.appChuKySo',
        function ($rootScope, $sce,xuLyHoSoLanhDaoBoService, appSession, appChuKySo) {
            var vm = this;

            function initThuTuc() {
                $rootScope.currentThuTuc = appChuKySo.THU_TUC_ID;
                $rootScope.currentMaThuTuc = appChuKySo.MA_THU_TUC;
            }
            initThuTuc();

            vm.form = 'lanh_dao_bo_duyet';
            vm.formId = 6;
            vm.show_mode = null; //'lanh_dao_bo_duyet'
            vm.saving = false;
            vm.closeModal = function () {
                vm.show_mode = null;
            }

            vm.filter = {
                formId: vm.formId,
                formCase: 1, //0: getAll(), 1: hồ sơ chưa duyệt, 2: hồ sơ đã duyệt và đang theo dõi
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

            if (appSession.user) {
                vm.filter.doanhNghiepId = appSession.user.doanhNghiepId;
                vm.filter.phongBanId = appSession.user.phongBanId;
            }

            //Common
            vm.showLichSu = false;
            vm.toggleLichSu = function () {
                vm.showLichSu = !vm.showLichSu;
            };

            //Xủ lý nhiều hồ sơ
            vm.arrCheckbox = [];
            vm.updateArrCheckbox = function (arrCheckbox) {
                vm.arrCheckbox = arrCheckbox;
            };

            //Begin

            //*** Function ***
            vm.trustSrc = function (src) {
                return $sce.trustAsResourceUrl(src);
            };
        }
    ]);
})();