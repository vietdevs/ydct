(function () {
    appModule.directive('quanlyhoso.thutuc37.directives.util.viewbienbanthamdinh', ['$compile', '$templateRequest', 'abp.services.app.xuLyHoSoChuyenVien37',
        function ($compile, $templateRequest, xuLyHoSoChuyenVienService) {

            var controller = ['$scope', function ($scope) {
                var vm = this;
                vm.QUI_TRINH_THAM_DINH = app.QUI_TRINH_THAM_DINH;
                vm.LOAI_DON_HANG = app.LOAI_HO_SO_TW37;
              vm.DON_VI_XU_LY = app.DON_VI_XU_LY;
                vm.dataItem = {};
                vm.hoSoXuLy = {};
                vm.bienBanThamDinh = {};

                //Config
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

                vm.isChuyenVienThuLy = false;
                vm.isChuyenVienPhoiHop = false;

                function init() {
                    if ($scope.dataitem) {
                        vm.dataItem = $scope.dataitem;
                        if ((vm.dataItem.donViGui == vm.DON_VI_XU_LY.TRUONG_PHONG || vm.dataItem.donViGui == vm.DON_VI_XU_LY.PHO_PHONG) && (vm.dataItem.donViXuLy == vm.DON_VI_XU_LY.CHUYEN_VIEN_TONG_HOP || vm.dataItem.donViXuLy == vm.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET)) {
                            vm.isChuyenVienThuLy = true;
                        }
                        else
                            vm.isChuyenVienThuLy = false;

                        if ((vm.dataItem.donViGui == vm.DON_VI_XU_LY.TRUONG_PHONG || vm.dataItem.donViGui == vm.DON_VI_XU_LY.PHO_PHONG) && vm.dataItem.donViXuLy == vm.DON_VI_XU_LY.CHUYEN_VIEN_PHOI_HOP_THAM_XET) {
                            vm.isChuyenVienPhoiHop = true;
                        }
                        else
                            vm.isChuyenVienPhoiHop = false;
                    }
                    vm.loaiDonHangTD = JSON.parse(vm.dataItem.jsonConfigThamDinh);
                    if ($scope.hosoxulyid && $scope.hosoxulyid > 0) {
                        
                        xuLyHoSoChuyenVienService.getBienBanThamDinh($scope.hosoxulyid)
                            .then(function (result) {
                              
                                if (result.data.hoSoXuLy) {
                                    vm.hoSoXuLy = result.data.hoSoXuLy;
                                }

                                //Người duyệt
                                vm.nguoiDuyet = result.data.nguoiDuyet;

                                if (result.data.bienBanThamXet) {
                                    vm.bienBanThamDinh = result.data.bienBanThamXet;    
                                    if (result.data.bienBanThamXet.arrNoiDungThamXet[0]) {
                                        vm.bienBanThamDinh.arrNoiDungThamXet[0].TenThuoc = result.data.bienBanThamXet.arrNoiDungThamXet[0].tenThuoc;
                                    }
                                    //vm.bienBanThamDinh.arrNoiDungThamXet = [];
                                    //if (vm.bienBanThamDinh.noiDungThamXetJson) {
                                    //    vm.bienBanThamDinh.arrNoiDungThamXet = JSON.parse(vm.bienBanThamDinh.noiDungThamXetJson);
                                    //}
                                }

                                //View Biên Bản Thẩm Định
                                {
                                    if (result.data.bienBanThamXet_ChuyenVienThuLy) {
                                        var _bienBanThamXetThuLy = result.data.bienBanThamXet_ChuyenVienThuLy;
                                        _bienBanThamXetThuLy.arrNoiDungThamXet = [];
                                        if (_bienBanThamXetThuLy.noiDungThamXetJson) {
                                            _bienBanThamXetThuLy.arrNoiDungThamXet = JSON.parse(_bienBanThamXetThuLy.noiDungThamXetJson);
                                        }
                                        vm.hoSoXuLy.bienBanThamXet_ChuyenVienThuLy = _bienBanThamXetThuLy;
                                    }
                                    if (result.data.bienBanThamXet_ChuyenVienPhoiHop) {
                                        var _bienBanThamXetPhoiHop = result.data.bienBanThamXet_ChuyenVienPhoiHop;
                                        _bienBanThamXetPhoiHop.arrNoiDungThamXet = [];
                                        if (_bienBanThamXetPhoiHop.noiDungThamXetJson) {
                                            _bienBanThamXetPhoiHop.arrNoiDungThamXet = JSON.parse(_bienBanThamXetPhoiHop.noiDungThamXetJson);
                                        }

                                        vm.hoSoXuLy.bienBanThamXet_ChuyenVienPhoiHop = _bienBanThamXetPhoiHop;
                                    }
                                }
                                vm.daThuLy = !app.isNullOrEmpty(vm.hoSoXuLy.bienBanThamXetId_ChuyenVienThuLy);
                                vm.daPhoiHop = !app.isNullOrEmpty(vm.hoSoXuLy.bienBanThamXetId_ChuyenVienPhoiHop);

                            }).finally(function () {
                                //vm.loading = false;
                            });
                    }
                    else {

                        if ($scope.hosoxuly) {
                            vm.hoSoXuLy = $scope.hosoxuly;
                      
                        }
                       
                        vm.nguoiDuyet = {};
                        if ($scope.nguoiduyet) {
                            vm.nguoiDuyet = $scope.nguoiduyet;
                        }

                        if ($scope.bienbanthamdinh) {
                            vm.bienBanThamDinh = $scope.bienbanthamdinh;
                        }

                        

                        //vm.isChuyenVienThuLy = false;
                        //vm.isChuyenVienPhoiHop = false;
                        vm.daThuLy = !app.isNullOrEmpty(vm.hoSoXuLy.bienBanThamXetId_ChuyenVienThuLy);
                        vm.daPhoiHop = !app.isNullOrEmpty(vm.hoSoXuLy.bienBanThamXetId_ChuyenVienPhoiHop);
                      
                     
                    }
                }
                init();
            }]

            return {
                restrict: 'EA',
                scope: {
                    hosoxulyid: '=',
                    dataitem: '=',
                    hosoxuly: '=',
                    nguoiduyet: '=',
                    bienbanthamdinh: '=?'
                },
                controller: controller,
                controllerAs: 'vm',
                bindToController: false,
                link: function (scope, elem, attr, ctrl) {
                    $templateRequest("/App/quanlyhoso/thutuc37/directives/util/viewBienBanThamDinh.cshtml").then(function (html) {
                        var template = angular.element(html);
                        elem.append(template);
                        $compile(template)(scope);
                    });
                }
            };
        }
    ]);
})();