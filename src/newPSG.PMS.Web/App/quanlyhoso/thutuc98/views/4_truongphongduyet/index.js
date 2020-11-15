(function () {
    appModule.controller('quanlyhoso.thutuc98.views.truongphongduyet.index', [
        '$scope', '$rootScope', '$window', '$uibModal', '$interval', '$filter', '$sce',
        'abp.services.app.xuLyHoSoTruongPhong98',
        'appSession', 'quanlyhoso.thutuc98.services.appChuKySo',
        function ($scope, $rootScope, $window, $uibModal, $interval, $filter, $sce,
            xuLyHoSoTruongPhongService,
            appSession, appChuKySo) {
            var vm = this;
            vm.DON_VI_XU_LY = app.DON_VI_XU_LY;
            vm.QUI_TRINH_THAM_DINH = app.QUI_TRINH_THAM_DINH;

            function initThuTuc() {
                $rootScope.currentThuTuc = appChuKySo.THU_TUC_ID;
                $rootScope.currentMaThuTuc = appChuKySo.MA_THU_TUC;
            }
            initThuTuc();

            // ---------------------------------variables------------------------------// 

            vm.dataItem = {};
            vm.form = 'truong_phong_duyet';
            vm.formId = 4;
            vm.show_mode = null; //'truong_phong_duyet'
            vm.saving = false;
            vm.closeModal = function () {
                vm.show_mode = null;
            };
            vm.arrCheckbox = [];
            vm.filter = {
                formId: vm.formId,
                formCase: 1, //0: getAll(), 1: hồ sơ chưa duyệt, 2: hồ sơ đã duyệt và đang theo dõi
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

            if (appSession.user) {
                vm.filter.doanhNghiepId = appSession.user.doanhNghiepId;
                vm.filter.phongBanId = appSession.user.phongBanId;
            }
           

            //-------------------controll on view ------------------------//

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

            // ---------------- function -----------------------------//

            vm.openTruongPhongDuyet = function (dataItem) {

                vm.dataItem = dataItem;
                vm.show_mode = 'truong_phong_duyet';
                
            };

            vm.duyetHoSo = function () {
                vm.saving = true;
                let input = {
                    HoSoXuLyId: vm.dataItem.hoSoXuLyId,
                    HoSoId: vm.dataItem.id
                }
                abp.message.confirm("Chắc chắn duyệt hồ sơ ?", "Duyệt hồ sơ", function (isConfirmed) {
                    if (isConfirmed) {
                        xuLyHoSoTruongPhongService.truongPhongDuyet(input)
                            .then(function (result) {
                                if (result.data) {
                                    vm.saving = false;
                                    abp.notify.info(app.localize('SavedSuccessfully'));
                                    $rootScope.$broadcast('refreshGridHoSo', 'ok');
                                    vm.show_mode = null;
                                }
                                else {
                                    abp.notify.error(app.localize("Có vấn đề trong quá trình sử lý , vui lòng liên hệ quản trị viên"));
                                }

                            }).finally(function () {
                                //vm.loading = false;
                            });
                    }
                })
                
            };

        }
    ]);
})();