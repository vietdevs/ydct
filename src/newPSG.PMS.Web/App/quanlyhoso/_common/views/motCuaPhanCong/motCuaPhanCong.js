(function () {
    appModule.controller('quanlyhoso.thutuccommon.views.motcuaphancong', [
        '$scope', '$sce', '$stateParams', '$uibModal', '$interval', '$location', '$timeout',
        function ($scope, $sce, $stateParams, $uibModal, $interval, $location, $timeout) {
            var vm = this;

            var initVar = () => {
                vm.formview = 'danh_sach';
                vm.baseFilter = {
                    formId: 2,
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
                }

                vm.filter = angular.copy(vm.baseFilter);
                // get form case
                vm.loadTotalLabel = () => {
                    _req = angular.copy(vm.filter);
                    _req.ngayNopTu = vm.ngayGuiModel.startDate;
                    _req.ngayNopToi = vm.ngayGuiModel.endDate;
                    _req.skipCount = 0;
                    _req.maxResultCount = 10;
                    abp.services.app.thuTucReport.getTotalFormCase(_req).done(function (result) {
                        let lstTotal = [result.case0
                            , result.case1
                            , result.case2
                            , result.case3
                            , result.case4
                            , result.case5
                            , result.case6
                            , result.case7
                            , result.case8
                            , result.case9]
                        angular.forEach(vm.listFormCase, function (formCase, idx) {
                            try {
                                formCase.totalItems = lstTotal[idx];
                            }
                            catch (err) {
                            }
                        });
                        $scope.$apply();
                    });
                }
                if (vm.filter.formId) {
                    abp.services.app.xuLyHoSoGridView38.getListFormCase(vm.filter.formId)
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
                }
                vm.isBtnXemHoSo = (dataItems) => {
                    return dataItems.formCase == 3 || dataItems.formCase == 4;
                }
                vm.isBtnChuyenLaiHoSo = (dataItems) => {
                    return dataItems.formCase == 2;
                }
            }
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
                        <button ng-if="vm.isBtnChuyenHoSo(dataItem)" class="btn btn-primary btn-xs blue ng-isolate-scope" ng-click="vm.mot_cua_phan_cong(this.dataItem)">
                            <span>Chuyển HS</span>
                        </button>
                        <button ng-if="vm.isBtnChuyenLaiHoSo(dataItem)" class="btn btn-primary btn-xs blue ng-isolate-scope" ng-click="vm.mot_cua_phan_cong(this.dataItem)">
                            <span>Chuyển lại HS</span>
                        </button>
                        <button ng-if="vm.isBtnXemHoSo(dataItem)" class="btn btn-primary btn-xs green ng-isolate-scope" ng-click="vm.xemHoSoFull(this.dataItem)">
                            <span>Xem HS</span>
                        </button>
                        </div > `,
                        width: 120
                    },
                    {
                        headerTemplate: "Thông tin đơn hàng"
                        , template: `<div ng-show="vm.filter.formCase == 1"> Mã HS:<b> </b><a ng-click="vm.mot_cua_phan_cong(this.dataItem)"><b>{{this.dataItem.maHoSo}}</b> </a><br/>Số ĐH: <b>{{this.dataItem.soDangKy}}</b></div>
                            <div ng-show="vm.filter.formCase != 1"> Mã HS: <b></b><b>{{this.dataItem.maHoSo}}</b> <br/>Số ĐH: <b>{{this.dataItem.soDangKy}}</b></div>`
                    },
                    {
                        title: "Doanh nghiệp",
                        template:
                            `<div>
                                    {{ this.dataItem.tenDoanhNghiep }}
                                </div>`
                    },
                    {
                        title: app.localize('Ngày Tiếp nhận'),
                        template: "{{this.dataItem.ngayTiepNhan | date:'dd/MM/yyyy'}}",
                        attributes: { class: "text-center" },
                        headerAttributes: { style: "text-align: center;" },
                        width: 130
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
                        width: 120, hidden: true
                    },
                    {
                        headerTemplate: app.localize('Trạng thái hồ sơ')
                        , template: "<div><text>{{this.dataItem.strTrangThai}}</text></div>"
                        , attributes: { class: "text-center" }
                        , headerAttributes: { style: "text-align: center;" }
                        , width: 150
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
                    columns: vm.gridColumn,
                };
                vm.quickSearchHoSo = () => {
                    vm.gridHoSoDataSource.read();
                    vm.loadTotalLabel();
                    vm.dropdownSearchIsOpen = false;
                }
            }
            var mainFunc = () => {
                vm.formCaseOnChange = (formCase) => {
                    vm.filter.formCase = formCase;
                    vm.gridHoSoDataSource.page(1);
                }
                vm.mot_cua_phan_cong = (dataItem) => {
                    vm.formview = 'phan_cong';
                    vm.hosoitems = dataItem;
                    console.log(dataItem);
                }
                vm.xemHoSoFull = function (dataItem) {
                    abp.ui.setBusy();
                    let _maThuTuc = dataItem.thuTucId;
                    if (_maThuTuc < 10) _maThuTuc = '0' + _maThuTuc;
                    if (dataItem && dataItem.id > 0) {
                        var modalData = {
                            title: 'Xem hồ sơ',
                            id: dataItem.id,
                            formview: vm.form,
                        }
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
                }
            }

            var init = () => {
                initVar();
                girdHoSo();
                mainFunc();
            }
            init();
        }
    ]);
})();