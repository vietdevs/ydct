MyApp.controller('frontend.congbonguyenlieu', [
    '$scope', '$location', '$http', '$window','$timeout','$uibModal',
    function ($scope, $location, $http, $window, $timeout, $uibModal) {
        var vm = this;
        vm.total = {
            common: 0,
            congbo: 0,
            tucongbo: 0,
            vipham: 0,
        };
        vm.filterCongBo = {
            FilterIsPhaiCapPhepNhapKhau:'1',
        };
        vm.switchFilter = 0;
        vm.changeSwitchFilter = function (d) {
            vm.switchFilter = d;
        }
        var filterNguyenLieuService = (input, ajaxParams) => {
            return abp.ajax($.extend({
                url: abp.appPath + 'api/services/app/nguyenLieuLamThuoc/FilterCongBoNguyenLieuLamThuoc',
                type: 'POST',
                data: JSON.stringify(input)
            }, ajaxParams));
        }
        vm.ngayHieuLucModel = {
            startDate: null,
            endDate : null
        };
        vm.ngayHieuLucOptions = {

        }
        vm.ngaySuaDoiCuoiModel = {

        }
        vm.initGridHoSo = function () {
            vm.filterText = "";
            vm.totalCommon = 0;
            vm.tuKhoa = "";
            vm.ngayHieuLucModel = "";
            vm.ngaySuaDoiCuoiModel = "";
            vm.filterCongBo = {
                FilterIsPhaiCapPhepNhapKhau: '1',
            };
            $("#result_search").hide();
        }
        vm.filterText = "";
        var gridNguyenLieu = () => {
            vm.filter = {
                FilterText: '',
            };
            vm.requestParams = {
                skipCount: 0,
                maxResultCount: 10,
                sorting: null
            };

            vm.nguyenLieuLamThuocGridOptions = {
                pageable:
                    {
                        "refresh": true,
                        "pageSizes": true,
                        messages: {
                            empty: "Không có dữ liệu",
                            display: "Tổng {2} bản ghi",
                            itemsPerPage: "Bản ghi mỗi trang"
                        }
                    },
                resizable: true,
                scrollable: true,
            }

            vm.nguyenLieuLamThuocColumn = [
                {
                    field: "STT",
                    title: "STT",
                    width: "50px",
                    template: `<div align='center' ng-class="dataItem.isPhaiCapPhepNhapKhau?'font-red':''">{{dataItem.STT}}</div>`
                },
                {
                    field: "tenThuoc",
                    title: "Tên thuốc",
                    template: `<div ng-class="dataItem.isPhaiCapPhepNhapKhau?'font-red':''">{{dataItem.tenThuoc}}</div>`
                },
                {
                    field: "soDangKy",
                    title: "Số đăng ký lưu hành thuốc",
                    template: `<div ng-class="dataItem.isPhaiCapPhepNhapKhau?'font-red':''">{{dataItem.soDangKy}}</div>`,
                },
                {
                    field: "ngayHetHanSoDangKy",
                    title: "Ngày hết hiệu lực của số đăng ký",
                    template: `<div align='center' ng-class="dataItem.isPhaiCapPhepNhapKhau ? 'font-red' : ''">{{dataItem.ngayHetHanSoDangKy | date:'dd/MM/yyyy'}}</div>`
                },
                {
                    field: "tenNguyenLieu",
                    title: "Tên nguyên liệu sản xuất thuốc",
                    template: `<div ng-class="dataItem.isPhaiCapPhepNhapKhau ? 'font-red' : ''">{{dataItem.tenNguyenLieu}}</div>`
                },
                {
                    field: "tenCoSoSanXuatThuoc",
                    title: "Tên cơ sở sản xuất thuốc",
                    template: `<div ng-class="dataItem.isPhaiCapPhepNhapKhau ? 'font-red' : ''">{{dataItem.tenCoSoSanXuatThuoc}}</div>`
                },                
                {
                    field: "tieuChuanChatLuongNguyenLieu",
                    title: "Tiêu chuẩn chất lượng của nguyên liệu",
                    template: `<div ng-class="dataItem.isPhaiCapPhepNhapKhau ? 'font-red' : ''">{{dataItem.tieuChuanChatLuongNguyenLieu}}</div>`
                },
                {
                    field: "tenCoSoSanXuatNguyenLieu",
                    title: "Tên cơ sở sản xuất nguyên liệu",
                    template: `<div ng-class="dataItem.isPhaiCapPhepNhapKhau ? 'font-red' : ''">{{dataItem.tenCoSoSanXuatNguyenLieu}}</div>`
                },
                {
                    field: "diaChiCoSoSanXuatNguyenLieu",
                    title: "Địa chỉ cơ sở sản xuất nguyên liệu",
                    template: `<div ng-class="dataItem.isPhaiCapPhepNhapKhau ? 'font-red' : ''">{{dataItem.diaChiCoSoSanXuatNguyenLieu}}</div>`
                },
                {
                    field: "nuocSanXuatNguyenLieu",
                    title: "Tên nước sản xuất nguyên liệu",
                    template: `<div ng-class="dataItem.isPhaiCapPhepNhapKhau ? 'font-red' : ''">{{dataItem.nuocSanXuatNguyenLieu}}</div>`
                },
                {
                    field: "",
                    title: "Phải cấp phép nhập khẩu",
                    template: '<p style="margin: 0;text-align:center;">' +
                        '<i ng-if="#: isPhaiCapPhepNhapKhau # == 1" class="fa fa-check fa-3 font-green-jungle" aria-hidden="true"></i>' +
                        '<i ng-if="#: isPhaiCapPhepNhapKhau # == 0" class="fa fa-times fa-3 font-red"></i></p>',
                    width: 100,
                },
                {
                   
                    title: "Công văn",

                    
                    template: `<div align='center'><a href="" style=" text-decoration: none;" ng-click="vm.viewCongVan(dataItem)"> {{dataItem.soCongVan}}/QLD-ĐK</a></div>
                                <div align='center'>Ngày ký: {{dataItem.ngayKyCongVan | date:'dd/MM/yyyy'}}</div>
                                <b ng-if='dataItem.ngayHetHanCongVan != null' class='font-red'>Được thay đổi bằng công văn mới: {{dataItem.soCongVanThayDoi}}/QLD-DK</b>`,     
                    width: 200,
                  
                }
            ]

            vm.nguyenLieuLamThuocDS = new kendo.data.DataSource({
                transport: {
                    read: function (options) {
                        abp.ui.setBusy();
                        vm.totalCommon = 0;
                      
                        let _req = {
                            skipCount: (options.data.page - 1) * options.data.pageSize,
                            maxResultCount: options.data.pageSize,
                            FilterText: vm.switchFilter == 0 ? vm.filterText : "",
                            //SoCongVan: vm.switchFilter ==1 ? vm.filterText : "",
                            //SoDangKy: vm.switchFilter == 2 ? vm.filterText : "",
                            //TenThuoc: vm.switchFilter == 3 ? vm.filterText : "",
                            //TenNguyenLieu: vm.switchFilter == 4 ? vm.filterText : "",
                            //NuocSanXuat: vm.switchFilter == 5 ? vm.filterText : "",
                            NgayHieuLucTu: angular.copy(vm.ngayHieuLucModel.startDate),
                            NgayHieuLucToi: angular.copy(vm.ngayHieuLucModel.endDate),
                            NgaySuaDoiTu: angular.copy(vm.ngaySuaDoiCuoiModel.startDate),
                            NgaySuaDoiDen: angular.copy(vm.ngaySuaDoiCuoiModel.endDate),
                            CongBoNguyenLieu: angular.copy(vm.filterCongBo),
                        }
                        switch (vm.switchFilter) {
                            case 1:
                                _req.CongBoNguyenLieu.SoCongVan = vm.filterText;
                                break;
                            case 2:
                                _req.CongBoNguyenLieu.SoDangKy = vm.filterText;
                                break;
                            case 3:
                                _req.CongBoNguyenLieu.TenThuoc = vm.filterText;
                                break;
                            case 4:
                                _req.CongBoNguyenLieu.TenNguyenLieu = vm.filterText;
                                break;
                            case 5:
                                _req.CongBoNguyenLieu.NuocSanXuatNguyenLieu = vm.filterText;
                                break;
                        }
                        filterNguyenLieuService(_req).then(function (ret) {
                            angular.forEach(ret.items,
                                function(it,idx) {
                                    it.STT = (options.data.page - 1) * options.data.pageSize + (idx + 1);
                                });
                            options.success(ret);
                            vm.totalCommon = ret.totalCount;
                            $scope.$apply();
                            abp.ui.clearBusy();
                        });

                    }
                },
                pageSize: 10,
                serverPaging: true,
                serverSorting: true,
                sortable: true,
                schema: {
                    data: "items",
                    total: "totalCount"
                }
            });

            vm.getGridData = function () {
                vm.nguyenLieuLamThuocDS.read();
            };
            vm.viewCongVan = (d) => {
                app.urlCongVanCongBoNguyenLieu = d.urlCongVan;
                var modalInstance = $uibModal.open({
                    template: `
                        <div class="modal-header">
                        <span class="caption-subject bold uppercase">Công văn đính kèm: `+ d.soCongVan +`/QLD-DK</span>
                        </div>
                        <div class="modal-body">
                        <div frontend.congbonguyenlieu.viewcongvan></div>
                        </div>
                        <div class="modal-footer">
                        <button class="btn red" ng-click="cancel()"><i class="glyphicon glyphicon-remove"></i> Đóng</button>
                        </div>
                        `,
                    size: 'lg',
                    windowClass: 'my-modal-popup',
					controller: ['$scope','$uibModalInstance',function ($scope, $uibModalInstance) {
                            $scope.cancel = function () {
                                $uibModalInstance.close();
                            };
                        }]
                });

            }
        }


        vm.search = function () {
            vm.nguyenLieuLamThuocDS.read();
            $("#result_search").show();
            vm.tuKhoa = vm.filterText;
        }

       

        function init() {
            gridNguyenLieu();

        }
        init();
       
    }
]);

(function () {
    MyApp.directive('frontend.congbonguyenlieu.viewcongvan', ['$compile', '$sce', '$templateRequest',
        function ($compile, $sce, $templateRequest) {
            var controller = ['$scope', '$window', function ($scope, $window) {
                var vm = this;
                vm.taiLieuDinhKemUrl = "/File/GoToViewTaiLieu?url=" + app.urlCongVanCongBoNguyenLieu + "#zoom=100";
                console.log("vm.taiLieuDinhKemUrl", vm.taiLieuDinhKemUrl);
                //Function
                vm.trustSrc = function (src) {
                    return $sce.trustAsResourceUrl(src);
                }
            }];
            return {
                restrict: 'EA',
                scope: {
                },
                controller: controller,
                controllerAs: 'vm',
                bindToController: false,
                template: `
                         <div class="row form-group">
                                    <div class="col-md-12">
                                        <div class="portlet light bordered" style="height: 630px">
                                            <iframe
                                                    title="Xem tài liệu" style="width: 100%; height: 100%;" ng-src="{{vm.trustSrc(vm.taiLieuDinhKemUrl)}}" frameborder="0"></iframe>
                                        </div>
                                    </div>
                                </div>

                        `,
            };
        }
    ]);


})();

(function () {
    MyApp.directive('frontend.nguyenlieulamthuoc.filtertieuchuanchatluong', ['$timeout',
        function ($timeout) {
            return {
                restrict: 'EA',
                replace: true,
                template: `<input kendo-drop-down-list
                                ng-model="selected"
                                k-options="options"
                                style="width: 100%;" />`,
                scope: {
                    selected: '=?',
                },
                link: function ($scope, element, attrs) {
                    _serviceGetTieuChuanChatLuong = function (ajaxParams) {
                        return abp.ajax($.extend({
                            url: abp.appPath + 'api/services/app/nguyenLieuLamThuoc/GetTieuChuanChatLuong',
                            type: 'POST',
                            data: JSON.stringify({})
                        }, ajaxParams));
                    };
                    $scope.dataSource = new kendo.data.DataSource({
                        transport: {
                            read: function (options) {
                                _serviceGetTieuChuanChatLuong().done(function (result) {
                                    options.success(result);
                                });
                            }
                        }
                    });

                    $scope.options = {
                        dataSource: $scope.dataSource,
                        dataValueField: "tenTieuChuan",
                        dataTextField: "tenTieuChuan",
                        optionLabel: 'Chọn ...',
                        filter: "contains",
                        dataBound: function () {

                        },
                        filtering: function (ev) {
                            var filterValue = ev.filter != undefined ? ev.filter.value : "";
                            ev.preventDefault();
                            //get filter descriptor
                            this.dataSource.filter({
                                logic: "or",
                                filters: [
                                    {
                                        field: "tenTieuChuan",
                                        operator: "contains",
                                        value: filterValue
                                    },
                                    {
                                        field: "tenKhongDau",
                                        operator: "contains",
                                        value: filterValue
                                    }
                                ]
                            });
                            // handle the event
                        },
                        noDataTemplate: ''
                    };
                }
            };
        }
    ]);
    MyApp.directive('frontend.nguyenlieulamthuoc.nuocsanxuat', ['$timeout',
        function ($timeout) {
            return {
                restrict: 'EA',
                replace: true,
                template: `<input kendo-drop-down-list
                                ng-model="selected"
                                k-options="options"
                                style="width: 100%;" />`,
                scope: {
                    selected: '=?',
                },
                link: function ($scope, element, attrs) {
                    _serviceGetNuocSanXuat = function (ajaxParams) {
                        return abp.ajax($.extend({
                            url: abp.appPath + 'api/services/app/nguyenLieuLamThuoc/GetNuocSanXuat',
                            type: 'POST',
                            data: JSON.stringify({})
                        }, ajaxParams));
                    };
                    $scope.dataSource = new kendo.data.DataSource({
                        transport: {
                            read: function (options) {
                                _serviceGetNuocSanXuat().done(function (result) {
                                    options.success(result);
                                });
                            }
                        }
                    });

                    $scope.options = {
                        dataSource: $scope.dataSource,
                        dataValueField: "tenNuoc",
                        dataTextField: "tenNuoc",
                        optionLabel: 'Chọn ...',
                        filter: "contains",
                        dataBound: function () {

                        },
                        filtering: function (ev) {
                            var filterValue = ev.filter != undefined ? ev.filter.value : "";
                            ev.preventDefault();
                            //get filter descriptor
                            this.dataSource.filter({
                                logic: "or",
                                filters: [
                                    {
                                        field: "tenNuoc",
                                        operator: "contains",
                                        value: filterValue
                                    },
                                    {
                                        field: "tenNuocKhongDau",
                                        operator: "contains",
                                        value: filterValue
                                    }
                                ]
                            });
                            // handle the event
                        },
                        noDataTemplate: ''
                    };
                }
            };
        }
    ]);
})();
