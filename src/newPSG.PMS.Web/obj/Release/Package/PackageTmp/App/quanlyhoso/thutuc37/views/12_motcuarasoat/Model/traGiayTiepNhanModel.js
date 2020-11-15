(function () {
    appModule.controller('app.quanlyhoso.thutuc37.views.motcuarasoat.model.tragiaytiepnhan', [
        '$sce', '$rootScope', 'appSession', 'quanlyhoso.thutuc37.services.appChuKySo',
        'abp.services.app.xuLyHoSoVanThu37', '$uibModalInstance', 'dataItem', '$uibModal', 'abp.services.app.xuLyHoSoDoanhNghiep37', '$filter', 'abp.services.app.xuLyHoSoView37',
        function ($sce, $rootScope, appSession, appChuKySo, xuLyHoSoVanThuService, $uibModalInstance, dataItem, $uibModal, xuLyHoSoDoanhNghiep37, $filter, xuLyHoSoViewService) {

            var vm = this;
            vm.ngayHenCap = null;
            var tinhNgayHenCap = () => {
                let now = new Date();
                let dateAfter20 = new Date(now.getFullYear(), now.getMonth(), now.getDate() + 20); // 20 ngày tính cả cuối tuần


                // 10 ngày không tính cuối tuần
                let days10 = null;
                let totalDayWithOutWeekend = 0;
                let datePlus = 1;
                for (let i = 0; i < 10; i++) {
                    totalDayWithOutWeekend++;
                    days10 = new Date(dateAfter20.getFullYear(), dateAfter20.getMonth(), dateAfter20.getDate() + datePlus);
                    if (days10.getDay() == 6 || days10.getDay() == 0) {
                        totalDayWithOutWeekend += 1;
                    }
                    datePlus++;
                }

                let finalDate = new Date(dateAfter20.getFullYear(), dateAfter20.getMonth(), dateAfter20.getDate() + totalDayWithOutWeekend);
                return finalDate;
            }

            vm.LOAI_TAI_LIEU_DINH_KEM = app.TT37_LOAI_TAI_LIEU_DINH_KEM;
            vm.hinhThucCapChungChiInit = [
                {
                    id:1,
                    name: 'Cấp lần đầu ',
                    value: true
                },
                {
                    id: 2,
                    name: 'Cấp thay đổi nội dung ',
                    value: false
                },
                {
                    id: 3,
                    name: 'Cấp lại ',
                    value: false
                }
            ]
            vm.hinhThucCapChungChi = angular.copy(vm.hinhThucCapChungChiInit);

            vm.listTaiLieuDaNhanInit = [
                {
                    STT: 1,
                    name: 'Đơn đề nghị cấp, cấp lại chứng chỉ hành nghề khám bệnh, chữa bệnh ',
                    value: false
                },
                {
                    STT: 2,
                    name: 'Bản sao hợp lệ văn bằng chuyên môn ',
                    loaiTepDinhKem: vm.LOAI_TAI_LIEU_DINH_KEM.VAN_BANG_CHUYEN_MON,
                    value: false
                },
                {
                    STT: 3,
                    name: 'Văn bản xác nhận quá trình thực hành ',
                    loaiTepDinhKem: vm.LOAI_TAI_LIEU_DINH_KEM.QUA_TRINH_THUC_HANH,
                    value: false
                },
                {
                    STT: 4,
                    name: 'Phiếu lý lịch tư pháp ',
                    loaiTepDinhKem: vm.LOAI_TAI_LIEU_DINH_KEM.LY_LICH_TU_PHAP,
                    value: false
                },
                {
                    STT: 5,
                    name: 'Sơ yếu lý lịch tự thuật ',
                    value: false
                },
                {
                    STT: 6,
                    name: 'Giấy chứng nhận sức khỏe',
                    loaiTepDinhKem: vm.LOAI_TAI_LIEU_DINH_KEM.DU_DIEU_KIEN_SUC_KHOE,
                    value: false
                },
                {
                    STT: 7,
                    name: `Bản sao hợp lệ giấy chứng nhận biết tiếng Việt thành thạo hoặc giấy chứng nhận sử dụngthành thạo ngôn ngữ khác hoặc giấy chứng nhận đủ trình độ phiên dịch trong khám bệnh, chữa bệnh( đối với người nước ngoài, người Việt Nam định cư ở nước ngoài)`,
                    value: false
                },
                {
                    STT: 8,
                    name: 'Bản sao hợp lệ giấy phép lao động (đối với người nước ngoài, người Việt Nam định cư ở nước ngoài) ',
                    value: false
                },
                {
                    STT: 9,
                    name: 'Bản sao hợp lệ chứng chỉ hành nghề đã được cấp (đối với cấp bổ sung PVHĐ chuyên môn) ',
                    value: false
                },
                {
                    STT: 10,
                    name: 'Bản gốc chứng chỉ hành nghề đã được cấp ',
                    value: false
                },
                {
                    STT: 11,
                    name: 'Bản sao hợp lệ giấy chứng nhận cập nhật kiến thức y khoa liên tục ',
                    value: false
                },
                {
                    STT: 12,
                    name: 'Văn bản xác nhận của cơ quan có thẩm quyền về việc thay đổi ngày tháng năm sinh hoặc địa chỉ cư trú ',
                    value: false
                },
                {
                    STT: 13,
                    name: 'Hai ảnh màu (nền trắng) 04 cm x 06 cm ',
                    loaiTepDinhKem: vm.LOAI_TAI_LIEU_DINH_KEM.ANH_4X6,
                    value: false
                },
            ]

            vm.listTaiLieuDaNhan = angular.copy(vm.listTaiLieuDaNhanInit);

            var checkValidate = () => {
                let hinhThucChoosed = false;
                let taiLieuChoosed = false;
                vm.hinhThucCapChungChi.forEach(function (item) {
                    if (item.value) {
                        hinhThucChoosed = true;
                        return;
                    }
                })
                vm.listTaiLieuDaNhan.forEach(function (item) {
                    if (item.value) {
                        taiLieuChoosed = true;
                        return;
                    }
                })
                if (hinhThucChoosed == false || taiLieuChoosed == false) {
                    abp.notify.error("Vui lòng điền đẩy đủ thông tin");
                    return false;
                }

                if (vm.ngayHenCap == null || vm.ngayHenCap == '') {
                    abp.notify.error("Ngày hẹn cấp không được để trống");
                    return false;
                }
                return true;
            }

            vm.xemPhieuTiepNhan = function () {
                let validate = checkValidate();
                if (!validate) {
                    return;
                }
                let modalData = {
                    HoSoId: vm.dataItem.id,
                    NgayHenCap: vm.ngayHenCap,
                    HinhThucCapChungChi: vm.hinhThucCapChungChi,
                    ListTaiLieuDaNhan: vm.listTaiLieuDaNhan
                }
                abp.ui.setBusy();
                xuLyHoSoVanThuService.goToViewPhieuTiepNhan(modalData).then(function (result) {
                    if (result) {
                        console.log(result, 'resultresultresult');
                        $uibModal.open({
                            templateUrl: '~/App/quanlyhoso/thutuc37/directives/modal/viewPdf.cshtml',
                            controller: 'app.quanlyhoso.thutuc37.directives.modal.viewpdf as vm',
                            backdrop: 'static',
                            size: 'lg',
                            resolve: {
                                base64: function () {
                                    return result.data
                                }
                            }
                        });
                    }
                }).finally(function () { abp.ui.clearBusy(); })
                
            }
            vm.save = function () {
                
                let validate = checkValidate();
                if (!validate) {
                    return;
                }

                let input = {
                    hoSoId: vm.dataItem.id,
                    ngayHenCap: $filter('date')(vm.ngayHenCap,'MM/dd/yyyy'),
                    hinhThucCapChungChi: vm.hinhThucCapChungChi,
                    listTaiLieuDaNhan: vm.listTaiLieuDaNhan,
                }

                appChuKySo.kyPhieuTiepNhan(input, function (param) {
                    var dataInput = {
                        HoSoId: vm.dataItem.id,
                        PhiDaNop: vm.dataItem.phiDaNop,
                        NgayHenCap: vm.ngayHenCap,
                        HinhThucCapChungChi: vm.hinhThucCapChungChi,
                        ListTaiLieuDaNhan: vm.listTaiLieuDaNhan,
                        GiayTiepNhanCA: param.duongDanTep
                    }
                    abp.ui.setBusy();
                    xuLyHoSoVanThuService.vanThuKyVaTraGiayTiepNhan(dataInput).then(function (item) {
                        abp.notify.success("Trả giấy tiếp nhận thành công");
                        vm.cancel(1, param.duongDanTep);
                    }).finally(function () {
                        abp.ui.clearBusy();
                    })
                })

            }

            vm.cancel = function (status,duongDanTep) {
                vm.ngayHenCap = null;
                vm.hinhThucCapChungChi = angular.copy(vm.hinhThucCapChungChiInit);
                vm.listTaiLieuDaNhan = angular.copy(vm.listTaiLieuDaNhanInit);
                if (status) {
                    $uibModalInstance.close(duongDanTep);
                }
                $uibModalInstance.dismiss();
            }

            var init = () => {
                if (dataItem.id > 0) {
                    vm.dataItem = dataItem;
                    xuLyHoSoDoanhNghiep37.getHoSoById(vm.dataItem.id).then(function (result) {
                        vm.attachFiles = result.data.teps;
                        vm.listTaiLieuDaNhan.forEach(function (item) {
                            let itemExisted = vm.attachFiles.find(x => x.loaiTepDinhKem == item.loaiTepDinhKem);
                            if (itemExisted) {
                                item.value = true;
                            }
                        })
                        result.data.teps.forEach(function (item) {
                            if (item.loaiTepDinhKem == vm.LOAI_TAI_LIEU_DINH_KEM.TAI_LIEU_KHAC) {
                                vm.listTaiLieuDaNhan.push({
                                    STT: vm.listTaiLieuDaNhan.length + 1,
                                    name: item.moTaTep != null ? item.moTaTep:'Tài liệu khác',
                                    value: true
                                });
                            }
                        })
                    })
                    vm.ngayHenCap = tinhNgayHenCap();
                    xuLyHoSoViewService.getViewHoSo(vm.dataItem.id).then(function (result) {
                        if (result.data) {
                            var _listCongVanYeuCauBoSung = result.data.danhSachCongVan;
                            if (_listCongVanYeuCauBoSung.length > 0)
                                vm.soLanBoSung = _listCongVanYeuCauBoSung.length;
                        }
                    })
                }
            }
            init();
        }
    ])

})();