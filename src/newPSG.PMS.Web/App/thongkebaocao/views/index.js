(function () {
    appModule.controller('quanlyhoso.thongkebaocao.views.index', [
        '$scope', '$rootScope', '$timeout', '$uibModal', '$interval', '$filter', 'appSession',
        function ($scope, $rootScope, $timeout, $uibModal, $interval, $filter, appSession, baseService) {
            var vm = this;
            var today = new Date();
            vm.loading = false;
            vm.isShowThongke = false;
            vm.filterInit = {
                HasMaHoSo: true,
                HasTenSanPham: true,
                HasNhomSanPham: true,
                HasMaSoThue: true,
                HasTenDoanhNghiep: true,
                HasDiachi: true,
                HasNgayNop: true,
                HasNgayTraKetQua: true,
                HasTenVaDiaChiNhaSanXuat: true,
                HasTinh: true,
                HasTrangThai: true,
                HasLanhDaoCucId: true,
                HasTruongPhongId: true,
                HasPhoPhongId: true,
                HasCv1Id: true,
                HasIsQuanLyGia: true,

                MaHoSo: "",
                TenSanPham: "",
                NhomSanPhamId: null,
                MaSoThue: "",
                TenDoanhNghiep: "",
                DiaChi: "",
                TenVaDiaChiNhaSanXuat: "",
                TinhId: null,
                TrangThai: null,
                NgayNopTu: new Date(today.getFullYear(), today.getMonth() - 1, today.getDate()),
                NgayNopToi: new Date(),
                NgayTraKetQuaTu: null,
                NgayTraKetQuaToi: null,
                Cv1Id: null,
                LanhDaoCucId: null,
                TruongPhongId: null,
                PhoPhongId: null,
                IsQuanLyGia: undefined
            }
            vm.tieuChi = {};
            vm.listThongKe = [];

            vm.filter = angular.copy(vm.filterInit);

            //Cau hinh controll
            {
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
                vm.nhomSanPhamOptions = {
                    dataSource: appSession.nhomSanPham,
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
                vm.trangThaiTraCuuOptions = {
                    dataSource: null,
                    dataValueField: "id",
                    dataTextField: "name",
                    optionLabel: app.localize('Chọn ...'),
                    filter: "contains",
                }
                vm.chuyenVienChinhOptions = {
                    dataSource: null,
                    dataValueField: "id",
                    dataTextField: "name",
                    optionLabel: app.localize('Chọn ...'),
                    filter: "contains",
                }
                vm.lanhDaoCucOptions = {
                    dataSource: null,
                    dataValueField: "id",
                    dataTextField: "name",
                    optionLabel: app.localize('Chọn ...'),
                    filter: "contains",
                }
                vm.truongPhongOptions = {
                    dataSource: null,
                    dataValueField: "id",
                    dataTextField: "name",
                    optionLabel: app.localize('Chọn ...'),
                    filter: "contains",
                }
                vm.phoPhongOptions = {
                    dataSource: null,
                    dataValueField: "id",
                    dataTextField: "name",
                    optionLabel: app.localize('Chọn ...'),
                    filter: "contains",
                }
            }

            //Function
            {
                vm.datLai = function () {
                    vm.filter = angular.copy(vm.filterInit);
                    vm.isShowThongke = false;
                }
                vm.thongKe = function () {
                    vm.loading = true;

                    vm.filter.NgayNopTu = vm.NgayNopModel.startDate;
                    vm.filter.NgayNopToi = vm.NgayNopModel.endDate;

                    vm.filter.NgayTraKetQuaTu = vm.NgayTraKetQuaModel.startDate;
                    vm.filter.NgayTraKetQuaToi = vm.NgayTraKetQuaModel.endDate;

                    //thongKeDangKyCongBoService.thongKeTheoYeuCau(vm.filter)
                    //    .then(function (result) {
                    //        var i = 1;
                    //        result.data.lstThongKe.forEach(function (item) {
                    //            item.sTT = i;
                    //            i++;
                    //        });
                    //        vm.listThongKe = result.data.lstThongKe;
                    //        console.log(vm.listThongKe, "vm.listThongKe");
                    //        vm.tieuChi = result.data.tieuChiThongKe;
                    //        options.success(result.data);

                    //    }).finally(function () {
                    //        vm.loading = false;
                    //        vm.isShowThongke = true;
                    //    });
                }

                vm.exportToExcel = function (tableId) {
                    baseService.excelReport(tableId, 'thong_ke_theo_yeu_cau');
                }
            }
        }
    ]);
})();