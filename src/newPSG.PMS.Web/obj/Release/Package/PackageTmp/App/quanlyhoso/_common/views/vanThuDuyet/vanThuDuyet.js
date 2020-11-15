(function () {
    appModule.controller('quanlyhoso.thutuccommon.views.vanthuduyet', [
        '$rootScope', '$scope', '$sce', '$stateParams', '$uibModal', '$interval', '$location', '$timeout',
        function ($rootScope, $scope, $sce, $stateParams, $uibModal, $interval, $location, $timeout) {
            var vm = this;
            var initVar = () => {
                vm.formview = 'danh_sach';
                vm.baseFilter = {
                    formId: app.FORM_ID.FORM_VAN_THU_DUYET,
                    formCase: 1, //0:TAT_CA, 1:CHUA_PHAN_CONG, 2:DA_PHAN_CONG, 3:DA_PHAN_CONG_TU_DONG
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
                vm.filter = angular.copy(vm.baseFilter);
                // get form case
                vm.loadTotalLabel = () => {
                    _req = angular.copy(vm.filter);
                    _req.ngayNopTu = vm.ngayGuiModel.startDate;
                    _req.ngayNopToi = vm.ngayGuiModel.endDate;
                    _req.skipCount = 0;
                    _req.maxResultCount = 10;
                    abp.services.app.thuTucReport.getTotalFormCase(_req).done(function (result) {
                        let lstTotal = [
                            result.case0,
                            result.case1
                            , result.case2
                            , result.case3
                            , result.case4
                            , result.case5
                            , result.case6
                            , result.case7
                            , result.case8
                            , result.case9];
                        angular.forEach(vm.listFormCase, function (formCase, idx) {
                            try {
                                formCase.totalItems = lstTotal[idx];
                            }
                            catch (err) {
                                console.log(err);
                            }
                        });
                        $scope.$apply();
                    });
                };
                if (vm.filter.formId) {
                    abp.services.app.xuLyHoSoGridView38
                        .getListFormCase(vm.filter.formId)
                        .done(function (result) {
                            if (result) {
                                vm.listFormCase = result.formCase;
                                vm.listFormCase2 = result.formCase2;
                                vm.loadTotalLabel();
                            }
                        });
                }
                vm.isBtnChuyenHoSo = (dataItems) => {
                    return dataItems.formCase == 1;
                };
                vm.isBtnXemHoSo = (dataItems) => {
                    return dataItems.formCase == 3 || dataItems.formCase == 4;
                };
                vm.isBtnChuyenLaiHoSo = (dataItems) => {
                    return dataItems.formCase == 2;
                };
            };
            var girdHoSo = () => {
                vm.gridHoSoDataSource = new kendo.data.DataSource({
                    transport: {
                        read: function (options) {
                            _req = angular.copy(vm.filter);
                            _req.ngayNopTu = vm.ngayGuiModel.startDate;
                            _req.ngayNopToi = vm.ngayGuiModel.endDate;
                            _req.skipCount = (options.data.page - 1) * options.data.pageSize;
                            _req.maxResultCount = options.data.pageSize;
                            abp.services.app.thuTucReport.getTraCuuHoSo(_req).done(function (result) {
                                console.log(result.items, 'result.items văn thư');
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
                    pageSize: 10,
                    serverPaging: true,
                    serverSorting: true,
                    scrollable: true,
                    sortable: true,
                    pageable: {
                        refresh: true,
                        pageSizes: 10,
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
                        headerTemplate: `<div class='text-center'>Thao tác</div>`,
                        template: `<div class='text-center'>
                        <button ng-if="vm.isBtnChuyenHoSo(dataItem)" class="btn btn-primary btn-xs blue ng-isolate-scope" ng-click="vm.van_thu_duyet(this.dataItem)">
                            <span>Đóng dấu</span>
                        </button>
                        <button ng-if="vm.isBtnXemHoSo(dataItem)" class="btn btn-primary btn-xs green ng-isolate-scope" ng-click="vm.xemHoSoFull(this.dataItem)">
                            <span>Xem HS</span>
                        </button>
                        </div > `,
                        width: 120
                    },
                    {
                        field: ""
                        , headerTemplate: "Thông tin hồ sơ"
                        , template: `<div> <b>Mã HS: </b><a ng-click="vm.xemHoSoFull(this.dataItem)"><b>{{this.dataItem.maHoSo}}</b></a>  Số ĐH: <b>{{this.dataItem.soDangKy}}</b></div>
                                         <div> <b>Doanh nghiệp: </b>{{this.dataItem.tenDoanhNghiep}} </div>`
                    },
                    {
                        field: ""
                        , headerTemplate: "Thông tin đơn hàng"
                        , template: `<div ng-repeat="thuoc in dataItem.donHang.danhSachThuoc" ng-if='dataItem.donHang.danhSachThuoc.length > 0'>
                                            {{thuoc.teninn}}{{thuoc.tenThuoc}} {{thuoc.tenNguyenLieu}}{{thuoc.tenDuocLieu}}{{thuoc.tenDuocChat}}{{thuoc.ten}}
                                        </div>`
                    },
                    {
                        field: "ngayNop",
                        title: app.localize('Ngày nộp'),
                        template: "{{this.dataItem.ngayTiepNhan | date:'dd/MM/yyyy'}}",
                        attributes: { class: "text-center" },
                        headerAttributes: { style: "text-align: center;" },
                        width: 120
                    },
                    {
                        field: "ngayHenTra",
                        title: app.localize('Ngày hẹn trả'),
                        attributes: { class: "text-center" },
                        headerAttributes: { style: "text-align: center;" },
                        template: `{{this.dataItem.ngayHenTra | date:'dd/MM/yyyy'}}<br/>
                                <div ng-if="this.dataItem.ngayHenTra">
                                    <div ng-show='this.dataItem.soNgayQuaHan<=0'><span class='label label-success'><i class='fa fa-clock-o'></i> {{this.dataItem.strQuaHan}}</span></div>
                                    <div ng-show='this.dataItem.soNgayQuaHan<=5 && this.dataItem.soNgayQuaHan>0'><span class='label bg-yellow-crusta'><i class='fa fa-clock-o'></i> {{this.dataItem.strQuaHan}}</span></div>
                                    <div ng-show='this.dataItem.soNgayQuaHan>5'><span class='label bg-red'><i class='fa fa-clock-o'></i> {{this.dataItem.strQuaHan}}</span></div>
                                </div>`,
                        width: 160, hidden: true
                    },
                    {
                        field: ""
                        , attributes: { class: "text-center" },
                        headerAttributes: { style: "text-align: center;" }
                        , headerTemplate: "Kết quả"
                        , template: "<span class='label bg-green-meadow bg-font-green-meadow' ng-show='this.dataItem.hoSoIsDat == true'>Hồ sơ đạt</span>"
                            + "<span class='label label-danger' ng-show='this.dataItem.hoSoIsDat == false'>Hồ sơ cần SĐBS</span>"
                        , width: 120
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
                        pageSizes: [5, 10, 20, "Tất cả"]
                    },
                    resizable: true,
                    scrollable: true,
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
                vm.quickSearchHoSo = () => {
                    vm.gridHoSoDataSource.read();
                    vm.loadTotalLabel();
                    vm.dropdownSearchIsOpen = false;
                };
            };
            var mainFunc = () => {
                vm.formCaseOnChange = (formCase) => {
                    vm.filter.formCase = formCase;
                    vm.gridHoSoDataSource.page(1);
                };
                vm.van_thu_duyet = (dataItem) => {
                    vm.formview = 'van_thu_duyet';
                    app.thutucprm = dataItem.thuTucId;
                    vm.hosoitems = dataItem;
                    vm.dirVanThuDuyet = `<div app.quanlyhoso.common.vanthuduyet.dongdauchung.` + dataItem.thuTucId + `  hosoitems="vm.hosoitems" viewform ="vm.formview" ng-if="vm.hosoitems.thuTucId > 0"></div>`;
                };
                vm.xemHoSoFull = function (dataItem) {
                    abp.ui.setBusy();
                   let  _maThuTuc = dataItem.thuTucId;
                    if (_maThuTuc < 10) _maThuTuc = '0' + _maThuTuc;
                    if (dataItem && dataItem.id > 0) {
                        var modalData = {
                            title: 'Xem hồ sơ',
                            id: dataItem.id,
                            formview: vm.form
                        };
                        var modalInstance = $uibModal.open({
                            templateUrl: '~/App/quanlyhoso/thutuc' + _maThuTuc + '/directives/modal/viewHoSoFullModal.cshtml',
                            controller: 'quanlyhoso.thutuc' + _maThuTuc + '.directives.modal.viewHoSoFullModal as vm',
                            backdrop: 'static',
                            size: 'lg',
                            resolve: {
                                modalData: modalData
                            }
                        });
                        abp.ui.clearBusy();
                    }
                };
            };

            var watchFunc = () => {
                $scope.$watch("vm.formview", function () {
                    if (vm.formview == "danh_sach_reload") {
                        $timeout(function () {
                            vm.quickSearchHoSo();
                        }, 500);
                    }
                });
            };

            var init = () => {
                initVar();
                girdHoSo();
                mainFunc();
                watchFunc();
            };
            init();
        }
    ]);
})();