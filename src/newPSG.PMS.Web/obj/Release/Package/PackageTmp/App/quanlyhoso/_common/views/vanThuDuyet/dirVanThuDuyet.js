(function () {
    var initControllerDongDauChung = (itemMa) => {
        var controller = ['$scope', '$rootScope', '$sce', '$uibModal', '$interval', '$filter',
            'abp.services.app.xuLyHoSoVanThu' + itemMa,
            'appSession', 'quanlyhoso.thutuc' + itemMa + '.services.appChuKySo'];

        console.log("controller", controller);

        controller.push(function ($scope, $rootScope, $sce, $uibModal, $interval, $filter,
            xuLyHoSoVanThuService,
            appSession, appChuKySo) {
            var vm = this;
            vm.dirviewhistory = `<quanlyhoso.thutuc` + itemMa + `.directives.viewhistory hosoid="vm.dataItem.id" show="vm.showLichSu"/>`;

            vm.form = 'van_thu_duyet';
            vm.formId = 7;
            vm.show_mode = null; //'van_thu_duyet'

            vm.closeModal = function () {
                $scope.viewform = 'danh_sach';
            }
            vm.closeAndReload = () => {
                $scope.viewform = 'danh_sach_reload';
            }

            //Common
            vm.showLichSu = false;
            vm.toggleLichSu = function () {
                vm.showLichSu = !vm.showLichSu;
            }

            //Begin
            vm.objInfo = {
                tenTruongPhong: null,
                tenLanhDaoCuc: null
            };
            vm.hoSoXuLy = {};
            vm.duyetHoSo = {
                hoSoXuLyId: null,
                trangThaiCV: null,
                donViKeTiep: null,
                yKien: null,
                isCoSaiSot: null
            }

            vm.hoSoXuLy_Reset = function () {
                vm.hoSoXuLy = {};
                vm.duyetHoSo = {
                    hoSoXuLyId: null,
                    trangThaiCV: null,
                    donViKeTiep: null,
                    yKien: null,
                    isCoSaiSot: null
                }
            }

            vm.dataItem = {};
            vm.openVanThuDuyet = function (dataItem) {
                vm.dataItem = dataItem;
                var _id = dataItem.hoSoXuLyId_Active;
                if (_id > 0) {
                    //RESET-INFO
                    vm.hoSoXuLy_Reset();

                    var params = {
                        HoSoXuLyId: _id
                    }
                    xuLyHoSoVanThuService.loadVanThuDuyet(params)
                        .then(function (result) {
                            if (result.data) {
                                vm.hoSoXuLy = result.data.hoSoXuLy;
                                vm.objInfo = result.data.objInfo;

                                vm.duyetHoSo.hoSoXuLyId = vm.hoSoXuLy.id;
                                vm.duyetHoSo.HoSoId = dataItem.id;

                                vm.duongDanTepCA = "/File/GoToViewTaiLieu?url=" + vm.hoSoXuLy.duongDanTepCA;
                                if (vm.hoSoXuLy.hoSoIsDat) {
                                    vm.giayTiepNhanCA = "/File/GoToViewTaiLieu?url=" + vm.hoSoXuLy.giayTiepNhanCA + "#zoom=105";
                                }

                                vm.show_mode = 'van_thu_duyet';
                            }
                        });
                }
            }

            vm.dongDauGiayTo = function () {
                abp.message.confirm(app.localize(""),
                    app.localize('Bạn chắc chắn muốn đóng dấu văn bản?'),
                    function (isConfirmed) {
                        if (isConfirmed) {
                            var params = {
                                id: vm.dataItem.id,
                                hoSoIsDat: vm.dataItem.hoSoIsDat
                            }
                            //Ký số
                            appChuKySo.vanThuDongDau(params, function (paramKySo) {
                                console.log(paramKySo, 'paramKySo');

                                var params = {
                                    hoSoId: vm.dataItem.id,
                                    hoSoXuLyId: vm.dataItem.hoSoXuLyId_Active,
                                    duongDanTepCA: paramKySo.duongDanTep,
                                    giayTiepNhanCA: paramKySo.giayTiepNhanCA
                                }

                                vm.saving = true;
                                xuLyHoSoVanThuService.dongDau(params)
                                    .then(function (result) {
                                        abp.notify.success(app.localize('Đóng dấu thành công'));
                                        vm.saving = false;
                                        vm.closeAndReload();
                                        if (vm.dataItem.hoSoIsDat == true) {
                                            appChuKySo.xemFilePDF(paramKySo.giayTiepNhanCA, 'Giấy tiếp nhận đã ký số');
                                        }
                                        else {
                                            appChuKySo.xemFilePDF(paramKySo.duongDanTep, 'Công văn đã ký số');
                                        }
                                    })
                            });
                        }
                    }
                );
            }

            vm.chuyenLaiTruongPhong = function () {
                vm.saving = true;
                if (!app.checkValidateForm("#form-van-thu-duyet")) {
                    abp.notify.error("Vui lòng nhập đầy đủ thông tin!");
                    vm.saving = false;
                    return;
                } else {
                    xuLyHoSoVanThuService.baoCaoSaiSot(vm.duyetHoSo)
                        .then(function (result) {
                            if (result) {
                                vm.saving = false;
                                abp.notify.success(app.localize('SavedSuccessfully'));
                                vm.closeAndReload();
                            }
                        })
                }
            }
            //*** Function ***
            vm.trustSrc = function (src) {
                return $sce.trustAsResourceUrl(src);
            }

            $scope.$watch("hosoitems", function () {
                vm.openVanThuDuyet($scope.hosoitems);
            });
        });
        return controller;
    }
    appModule.directive('app.quanlyhoso.common.vanthuduyet.dongdauchung.18', ['$compile', '$timeout', '$templateRequest',
        function ($compile, $timeout, $templateRequest) {
            return {
                restrict: 'EA',
                replace: true,
                scope: {
                    hosoitems: '=?',
                    viewform: '=?',
                },
                controller: initControllerDongDauChung(18),
                controllerAs: 'vm',
                bindToController: false,
                link: function (scope, elem, attr, ctrl) {
                    $templateRequest("/App/quanlyhoso/_common/views/vanThuDuyet/dirVanThuDuyet.html").then(function (html) {
                        var template = angular.element(html);
                        elem.append(template);
                        $compile(template)(scope);
                    });
                }
            };
        }
    ]);
    appModule.directive('app.quanlyhoso.common.vanthuduyet.dongdauchung.19', ['$compile', '$timeout', '$templateRequest',
        function ($compile, $timeout, $templateRequest) {
            return {
                restrict: 'EA',
                replace: true,
                scope: {
                    hosoitems: '=?',
                    viewform: '=?',
                },
                controller: initControllerDongDauChung(19),
                controllerAs: 'vm',
                bindToController: false,
                link: function (scope, elem, attr, ctrl) {
                    $templateRequest("/App/quanlyhoso/_common/views/vanThuDuyet/dirVanThuDuyet.html").then(function (html) {
                        var template = angular.element(html);
                        elem.append(template);
                        $compile(template)(scope);
                    });
                }
            };
        }
    ]);
    appModule.directive('app.quanlyhoso.common.vanthuduyet.dongdauchung.20', ['$compile', '$timeout', '$templateRequest',
        function ($compile, $timeout, $templateRequest) {
            return {
                restrict: 'EA',
                replace: true,
                scope: {
                    hosoitems: '=?',
                    viewform: '=?',
                },
                controller: initControllerDongDauChung(20),
                controllerAs: 'vm',
                bindToController: false,
                link: function (scope, elem, attr, ctrl) {
                    $templateRequest("/App/quanlyhoso/_common/views/vanThuDuyet/dirVanThuDuyet.html").then(function (html) {
                        var template = angular.element(html);
                        elem.append(template);
                        $compile(template)(scope);
                    });
                }
            };
        }
    ]);
    appModule.directive('app.quanlyhoso.common.vanthuduyet.dongdauchung.21', ['$compile', '$timeout', '$templateRequest',
        function ($compile, $timeout, $templateRequest) {
            return {
                restrict: 'EA',
                replace: true,
                scope: {
                    hosoitems: '=?',
                    viewform: '=?',
                },
                controller: initControllerDongDauChung(21),
                controllerAs: 'vm',
                bindToController: false,
                link: function (scope, elem, attr, ctrl) {
                    $templateRequest("/App/quanlyhoso/_common/views/vanThuDuyet/dirVanThuDuyet.html").then(function (html) {
                        var template = angular.element(html);
                        elem.append(template);
                        $compile(template)(scope);
                    });
                }
            };
        }
    ]);
    appModule.directive('app.quanlyhoso.common.vanthuduyet.dongdauchung.22', ['$compile', '$timeout', '$templateRequest',
        function ($compile, $timeout, $templateRequest) {
            return {
                restrict: 'EA',
                replace: true,
                scope: {
                    hosoitems: '=?',
                    viewform: '=?',
                },
                controller: initControllerDongDauChung(22),
                controllerAs: 'vm',
                bindToController: false,
                link: function (scope, elem, attr, ctrl) {
                    $templateRequest("/App/quanlyhoso/_common/views/vanThuDuyet/dirVanThuDuyet.html").then(function (html) {
                        var template = angular.element(html);
                        elem.append(template);
                        $compile(template)(scope);
                    });
                }
            };
        }
    ]);
    appModule.directive('app.quanlyhoso.common.vanthuduyet.dongdauchung.23', ['$compile', '$timeout', '$templateRequest',
        function ($compile, $timeout, $templateRequest) {
            return {
                restrict: 'EA',
                replace: true,
                scope: {
                    hosoitems: '=?',
                    viewform: '=?',
                },
                controller: initControllerDongDauChung(23),
                controllerAs: 'vm',
                bindToController: false,
                link: function (scope, elem, attr, ctrl) {
                    $templateRequest("/App/quanlyhoso/_common/views/vanThuDuyet/dirVanThuDuyet.html").then(function (html) {
                        var template = angular.element(html);
                        elem.append(template);
                        $compile(template)(scope);
                    });
                }
            };
        }
    ]);
    appModule.directive('app.quanlyhoso.common.vanthuduyet.dongdauchung.24', ['$compile', '$timeout', '$templateRequest',
        function ($compile, $timeout, $templateRequest) {
            return {
                restrict: 'EA',
                replace: true,
                scope: {
                    hosoitems: '=?',
                    viewform: '=?',
                },
                controller: initControllerDongDauChung(24),
                controllerAs: 'vm',
                bindToController: false,
                link: function (scope, elem, attr, ctrl) {
                    $templateRequest("/App/quanlyhoso/_common/views/vanThuDuyet/dirVanThuDuyet.html").then(function (html) {
                        var template = angular.element(html);
                        elem.append(template);
                        $compile(template)(scope);
                    });
                }
            };
        }
    ]);
    appModule.directive('app.quanlyhoso.common.vanthuduyet.dongdauchung.25', ['$compile', '$timeout', '$templateRequest',
        function ($compile, $timeout, $templateRequest) {
            return {
                restrict: 'EA',
                replace: true,
                scope: {
                    hosoitems: '=?',
                    viewform: '=?',
                },
                controller: initControllerDongDauChung(25),
                controllerAs: 'vm',
                bindToController: false,
                link: function (scope, elem, attr, ctrl) {
                    $templateRequest("/App/quanlyhoso/_common/views/vanThuDuyet/dirVanThuDuyet.html").then(function (html) {
                        var template = angular.element(html);
                        elem.append(template);
                        $compile(template)(scope);
                    });
                }
            };
        }
    ]);
    appModule.directive('app.quanlyhoso.common.vanthuduyet.dongdauchung.36', ['$compile', '$timeout', '$templateRequest',
        function ($compile, $timeout, $templateRequest) {
            return {
                restrict: 'EA',
                replace: true,
                scope: {
                    hosoitems: '=?',
                    viewform: '=?',
                },
                controller: initControllerDongDauChung(36),
                controllerAs: 'vm',
                bindToController: false,
                link: function (scope, elem, attr, ctrl) {
                    $templateRequest("/App/quanlyhoso/_common/views/vanThuDuyet/dirVanThuDuyet.html").then(function (html) {
                        var template = angular.element(html);
                        elem.append(template);
                        $compile(template)(scope);
                    });
                }
            };
        }
    ]);
    appModule.directive('app.quanlyhoso.common.vanthuduyet.dongdauchung.38', ['$compile', '$timeout', '$templateRequest',
        function ($compile, $timeout, $templateRequest) {
            return {
                restrict: 'EA',
                replace: true,
                scope: {
                    hosoitems: '=?',
                    viewform: '=?',
                },
                controller: initControllerDongDauChung(38),
                controllerAs: 'vm',
                bindToController: false,
                link: function (scope, elem, attr, ctrl) {
                    $templateRequest("/App/quanlyhoso/_common/views/vanThuDuyet/dirVanThuDuyet.html").then(function (html) {
                        var template = angular.element(html);
                        elem.append(template);
                        $compile(template)(scope);
                    });
                }
            };
        }
    ]);
    appModule.directive('app.quanlyhoso.common.vanthuduyet.dongdauchung.39', ['$compile', '$timeout', '$templateRequest',
        function ($compile, $timeout, $templateRequest) {
            return {
                restrict: 'EA',
                replace: true,
                scope: {
                    hosoitems: '=?',
                    viewform: '=?',
                },
                controller: initControllerDongDauChung(39),
                controllerAs: 'vm',
                bindToController: false,
                link: function (scope, elem, attr, ctrl) {
                    $templateRequest("/App/quanlyhoso/_common/views/vanThuDuyet/dirVanThuDuyet.html").then(function (html) {
                        var template = angular.element(html);
                        elem.append(template);
                        $compile(template)(scope);
                    });
                }
            };
        }
    ]);
    appModule.directive('app.quanlyhoso.common.vanthuduyet.dongdauchung.40', ['$compile', '$timeout', '$templateRequest',
        function ($compile, $timeout, $templateRequest) {
            return {
                restrict: 'EA',
                replace: true,
                scope: {
                    hosoitems: '=?',
                    viewform: '=?',
                },
                controller: initControllerDongDauChung(40),
                controllerAs: 'vm',
                bindToController: false,
                link: function (scope, elem, attr, ctrl) {
                    $templateRequest("/App/quanlyhoso/_common/views/vanThuDuyet/dirVanThuDuyet.html").then(function (html) {
                        var template = angular.element(html);
                        elem.append(template);
                        $compile(template)(scope);
                    });
                }
            };
        }
    ]);
    appModule.directive('app.quanlyhoso.common.vanthuduyet.dongdauchung.41', ['$compile', '$timeout', '$templateRequest',
        function ($compile, $timeout, $templateRequest) {
            return {
                restrict: 'EA',
                replace: true,
                scope: {
                    hosoitems: '=?',
                    viewform: '=?',
                },
                controller: initControllerDongDauChung(41),
                controllerAs: 'vm',
                bindToController: false,
                link: function (scope, elem, attr, ctrl) {
                    $templateRequest("/App/quanlyhoso/_common/views/vanThuDuyet/dirVanThuDuyet.html").then(function (html) {
                        var template = angular.element(html);
                        elem.append(template);
                        $compile(template)(scope);
                    });
                }
            };
        }
    ]);
    appModule.directive('app.quanlyhoso.common.vanthuduyet.dongdauchung.42', ['$compile', '$timeout', '$templateRequest',
        function ($compile, $timeout, $templateRequest) {
            return {
                restrict: 'EA',
                replace: true,
                scope: {
                    hosoitems: '=?',
                    viewform: '=?',
                },
                controller: initControllerDongDauChung(42),
                controllerAs: 'vm',
                bindToController: false,
                link: function (scope, elem, attr, ctrl) {
                    $templateRequest("/App/quanlyhoso/_common/views/vanThuDuyet/dirVanThuDuyet.html").then(function (html) {
                        var template = angular.element(html);
                        elem.append(template);
                        $compile(template)(scope);
                    });
                }
            };
        }
    ]);
    appModule.directive('app.quanlyhoso.common.vanthuduyet.dongdauchung.43', ['$compile', '$timeout', '$templateRequest',
        function ($compile, $timeout, $templateRequest) {
            return {
                restrict: 'EA',
                replace: true,
                scope: {
                    hosoitems: '=?',
                    viewform: '=?',
                },
                controller: initControllerDongDauChung(43),
                controllerAs: 'vm',
                bindToController: false,
                link: function (scope, elem, attr, ctrl) {
                    $templateRequest("/App/quanlyhoso/_common/views/vanThuDuyet/dirVanThuDuyet.html").then(function (html) {
                        var template = angular.element(html);
                        elem.append(template);
                        $compile(template)(scope);
                    });
                }
            };
        }
    ]);
})();