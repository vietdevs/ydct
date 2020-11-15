(function () {
    appModule.controller('quanlyhoso.thutuc37.views.vanthuduyetthamdinh.index', [
        '$rootScope', 'appSession', 'quanlyhoso.thutuc37.services.appChuKySo',
        'abp.services.app.xuLyHoSoVanThu37', '$filter','$uibModal',
        function ($rootScope, appSession, appChuKySo,
            xuLyHoSoVanThuService, $filter, $uibModal) {
            var vm = this;
            vm.now = new Date();
            console.log(appSession, 'appSessionappSessionappSession');
            //======================== variable ============================//
            vm.userId = appSession.user.id;
            vm.QUI_TRINH_THAM_DINH = app.QUI_TRINH_THAM_DINH;
            vm.TRANG_THAI_DUYET_NHAP = app.TRANG_THAI_DUYET_NHAP;

            function initThuTuc() {
                $rootScope.currentThuTuc = appChuKySo.THU_TUC_ID;
                $rootScope.currentMaThuTuc = appChuKySo.MA_THU_TUC;
            }
            initThuTuc();

            vm.form = 'van_thu_duyet_tham_dinh';
            vm.formId = 375;
            vm.show_mode = null; //'tham_dinh_ho_so', 'tham_dinh_ho_so_cv1', 'tham_dinh_ho_so_cv2', 'tong_hop_tham_dinh', 'tham_dinh_bo_sung', 'tham_dinh_lai'
            vm.filter = {
                formId: vm.formId,
                formCase: 1, //0: all, 1: hồ sơ thẩm định mới, 2: hồ sơ thẩm định bổ sung, 3: hồ sơ thẩm định lại, 4: hồ sơ đang theo dõi
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
            // ======================== control ============================//
            
            vm.vanThuDuyetThamDinh = function (dataItem) {
                abp.message.confirm('', 'Trả kết quả',
                    function (isConfirmed) {
                        if (isConfirmed) {
                            xuLyHoSoVanThuService.vanThuTraKetQua(dataItem.id).then(function (result) {
                                abp.notify.success('Trả kết quả thành công');
                                $rootScope.$broadcast('refreshGridHoSo', 'ok');
                            })
                        }
                })
            }

            vm.closeModal = function () {
                vm.show_mode = null;
            };
        }
    ]);
})();