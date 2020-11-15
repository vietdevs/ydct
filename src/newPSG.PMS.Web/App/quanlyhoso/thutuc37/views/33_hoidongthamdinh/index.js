(function () {
    appModule.controller('quanlyhoso.thutuc37.views.thamdinhhoso.index', [
        '$rootScope', 'appSession', 'quanlyhoso.thutuc37.services.appChuKySo',
        'abp.services.app.xuLyHoSoChuyenVien37',
        function ($rootScope, appSession, appChuKySo,
            xuLyHoSoChuyenVienService) {
            var vm = this;

            //======================== variable ============================//
            vm.userId = appSession.user.id;
            vm.QUI_TRINH_THAM_DINH = app.QUI_TRINH_THAM_DINH;
            vm.TRANG_THAI_DUYET_NHAP = app.TRANG_THAI_DUYET_NHAP;

            function initThuTuc() {
                $rootScope.currentThuTuc = appChuKySo.THU_TUC_ID;
                $rootScope.currentMaThuTuc = appChuKySo.MA_THU_TUC;
            }
            initThuTuc();

            vm.form = 'tham_dinh_ho_so';
            vm.formId = 371;
            console.log(vm.formId, 'tham_dinh_ho_sotham_dinh_ho_so');
            vm.show_mode = null; //'tham_dinh_ho_so', 'tham_dinh_ho_so_cv1', 'tham_dinh_ho_so_cv2', 'tong_hop_tham_dinh', 'tham_dinh_bo_sung', 'tham_dinh_lai'
            vm.filter = {
                formId: vm.formId,
                formCase: '1', //0: all, 1: hồ sơ thẩm định mới, 2: hồ sơ thẩm định bổ sung, 3: hồ sơ thẩm định lại, 4: hồ sơ đang theo dõi
                formCase2: 0,  //0: getAll(), 1: hồ sơ thẩm định 1, 2: hồ sơ thẩm định 2
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
            console.log(vm.filter, 'tham_dinh_ho_sotham_dinh_ho_so');
            if (appSession.user) {
                vm.filter.doanhNghiepId = appSession.user.doanhNghiepId;
                vm.filter.phongBanId = appSession.user.phongBanId;
            }

            vm.duyetHoSo = {};

            // ======================== control ============================//
            vm.controlGridHoSo = {};
            vm.summernote_options = {
                toolbar: [
                    ['style', ['clear']]
                ],
                height: 80,
                callbacks: {
                    onPaste: function (e) {
                        var bufferText = ((e.originalEvent || e).clipboardData || window.clipboardData).getData('Text');

                        e.preventDefault();
                        setTimeout(function () {
                            document.execCommand('insertText', false, bufferText);
                        }, 10);
                    }
                }
            };


            //=================== function =================================//

            vm.openThamDinhHoSo = function (dataItem) {
                vm.dataItem = dataItem;
                console.log(dataItem, 'dataItem');
                vm.show_mode = 'tham_dinh_ho_so';
            }

            vm.chuyenTongHop = function () {
                if (!app.checkValidateForm('#thamDinhHoSo')) {
                    abp.notify.error("Nội dung ý kiến không được để trống");
                    return;
                }
                let input = {
                    hoSoId: vm.dataItem.id,
                    trangThaiXuLy: vm.duyetHoSo.trangThaiXuLy,
                    noiDungYkien: vm.duyetHoSo.noiDungYkien
                }
                console.log(input, 'input');
                xuLyHoSoChuyenVienService.thamDinhHoSo(input).then(function (result) {
                    abp.notify.success("Chuyển hồ sơ thành công");
                    vm.closeModal();
                    $rootScope.$broadcast('refreshGridHoSo', 'ok');
                })
            }

            vm.closeModal = function () {
                vm.show_mode = null;
                vm.duyetHoSo = {};
            };
        }
    ]);
})();