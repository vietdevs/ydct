(function () {
    appModule.controller('quanlyhoso.thutuc37.views.phongbanphancong.index', [
        '$rootScope', 'abp.services.app.xuLyHoSoPhanCong37',
        'appSession', 'quanlyhoso.thutuc37.services.appChuKySo','$uibModal',
        function ($rootScope, xuLyHoSoPhanCongService, appSession, appChuKySo, $uibModal) {
            var vm = this;

            function initThuTuc() {
                $rootScope.currentThuTuc = appChuKySo.THU_TUC_ID;
                $rootScope.currentMaThuTuc = appChuKySo.MA_THU_TUC;
            }
            initThuTuc();

            vm.form = 'phong_ban_phan_cong';
            vm.formId = 21;
            vm.show_mode = null; //'phong_ban_phan_cong'
            vm.closeModal = function () {
                $('#ModalPhongBanPhanCong').modal('hide');
            };

            vm.filter = {
                formId: vm.formId,
                formCase: 1, //1:DA_PHAN_CONG, 2:CHUA_PHAN_CONG
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
            vm.QT_THAMDINH = app.QUI_TRINH_THAM_DINH;

            vm.arrCheckbox = [];
            vm.updateArrCheckbox = function (arrCheckbox) {
                vm.arrCheckbox = arrCheckbox;
            };

            //--- Can Bo Phan Cong ---//

            {
                vm.cv1Options = {
                    dataValueField: "id",
                    dataTextField: "name",
                    optionLabel: app.localize('Chọn ...'),
                    filter: "contains"
                };

                vm.cv2Options = {
                    dataValueField: "id",
                    dataTextField: "name",
                    optionLabel: app.localize('Chọn ...'),
                    filter: "contains"
                };
            }

            vm.paramInit = {
                phongBanId: appSession.user.phongBanId,
                userId: appSession.user.id,
                roleLevel: appSession.user.roleLevel
            };
            
            vm.listThongKePhanCong = [];
            vm.listTruongPhong = [];
            vm.truongPhongId = null;

            vm.listChuyenVien = [];
            vm.listChuyenVien2 = [];
            vm.phanCongConfigLocalStorage = [];

            var init = function () {
                xuLyHoSoPhanCongService.initPhanCongCanBo(vm.paramInit)
                    .then(function (result) {
                        if (result.data) {
                            vm.listThongKePhanCong = result.data.listThongKePhanCong;
                            vm.listTruongPhong = result.data.listTruongPhong;
                            vm.listPhoPhong = result.data.listPhoPhong;
                            vm.truongPhongId = result.data.truongPhongId;
                            vm.phanCongInfo.truongPhongId = angular.copy(vm.truongPhongId);

                            vm.listChuyenVien = result.data.listChuyenVien;
                            var arrCv1 = angular.copy(vm.listChuyenVien);
                            setDataSource('#ddlCv1', vm.listChuyenVien);
                            if (vm.listChuyenVien && vm.listChuyenVien.length > 0) {
                                var phongBanId = undefined;
                                if (appSession.phongBan) {
                                    phongBanId = appSession.phongBan.id;
                                }
                                vm.phanCongConfigLocalStorage = getPhanCongConfig(vm.listChuyenVien, phongBanId);
                            }
                        }
                    }).finally(function () {
                        //vm.loading = false;
                    });
            };

            setTimeout(function () {
                init();
            }, 1000);

            vm.phanCongInfoInit = {
                arrHoSoId: [],
                truongPhongId: null,
                phoPhongId: null,
                chuyenVienThuLyId: null,
                chuyenVienPhoiHopId: null,
                isHoSoUuTien: null,
                isChuyenNhanh: true
            };

            vm.phanCongInfo = angular.copy(vm.phanCongInfoInit);
           
            vm.loaiHoSo = null;

            vm.openPhongBanPhanCong = function (dataItem) {

                if (dataItem) {
                    vm.loaiHoSo = dataItem.loaiHoSo;

                    vm.phanCongInfo.title = "Phân công cán bộ thẩm định - Số đăng ký: [ " + dataItem.soDangKy + " ]";

                    vm.phanCongInfo.arrHoSoId.push(dataItem.hoSoXuLyId_Active);

                    if (dataItem.chuyenVienThuLyId) {
                        vm.phanCongInfo.chuyenVienThuLyId = angular.copy(dataItem.chuyenVienThuLyId);
                    }
                    if (dataItem.truongPhongId) {
                        vm.phanCongInfo.truongPhongId = angular.copy(dataItem.truongPhongId);
                    }
                    //if (dataItem.phoPhongId && dataItem.truongPhongId) {
                    //    vm.phanCongInfo.phoPhongId = angular.copy(dataItem.phoPhongId);
                    //    vm.phanCongInfo.isChuyenNhanh = false;
                    //}
                    //else {
                    //    vm.phanCongInfo.isChuyenNhanh = true;
                    //}
                    //if (vm.phanCongInfo.chuyenVienThuLyId) {
                    //    vm.listChuyenVien2 = vm.listChuyenVien.filter(function (obj) {
                    //        return obj.id != vm.phanCongInfo.chuyenVienThuLyId;
                    //    });
                    //    var arrCv2 = angular.copy(vm.listChuyenVien2);
                    //    var _ddl = setDataSource('#ddlCv2', arrCv2);

                    //    vm.phanCongInfo.chuyenVienPhoiHopId = angular.copy(dataItem.chuyenVienPhoiHopId);
                    //    _ddl.value(vm.phanCongInfo.chuyenVienPhoiHopId);
                    //}
                } else {
                    //Not checked
                    if (vm.arrCheckbox == null || vm.arrCheckbox.length == 0) {
                        abp.notify.error('Chưa chọn đối tượng nào để phân công.');
                        return;
                    }
                    vm.loaiHoSo = vm.arrCheckbox[0].loaiHoSo;
                    vm.phanCongInfo.title = "Phân công cán bộ thẩm định - nhiều hồ sơ";

                    vm.phanCongInfo.arrHoSoId = [];
                    vm.arrCheckbox.forEach(function (item) {
                        if (item.checkbox) {
                            vm.phanCongInfo.arrHoSoId.push(item.hoSoXuLyId_Active);
                        }
                    });
                }

                //vm.show_mode = 'phong_ban_phan_cong';
                $('#ModalPhongBanPhanCong').modal('show');
            };

            vm.cv1OnChange = function () {
                vm.phanCongInfo.chuyenVienPhoiHopId = null;
                if (vm.phanCongInfo.chuyenVienThuLyId) {
                    vm.listChuyenVien2 = vm.listChuyenVien.filter(function (obj) {
                        return obj.id != vm.phanCongInfo.chuyenVienThuLyId;
                    });

                    var arrCv2 = angular.copy(vm.listChuyenVien2);
                    var _ddl = setDataSource('#ddlCv2', arrCv2);

                    var _config = vm.phanCongConfigLocalStorage.find(function (item) {
                        return item.chuyenVienThuLyId == vm.phanCongInfo.chuyenVienThuLyId;
                    });

                    if (_config && _config.chuyenVienPhoiHopId) {
                        vm.phanCongInfo.chuyenVienPhoiHopId = _config.chuyenVienPhoiHopId;
                        _ddl.value(vm.phanCongInfo.chuyenVienPhoiHopId);
                    } else {
                        if (vm.listChuyenVien2[0]) {
                            vm.phanCongInfo.chuyenVienPhoiHopId = vm.listChuyenVien2[0].id;
                            _ddl.value(vm.phanCongInfo.chuyenVienPhoiHopId);
                        }
                    }

                } else {
                    setDataSource('#ddlCv2', []);
                }
            };

            vm.luuPhongBanPhanCong = function () {

                var flag1 = vm.phanCongInfo.truongPhongId && vm.phanCongInfo.chuyenVienThuLyId;
                var flag2 = true;

                if (!(vm.phanCongInfo.isChuyenNhanh == true
                    || (vm.phanCongInfo.isChuyenNhanh != true && vm.phanCongInfo.phoPhongId))) {
                    flag2 = false;
                }

                if (!flag1 || !flag2) {
                    abp.notify.error("Mời nhập đủ dữ liệu");
                    return;
                } else {
                    console.log(vm.phanCongInfo, 'vm.phanCongInfovm.phanCongInfo');
                    //return;
                    abp.ui.setBusy();
                    xuLyHoSoPhanCongService.phanCongThamDinh(vm.phanCongInfo)
                        .then(function (result) {
                            if (result.data) {
                                //Thông kê lại
                                xuLyHoSoPhanCongService.getThongKePhanCong(vm.listChuyenVien)
                                    .then(function (result) {
                                        //Lưu lại lựa chọn cặp chuyên viên để gợi nhớ
                                        vm.phanCongConfigLocalStorage.forEach(function (item) {
                                            if (item.chuyenVienThuLyId == vm.phanCongInfo.chuyenVienThuLyId) {
                                                item.chuyenVienPhoiHopId = vm.phanCongInfo.chuyenVienPhoiHopId;
                                            }
                                        });
                                        var phongBanId = undefined;
                                        if (appSession.phongBan) {
                                            phongBanId = appSession.phongBan.id;
                                        }
                                        setPhanCongConfig(vm.phanCongConfigLocalStorage, phongBanId);

                                        if (result.data) {
                                            vm.listThongKePhanCong = result.data;
                                            abp.notify.info(app.localize('SavedSuccessfully'));
                                            vm.phanCongInfo = angular.copy(vm.phanCongInfoInit);
                                            vm.phanCongInfo.truongPhongId = angular.copy(vm.truongPhongId);
                                            $('#ModalPhongBanPhanCong').modal('hide');
                                            $rootScope.$broadcast('refreshGridHoSo', 'ok');
                                        }
                                    }).finally(function () {
                                        //vm.loading = false;
                                    });


                            }
                        }).finally(function () {
                            abp.ui.clearBusy();
                        });
                }
            };

            /***----- END List HoSo -----***/

            /*** Function ***/

            function arrExist(arrId, id) {
                if (arrId && arrId.length) {
                    for (var i = 0; i < arrId.length; i++) {
                        if (arrId[i] == id) {
                            return true;
                        }
                    }
                }
                return false;
            }

            function setDataSource(el_id, dataSourcec) {
                var _data = angular.copy(dataSourcec);
                var ddl = $(el_id).data("kendoDropDownList");
                if (ddl) {
                    ddl.setDataSource(_data);
                    ddl.dataSource.read();
                }
                return ddl;
            }

            function getPhanCongConfig(_listChuyenVien, _phongBanId) {

                var _key = "phanCongConfig" + "_phongBanId_" + _phongBanId;

                var phanCongConfig = app.localStorage.get(_key);
                if (phanCongConfig == null) {
                    if (_listChuyenVien && _listChuyenVien.length > 0) {
                        phanCongConfig = [];
                        _listChuyenVien.forEach(function (item) {
                            phanCongConfig.push({
                                chuyenVienThuLyId: item.id,
                                chuyenVienPhoiHopId: null,
                                phoPhongId: null,
                                truongPhongId: null
                            });
                        });
                        app.localStorage.set(_key, phanCongConfig);
                    }
                }
                return phanCongConfig;
            }

            function setPhanCongConfig(_phanCongConfig, _phongBanId) {

                var _key = "phanCongConfig" + "_phongBanId_" + _phongBanId;

                app.localStorage.set(_key, _phanCongConfig);
            }

            vm.phongBanTraLai = function (dataItem) {
                var modelInstance = $uibModal.open({
                    templateUrl: '~/App/quanlyhoso/thutuc37/views/21_phongbanphancong/model/traLaiHoSoModel.cshtml',
                    controller: 'app.quanlyhoso.thutuc37.views.phongbanphancong.model.tralaihosomodel as vm',
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
            /*** End Function ***/

            vm.xemBanCongBo = function (_dataItem) {
                if (_dataItem) {
                    var item = {
                        id: _dataItem.id
                    };
                    appChuKySo.xemBanCongBoDaKySo(item, function () {
                    });
                }
            };

            vm.xemHoSo = function (_dataItem) {
                if (_dataItem && _dataItem.id > 0) {
                    appChuKySo.xemKetQuaHoSo(_dataItem);
                }
            };
        }
    ]);
})();