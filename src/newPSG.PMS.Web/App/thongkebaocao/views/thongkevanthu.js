(function () {
    appModule.controller('quanlyhoso.thongkebaocao.views.thongkevanthu', [
        '$scope', '$rootScope', '$timeout', '$uibModal', '$interval', '$filter', 'appSession', 'abp.services.app.thuTucReport',
        function ($scope, $rootScope, $timeout, $uibModal, $interval, $filter, appSession, thuTucReportService, baseService) {
            var vm = this;
            var today = new Date();
            vm.loading = false;
            vm.isShowThongke = false;
            vm.controlLoaiDonHangCombo = {};
            vm.filterInit = {
                formId: 15,
                formCase: 0,
                formCase2: 0,
                page: 1,
                pageSize: 10,
                keyword: null,
                DoanhNghiepId: null,
                MaHoSo: "",
                LoaiDonHangIds: null,
                TrangThai: null
            };
            vm.trangThaiTraCuuData = [
                {
                    id: 1,
                    name: 'Hồ sơ đã đóng dấu'
                },
                {
                    id: 2,
                    name: 'Hồ sơ trả lại'
                }
            ];
            vm.tieuChi = {};
            vm.listThongKe = [];
            vm.filter = angular.copy(vm.filterInit);
            {
                vm.thoiGianTimKiemOptions = app.createDateRangePickerOptions();
                vm.thoiGianTimKiemOptions.locale.format = "DD/MM/YYYY";
                vm.NgayTiepNhan = {
                    startDate: vm.filter.TuNgay,
                    endDate: vm.filter.DenNgay
                };
                vm.trangThaiTraCuuOptions = {
                    dataSource: vm.trangThaiTraCuuData,
                    dataValueField: "id",
                    dataTextField: "name",
                    optionLabel: app.localize('Chọn ...'),
                    filter: "contains"
                };
            }
            {
                var girdHoSo = () => {
                    vm.gridHoSoDataSource = new kendo.data.DataSource({
                        transport: {
                            read: function (options) {
                                _req = angular.copy(vm.filter);
                                _req.skipCount = (options.data.page - 1) * options.data.pageSize;
                                _req.maxResultCount = options.data.pageSize;
                                abp.services.app.thuTucReport.getTraCuuHoSo(_req).done(function (result) {
                                    result.items.forEach(function (item) {
                                        item.donHang = [];
                                        try {
                                            item.donHang = JSON.parse(item.jsonDonHang);
                                        }
                                        catch (ex) {
                                            console.log(ex);
                                        }
                                    });
                                    options.success(result);
                                });
                            }
                        },
                        pageSize: 15,
                        serverPaging: true,
                        serverSorting: true,
                        scrollable: true,
                        sortable: true,
                        pageable: {
                            refresh: true,
                            pageSizes: 15,
                            buttonCount: 5,
                        },
                        schema: {
                            data: "items",
                            total: "totalCount"
                        },
                    });
                    vm.gridColumn = [
                        {
                            headerTemplate: `<div class='text-center'>STT</div>`,
                            template: "<div class='row-number text-center'></div>",
                            width: 50
                        },
                        {
                            field: "vanThuNgayDuyet",
                            title: app.localize('Ngày phê duyệt'),
                            template: "{{this.dataItem.vanThuNgayDuyet | date:'dd/MM/yyyy'}}",
                            attributes: { class: "text-center" },
                            headerAttributes: { style: "text-align: center;" },
                            width: 135
                        },
                        {
                            field: ""
                            , headerTemplate: "Mã đơn hàng"
                            , template: `{{this.dataItem.maHoSo}}`,
                            attributes: { class: "text-center" },
                            headerAttributes: { style: "text-align: center;" },
                            width: 130
                        },
                        {
                            field: ""
                            , headerTemplate: "Tên thuốc/nguyên liệu"
                            , template: `<div ng-repeat="thuoc in dataItem.donHang.danhSachThuoc" ng-if='dataItem.donHang.danhSachThuoc.length > 0'>
                                            {{thuoc.teninn}}{{thuoc.tenThuoc}} {{thuoc.tenNguyenLieu}}{{thuoc.tenDuocLieu}}{{thuoc.tenDuocChat}}{{thuoc.ten}}
                                        </div>`
                        },
                        {
                            field: ""
                            , headerTemplate: "Loại đơn hàng"
                            , template: `{{this.dataItem.tenLoaiDonHang}}`,
                            width: 240
                        },
                        {
                            field: ""
                            , headerTemplate: "Tên công ty"
                            , template: `{{this.dataItem.tenDoanhNghiep}}`
                        }

                    ];
                    vm.hoSoGridOptions = {
                        dataSource: vm.gridHoSoDataSource,
                        pageable:
                        {
                            "refresh": true,
                            messages: {
                                empty: "Không có dữ liệu",
                                display: "Tổng {2} hồ sơ",
                                itemsPerPage: "Hồ sơ mỗi trang"
                            },
                            pageSizes: [5, 10, 15, 20, "Tất cả"]
                        },
                        noRecords: {
                            template: "<div class='k-nodata' style='margin-top: 134px;'><div><div><i class='fa fa-info-circle fa-2x'></i><br><br>Không tìm thấy dữ liệu!</div></div></div>"
                        },
                        height: 500,
                        resizable: true,
                        scrollable: true,
                        autoBind: false,
                        dataBound: function (e) {
                            var grid = this;
                            let record = (e.sender.dataSource.page() - 1) * e.sender.dataSource.pageSize() + 1;
                            grid.tbody.find(".row-number").each(function () {
                                var index = record++;
                                $(this).html(index);
                            });
                        },
                        columns: vm.gridColumn
                    };
                }
            }
            {
                vm.datLai = function () {
                    vm.NgayTiepNhan = app.dateRangeDefault;
                    vm.filter = angular.copy(vm.filterInit);
                    $("#gridThongke").data("kendoGrid").dataSource.data([]);
                    vm.isShowThongke = false;
                };
                vm.thongKe = function () {
                    vm.isShowThongke = true;
                    vm.loading = true;
                    vm.filter.TuNgay = vm.NgayTiepNhan.startDate;
                    vm.filter.DenNgay = vm.NgayTiepNhan.endDate;
                    vm.gridHoSoDataSource.read();
                    vm.loading = false;
                };
            }
            var init = () => {
                vm.NgayTiepNhan = app.dateRangeDefault;
                girdHoSo();
            };
            init();
            vm.exportToExcel = function () {
                thuTucReportService.exportExcelSoTheoDoi(vm.filter)
                    .then(function (result) {
                        app.downloadTempFile(result.data);
                    });
            };
        }
    ]);
})();