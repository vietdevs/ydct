(function () {
    appModule.controller('quanlyhoso.thutuc99.views.xacnhanthanhtoan.index', [
        '$uibModal', 'baseService', 'appSession', 'abp.services.app.commonLookup', 'abp.services.app.xuLyHoSoKeToan99',
        function ($uibModal,baseService, appSession, CommonLookupAppService, xuLyHoSoKeToanService ) {
            var vm = this;
            vm.show_mode = null;
            vm.loading = false;
            vm.requestParams = {
                skipCount: 0,
                maxResultCount: 10,
                sorting: null
            };
            vm.listFormCase = [];
            vm.filterInit = {
                formCase: '1', // 0:ALL, 1:CHUA_XAC_NHAN, 2:DA_XAC_NHAN
                page: 1,
                pageSize: 10,
                Filter: null,
                nhomSanPhamId: null,
                tinhId: null
            };
            vm.filter = angular.copy(vm.filterInit);

            vm.hoSoGridOptions = {
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
                scrollable: true
            };

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
                        `<div class="ui-grid-cell-contents">
                            <div class="btn-group dropdown" uib-dropdown="" dropdown-append-to-body>
                                <button class="btn btn-xs blue-steel" uib-dropdown-toggle="" aria-haspopup="true" aria-expanded="false">
                                    <em class="fa fa-cog"></em> Thao Tác <span class="caret"></span>
                                </button>
                                <ul uib-dropdown-menu busy-if= "vm.action_loading">
                                    <li><a ng-click="vm.xemThanhToan(this.dataItem)">Xem thanh toán</a></li>
                                    <li><a ng-if="this.dataItem.trangThaiKeToan == 0" ng-click="vm.openXacNhanThanhToan(this.dataItem)">Xác nhận thanh toán</a></li>
                                </ul>
                            </div>
                        </div>`,
                    width: "150px"
                },
                {
                    field: "",
                    title: app.localize('Sản phẩm'),
                    template:
                        '<div>'
                        + '     <strong>Số đơn hàng:</strong> {{ this.dataItem.soDangKy }}<br/>'
                        + '     <strong>Tên tổ chức:</strong> {{ this.dataItem.tenDoanhNghiep }} - <em class="fa fa-map-marker"></em> <em>{{ this.dataItem.strTinh }}</em><br/>'
                        + '     <strong>Loại hồ sơ:</strong> {{ this.dataItem.strLoaiHoSo }}<br/>'
                        + '     <strong>Tên sản phẩm:</strong> {{ this.dataItem.tenSanPham}}<br/>'
                        + '     <strong ng-if="this.dataItem.isNhapKhau == false">Hàng trong nước</strong> <strong ng-if="this.dataItem.isNhapKhau == true">Nhập khẩu từ {{this.dataItem.strTenQuocGia}}</strong>'
                        + '</div>'
                },
                {
                    field: "",
                    title: app.localize('Thông tin thanh toán'),
                    template:
                        '<div>'
                        + '     <strong>Mã giao dịch:</strong> {{ this.dataItem.maGiaoDich }}<br/>'
                        + '     <strong>Mã đơn hàng:</strong> {{ this.dataItem.maDonHang }}<br/>'
                        + '     <strong>Số tài khoản nộp:</strong> {{ this.dataItem.soTaiKhoanNop }}<br/>'
                        + '     <strong>Số tài khoản hưởng:</strong> {{ this.dataItem.soTaiKhoanHuong }}<br/>'
                        + '</div>'
                },
                {
                    field: "phiDaNop",
                    title: app.localize('Phí đã nộp'),
                    //template: "<div align='center'>{{this.dataItem.phiDaNop | number: 0}} VNĐ</div>",
                    format: "{0:n0} VNĐ",
                    width: "200px"
                },
                {
                    field: "trangThaiKeToan",
                    title: app.localize('Trạng thái xác nhận'),
                    template: '<p style="margin: 0;text-align:center;"><label ng-if="#: trangThaiKeToan # == 0" class="label label-default">Đang chờ xác nhận</label><label ng-if="#: trangThaiKeToan # == 1" class="label label-success">Thanh toán hợp lệ</label><label ng-if="#: trangThaiKeToan # == 2" class="label label-danger">Không hợp lệ</label></p>',
                    width: "150px"
                },
                {
                    field: "ngayGiaoDich",
                    title: app.localize('Ngày thanh toán'),
                    template: `<strong>Ngày nộp:</strong> {{this.dataItem.ngayGiaoDich | date:'dd/MM/yyyy'}}`,
                    width: "150px"
                }
            ];


            //Dropdownlist
            {

                vm.nhomSanPhamOptions = {
                    dataSource: appSession.nhomSanPham,
                    dataValueField: "id",
                    dataTextField: "name",
                    optionLabel: app.localize('Chọn ...'),
                    filter: "contains"
                };
                vm.tinhOptions = {
                    dataSource: appSession.get_tinh(),
                    dataValueField: "id",
                    dataTextField: "ten",
                    optionLabel: app.localize('Chọn ...'),
                    filter: "contains"
                };
            }

            vm.gridHoSoDataSource = new kendo.data.DataSource({
                transport: {
                    read: function (options) {
                        vm.requestParams.skipCount = (options.data.page - 1) * options.data.pageSize;
                        vm.requestParams.maxResultCount = options.data.pageSize;
                        vm.loading = true;
                        xuLyHoSoKeToanService.getListThanhToanChuyenKhoanPaging($.extend(vm.filter, vm.requestParams))
                            .then(function (result) {
                                vm.gvHoSoCallBack = options;
                                var i = 1;
                                result.data.items.forEach(function (item) {
                                    item.STT = i;
                                    i++;
                                });
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
            };

            vm.refreshControll = function () {
                vm.filter = angular.copy(vm.filterInit);
                vm.refreshGridHS();
            };

            vm.xemThanhToan = function (hoSo) {
                var modalInstance = $uibModal.open({
                    templateUrl: '~/App/quanlythanhtoan/common/modal/xemThanhToanHoSoModal.cshtml',
                    controller: 'quanlythanhtoan.common.modal.xemThanhToanHoSoModal as vm',
                    backdrop: 'static',
                    size: 'lg',
                    resolve: {
                        hoSo: { id: hoSo.hoSoId, soDangKy: hoSo.soDangKy }
                    }
                });
                modalInstance.result.then(function (result) {
                    vm.refreshGridHS();
                });
            };
            
            vm.duyetThanhToan = {};
            

            vm.thanhToan_Reset = function () {
                vm.duyetThanhToan = {
                    thanhToanIsDat_Radio: '1',
                    yKienXacNhan: ''
                };
            };

            vm.openXacNhanThanhToan = function (dataItem) {
                vm.dataItem = dataItem;
                var _id = dataItem.thanhToanId_Active;
                if (_id > 0) {
                    //RESET-INFO
                    vm.thanhToan_Reset();

                    var params = {
                        thanhToanId: _id
                    };
                    xuLyHoSoKeToanService.loadXacNhanThanhToan(params)
                        .then(function (result) {
                            if (result.data) {
                                vm.thanhToan = result.data.thanhToan;
                                if (!baseService.isNullOrEmpty(vm.thanhToan.ghiChu)) {
                                    vm.thanhToan.ghiChu = vm.thanhToan.ghiChu.replace('\n', '<br/>');
                                }
                                vm.show_mode = 'xac_nhan_thanh_toan';
                            }
                        }).finally(function () {

                        });
                }
            };

            vm.saveXacNhanThanhToan = function () {

                //Validate
                var flagNull = (vm.duyetThanhToan.thanhToanIsDat_Radio == '1') || (vm.duyetThanhToan.thanhToanIsDat_Radio == '0' && baseService.isNullOrEmpty(vm.duyetThanhToan.yKienXacNhan) == false);

                if (!flagNull) {
                    abp.notify.error('Mời nhập dữ liệu');
                    vm.saving = false;
                    return;
                } else {
                    abp.message.confirm(
                        app.localize('Chắc chắn xác nhận'),
                        "Bạn chắc chắn xác nhận thanh toán?",
                        function (isConfirmed) {
                            if (isConfirmed) {
                                vm.saving = true;
                                var input = {
                                    XacNhanThanhToan: vm.duyetThanhToan.thanhToanIsDat_Radio,
                                    YKienXacNhan: vm.duyetThanhToan.yKienXacNhan,
                                    HoSoId: vm.dataItem.id,
                                    ThanhToanId: vm.thanhToan.id,
                                    PhiXacNhan: vm.thanhToan.phiDaNop
                                };
                                xuLyHoSoKeToanService.keToanDuyet_Chuyen(input)
                                    .then(function (result) {
                                        vm.saving = false;
                                        abp.notify.info(app.localize('SavedSuccessfully'));
                                        vm.refreshGridHS();
                                        vm.show_mode = null;

                                    }).finally(function () {

                                    });
                            }
                        }
                    );
                }
            };

            vm.closeModal = function () {
                vm.show_mode = null;
            };

            function init() {
                CommonLookupAppService.getFormCaseXacNhanThanhToan()
                    .then(function (result) {
                        if (result && result.data) {
                            vm.listFormCase = result.data;
                        }
                    });
            }
            init();
        }
    ]);
})();