(function () {
    appModule.controller('quanlyhoso.thutuc37.views.tracuuhoso.index', [
        '$rootScope','$uibModal', '$sce', 'abp.services.app.traCuuHoSo37', 'appSession', 'quanlyhoso.thutuc37.services.appChuKySo',
        function ($rootScope, $uibModal,$sce, xuLyHoSoTraCuuService, appSession, appChuKySo) {
            var vm = this;
            abp.ui.clearBusy();
            var today = new Date();
            vm.loading = false;
            vm.exporting = false;
            vm.isCollapsed = true;
            vm.requestParams = {
                skipCount: 0,
                maxResultCount: 10,
                sorting: null
            };
            vm.filterInit = {
                HoTenNguoiDeNghi: "",
                MaSoThue: "",
                DiaChi: "",
                TinhId: null,
                HuyenId: null,
                XaId: null,
                ChuyenVienThuLyId: null,
                TruongPhongId: null,
                LanhDaoCucId: null,
                TrangThaiTraCuu: null,
                LuongXuLy: null,
                TrangThaiXuLy: null,
                MaHoSo: "",
                IsQuaHan: null,
                trangThaiXuLy: null,
                VanThuId: null,

            }
            vm.filter = angular.copy(vm.filterInit);

            vm.ngayNopOptions = app.createDateRangePickerOptions();
            vm.ngayNopOptions.locale.format = "DD/MM/YYYY";
            vm.NgayNopModel = {
                startDate: vm.filter.NgayNopTu,
                endDate: vm.filter.NgayNopToi,
            };

            vm.ngayTraKetQuaOptions = app.createDateRangePickerOptions();
            vm.ngayTraKetQuaOptions.locale.format = "DD/MM/YYYY";
            vm.NgayTraKetQuaModel = {
                startDate: vm.filter.NgayTraKetQuaTu,
                endDate: vm.filter.NgayTraKetQuaToi,
            };
           
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
            }

            vm.columnGridHoSo = [
                {
                    field: "STT",
                    title: app.localize('STT'),
                    width: "50px",
                    template: "<div align='center'>{{this.dataItem.STT}}</div>"
                },
                {
                    field: "",
                    title: app.localize('Thao Tác'),
                    template:
                        '<button class="btn btn-xs btn-primary" ng-click="vm.xemHoSo(this.dataItem)">Xem hồ sơ</button>',
                    width: "120px"
                },
                {
                    field: "",
                    title: app.localize('Thông tin hồ sơ'),
                    template:
                        `<div>
                             <strong>Họ và tên: <span style="font-size:16px;">{{ this.dataItem.hoTenNguoiDeNghi }}</span></strong><br/>
                             <strong>Ngày sinh:</strong> {{ this.dataItem.ngaySinh | date:'dd/MM/yyyy'}}<br/>
                             <strong>Địa chỉ:</strong> {{ this.dataItem.diaChiCuTru }}<br/>
                             <strong>Mã hồ sơ:</strong> {{ this.dataItem.maHoSo }}
                         </div>`,
                    width: "500px"
                },
                {
                    field: "ngayNop",
                    title: app.localize('Ngày nộp và trả'),
                    template: `<b>Ngày nộp:</b> {{this.dataItem.ngayTiepNhan | date:'dd/MM/yyyy'}} <br/>
                               `,

                    width: "150px",
                },
                {
                    field: ""
                    , headerTemplate: 'Đơn vị gửi'
                    , template: `<div>
                                 <p style='margin:0'><b>{{this.dataItem.strDonViGui}}</b></p>
                                 <p style='margin:0; font-size:12px;'><em>{{dataItem.tenNguoiGui}}</em></p>
                                 <p style='margin:0; font-size:12px;'><em>{{dataItem.ngayGui | date:'dd/MM/yyyy HH:mm:ss'}}</em></p>
                                 </div>`
                    , width: "150px"
                },
                {
                    field: ""
                    , headerTemplate: 'Đơn vị đang xử lý'
                    , template: "<div>\
                                            <p style='margin:0'><b>{{this.dataItem.strDonViXuLy}}</b></p>\
                                            <p style='margin:0; font-size:12px;'><em>{{dataItem.tenNguoiXuLy}}</em></p>\
                                        </div>"
                    , width: "150px"
                },
                {
                    field: ""
                    , headerTemplate: app.localize('Trạng thái xử lý')
                    , template: "<div><text>{{this.dataItem.strTrangThai}}</text></div>"
                    , width: "150px"
                }
            ];

            //Dropdownlist
            {
                vm.cv1Options = {
                    dataSource: {
                        transport: {
                            read: function (options) {
                                xuLyHoSoTraCuuService.getListChuyenVien().then(function (result) {
                                    options.success(result.data);
                                });
                            }
                        }
                    },
                    dataValueField: "id",
                    dataTextField: "name",
                    optionLabel: app.localize('Chọn ...'),
                    filter: "contains",
                }

                vm.truongPhongOptions = {
                    dataSource: {
                        transport: {
                            read: function (options) {
                                xuLyHoSoTraCuuService.getListTruongPhong().then(function (result) {
                                    options.success(result.data);
                                });
                            }
                        }
                    },
                    dataValueField: "id",
                    dataTextField: "name",
                    optionLabel: app.localize('Chọn ...'),
                    filter: "contains",
                }
                vm.lanhDaoCucOptions = {
                    dataSource: {
                        transport: {
                            read: function (options) {
                                xuLyHoSoTraCuuService.getListLanhDaoCuc().then(function (result) {
                                    options.success(result.data);
                                });
                            }
                        }
                    },
                    dataValueField: "id",
                    dataTextField: "name",
                    optionLabel: app.localize('Chọn ...'),
                    filter: "contains",
                }

                vm.vanThuOptions = {
                    dataSource: {
                        transport: {
                            read: function (options) {
                                xuLyHoSoTraCuuService.getListVanThu().then(function (result) {
                                    options.success(result.data);
                                });
                            }
                        }
                    },
                    dataValueField: "id",
                    dataTextField: "name",
                    optionLabel: app.localize('Chọn ...'),
                    filter: "contains",
                }
                
                vm.PhongBanXuLyOptions = {
                    dataSource: {
                        transport: {
                            read: function (options) {
                                xuLyHoSoTraCuuService.getListPhongBanXuLy()
                                    .then(function (result) {
                                        options.success(result.data);
                                    });
                            }
                        }
                    },
                    dataValueField: "id",
                    dataTextField: "name",
                    optionLabel: app.localize('Chọn ...'),
                    filter: "contains",
                }

                vm.tinhOptions = {
                    dataSource: appSession.get_tinh(),
                    dataValueField: "id",
                    dataTextField: "ten",
                    optionLabel: app.localize('Chọn ...'),
                    filter: "contains",
                }

                vm.huyenOptions = {
                    dataSource: appSession.get_huyen(),
                    dataValueField: "id",
                    dataTextField: "ten",
                    optionLabel: app.localize('Chọn sau khi chọn tỉnh ...'),
                    filter: "contains",
                }

                vm.xaOptions = {
                    dataSource: appSession.get_xa(),
                    dataValueField: "id",
                    dataTextField: "ten",
                    optionLabel: app.localize('Chọn sau khi chọn huyện ...'),
                    filter: "contains",
                }

                vm.trangThaiTraCuuOptions = {
                    dataSource: {
                        transport: {
                            read: function (options) {
                                xuLyHoSoTraCuuService.getTrangThaiTraCuu().then(function (result) {
                                    options.success(result.data);
                                });
                            }
                        }
                    },
                    dataValueField: "id",
                    dataTextField: "name",
                    optionLabel: app.localize('Chọn ...'),
                    filter: "contains",
                }
            }

            vm.gridHoSoDataSource = new kendo.data.DataSource({
                transport: {
                    read: function (options) {
                        console.log(vm.filter, "vm.filter");
                        vm.requestParams.skipCount = (options.data.page - 1) * options.data.pageSize;
                        vm.requestParams.maxResultCount = options.data.pageSize;

                        vm.filter.NgayNopTu = vm.NgayNopModel.startDate;
                        vm.filter.NgayNopToi = vm.NgayNopModel.endDate;

                        vm.filter.NgayTraKetQuaTu = vm.NgayTraKetQuaModel.startDate;
                        vm.filter.NgayTraKetQuaToi = vm.NgayTraKetQuaModel.endDate;

                        vm.loading = true;
                        xuLyHoSoTraCuuService.getListHoSoTraCuuPaging($.extend(vm.filter, vm.requestParams))
                            .then(function (result) {
                                vm.gvHoSoCallBack = options;
                                var i = 1;
                                result.data.items.forEach(function (item) {
                                    item.STT = i;
                                    i++;
                                });
                                console.log(result.data, "result.data");
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

            vm.refreshGridHS = function () {
                vm.gridHoSoDataSource.transport.read(vm.gvHoSoCallBack);
                console.log('vm.PhongBanXuLyOptions', vm.PhongBanXuLyOptions);
            }

            vm.xemHoSo = function (dataItem) {
                if (dataItem && dataItem.id > 0) {
                    var modalData = {
                        title: 'Xem hồ sơ',
                        id: dataItem.id
                    };
                    var modalInstance = $uibModal.open({
                        templateUrl: '~/App/quanlyhoso/thutuc37/directives/modal/viewHoSoFullModal.cshtml',
                        controller: 'quanlyhoso.thutuc37.directives.modal.viewHoSoFullModal as vm',
                        backdrop: 'static',
                        size: 'lg',
                        resolve: {
                            modalData: modalData
                        }
                    });
                }
            }

            vm.refreshControll = function () {
                vm.filter = angular.copy(vm.filterInit);
                vm.NgayNopModel = {
                    startDate: vm.filter.NgayNopTu,
                    endDate: vm.filter.NgayNopToi
                };
                vm.NgayTraKetQuaModel = {
                    startDate: vm.filter.NgayTraKetQuaTu,
                    endDate: vm.filter.NgayTraKetQuaToi,
                };
                vm.refreshGridHS();
            }

            vm.checkCollapse = function () {
                vm.isCollapsed = angular.element("#nangcao").hasClass('collapse in');
            }

            vm.xuatExel = () => {
                vm.filter.NgayNopTu = vm.NgayNopModel.startDate;
                vm.filter.NgayNopToi = vm.NgayNopModel.endDate;
                vm.filter.NgayTraKetQuaTu = vm.NgayTraKetQuaModel.startDate;
                vm.filter.NgayTraKetQuaToi = vm.NgayTraKetQuaModel.endDate;
                vm.exporting = true;
                xuLyHoSoTraCuuService.exportToExcel(vm.filter)
                    .then(function (result) {
                        app.downloadTempFile(result.data);
                    }).finally(function () {
                        vm.exporting = false;
                    });;
            }

            function init() {
            }

            init();
            
        }
    ]);
})();