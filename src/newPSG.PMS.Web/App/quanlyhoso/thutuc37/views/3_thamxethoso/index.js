(function () {
    appModule.controller('quanlyhoso.thutuc37.views.thamxethoso.index', [
        '$rootScope', 'appSession', 'quanlyhoso.thutuc37.services.appChuKySo',
        'abp.services.app.xuLyHoSoChuyenVien37', '$filter','$uibModal',
        function ($rootScope, appSession, appChuKySo,
            xuLyHoSoChuyenVienService, $filter, $uibModal) {
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

            vm.form = 'tham_xet_ho_so';
            vm.formId = app.FORM_ID.FORM_THAM_XET_HO_SO;
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

            if (appSession.user) {
                vm.filter.doanhNghiepId = appSession.user.doanhNghiepId;
                vm.filter.phongBanId = appSession.user.phongBanId;
            }
            vm.duyetHoSoInit = {
                soCongVan: null,
                ngayYeuCauBoSong: vm.now,
                noiDungYeuCauGiaiQuyet: null,
                noiDungCV: null,
                lyDoYeuCauBoSung: null,
                tenCanBoHoTro: null,
                dienThoaiCanBo: null,
                tenNguoiDaiDien:null,
                diaChi: null,
                soDienThoai: null,
                email: null
            };
            vm.duyetHoSo = angular.copy(vm.duyetHoSoInit);

            // ======================== control ============================//
            
            vm.summernote_options = {
                toolbar: [
                    ['style', ['bold', 'italic', 'underline', 'clear']],
                    ['view', ['fullscreen', 'codeview']],
                ],
                lineHeight: 30,
                height: 200,
                callbacks: {
                    onPaste: function (e) {
                        var bufferText = ((e.originalEvent || e).clipboardData || window.clipboardData).getData('Text');
                        console.log(bufferText, "bufferText");
                        e.preventDefault();
                        setTimeout(function () {
                            document.execCommand('insertText', false, bufferText);
                        }, 10);
                    }
                }
            };


            //=================== function =================================//

            vm.openThamXetHoSo = function (dataItem) {
                vm.dataItem = dataItem;
                if (!vm.dataItem.truongPhongDaDuyet) {
                    vm.duyetHoSo.hoSoId = vm.dataItem.id;
                    vm.duyetHoSo.ngayYeuCauBoSung = vm.now;
                    vm.duyetHoSo.noiDungCV = vm.dataItem.noiDungCV;
                    vm.duyetHoSo.tenCanBoHoTro = vm.dataItem.chuyenVienThuLyName;
                    vm.duyetHoSo.tenNguoiDaiDien = vm.dataItem.tenNguoiDaiDien;
                    vm.duyetHoSo.diaChiCoSo = vm.dataItem.diaChi;
                    vm.duyetHoSo.soDienThoai = vm.dataItem.soDienThoai;
                    vm.duyetHoSo.email = vm.dataItem.email;
                    console.log(dataItem, 'dataItem');
                }
                else {
                    vm.duyetHoSo.hoSoId = vm.dataItem.id;
                    vm.duyetHoSo.soCongVan = vm.dataItem.soCongVan;
                    vm.duyetHoSo.ngayYeuCauBoSung = vm.dataItem.ngayYeuCauBoSung
                    vm.duyetHoSo.noiDungYeuCauGiaiQuyet = vm.dataItem.noiDungYeuCauGiaiQuyet;
                    vm.duyetHoSo.lyDoYeuCauBoSung = vm.dataItem.lyDoYeuCauBoSung;
                    vm.duyetHoSo.dienThoaiCanBo = vm.dataItem.dienThoaiCanBo;
                    vm.duyetHoSo.noiDungCV = vm.dataItem.noiDungCV;
                    vm.duyetHoSo.tenCanBoHoTro = vm.dataItem.tenCanBoHoTro;
                    vm.duyetHoSo.tenNguoiDaiDien = vm.dataItem.tenNguoiDaiDien;
                    vm.duyetHoSo.diaChiCoSo = vm.dataItem.diaChi;
                    vm.duyetHoSo.soDienThoai = vm.dataItem.soDienThoai;
                    vm.duyetHoSo.email = vm.dataItem.email;
                    console.log(dataItem, 'dataItem');
                }

                

                vm.show_mode = 'tham_xet_ho_so';
            }

            var checkValidate = () => {
                if (!app.checkValidateForm("#formDuyetHoSo")) {
                    abp.notify.error("Vui lòng nhập đầy đủ thông tin");
                    return false;
                }
                if (vm.duyetHoSo.noiDungCV == null || vm.duyetHoSo.noiDungCV == '') {
                    abp.notify.error("Vui lòng nhập nội dung công văn");
                    return false;
                }
                return true;
            }

            vm.xemTruocCongVanBoSung = function () {
                if (!checkValidate()) {
                    return;
                }
                
                if (vm.dataItem) {
                    var item = {
                        hoSoId: vm.dataItem.id,
                        soCongVan: vm.duyetHoSo.soCongVan,
                        ngayYeuCauBoSung: $filter('date')(vm.duyetHoSo.ngayYeuCauBoSung,'MM/dd/yyyy'),
                        noiDungYeuCauGiaiQuyet: vm.duyetHoSo.noiDungYeuCauGiaiQuyet,
                        noiDungCV: vm.duyetHoSo.noiDungCV,
                        lyDo: vm.duyetHoSo.lyDoYeuCauBoSung,
                        tenCanBoHoTro: vm.duyetHoSo.tenCanBoHoTro,
                        dienThoaiCanBo: vm.duyetHoSo.dienThoaiCanBo,
                        tenNguoiDaiDien: vm.duyetHoSo.tenNguoiDaiDien,
                        diaChiCoSo: vm.duyetHoSo.diaChi,
                        soDienThoai: vm.duyetHoSo.soDienThoai,
                        email: vm.duyetHoSo.email,
                    };
                    console.log(item, 'itemmm');
                    appChuKySo.xemTruocCongVanBoSung(item, function () {
                    });
                }
            }

            vm.lapDoanThamDinh = function () {
                var modelInstance = $uibModal.open({
                    templateUrl: '~/App/quanlyhoso/thutuc37/views/3_thamxethoso/model/lapDoanThamDinhModel.cshtml',
                    controller: 'app.quanlyhoso.thutuc37.views.thamxethoso.model.lapdoanthamdinh as vm',
                    backdrop: 'static',
                    size: 'lg',
                    resolve: {
                        dataItem: function () {
                            return vm.dataItem;
                        }
                    }
                })
                modelInstance.result.then(function () {
                    vm.closeModal();
                    $rootScope.$broadcast('refreshGridHoSo', 'ok');
                })
            }

            vm.duyetChuyenHoSo = function () {
                if (!checkValidate()) {
                    return;
                }
               
                abp.message.confirm('', 'Chắc chắn duyệt hồ sơ bổ sung', function (isConfirmed) {
                    if (isConfirmed) {
                        abp.ui.setBusy();
                        xuLyHoSoChuyenVienService.hoSoBoSungDuyetChuyen(vm.duyetHoSo).then(function (result) {
                            abp.notify.success("Duyệt hồ sơ thành công");
                            $rootScope.$broadcast('refreshGridHoSo', 'ok');
                            vm.closeModal();
                        }).finally(function () {
                            abp.ui.clearBusy();
                        })
                    }
                })
            }

            vm.chuyenVienTraLaiHoSo = function (dataItem) {
                var modelInstance = $uibModal.open({
                    templateUrl: '~/App/quanlyhoso/thutuc37/views/3_thamxethoso/model/traLaiHoSoModel.cshtml',
                    controller: 'app.quanlyhoso.thutuc37.views.thamxethoso.model.tralaihosomodel as vm',
                    backdrop: 'static',
                    size: 'md',
                    resolve: {
                        dataItem: function () {
                            return dataItem;
                        }
                    }
                })
                modelInstance.result.then(function () {
                    vm.closeModal();
                    $rootScope.$broadcast('refreshGridHoSo', 'ok');
                })
            }

            vm.closeModal = function () {
                vm.duyetHoSo = angular.copy(vm.duyetHoSoInit);
                vm.show_mode = null;
            };
        }
    ]);
})();