(function () {
    appModule.controller('quanlyhoso.thutuc37.views.tonghopthamdinh.index', [
        '$rootScope', 'appSession', 'quanlyhoso.thutuc37.services.appChuKySo',
        'abp.services.app.xuLyHoSoChuyenVien37', '$filter','$uibModal',
        function ($rootScope, appSession, appChuKySo,
            xuLyHoSoChuyenVienService, $filter, $uibModal) {
            var vm = this;
            vm.now = new Date();
            //======================== variable ============================//
            vm.userId = appSession.user.id;
            vm.QUI_TRINH_THAM_DINH = app.QUI_TRINH_THAM_DINH;
            vm.TRANG_THAI_DUYET_NHAP = app.TRANG_THAI_DUYET_NHAP;

            function initThuTuc() {
                $rootScope.currentThuTuc = appChuKySo.THU_TUC_ID;
                $rootScope.currentMaThuTuc = appChuKySo.MA_THU_TUC;
            }
            initThuTuc();

            vm.form = 'tong_hop_tham_dinh';
            vm.formId = 372;
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

            if (appSession.user) {
                vm.filter.doanhNghiepId = appSession.user.doanhNghiepId;
                vm.filter.phongBanId = appSession.user.phongBanId;
            }

            vm.duyetHoSoInit = {
                soCongVan: null,
                ngayCongVan: null,
                ngayYeuCauBoSong: vm.now,
                noiDungYeuCauGiaiQuyet: null,
                noiDungCV: null,
                lyDoYeuCauBoSung: null,
                tenCanBoHoTro: null,
                dienThoaiCanBo: null,
                tenNguoiDaiDien: null,
                diaChi: null,
                soDienThoai: null,
                email: null
            };

            vm.duyetHoSo = angular.copy(vm.duyetHoSoInit);

            vm.duyetHoSo.headerCV = `<div>&emsp;&emsp;Ban An toàn thực phẩm TP Hồ Chí Minh đã nhận được hồ sơ đề nghị cấp Giấy chứng nhận cơ sở đủ
                        điều kiện ATTP ngày ` + ` của ` + ` cho loại hình: ` + ' ' + `. Sau khi xem xét, đối chiếu với các quy định hiện hành về an toàn thực phẩm,
                        Ban quản lý an toàn thực phẩm kết luận cơ sở không đủ điều kiện để được cấp giấy chứng nhận cơ sở đủ điều kiện an toàn thực phẩm:</div>`;

            vm.duyetHoSo.footerCV = `<div>&emsp;&emsp; Ban Quản lý An toàn thực phẩm thông báo cho cơ sở được biết để thực hiện./.</div>`;

            // ======================== control ============================//
            vm.controlGridHoSo = {};
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

            var checkValidate = () => {
                if (!app.checkValidateForm("#formDuyetHoSo")) {
                    abp.notify.error("Vui lòng nhập đầy đủ thông tin");
                    return false;
                }
                //if (vm.duyetHoSo.noiDungCV == null || vm.duyetHoSo.noiDungCV == '') {
                //    abp.notify.error("Vui lòng nhập nội dung công văn");
                //    return false;
                //}
                return true;
            }
            vm.xemTruocCongVanHoSoDat = function () {
                if (!checkValidate()) {
                    return;
                }

                if (vm.dataItem) {
                    var item = {
                        hoSoId: vm.dataItem.id,
                        soCongVan: vm.duyetHoSo.soCongVan,
                        ngayCongVan: $filter('date')(vm.duyetHoSo.ngayCongVan, 'MM/dd/yyyy'),
                        noiDungCV: vm.duyetHoSo.noiDungCV,
                        tenNguoiDaiDien: vm.duyetHoSo.tenNguoiDaiDien,
                        diaChiCoSo: vm.duyetHoSo.diaChiCoSo,
                        soDienThoai: vm.duyetHoSo.soDienThoai,
                        email: vm.duyetHoSo.email,
                    };
                    console.log(item, 'itemmm');
                    appChuKySo.xemTruocCongVanDat(item, function () {
                    });
                }
            }

            vm.xemTruocCongVanHoSoTuChoi = function () {
                if (!checkValidate()) {
                    return;
                }

                if (vm.dataItem) {
                    var item = {
                        hoSoId: vm.dataItem.id,
                        soCongVan: vm.duyetHoSo.soCongVan,
                        ngayCongVan: $filter('date')(vm.duyetHoSo.ngayCongVan, 'MM/dd/yyyy'),
                        noiDungCV: vm.duyetHoSo.noiDungCV,
                        tenNguoiDaiDien: vm.duyetHoSo.tenNguoiDaiDien,
                        diaChiCoSo: vm.duyetHoSo.diaChiCoSo,
                        soDienThoai: vm.duyetHoSo.soDienThoai,
                        email: vm.duyetHoSo.email,
                    };
                    console.log(item, 'itemmm');
                    appChuKySo.xemTruocCongVanTuChoi(item, function () {
                    });
                }
            }

            vm.xemTruocCongVanBoSung = function () {
                if (!checkValidate()) {
                    return;
                }

                if (vm.dataItem) {
                    var item = {
                        hoSoId: vm.dataItem.id,
                        soCongVan: vm.duyetHoSo.soCongVan,
                        ngayYeuCauBoSung: $filter('date')(vm.duyetHoSo.ngayYeuCauBoSung, 'MM/dd/yyyy'),
                        noiDungYeuCauGiaiQuyet: vm.duyetHoSo.noiDungYeuCauGiaiQuyet,
                        noiDungCV: vm.duyetHoSo.noiDungCV,
                        lyDo: vm.duyetHoSo.lyDoYeuCauBoSung,
                        tenCanBoHoTro: vm.duyetHoSo.tenCanBoHoTro,
                        dienThoaiCanBo: vm.duyetHoSo.dienThoaiCanBo,
                        tenNguoiDaiDien: vm.duyetHoSo.tenNguoiDaiDien,
                        diaChiCoSo: vm.duyetHoSo.diaChiCoSo,
                        soDienThoai: vm.duyetHoSo.soDienThoai,
                        email: vm.duyetHoSo.email,
                    };
                    console.log(item, 'itemmm');
                    appChuKySo.xemTruocCongVanBoSung(item, function () {
                    });
                }
            }

            vm.openTongHopThamDinh = function (dataItem) {
                vm.dataItem = dataItem;
                if (!vm.dataItem.truongPhongDaDuyet) {
                    vm.duyetHoSo.hoSoId = vm.dataItem.id;
                    vm.duyetHoSo.ngayCongVan = vm.now;
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

                xuLyHoSoChuyenVienService.xemDoanThamDinh(vm.dataItem.id).then(function (result) {
                    if (result.data) {
                        vm.listDoanThamDinh = result.data;
                    }

                })

                vm.show_mode = 'tong_hop_tham_dinh';
            }

            vm.tongHopLuu = function () {
                if (!checkValidate()) {
                    return;
                }
                let input = {
                    hoSoId: vm.dataItem.id,
                    trangThaiXuLy: vm.duyetHoSo.trangThaiXuLy
                }
                xuLyHoSoChuyenVienService.tongHopThamDinhLuu(input).then(function (result) {
                    abp.notify.success("Lưu tổng hợp thành công");
                    $rootScope.$broadcast('refreshGridHoSo', 'ok');
                    vm.closeModal();
                })
            }

            vm.chuyenTruongPhong = function () {

                if (!checkValidate()) {
                    return;
                }
                abp.message.confirm('', 'Chắc chắn duyệt hồ sơ bổ sung', function (isConfirmed) {
                    if (isConfirmed) {
                        abp.ui.setBusy();
                        xuLyHoSoChuyenVienService.tongHopThamDinhBoSungChuyen(vm.duyetHoSo).then(function (result) {
                            abp.notify.success("Duyệt hồ sơ thành công");
                            $rootScope.$broadcast('refreshGridHoSo', 'ok');
                            vm.closeModal();
                        }).finally(function () {
                            abp.ui.clearBusy();
                        })
                    }
                })
            }

            vm.capNhatKetQua = function (dataItem) {
                console.log(dataItem, 'cap nhat ket qua ho so');
                var modelInstance = $uibModal.open({
                    templateUrl: '~/App/quanlyhoso/thutuc37/views/33_tonghopthamdinh/model/capNhatKetQuaModel.cshtml',
                    controller: 'app.quanlyhoso.thutuc37.views.tonghopthamdinh.model.capnhatketquamodel as vm',
                    backdrop: 'static',
                    size: 'lg',
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