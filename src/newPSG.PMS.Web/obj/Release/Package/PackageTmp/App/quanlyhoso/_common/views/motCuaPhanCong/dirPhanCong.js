(function () {
    appModule.directive('quanlyhoso.thutuccommon.views.motcuaphancong.dirphancong', ['$compile', '$timeout', '$templateRequest', '$linq', 'quanlyhoso.thutuc57.services.appChuKySo',
        function ($compile, $timeout, $templateRequest, $linq, appChuKySo) {
            var controller = ['$scope', '$sce',
                function ($scope, $sce) {
                    var vm = this;
                    var initVar = () => {
                        vm.maThuTuc = $scope.hosoitem.thuTucId;
                        if (vm.maThuTuc < 10) vm.maThuTuc = '0' + vm.maThuTuc;
                        vm.phanCongInfo = {
                            isTuChoi: false,
                            isThanhToan: $scope.hosoitem.trangThaiHoSo == 2,
                            thuTucId: $scope.hosoitem.thuTucId,
                            phongBanId: $scope.hosoitem.phongBanId
                        };
                        vm.pageTitle = "Chuyển hồ sơ [" + $scope.hosoitem.maHoSo + "]";
                    }
                    initVar();
                    var appService = {
                        getViewHoSo: (hoSoId, ajaxParams) => {
                            return abp.ajax($.extend({
                                url: abp.appPath + 'api/services/app/xuLyHoSoView' + vm.maThuTuc + '/GetViewHoSo' + abp.utils.buildQueryString([{ name: 'hoSoId', value: hoSoId }]) + '',
                                type: 'POST',
                                data: JSON.stringify({})
                            }, ajaxParams));
                        },
                        loadPhanCongPhongBan: (input, ajaxParams) => {
                            return abp.ajax($.extend({
                                url: abp.appPath + 'api/services/app/xuLyHoSoPhanCong' + vm.maThuTuc + '/LoadPhanCongPhongBan',
                                type: 'POST',
                                data: JSON.stringify(input)
                            }, ajaxParams));
                        },
                        phanCongPhongBan: (input, ajaxParams) => {
                            return abp.ajax($.extend({
                                url: abp.appPath + 'api/services/app/xuLyHoSoPhanCong' + vm.maThuTuc + '/PhanCongPhongBan',
                                type: 'POST',
                                data: JSON.stringify(input)
                            }, ajaxParams));
                        },
                        tuChoiTiepNhan: function (input, ajaxParams) {
                            return abp.ajax($.extend({
                                url: abp.appPath + 'api/services/app/xuLyHoSoPhanCong' + vm.maThuTuc + '/TuChoiTiepNhan',
                                type: 'POST',
                                data: JSON.stringify(input)
                            }, ajaxParams));
                        },
                        yeuCauThanhToan: (input, ajaxParams) => {
                            return abp.ajax($.extend({
                                url: abp.appPath + 'api/services/app/xuLyHoSoPhanCong' + vm.maThuTuc + '/YeuCauThanhToan',
                                type: 'POST',
                                data: JSON.stringify(input)
                            }, ajaxParams));
                        },
                        kyGiayTiepNhanVaTraDoanhNghiep: (input, ajaxParams) => {
                            return abp.ajax($.extend({
                                url: abp.appPath + 'api/services/app/xuLyHoSoPhanCong' + vm.maThuTuc + '/KyGiayTiepNhanVaTraDoanhNghiep',
                                type: 'POST',
                                data: JSON.stringify(input)
                            }, ajaxParams));
                        }
                    };
                    var mainFunc = () => {
                        vm.loadViewHoSo = () => {
                            appService.getViewHoSo($scope.hosoitem.id)
                                .then(function (result) {
                                    $timeout(function () {
                                        vm.giayTiepNhanUrl = result.urlGiayTiepNhan + "#zoom=70";
                                        vm.dangKyHoSoUrl = result.urlBanDangKy + "#zoom=70";
                                        vm.lstTaiLieu = result.danhSachTepDinhKem;
                                        if (vm.lstTaiLieu != null && vm.lstTaiLieu.length > 0) {
                                            vm.lstTaiLieu.forEach(function (item, idx) {
                                                if (app.isNullOrEmpty(item.moTaTep)) {
                                                    item.moTaTep = "Têp đính kèm khác " + (idx + 1);
                                                }
                                            });
                                            vm.xemTaiLieu(vm.lstTaiLieu[0]);
                                        }
                                    })
                                });
                            // thong ke  ho so  phan cong cho phong ban
                            var hoSo = $scope.hosoitem;
                            let params = {
                                loaiHoSoId: hoSo.loaiHoSoId,
                                tenLoaiHoSo: hoSo.tenLoaiHoSo,
                                arrPhongBanXuLy: hoSo.arrPhongBanXuLy
                            };
                            if (params.loaiHoSoId) {
                                appService.loadPhanCongPhongBan(params)
                                    .done(function (result) {
                                        if (result && result.listThongKePhanCong) {
                                            vm.listThongKePhanCong = result.listThongKePhanCong;
                                        } else {
                                            vm.listThongKePhanCong = [];
                                        }
                                    });
                            }
                            else {
                                abp.notify.error("Loại hồ sơ bị null hoặc chưa có phòng ban nào xử lý.");
                            }
                            // danh sach phong ban phu trach ho so
                            vm.lstPhongBan = hoSo.arrPhongBanXuLy;
                        }
                        vm.xemTaiLieu = function (item) {
                            vm.lstTaiLieu.forEach(function (taiLieu) {
                                taiLieu.active = false;
                            });
                            if (item != null) {
                                item.active = true;
                                vm.taiLieuDinhKemUrl = "/File/GoToViewTaiLieu?url=" + item.duongDanTep + "#zoom=100";
                            }
                        };
                        vm.closeModal = function () {
                            $scope.formview = 'danh_sach';
                        };
                        vm.save = () => {
                            if (!app.checkValidateForm("#form-chuyen-ho-so")) {
                                abp.notify.error("Vui lòng nhập đầy đủ thông tin!");
                                return;
                            }
                            else {
                                if (vm.phanCongInfo.thuTucId == 57) {
                                    if (vm.phanCongInfo.isTuChoi) {
                                        vm.tuChoiHoSo();
                                    } else if (!vm.phanCongInfo.isThanhToan && !vm.phanCongInfo.isTuChoi) {
                                        vm.yeuCauThanhToan();
                                    }
                                    else {
                                        vm.kyGiayTiepNhanVaTraDoanhNghiep();
                                        vm.chuyenHoSo();
                                    }
                                } else {
                                    if (vm.phanCongInfo.isTuChoi) {
                                        vm.tuChoiHoSo();
                                    }
                                    else {
                                        vm.chuyenHoSo();
                                    }
                                }
                            }
                        };
                        vm.chuyenHoSo = () => {
                            let phongBanSelected = $linq.Enumerable().From(vm.lstPhongBan)
                                .Where(function (x) {
                                    return x.id == vm.phanCongInfo.phongBanId
                                }).FirstOrDefault();

                            let _req = {
                                ArrHoSoId: [$scope.hosoitem.id],
                                PhongBanId: vm.phanCongInfo.phongBanId,
                                TenPhongBan: phongBanSelected.name,
                                lyDoTuChoi: null,
                            }
                            abp.ui.setBusy();
                            appService.phanCongPhongBan(_req)
                                .done(function (result) {
                                    abp.notify.info("Chuyển hồ sơ thành công");
                                    abp.ui.clearBusy();
                                    vm.closeModal();
                                    $scope.reload()();
                                });
                        }
                        vm.tuChoiHoSo = () => {
                            abp.ui.setBusy();
                            let _req = {
                                ArrHoSoId: [$scope.hosoitem.id],
                                PhongBanId: null,
                                TenPhongBan: null,
                                lyDoTuChoi: vm.phanCongInfo.lyDoTuChoi,
                            };
                            appService.tuChoiTiepNhan(_req)
                                .done(function (result) {
                                    abp.notify.info("Từ chối hồ sơ thành công");
                                    abp.ui.clearBusy();
                                    vm.closeModal();
                                    $scope.reload()();
                                });
                        };
                        vm.yeuCauThanhToan = () => {
                            abp.ui.setBusy();
                            let _req = {
                                ArrHoSoId: [$scope.hosoitem.id],
                                PhongBanId: null,
                                TenPhongBan: null,
                                lyDoTuChoi: null,
                            };
                            appService.yeuCauThanhToan(_req)
                                .done(function (result) {
                                    abp.notify.info("Yêu cầu thanh toán hồ sơ thành công");
                                    abp.ui.clearBusy();
                                    vm.closeModal();
                                    $scope.reload()();
                                });
                        };
                        vm.kyGiayTiepNhanVaTraDoanhNghiep = () => {
                            abp.ui.setBusy();
                            appChuKySo.kySoGiayTiepNhan($scope.hosoitem, function (paramKySo) {
                                console.log(paramKySo, 'paramKySo');
                                var duyetHoSo = {};
                                duyetHoSo.hoSoXuLyId = vm.dataItem.hoSoXuLyId_Active;
                                duyetHoSo.hoSoId = $scope.hosoitem.id;
                                duyetHoSo.duongDanTepCA = paramKySo.duongDanTep;
                                duyetHoSo.giayTiepNhanCA = paramKySo.giayTiepNhanCA;
                                duyetHoSo.soTiepNhan = paramKySo.soTiepNhan;
                                vm.saving = true;
                                //Chuyển đơn vị xử lý
                                appService.kyGiayTiepNhanVaTraDoanhNghiep(duyetHoSo)
                                    .then(function (result) {
                                        abp.notify.info(app.localize('SavedSuccessfully'));

                                        $rootScope.$broadcast('refreshGridHoSo', 'ok');
                                        vm.show_mode = null;

                                        appChuKySo.xemFilePDF(paramKySo.giayTiepNhanCA, 'Giấy chứng nhận đã ký số');
                                    }).finally(function () {
                                        vm.saving = false;
                                    });
                            });
                        };
                    }
                    $scope.$watch("hosoitem", function () {
                        if ($scope.hosoitem) {
                            vm.loadViewHoSo();
                        }
                    });
                    var init = () => {
                        mainFunc();
                    }
                    init();
                    vm.trustSrc = function (src) {
                        return $sce.trustAsResourceUrl(src);
                    }
                }]
            return {
                restrict: 'EA',
                scope: {
                    hosoitem: '=?',
                    formview: '=?',
                    reload: '&',
                },
                controller: controller,
                controllerAs: 'vm',
                bindToController: false,
                link: function (scope, elem, attr, ctrl) {
                    $templateRequest("/App/quanlyhoso/_common/views/motCuaPhanCong/dirPhanCong.html").then(function (html) {
                        var template = angular.element(html);
                        elem.append(template);
                        $compile(template)(scope);
                    });
                }
            };
        }
    ]);
})();