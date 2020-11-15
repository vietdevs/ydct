(function () {
    appModule.controller('danhmuc.views.lichlamviec.index', [
        '$scope', '$uibModal', '$stateParams', 'uiGridConstants', 'baseService', 'abp.services.app.lichLamViec', 'abp.services.app.ngayNghi', 'appSession',
        function ($scope, $uibModal, $stateParams, uiGridConstants, baseService, lichLamViecService, ngayNghiService, appSession) {
            //variable
            var vm = this;
            vm.saving = false;
            vm.loading = false;
            vm.loadingNgayNghi = false;
            vm.requestParams = {
                skipCount: 0,
                maxResultCount: 10,
                sorting: null
            };
            vm.filter = {
                page: 1,
                pageSize: 10,
                Filter: null,
            };
            vm.filterNgayNghi = {
                page: 1,
                pageSize: 10,
                Filter: null,
            };
            vm.lichlamviecInit = angular.copy(vm.lichlamviec);
            vm.ngayBatDau = null;
            vm.ngayKetThuc = null;
            vm.ngayLamViecModel = {
                startDate: null,
                endDate: null,
            };

            vm.openGridFormHS = function () {
                vm.show_mode = null;
                vm.lichlamviec = angular.copy(vm.lichlamviecInit);
                vm.isCopy = false;
                vm.lichLamViecGridOptions.read();
            }

            vm.testGetNgayTraHoSO = function () {
                var ngay1 = new Date(2018, 0, 1);
                var ngay2 = new Date(2018, 1, 9);
                var ngay3 = new Date(2018, 3, 24);
                var ngay4 = new Date(2018, 3, 25);
                var soNgayXuLy1 = 7;
                var soNgayXuLy2 = 7;
                var soNgayXuLy3 = 21;
                var soNgayXuLy4 = 21;
                lichLamViecService.getNgayHenTra(ngay1, soNgayXuLy1).then(function (result) {
                    console.log(ngay1, "ngày hẹn trả 1");
                    console.log(result.data, "ngày hẹn trả 1, Giá trị đúng là ngày 10-1-2018");
                    lichLamViecService.getNgayHenTra(ngay2, soNgayXuLy2).then(function (result) {
                        console.log(ngay2, "ngày hẹn trả 2");
                        console.log(result.data, "ngày hẹn trả 2, Giá trị đúng là ngày 27-2-2018");
                        lichLamViecService.getNgayHenTra(ngay3, soNgayXuLy3).then(function (result) {
                            console.log(ngay3, "ngày hẹn trả 3");
                            console.log(result.data, "ngày hẹn trả 3, Giá trị đúng là ngày 28-5-2018");
                            lichLamViecService.getNgayHenTra(ngay4, soNgayXuLy4).then(function (result) {
                                console.log(ngay4, "ngày hẹn trả 4");
                                console.log(result.data, "ngày hẹn trả 4, Giá trị đúng là ngày 28-5-2018");
                            }).finally(function () {
                            });
                        }).finally(function () {
                        });
                    }).finally(function () {
                    });
                }).finally(function () {
                });
            }

            vm.testGetNgayTraHoSO();

            // Ngày nghỉ
            vm.initGridNgayNghi = function () {
                vm.filterNgayNghi = {
                    page: 1,
                    pageSize: 10,
                    Filter: null,
                };
                if (vm.lichlamviec != null) {
                    if (vm.lichlamviec.id != null) {
                        vm.ngayNghiGridOptions.transport.read(vm.gvNgayNghiCallBack);
                    }
                }
            }

            vm.ngayNghiColumns = [
                {
                    field: "STT",
                    headerTemplate: "<div align='center'><span>STT</span></div>",
                    width: 50,
                    template: "<div align='center'>{{this.dataItem.STT}}</div>"
                },
                {
                    field: "",
                    title: "",
                    template: '<div class=\"ui-grid-cell-contents\" align=\"center\">' +
                        '  <div class="btn-group dropdown" uib-dropdown="" dropdown-append-to-body>' +
                        '    <button class="btn btn-xs green-meadow" uib-dropdown-toggle="" aria-haspopup="true" aria-expanded="false"><i class="fa fa-cog"></i> ' + "Thao Tác" + ' <span class="caret"></span></button>' +
                        '    <ul uib-dropdown-menu>' +
                        '<li><a ng-click="vm.suaNgayNghi(this.dataItem)">' + "Sửa" + '</a></li>' +
                        '<li><a ng-click="vm.xoaNgayNghi(this.dataItem)">' + "Xóa" + '</a></li>' +
                        '    </ul>' +
                        '  </div>' +
                        '</div>',
                    width: 180
                },
                {
                    field: "ngayBatDau"
                    , headerTemplate: "<div align='center'><span>Ngày bắt đầu</span></div>"
                    , template: "<div align='center'>{{dataItem.ngayBatDau | date:'dd/MM/yyyy'}}</div>"
                    , width: 120
                },
                {
                    field: "ngayKetThuc"
                    , headerTemplate: "<div align='center'><span>Ngày kết thúc</span></div>"
                    , template: "<div align='center'>{{dataItem.ngayKetThuc | date:'dd/MM/yyyy'}}</div>"
                    , width: 120,
                },
                {
                    field: "lyDo",
                    title: "Lý do",
                },
                {
                    field: "isActive",
                    title: "Trạng Thái",
                    template: '<p style="margin: 0;text-align:center;"><i ng-if="#: isActive # == 1" class="fa fa-check fa-3 font-green-meadow" aria-hidden="true"></i><i ng-if="#: isActive # == 0" class="fa fa-times fa-3 font-red"></i></p>',
                    width: 120,
                }

            ];

            vm.ngayNghiGridOptions = new kendo.data.DataSource({
                transport: {
                    read: function (options) {
                        vm.gvNgayNghiCallBack = options;
                        vm.requestParams.skipCount = (options.data.page - 1) * options.data.pageSize;
                        vm.requestParams.maxResultCount = options.data.pageSize;
                        if (vm.lichlamviec != null) {
                            if (vm.lichlamviec.id != null) {
                                vm.loadingNgayNghi = true;
                                ngayNghiService.getAllServerPaging($.extend({ filter: vm.filterNgayNghi.Filter }, vm.requestParams), vm.lichlamviec.id)
                                    .then(function (result) {
                                        var i = 1;
                                        result.data.items.forEach(function (item) {
                                            item.STT = i;
                                            i++;
                                        });
                                        vm.optionCallback = options;
                                        options.success(result.data);
                                        console.log(result.data, "Danh sách ngày nghỉ");
                                    }).finally(function () {
                                        vm.loadingNgayNghi = false;
                                    });
                            }
                        }
                    }
                },
                pageSize: 10,
                serverPaging: true,
                serverSorting: true,
                scrollable: true,
                sortable: true,
                autoBind: false,
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

            vm.getNgayNghiGridData = function () {
                vm.ngayNghiGridOptions.transport.read(vm.gvNgayNghiCallBack);
            };

            vm.taoNgayNghi = function () {
                var ngaynghiCopy = {
                    lichLamViecId: vm.lichlamviec.id,
                    isActive: true,
                };
                openCreateOrEditNgayNghiModal(ngaynghiCopy, vm.lichlamviec);
            };

            vm.suaNgayNghi = function (ngaynghi) {
                var ngaynghiCopy = angular.copy(ngaynghi);
                openCreateOrEditNgayNghiModal(ngaynghiCopy, vm.lichlamviec);
            }

            vm.xoaNgayNghi = function (ngaynghi) {
                abp.message.confirm(
                    app.localize('DeleteNgayNghiWarningMessage'),
                    "Bạn chắc chắn muốn xóa?",
                    function (isConfirmed) {
                        if (isConfirmed) {
                            ngayNghiService.deleteNgayNghi(ngaynghi.id).then(function () {
                                vm.ngayNghiGridOptions.transport.read(vm.gvNgayNghiCallBack);
                                abp.notify.success(app.localize('SuccessfullyDeleted'));
                            });
                        }
                    }
                );
            };

            // Lịch làm việc
            vm.initGridLichLamViec = function () {
                vm.filter = {
                    page: 1,
                    pageSize: 10,
                    Filter: null,
                };
                vm.lichLamViecGridOptions.read();
            }

            vm.lichLamViecColumns = [
                {
                    field: "STT"
                    , headerTemplate: "<div align='center'><span>STT</span></div>"
                    , width: "50px",
                    template: "<div align='center'>{{this.dataItem.STT}}</div>"
                },
                {
                    field: "",
                    title: "",
                    template: '<div class=\"ui-grid-cell-contents\" align=\"center\">' +
                        '  <div class="btn-group dropdown" uib-dropdown="" dropdown-append-to-body>' +
                        '    <button class="btn btn-xs green-meadow" uib-dropdown-toggle="" aria-haspopup="true" aria-expanded="false"><i class="fa fa-cog"></i> ' + "Thao Tác" + ' <span class="caret"></span></button>' +
                        '    <ul uib-dropdown-menu>' +
                        '<li><a ng-click="vm.suaLichLamViec(this.dataItem)">' + "Sửa" + '</a></li>' +
                        '<li><a ng-click="vm.xoaLichLamViec(this.dataItem)">' + "Xóa" + '</a></li>' +
                        '    </ul>' +
                        '  </div>' +
                        '</div>',
                    width: 180
                },
                {
                    field: "tenLich",
                    title: "Tên lịch",
                },
                {
                    field: "ngayBatDau"
                    , headerTemplate: "<div align='center'><span>Ngày bắt đầu</span></div>"
                    , template: "<div align='center'>{{dataItem.ngayBatDau | date:'dd/MM/yyyy'}}</div>"
                    , width: 120
                },
                {
                    field: "ngayKetThuc"
                    , headerTemplate: "<div align='center'><span>Ngày kết thúc</span></div>"
                    , template: "<div align='center'>{{dataItem.ngayKetThuc | date:'dd/MM/yyyy'}}</div>"
                    , width: 120,
                },
                {
                    field: ""
                    , headerTemplate: "<div align='center'><span>Ngày làm việc</span></div>"
                    , template: '<div align=\"center\" class=\"row\">' +
                        '<div class=\"col-md-1\">' +
                        '<label><b>T2</b></label>' + "<div><i class='fa fa-check font-blue-steel' ng-show='this.dataItem.t2==true'></i> <i class='fa fa-times fa-3 font-red' ng-show='this.dataItem.t2==false'></i></div>" +
                        '</div>' +
                        '<div class=\"col-md-1\">' +
                        '<label><b>T3</b></label>' + "<div><i class='fa fa-check font-blue-steel' ng-show='this.dataItem.t3==true'></i> <i class='fa fa-times fa-3 font-red' ng-show='this.dataItem.t3==false'></i></div>" +
                        '</div>' +
                        '<div class=\"col-md-1\">' +
                        '<label><b>T4</b></label>' + "<div><i class='fa fa-check font-blue-steel' ng-show='this.dataItem.t4==true'></i> <i class='fa fa-times fa-3 font-red' ng-show='this.dataItem.t4==false'></i></div>" +
                        '</div>' +
                        '<div class=\"col-md-1\">' +
                        '<label><b>T5</b></label>' + "<div><i class='fa fa-check font-blue-steel' ng-show='this.dataItem.t5==true'></i> <i class='fa fa-times fa-3 font-red' ng-show='this.dataItem.t5==false'></i></div>" +
                        '</div>' +
                        '<div class=\"col-md-1\">' +
                        '<label><b>T6</b></label>' + "<div><i class='fa fa-check font-blue-steel' ng-show='this.dataItem.t6==true'></i> <i class='fa fa-times fa-3 font-red' ng-show='this.dataItem.t6==false'></i></div>" +
                        '</div>' +
                        '<div class=\"col-md-1\">' +
                        '<label><b>T7</b></label>' + "<div><i class='fa fa-check font-blue-steel' ng-show='this.dataItem.t7==true'></i> <i class='fa fa-times fa-3 font-red' ng-show='this.dataItem.t7==false'></i></div>" +
                        '</div >' +
                        '<div class=\"col-md-1\">' +
                        '<label><b>CN</b></label>' + "<div><i class='fa fa-check font-blue-steel' ng-show='this.dataItem.cn==true'></i> <i class='fa fa-times fa-3 font-red' ng-show='this.dataItem.cn==false'></i></div>" +
                        '</div>' + '</div>'
                },
                {
                    field: "isActive"
                    , headerTemplate: "<div align='center'><span>Trạng Thái</span></div>"
                    , template: '<p style="margin: 0;text-align:center;"><i ng-if="#: isActive # == 1" class="fa fa-check fa-3 font-blue-steel" aria-hidden="true"></i><i ng-if="#: isActive # == 0" class="fa fa-times fa-3 font-red"></i></p>',
                    width: "100px",
                }
            ];

            vm.lichLamViecGridOptions = new kendo.data.DataSource({
                transport: {
                    read: function (options) {
                        vm.requestParams.skipCount = (options.data.page - 1) * options.data.pageSize;
                        vm.requestParams.maxResultCount = options.data.pageSize;
                        vm.loading = true;
                        lichLamViecService.getAllServerPaging($.extend({ filter: vm.filter.Filter }, vm.requestParams))
                            .then(function (result) {
                                var i = 1;
                                result.data.items.forEach(function (item) {
                                    item.STT = i;
                                    i++;
                                });
                                vm.optionCallback = options;
                                options.success(result.data);
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

            vm.getGridData = function () {
                vm.lichLamViecGridOptions.read();
            };

            vm.taoLichLamViec = function () {
                openCreateOrEditLichLamViec(null);
            };

            vm.suaLichLamViec = function (lichLamViec) {
                var lichLamViecCopy = angular.copy(lichLamViec);
                openCreateOrEditLichLamViec(lichLamViecCopy);
            }

            vm.xoaLichLamViec = function (lichLamViec) {
                abp.message.confirm(
                    app.localize('DeleteLichLamViecWarningMessage', lichLamViec.tenLich),
                    "Bạn chắc chắn muốn xóa?",
                    function (isConfirmed) {
                        if (isConfirmed) {
                            lichLamViecService.delete(lichLamViec.id).then(function () {
                                vm.getGridData();
                                abp.notify.success(app.localize('SuccessfullyDeleted'));
                            });
                        }
                    }
                );
            };

            vm.save = function (type) {
                baseService.ValidatorForm("#LichLamViecCreateOrEditForm");
                var frmValidatorForm = angular.element(document.querySelector('#LichLamViecCreateOrEditForm'));
                var formValidation = frmValidatorForm.data('formValidation').validate();
                if (formValidation.isValid()) {
                    var ngayBatDau = new Date(vm.ngayLamViecModel.startDate);
                    var ngayKetThuc = new Date(vm.ngayLamViecModel.endDate);
                    vm.lichlamviec.ngayBatDau = new Date(ngayBatDau.getFullYear(), ngayBatDau.getMonth(), ngayBatDau.getDate(), 0, 0, 0);
                    vm.lichlamviec.ngayKetThuc = new Date(ngayKetThuc.getFullYear(), ngayKetThuc.getMonth(), ngayKetThuc.getDate(), 23, 59, 59);
                    vm.saving = true;
                    lichLamViecService.createOrUpdate(vm.lichlamviec).then(function (result) {
                        abp.notify.success(app.localize('SavedSuccessfully'));
                        vm.openGridFormHS();
                    }).finally(function () {
                        vm.saving = false;
                    });
                }
                else {
                    vm.saving = false;
                }
            }

            function openCreateOrEditLichLamViec(lichLamViec) {
                vm.lichlamviec = angular.copy(lichLamViec);
                if (vm.lichlamviec != null) {
                    vm.ngayLamViecModel = {
                        startDate: vm.lichlamviec.ngayBatDau,
                        endDate: vm.lichlamviec.ngayKetThuc,
                    };
                } else {
                    vm.ngayLamViecModel = {
                        startDate: null,
                        endDate: null,
                    };
                }
                vm.show_mode = "lichlamviec";
                vm.initGridNgayNghi();
                if (vm.lichlamviec == null) {
                    vm.lichlamviec = {
                        isActive: true,
                        t2: true,
                        t3: true,
                        t4: true,
                        t5: true,
                        t6: true,
                        t7: false,
                        cn: false,
                    };
                }
            }

            function openCreateOrEditNgayNghiModal(ngaynghi, lichlamviec) {
                var modalInstance = $uibModal.open({
                    templateUrl: '~/App/danhmuc/views/lichlamviec/createOrEditModal.cshtml',
                    controller: 'danhmuc.views.lichlamviec.createOrEditModal as vm',
                    backdrop: 'static',
                    resolve: {
                        ngaynghi: ngaynghi,
                        lichlamviec: lichlamviec,
                    }
                });

                modalInstance.result.then(function (result) {
                    vm.ngayNghiGridOptions.transport.read(vm.gvNgayNghiCallBack);
                });
            }

            $scope.$on('$viewContentLoaded', function () {
                App.initAjax();
            });
        }
    ]);
})()