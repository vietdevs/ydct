﻿(function () {
    var initControllerTraCuu = (tenantName) => {
        var controller = ['$scope', '$http', '$window',
            function ($scope, $http, $window) {
                var vm = this;
                vm.countKetQua = 0;
                var today = new Date();
                vm.saving = false;
                vm.requestParams = {
                    skipCount: 0,
                    maxResultCount: 10,
                    sorting: null
                };
                vm.filter = {
                    tenantName: tenantName,
                    tenDoanhNghiep: null,
                    diaChi: null,
                    soCongBo: null,
                    tinhId: null,
                    loaiHoSoId: null,
                    tenSanPhamDangKy: null,
                    tenLoaiHoSo: null
                    //ngayCapTu: new Date(today.getFullYear() - 1, today.getMonth(), today.getDate()),
                    //ngayCapToi: new Date()
                };
                vm.filterInit = angular.copy(vm.filter);

                vm.loaiHoSoOptions = {
                    dataSource: {
                        transport: {
                            read: function (options) {
                                $http.post('/api/services/app/loaiHoSo/getLoaiHoSoTT02ToDDL').then(function (response) {
                                    options.success(response.data.result);
                                });
                            }
                        }
                    },
                    dataValueField: "id",
                    dataTextField: "tenLoaiHoSo",
                    optionLabel: app.localize('Chọn loại hồ sơ ...'),
                    filter: "contains"
                };
                vm.tinhOptions = {
                    dataSource: {
                        transport: {
                            read: function (options) {
                                $http.post('/api/services/app/tinh/getAllToDDL').then(function (response) {
                                    options.success(response.data.result);
                                });
                            }
                        }
                    },
                    dataValueField: "id",
                    dataTextField: "ten",
                    optionLabel: app.localize('Chọn tỉnh ...'),
                    filter: "contains"
                };
                vm.ngayCapOptions = app.createDateRangePickerOptions();
                vm.ngayCapOptions.locale.format = "DD/MM/YYYY";
                vm.ngayCapModel = {
                    startDate: vm.filter.ngayCapTu,
                    endDate: vm.filter.ngayCapToi
                };

                vm.search = function () {
                    app.localStorage.set("fillterVar", vm.filter);
                    $window.location.pathname = '/Tracuu';
                };
                vm.refreshControll = function () {
                    vm.filter = angular.copy(vm.filterInit);
                    vm.refreshHoSo();
                };
                vm.auto_complete_doanh_nghiep_options = {
                    dataTextField: "tenDoanhNghiep",
                    headerTemplate: "",
                    template: `<div style="margin: 7px 0px;">
                            <h5 style="margin: 0px;" class ='bold font-blue'>#:data.tenDoanhNghiep #</h5>
                            <span>Địa chỉ: #: data.diaChi #</span>
                       </div>`,
                    minLength: 3,
                    dataSource: {
                        serverFiltering: true,
                        serverPaging: true,
                        transport: {
                            read: function (options) {
                                _request = {
                                    skipCount: 0,
                                    maxResultCount: 20,
                                    filter: vm.filter.tenDoanhNghiep.trim()
                                };
                                $http.post('/api/services/app/doanhNghiepPublish/getDoanhNghiepFilter', _request).then(function (ret) {
                                    console.log(ret);
                                    options.success(ret.data.result.items);
                                });
                            }
                        },
                        sortable: true,
                        selectable: false
                    }
                };
                vm.auto_complete_ten_san_pham_options = {
                    dataTextField: "tenSanPhamDangKy",
                    headerTemplate: "",
                    template: `<div style="margin: 7px 0px;">
                            <h5 style="margin: 0px;" class ='bold font-blue'>#:data.tenSanPhamDangKy #</h5>
                            <span>Địa chỉ: #: data.doanhNghiep #</span>
                       </div>`,
                    minLength: 3,
                    dataSource: {
                        serverFiltering: true,
                        serverPaging: true,
                        transport: {
                            read: function (options) {
                                _request = {
                                    Filter: vm.filter.tenSanPhamDangKy.trim()
                                };
                                $http.post('/api/services/app/traCuuHoSo02/getTenSanPhamCongBo', _request).then(function (ret) {
                                    console.log(ret, "san pham tu cb");
                                    options.success(ret.data.result);
                                });
                            }
                        },
                        sortable: true,
                        selectable: false
                    }
                };

                vm.gridHoSoDataSource = new kendo.data.DataSource({
                    transport: {
                        read: function (options) {
                            vm.requestParams.skipCount = (options.data.page - 1) * options.data.pageSize;
                            vm.requestParams.maxResultCount = options.data.pageSize;
                            vm.loading = true;
                            $http.post('/api/services/app/traCuuHoSo02/getListHoSoTraCuuPaging', $.extend(vm.filter, vm.requestParams))
                                .then(function (result) {
                                    vm.gvHoSoCallBack = options;
                                    console.log(result, "result.data.items dang ky cong bo");
                                    vm.countKetQua = result.data.result.totalCount;
                                    if (result.data.result.items) {
                                        result.data.result.items.forEach(function (item, idx) {
                                            item.STT = idx + 1;
                                        });
                                        options.success(result.data.result);
                                    }
                                }).finally(function () {
                                    vm.loading = false;
                                });
                        }
                    },
                    pageSize: 10,
                    serverPaging: true,
                    serverSorting: true,
                    scrollable: true,
                    sortable: true,
                    pageable: {
                        refresh: true,
                        pageSizes: 10,
                        buttonCount: 5
                    },
                    schema: {
                        data: "items",
                        total: "totalCount"
                    }
                });

                vm.hoSoGridOptions = {
                    pageable:
                    {
                        "refresh": true,
                        "pageSizes": true,
                        messages: {
                            empty: "Không có dữ liệu",
                            display: "Tổng {2} hồ sơ",
                            itemsPerPage: "Hồ sơ mỗi trang"
                        }
                    },
                    resizable: true,
                    scrollable: true,
                    dataBound: function (e) {
                    }
                };

                vm.columnGridHoSo = [
                    {
                        field: "STT",
                        title: app.localize('STT'),
                        headerAttributes: { style: "text-align: center;" },
                        width: "50px",
                        template: "<div align='center'>{{this.dataItem.STT}}</div>"
                    },
                    {
                        field: "",
                        title: "Thao tác",
                        headerAttributes: { style: "text-align: center;" },
                        template: `<div class="ui-grid-cell-contents">
                                <button class="btn btn-blue" ng-click="vm.xemGiayXacNhan(this.dataItem)"> <em style="margin-right: 4px;" class="fa fa-file-pdf-o"></em>Xem giấy công bố</button >
                            </div>`,
                        width: 170
                    },
                    {
                        field: "soGiayChungNhanATTP",
                        title: "Số chứng nhận",
                        width: 170

                    },
                    {
                        field: "tenDoanhNghiep",
                        title: "Tên doanh nghiệp",
                        //width: "200px",
                        template: "<div>{{this.dataItem.tenDoanhNghiep}}</div>"
                    },
                    {
                        field: "tenLoaiHoSo",
                        title: "Loại hồ sơ",
                        width: "250px",
                        template: "<div>{{this.dataItem.tenLoaiHoSo}}</div>"
                    },
                    {
                        field: "tenSanPhamDangKy",
                        title: "Tên sản phẩm",
                        width: "300px",
                        template: "<div>{{this.dataItem.tenSanPhamDangKy}}</div>"
                    },
                    {
                        field: "ngayTraKetQua",
                        headerAttributes: { style: "text-align: center;" },
                        title: "Ngày cấp",
                        width: "170px",
                        template: "<div align='center'>{{this.dataItem.ngayTraKetQua | date:'dd/MM/yyyy HH:mm'}}</div>"
                    }
                ];

                vm.refreshHoSo = function () {
                    vm.gridHoSoDataSource.transport.read(vm.gvHoSoCallBack);
                };

                vm.xemGiayXacNhan = function (item) {
                    $window.open("/File/GoToViewTaiLieu?url=" + item.giayTiepNhan, '_blank');
                };

            }
        ];

        return controller;
    };

    MyApp.directive('tracuu.dongnai.dangkycongbo', ['$compile', '$timeout', '$templateRequest',
        function ($compile, $timeout, $templateRequest) {
            return {
                restrict: 'EA',
                replace: true,
                scope: {

                },
                controller: initControllerTraCuu('dongnai'),
                controllerAs: 'vm',
                bindToController: false,
                link: function (scope, elem, attr, ctrl) {
                    $templateRequest("/Scripts/FrontendApp/TraCuuDirectives/2_DangKyCongBo/Index.html").then(function (html) {
                        var template = angular.element(html);
                        elem.append(template);
                        $compile(template)(scope);
                    });
                }
            };
        }
    ]);

    MyApp.directive('tracuu.danang.dangkycongbo', ['$compile', '$timeout', '$templateRequest',
        function ($compile, $timeout, $templateRequest) {
            return {
                restrict: 'EA',
                replace: true,
                scope: {

                },
                controller: initControllerTraCuu('danang'),
                controllerAs: 'vm',
                bindToController: false,
                link: function (scope, elem, attr, ctrl) {
                    $templateRequest("/Scripts/FrontendApp/TraCuuDirectives/2_DangKyCongBo/Index.html").then(function (html) {
                        var template = angular.element(html);
                        elem.append(template);
                        $compile(template)(scope);
                    });
                }
            };
        }
    ]);

    MyApp.directive('tracuu.hanoi.dangkycongbo', ['$compile', '$timeout', '$templateRequest',
        function ($compile, $timeout, $templateRequest) {
            return {
                restrict: 'EA',
                replace: true,
                scope: {

                },
                controller: initControllerTraCuu('hanoi'),
                controllerAs: 'vm',
                bindToController: false,
                link: function (scope, elem, attr, ctrl) {
                    $templateRequest("/Scripts/FrontendApp/TraCuuDirectives/2_DangKyCongBo/Index.html").then(function (html) {
                        var template = angular.element(html);
                        elem.append(template);
                        $compile(template)(scope);
                    });
                }
            };
        }
    ]);

    MyApp.directive('tracuu.laocai.dangkycongbo', ['$compile', '$timeout', '$templateRequest',
        function ($compile, $timeout, $templateRequest) {
            return {
                restrict: 'EA',
                replace: true,
                scope: {

                },
                controller: initControllerTraCuu('laocai'),
                controllerAs: 'vm',
                bindToController: false,
                link: function (scope, elem, attr, ctrl) {
                    $templateRequest("/Scripts/FrontendApp/TraCuuDirectives/2_DangKyCongBo/Index.html").then(function (html) {
                        var template = angular.element(html);
                        elem.append(template);
                        $compile(template)(scope);
                    });
                }
            };
        }
    ]);

    MyApp.directive('tracuu.vinhphuc.dangkycongbo', ['$compile', '$timeout', '$templateRequest',
        function ($compile, $timeout, $templateRequest) {
            return {
                restrict: 'EA',
                replace: true,
                scope: {

                },
                controller: initControllerTraCuu('vinhphuc'),
                controllerAs: 'vm',
                bindToController: false,
                link: function (scope, elem, attr, ctrl) {
                    $templateRequest("/Scripts/FrontendApp/TraCuuDirectives/2_DangKyCongBo/Index.html").then(function (html) {
                        var template = angular.element(html);
                        elem.append(template);
                        $compile(template)(scope);
                    });
                }
            };
        }
    ]);
})();


