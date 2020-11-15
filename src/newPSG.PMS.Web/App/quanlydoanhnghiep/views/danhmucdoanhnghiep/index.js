(function () {
    appModule.controller('quanlydoanhnghiep.views.danhmucdoanhnghiep.index', [
        '$scope', '$uibModal', '$interval', '$filter', 'abp.services.app.tinh', 'abp.services.app.loaiHinhDoanhNghiep', 'abp.services.app.commonLookup', 'abp.services.app.doanhNghiep', 'abp.services.app.user', 'appSession',
        function ($scope, $uibModal, $interval, $filter, tinhService, loaiHinhService, commonLookupService, doanhNghiepService, userService, appSession) {
            var vm = this;
            var today = new Date();
            vm.dsdoanhnghiep_loading = false;
            vm.filter = {
                page: 1,
                pageSize: 10,
                formCase: "1",
                Filter: "",
                TinhId: null,
                IsActive: true,
                LoaiHinhDoanhNghiepId: null
            };
            vm.permissions = {
                create: abp.auth.hasPermission('Pages.QuanLyDoanhNghiep.Create'),
                edit: abp.auth.hasPermission('Pages.QuanLyDoanhNghiep.Edit'),
                delete: abp.auth.hasPermission('Pages.QuanLyDoanhNghiep.Delete')
            };
            vm.tinhOptions = {
                dataSource: appSession.get_tinh(),
                dataValueField: "id",
                dataTextField: "ten",
                optionLabel: app.localize('Chọn ...'),
                filter: "contains"
            };
            vm.loaiHinhOptions = {
                dataSource: appSession.get_loaihinh(),
                dataValueField: "id",
                dataTextField: "tenLoaiHinh",
                optionLabel: app.localize('Chọn ...'),
                filter: "contains"
            };
            //-----Search khach hang-----//
            vm.refreshGridDoanhNghiep = function () {
                vm.gridDoanhNghiepDataSource.transport.read(vm.gvDoanhNghiepCallBack);
            };
            vm.doanhNghiepGridOptions = {
                pageable:
                {
                    "refresh": true,
                    "pageSizes": true,
                    messages: {
                        empty: "Không có dữ liệu",
                        display: "Tổng {2} doanh nghiệp",
                        itemsPerPage: "Doanh nghiệp mỗi trang"
                    }
                },
                resizable: true,
                scrollable: true
            };
            var gvDoanhNghiepDS = function () {
                vm.dsdoanhnghiep_loading = true;
                vm.gridDoanhNghiepDataSource = new kendo.data.DataSource({
                    transport: {
                        read: function (options) {
                            vm.filter.skipCount = (options.data.page - 1) * options.data.pageSize;
                            vm.filter.maxResultCount = options.data.pageSize;
                            doanhNghiepService.getAllDoanhNghiepServerPaging(vm.filter)
                                .then(function (result) {
                                    vm.gvDoanhNghiepCallBack = options;
                                    var i = 1;
                                    result.data.items.forEach(function (item) {
                                        item.STT = i;
                                        i++;
                                    });
                                    options.success(result.data);
                                }).finally(function () {
                                    vm.dsdoanhnghiep_loading = false;
                                });
                        }
                    },
                    pageSize: 10,
                    serverPaging: true,
                    sortable: true,
                    selectable: false,
                    schema: {
                        data: "items",
                        total: "totalCount"
                    }
                });
            };
            vm.initGridDoanhNghiep = function () {
                gvDoanhNghiepDS();
            };
            var init = function () {
                doanhNghiepService.getListFormCaseDoanhNghiep()
                    .then(function (result) {
                        console.log(result.data, 'vm.listFormCase');
                        if (result.data) {
                            vm.listFormCase = result.data;
                        }
                    }).finally(function () {
                    });
                vm.initGridDoanhNghiep();
            };
            init();
            vm.columnGridDoanhNghiep = [
                {
                    field: "STT",
                    title: app.localize('STT'),
                    width: "50px",
                    attributes: { class: "text-center" },
                    headerAttributes: { style: "text-align: center;" },
                    template: "<div>{{this.dataItem.STT}}</div>"
                },
                {
                    field: "",
                    title: "Thao Tác",
                    attributes: { class: "text-center" },
                    headerAttributes: { style: "text-align: center;" },
                    template: '<div class=\"ui-grid-cell-contents\">' +
                        '  <div class="btn-group dropdown" uib-dropdown="" dropdown-append-to-body>' +
                        '    <button class="btn btn-xs green-meadow" uib-dropdown-toggle="" aria-haspopup="true" aria-expanded="false"><i class="fa fa-cog"></i> ' + 'Thao Tác' + ' <span class="caret"></span></button>' +
                        '    <ul uib-dropdown-menu>' +
                        '      <li><a ng-click="vm.viewDetail(this.dataItem)">' + '<text ng-if="this.dataItem.isDaXuLy == true">Xem chi tiết</text>' + '<text ng-if="this.dataItem.isDaXuLy != true"> Xem và duyệt</text>' + '</a></li>' +
                        '      <li ng-if="this.dataItem.isDaXuLy != true"><a ng-click="vm.kichHoatNhanh(this.dataItem)">' + '<text> Kích hoạt nhanh</text>' + '</a></li>' +
                        '      <li ng-if="this.dataItem.userActive == true && this.dataItem.isDaXuLy == true"><a ng-click="vm.changeActiveDoanhNghiep(this.dataItem)">' + 'Khóa tài khoản' + '</a></li>' +
                        '      <li ng-if="this.dataItem.userActive == false && this.dataItem.isDaXuLy == true"><a ng-click="vm.changeActiveDoanhNghiep(this.dataItem)">' + 'Kích hoạt' + '</a></li>' +
                        '      <li ng-if="this.dataItem.isDaXuLy == true && this.dataItem.userActive!=null"><a ng-click="vm.changePassword(this.dataItem)">' + 'Đổi mật khẩu' + '</a></li>' +
                        '    </ul>' +
                        '  </div>' +
                        '</div>',
                    width: 120
                },
                {
                    field: "",
                    title: app.localize('Thông tin doanh nghiệp'),
                    template:
                        `<div>
                            <b>Tên doanh nghiệp:</b> {{ this.dataItem.tenDoanhNghiep }} - <i class="fa fa-map-marker"></i> <i>{{ this.dataItem.tinh }}</i><br/>
                            <b>Mã số doanh nghiệp:</b> {{ this.dataItem.maSoThue }} <br/>
                            <b>Loại hình doanh nghiệp:</b> {{ this.dataItem.tenLoaiHinh }}
                        </div>`,
                    width: '30%'
                }
                , {
                    field: "tenDoanhNghiep",
                    title: app.localize('Thông tin liên hệ'),
                    template:
                        `<div>
                            <b>Người đại diện:</b> {{ this.dataItem.tenNguoiDaiDien }}<br/>
                            <b>Số điện thoại:</b> {{this.dataItem.soDienThoai}}<br/>
                            <b>Email:</b> {{ this.dataItem.emailDoanhNghiep }}<br/>
                            <b>Địa chỉ:</b> {{ this.dataItem.diaChi }}<br/>
                        </div>`,
                    width: '30%'
                },
                {
                    field: "creationTime",
                    title: app.localize('Ngày đăng ký'),
                    attributes: { class: "text-center" },
                    headerAttributes: { style: "text-align: center;" },
                    template: "{{dataItem.creationTime | date:'dd/MM/yyyy HH:mm:ss'}}",
                    width: 120
                },
                {
                    field: "isDaXuLy",
                    title: "Đã duyệt",
                    attributes: { class: "text-center" },
                    headerAttributes: { style: "text-align: center;" },
                    template: '<p style="margin: 0;text-align:center;"><i ng-if="#: isDaXuLy # == 1" class="fa fa-check fa-3 font-green-meadow" aria-hidden="true"></i><i ng-if="#: isDaXuLy # == 0" class="fa fa-times fa-3 font-red"></i></p>',
                    width: 100
                },
                {
                    field: "userActive",
                    title: "Trạng Thái",
                    attributes: { class: "text-center" },
                    headerAttributes: { style: "text-align: center;" },
                    template: '<p style="margin: 0;text-align:center;"><label class="label label-success" ng-if="#: userActive # == 1">Kích hoạt</label><label ng-if="#: userActive # == 0" class="label label-danger"> Chưa kích hoạt</label> <label ng-if="#: userActive # == null" class="label label-warning"> Không chấp nhận</label></p>',
                    width: 120
                }
            ];
            vm.changeActiveDoanhNghiep = function (_data) {
                doanhNghiepService.changeActiveDoanhNghiep(_data.id).then(function (result) {
                    abp.notify.success(app.localize('SavedSuccessfully'));
                    vm.refreshGridDoanhNghiep();
                });
            };
            vm.viewDetail = function (e) {
                var modalInstance = $uibModal.open({
                    templateUrl: '~/App/quanlydoanhnghiep/views/danhmucdoanhnghiep/detailModal.cshtml',
                    controller: 'quanlydoanhnghiep.views.danhmucdoanhnghiep.detailModal as vm',
                    backdrop: 'static',
                    resolve: {
                        detailData: e
                    }
                });
                modalInstance.result.then(function (result) {
                    vm.refreshGridDoanhNghiep();
                });
            };
            vm.changePassword = function (_data) {
                var modalInstance = $uibModal.open({
                    templateUrl: '~/App/quanlydoanhnghiep/views/danhmucdoanhnghiep/changePasswordModal.cshtml',
                    controller: 'quanlydoanhnghiep.views.danhmucdoanhnghiep.changePasswordModal as vm',
                    backdrop: 'static',
                    resolve: {
                        detailData: _data
                    }
                });
                modalInstance.result.then(function (result) {
                    vm.refreshGridDoanhNghiep();
                });
            };
            vm.kichHoatNhanh = function (data) {
                abp.message.confirm('Chắc chắn kích hoạt tài khoản cho doanh nghiệp ' + data.tenDoanhNghiep
                    , app.localize('Bạn chắc chắn muốn kích hoạt tài khoản cho doanh nghiệp?'),
                    function (isConfirmed) {
                        if (isConfirmed) {
                            vm.dsdoanhnghiep_loading = true;
                            doanhNghiepService.moTaiKhoanDoanhNghiep(data.id).then(function (result) {
                                vm.dsdoanhnghiep_loading = false;
                                vm.refreshGridDoanhNghiep();
                                abp.notify.success(app.localize('SavedSuccessfully'));
                            });
                        }
                    }
                );
            };
            vm.formCaseOnChange = function () {
                vm.refreshGridDoanhNghiep();
            };
            vm.refreshDoanhNghiep = function () {
                vm.filter.page = 1;
                vm.filter.pageSize = 10;
                vm.filter.Filter = null;
                vm.filter.TinhId = null;
                vm.filter.LoaiHinhDoanhNghiepId = null;
                vm.filter.IsActive = true;
                vm.initGridDoanhNghiep();
            };
        }
    ]);
})();