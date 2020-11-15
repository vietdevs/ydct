(function () {
    appModule.directive('quanlyhoso.thutuc99.directives.viewtailieu', ['$compile', '$sce', '$templateRequest',
        'abp.services.app.xuLyHoSoView99',
        'baseService', 'appSession',
        function ($compile, $sce, $templateRequest,
            xuLyHoSoViewService,
            baseService, appSession) {
            var controller = ['$scope', function ($scope) {
                var vm = this;
                vm.dataItem = $scope.hoso;

                vm.dangKyUrl = "";
                vm.taiLieuDinhKemUrl = "";
                vm.congVanUrl = "";

                vm.lstTaiLieu = [];
                vm.lstCongVan = [];

                vm.xemTaiLieu = function (item) {
                    vm.lstTaiLieu.forEach(function (taiLieu) {
                        taiLieu.active = false;
                    });
                    if (item.duongDanTep != null) {
                        item.active = true;
                        if (item.duongDanTep && item.duongDanTep.includes("readfile?id")) {
                            vm.taiLieuDinhKemUrl = item.duongDanTep + "#zoom=100";
                        } else {
                            vm.taiLieuDinhKemUrl = "/File/GoToViewTaiLieu?url=" + item.duongDanTep + "#zoom=100";
                        }
                    }
                    else
                        vm.taiLieuDinhKemUrl = "";
                };

                vm.xemCongVanBoSung = function (item) {
                    if (vm.lstCongVan.length > 0) {
                        vm.lstCongVan.forEach(function (taiLieu) {
                            taiLieu.active = false;
                        });
                        if (item != null) {
                            item.active = true;
                            if (!baseService.isNullOrEmpty(item.duongDanTepCA) && item.lanhDaoCucIsCA == true) {
                                vm.congVanUrl = "/File/GoToViewTaiLieu?url=" + item.duongDanTepCA + "#zoom=70";
                            }
                            else {
                                vm.congVanUrl = "/Report99/TemplateCongVan?hoSoId=" + item.hoSoId + "#zoom=70";
                            }
                        }
                    }
                };

                vm.hoSo = {};
                var init = function () {
                    xuLyHoSoViewService.getViewHoSo(vm.dataItem.id).then(function (result) {
                        if (result.data) {
                            vm.dangKyUrl = result.data.urlBanDangKy + "#zoom=68";
                            vm.lstTaiLieu = result.data.danhSachTepDinhKem;

                            if (result.data.hoSo) {
                                vm.hoSo = result.data.hoSo;
                            }
                            else {
                                vm.hoSo = {};
                            }

                            if (vm.lstTaiLieu != null && vm.lstTaiLieu.length > 0) {
                                vm.lstTaiLieu.forEach(function (item, idx) {
                                    if (baseService.isNullOrEmpty(item.moTaTep)) {
                                        item.moTaTep = "Têp đính kèm khác " + (idx + 1);
                                    }
                                });
                                vm.xemTaiLieu(vm.lstTaiLieu[0]);
                            }

                            //Biên bản thẩm định
                            if (appSession.user.roleLevel != app.ROLE_LEVEL.BO_PHAN_MOT_CUA) {
                                vm.bienBanThamDinhUrl = "/Report99/TemplateBienBanThamDinh?hoSoId=" + vm.dataItem.id + "#zoom=70";
                            }

                            var _listCongVan = result.data.danhSachCongVan;
                            if (_listCongVan != null && _listCongVan.length > 0) {
                                _listCongVan.forEach(function (item, idx) {
                                    item.active = false;
                                    var str_date = "(" + new Date(item.ngayTra).getDate() + "/" + (new Date(item.ngayTra).getMonth() + 1) + "/" + new Date(item.ngayTra).getFullYear() + ")";
                                    if (idx + 1 == _listCongVan.length) {
                                        _listCongVan[idx].displayText = "Công văn " + str_date;
                                    } else {
                                        item.displayText = "Thông báo " + str_date;
                                    }
                                    if (idx == 0) {
                                        item.active = true;
                                        if (!baseService.isNullOrEmpty(item.duongDanTepCA) && item.lanhDaoCucIsCA == true) {
                                            vm.congVanUrl = "/File/GoToViewTaiLieu?url=" + item.duongDanTepCA + "#zoom=70";
                                        }
                                        else {
                                            vm.congVanUrl = "/Report99/TemplateCongVan?hoSoId=" + item.hoSoId + "#zoom=70";
                                        }
                                    }
                                });

                            }
                            vm.lstCongVan = _listCongVan;
                            vm.activeIndex = vm.lstCongVan.length > 0 ? 0 : 1;
                        }
                    });
                };
                init();

                //Function
                vm.trustSrc = function (src) {
                    return $sce.trustAsResourceUrl(src);
                };
            }];
            return {
                restrict: 'EA',
                scope: {
                    hoso: '='
                },
                controller: controller,
                controllerAs: 'vm',
                bindToController: false,
                link: function (scope, elem, attr, ctrl) {
                    $templateRequest("/App/quanlyhoso/thutuc99/directives/viewTaiLieu.cshtml").then(function (html) {
                        var template = angular.element(html);
                        elem.append(template);
                        $compile(template)(scope);
                    });
                }
            };
        }
    ]);
})();