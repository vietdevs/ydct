(function () {
    appModule.directive('quanlyhoso.thutuc37.directives.gridhoso', ['$compile', '$templateRequest',
        'abp.services.app.xuLyHoSoGridView37', 'abp.services.app.xuLyHoSoDoanhNghiep37', 'abp.services.app.xuLyHoSoTruongPhong37', 'abp.services.app.xuLyHoSoLanhDaoCuc37', 'abp.services.app.xuLyHoSoVanThu37',
        'appSession', 'quanlyhoso.thutuc37.services.appChuKySo',
        function ($compile, $templateRequest, xuLyHoSoGridViewService, xuLyHoSoDoanhNghiepService, xuLyHoSoTruongPhongService, xuLyHoSoLanhDaoCucService, xuLyHoSoVanThuService, appSession, appChuKySo) {
            var controller = ['$rootScope', '$scope', '$uibModal',
                function ($rootScope, $scope, $uibModal) {
                    abp.ui.clearBusy();
                    var vm = this;
                    vm.loading = false;
                    vm.roleLevel = appSession.user.roleLevel;
                    vm.user = appSession.user;
                    console.log(vm.user, 'user');
                    vm.isTruongPhong = vm.roleLevel == app.ROLE_LEVEL.TRUONG_PHONG ? true : false;
                    vm.DON_VI_XU_LY = app.DON_VI_XU_LY;
                    vm.listFormCase = [];
                    vm.form = angular.copy($scope.form);
                    vm.filter = angular.copy($scope.filterobj);
                    vm.filter.formCase = angular.copy(vm.filter.formCase) + ''; //int?: lọc theo Radio Button => phải chuyển thành string
                    vm.filter.formCase2 = angular.copy(vm.filter.formCase2); //int?: lọc theo DropDownList => ko phải chuyển thành string
                    vm.classCssFormCase = 'form_' + vm.filter.formId + '_';

                    vm.requestParams = {
                        skipCount: 0,
                        maxResultCount: 10,
                        sorting: null
                    };
                    //--- Loại hồ sơ ---//
                    {
                        vm.arrLoaiHoSo = [];
                        const _arr = appSession.get_loaihoso().filter(item => item.thuTucId == appChuKySo.THU_TUC_ID);
                        if (_arr && _arr.length > 0) {
                            vm.arrLoaiHoSo = _arr;
                            vm.filter.loaiHoSoId = _arr[0].id;
                        }
                    }

                    //--- Danh sách hồ sơ ---//
                    {
                        //Search
                        {
                            vm.filter.ngayGuiTu = null;
                            vm.filter.ngayGuiToi = null;
                            vm.ngayNopModel = {
                                startDate: null,
                                endDate: null
                            };

                            vm.fMoRong = false;
                            vm.fMoRong_Change = function (fToggle) {
                                if (fToggle != true) {
                                    vm.fMoRong = true;
                                } else {
                                    vm.fMoRong = false;
                                }
                            };

                            vm.checkboxAll = false;
                            vm.quickSearchHoSo = function () {
                                vm.checkboxAll = false;
                                vm.refreshHoSo();
                            };
                            vm.initGridHoSo = function () {
                                vm.checkboxAll = false;
                                vm.filter = angular.copy($scope.filterobj);
                                vm.filter.ngayGuiTu = null;
                                vm.filter.ngayGuiToi = null;
                                vm.refreshHoSo();
                            }

                        }

                        //Gridview
                        {
                            vm.refreshColumns = function () {
                                vm.columns = [];
                                //1. STT
                                var _stt = {
                                    field: "STT",
                                    title: app.localize('STT'),
                                    attributes: { class: "text-center" },
                                    headerAttributes: { style: "text-align: center;" },
                                    width: 50, //locked: true,
                                    template: "<div align='center' title='{{this.dataItem.id + \"-\" + this.dataItem.hoSoXuLyId_Active}}'>{{this.dataItem.STT}}</div>"
                                };

                                //2. Thao tác
                                var _thaotac = function () {
                                    var _colthaotac = {
                                        field: "",
                                        title: app.localize('Thao Tác'),
                                        attributes: { class: "text-center" },
                                        headerAttributes: { style: "text-align: center;" },
                                        width: 120 //locked: true
                                    };
                                    switch (vm.form) {
                                        case 'dang_ky_ho_so':
                                            _colthaotac.template =
                                                `<div class="ui-grid-cell-contents text-center">
                                                <div class="btn-group dropdown" uib-dropdown="" dropdown-append-to-body>
                                                    <button class="btn btn-xs blue-soft" uib-dropdown-toggle="" aria-haspopup="true" aria-expanded="false"><em class="fa fa-cog"></em>Thao Tác<span class="caret"></span></button>
                                                    <ul uib-dropdown-menu busy-if= "vm.action_loading">
                                                        <li><a ng-click="vm.xemHoSoFull(this.dataItem)" ng-if="vm.arrExist(this.dataItem.arrChucNang, 9)"><em class="fa fa-eye"></em> Xem hồ sơ</a></li>
                                                        <li><a ng-click="vm.sua_ho_so(this.dataItem)" ng-if="vm.arrExist(this.dataItem.arrChucNang, 11)"><em class="fa fa-edit"></em> Sửa hồ sơ</a></li>
                                                        <li><a ng-click="vm.xemLyDoTraLai(this.dataItem)" ng-if="vm.arrExist(this.dataItem.arrChucNang, 96)"><em class="fa fa-ban"></em> Xem lý do trả lại</a></li>
                                                        <li><a ng-click="vm.nopHoSoDeRaSoat(this.dataItem)"  ng-if="vm.arrExist(this.dataItem.arrChucNang, 112)"><em class="fa fa-paper-plane-o"></em> Nộp hồ sơ</a></li>
                                                        <li><a ng-click="vm.nopHoSoBiTraLai(this.dataItem)" ng-if="vm.arrExist(this.dataItem.arrChucNang, 18)"><em class="fa fa-support"></em> Nộp hồ sơ bị trả lại</a></li>
                                                        <li><a ng-click="vm.nopHoSoBoSung(this.dataItem)" ng-if="vm.arrExist(this.dataItem.arrChucNang, 16)"><em class="fa fa-send"></em> Nộp hồ sơ bổ sung</a></li>
                                                        <li><a ng-click="vm.huyHoSo(this.dataItem)" ng-if="vm.arrExist(this.dataItem.arrChucNang, 12)"><em class="fa fa-ban"></em> Hủy hồ sơ</a></li>
                                                    </ul>
                                                </div>
                                            </div>`;
                                            break;
                                        case 'mot_cua_ra_soat':
                                            _colthaotac.template =
                                                `<div class="ui-grid-cell-contents">
                                                <div class="btn-group dropdown" uib-dropdown="" dropdown-append-to-body>
                                                    <button class="btn btn-xs blue-soft" uib-dropdown-toggle="" aria-haspopup="true" aria-expanded="false"><em class="fa fa-cog"></em> Thao Tác <span class="caret"></span></button>
                                                    <ul uib-dropdown-menu busy-if= "vm.action_loading">
                                                        <li><a ng-click="vm.xemHoSoFull(this.dataItem)" ng-if="vm.arrExist(this.dataItem.arrChucNang, 9)"><em class="fa fa-eye"></em> Xem hồ sơ</a></li>
                                                        <li><a ng-click="vm.mot_cua_ra_soat(this.dataItem)" ng-if="vm.arrExist(this.dataItem.arrChucNang, 21) && this.dataItem.giayTiepNhanCA == null"><em class="fa fa-search-plus"></em> Rà soát hồ sơ</a></li>
                                                        <li><a ng-click="vm.mot_cua_gui_phan_cong(this.dataItem)" ng-if="this.dataItem.giayTiepNhanCA != null && vm.filter.formCase == 1"><em class="fa fa-bars"></em> Gửi phân công</a></li>
                                                        
                                                    </ul>
                                                </div>
                                            </div>`;
                                            break;
                                        case 'mot_cua_phan_cong':
                                            _colthaotac.template =
                                                `<div class="ui-grid-cell-contents  text-center">
                                                <div class="btn-group dropdown" uib-dropdown="" dropdown-append-to-body>
                                                    <button class="btn btn-xs blue-soft" uib-dropdown-toggle="" aria-haspopup="true" aria-expanded="false"><em class="fa fa-cog"></em>Thao Tác<span class="caret"></span></button>
                                                    <ul uib-dropdown-menu busy-if= "vm.action_loading">
                                                        <li><a ng-click="vm.xemHoSoFull(this.dataItem)" ng-if="vm.arrExist(this.dataItem.arrChucNang, 9)"><em class="fa fa-eye"></em> Xem hồ sơ</a></li>
                                                        <li><a ng-click="vm.xemLyDoTraLai(this.dataItem)" ng-if="vm.filter.formCase == 1 && this.dataItem.trangThaiHoSo == 116"><em class="fa fa-eye"></em> Xem lý do trả lại</a></li> 
                                                        <li><a ng-click="vm.mot_cua_phan_cong(this.dataItem)" ng-if="vm.arrExist(this.dataItem.arrChucNang, 20)"><text ng-if="!this.dataItem.phongBanId"><em class="fa fa-bars"></em> Phân công phòng ban</text> <text ng-if="this.dataItem.phongBanId"><em class="fa fa-bars"></em>Phân công phòng ban</text></a></li>
                                                        
                                                    </ul>
                                                </div>
                                            </div>`;
                                            break;
                                        case 'phong_ban_phan_cong':
                                            _colthaotac.template =
                                                `<div class="ui-grid-cell-contents  text-center">
                                                <div class="btn-group dropdown" uib-dropdown="" dropdown-append-to-body>
                                                    <button class="btn btn-xs blue-soft" uib-dropdown-toggle="" aria-haspopup="true" aria-expanded="false"><em class="fa fa-cog"></em>Thao Tác<span class="caret"></span></button>
                                                    <ul uib-dropdown-menu busy-if= "vm.action_loading">
                                                        <li><a ng-click="vm.xemHoSoFull(this.dataItem)" ng-if="vm.arrExist(this.dataItem.arrChucNang, 9)"><em class="fa fa-eye"></em> Xem hồ sơ</a></li>
                                                        <li><a ng-click="vm.xemLyDoTraLai(this.dataItem)" ng-if="vm.filter.formCase == 1 && this.dataItem.trangThaiHoSo == 117"><em class="fa fa-eye"></em> Xem lý do trả lại</a></li> 
                                                        <li><a ng-click="vm.phong_ban_phan_cong(this.dataItem)" ng-if="vm.arrExist(this.dataItem.arrChucNang, 25)"><em class="fa fa-users"></em> Phân công cán bộ</a></li>
                                                        <li><a ng-click="vm.phong_ban_phan_cong(this.dataItem)" ng-if="vm.arrExist(this.dataItem.arrChucNang, 26)"><em class="fa fa-users"></em> Phân công lại</a></li>
                                                        <li><a ng-click="vm.phong_ban_tra_lai(this.dataItem)" ng-if="vm.filter.formCase == 1 && this.dataItem.trangThaiHoSo == 111"><em class="fa fa-ban"></em> Trả lại lãnh đạo</a></li> 
                                                       
                                                    </ul>
                                                </div>
                                            </div>`;
                                            break;
                                        case 'tham_xet_ho_so':
                                            _colthaotac.template =
                                                `<div class="ui-grid-cell-contents">
                                            <div class="btn-group dropdown" uib-dropdown="" dropdown-append-to-body>
                                                <button class="btn btn-xs blue-soft" uib-dropdown-toggle="" aria-haspopup="true" aria-expanded="false"><em class="fa fa-cog"></em> Thao Tác<span class="caret"></span></button>
                                                <ul uib-dropdown-menu busy-if= "vm.action_loading">
                                                    <li><a ng-click="vm.xemHoSoFull(this.dataItem)" ng-if="vm.arrExist(this.dataItem.arrChucNang, 9)"><em class="fa fa-eye"></em>Xem hồ sơ</a></li>
                                                    <li><a ng-click="vm.xemLyDoTraLai(this.dataItem)" ng-if="vm.arrExist(this.dataItem.arrChucNang, 96)"><em class="fa fa-eye"></em> Xem lý do trả lại</a></li>
                                                    <li><a ng-click="vm.tham_xet_ho_so(this.dataItem)" ng-if="vm.arrExist(this.dataItem.arrChucNang, 3)"><em class="fa fa-bars"></em> Thẩm xét hồ sơ</a></li>
                                                    <li><a ng-click="vm.tham_xet_ho_so(this.dataItem)" ng-if="vm.arrExist(this.dataItem.arrChucNang, 32)"><em class="fa fa-bars"></em> Thẩm xét lại hồ sơ</a></li>
                                                    <li><a ng-click="vm.chuyen_vien_tra_lai_ho_so(this.dataItem)" ng-if="vm.filter.formCase == 1 && this.dataItem.trangThaiHoSo == 112"><em class="fa fa-ban"></em> Trả lại trưởng phòng</a></li>
                                                </ul>
                                            </div>
                                         </div>`;
                                            break;
                                        case 'truong_phong_duyet':
                                            _colthaotac.template =
                                                `<div class="ui-grid-cell-contents  text-center">
                                                <div class="btn-group dropdown" uib-dropdown="" dropdown-append-to-body>
                                                    <button class="btn btn-xs blue-soft" uib-dropdown-toggle="" aria-haspopup="true" aria-expanded="false"><em class="fa fa-cog"></em>Thao Tác<span class="caret"></span></button>
                                                    <ul uib-dropdown-menu busy-if= "vm.action_loading">
                                                        <li><a ng-click="vm.xemHoSoFull(this.dataItem)" ng-if="vm.arrExist(this.dataItem.arrChucNang, 9)"><em class="fa fa-eye"></em> Xem hồ sơ</a></li>
                                                        <li><a ng-click="vm.xemCongVan(this.dataItem)" ng-if="vm.arrExist(this.dataItem.arrChucNang, 92)"><em class="fa fa-eye"></em> Xem công văn</a></li>
                                                        <li><a ng-click="vm.xemLyDoTraLai(this.dataItem)" ng-if="vm.arrExist(this.dataItem.arrChucNang, 96)"><em class="fa fa-eye"></em> Xem lý do trả lại</a></li>
                                                        <li><a ng-click="vm.truong_phong_duyet(this.dataItem)"      ng-if="vm.arrExist(this.dataItem.arrChucNang, 4)"><em class="fa fa-star"></em> Duyệt hồ sơ</a></li>
                                                        <li><a ng-click="vm.truong_phong_duyet(this.dataItem)"      ng-if="vm.arrExist(this.dataItem.arrChucNang, 43)"><em class="fa fa-star"></em> Duyệt hồ sơ lại</a></li>
                                                        <li><a ng-click="vm.xemBienBanThamDinh(this.dataItem)"  ng-if="vm.arrExist(this.dataItem.arrChucNang, 95)"><em class="fa fa-folder-open"></em> Xem biên bản thẩm định</a></li>
                                                    </ul>
                                                </div>
                                            </div>`;
                                            break;
                                        case 'lanh_dao_cuc_duyet':
                                            _colthaotac.template =
                                                `<div class="ui-grid-cell-contents  text-center">
                                                <div class="btn-group dropdown" uib-dropdown="" dropdown-append-to-body>
                                                <button class="btn btn-xs blue-soft" uib-dropdown-toggle="" aria-haspopup="true" aria-expanded="false"><em class="fa fa-cog"></em>Thao Tác<span class="caret"></span></button>
                                                <ul uib-dropdown-menu busy-if= "vm.action_loading">
                                                    <li><a ng-click="vm.xemHoSoFull(this.dataItem)" ng-if="vm.arrExist(this.dataItem.arrChucNang, 9)"><em class="fa fa-eye"></em> Xem hồ sơ</a></li>
                                                    <li><a ng-click="vm.lanh_dao_cuc_duyet(this.dataItem)"   ng-if="vm.arrExist(this.dataItem.arrChucNang, 5)"><em class="fa fa-check-circle-o"></em> Duyệt hồ sơ</a></li>
                                                    <li><a ng-click="vm.xemBienBanThamDinh(this.dataItem)"  ng-if="vm.arrExist(this.dataItem.arrChucNang, 95)"><em class="fa fa-folder-open"></em> Xem biên bản thẩm định</a></li>
                                                </ul>
                                                </div>
                                            </div>`;
                                            break;
                                       
                                        case 'van_thu_duyet':
                                            _colthaotac.template =
                                                `<div class="ui-grid-cell-contents  text-center">
                                                <div class="btn-group dropdown" uib-dropdown="" dropdown-append-to-body>
                                                    <button class="btn btn-xs blue-soft" uib-dropdown-toggle="" aria-haspopup="true" aria-expanded="false"><em class="fa fa-cog"></em>Thao Tác<span class="caret"></span></button>
                                                    <ul uib-dropdown-menu busy-if= "vm.action_loading">
                                                        <li><a ng-click="vm.xemHoSoFull(this.dataItem)" ng-if="vm.arrExist(this.dataItem.arrChucNang, 9)"><em class="fa fa-eye"></em> Xem hồ sơ</a></li>
                                                        <li><a ng-click="vm.van_thu_duyet(this.dataItem)"  ng-if="vm.arrExist(this.dataItem.arrChucNang, 7)"><em class="fa fa-hourglass"></em> Xem & đóng dấu</a></li>
                                                        <li><a ng-click="vm.vanThuDongDauNhanh(this.dataItem)"  ng-if="vm.arrExist(this.dataItem.arrChucNang,1000)"><em class="fa fa-flash"></em> Đóng dấu nhanh</a></li>
                                                    </ul>
                                                </div>
                                            </div>`;
                                            break;
                                        case 'tham_dinh_ho_so':
                                            _colthaotac.template =
                                                `<div class="ui-grid-cell-contents">
                                            <div class="btn-group dropdown" uib-dropdown="" dropdown-append-to-body>
                                                <button class="btn btn-xs blue-soft" uib-dropdown-toggle="" aria-haspopup="true" aria-expanded="false"><em class="fa fa-cog"></em> Thao Tác<span class="caret"></span></button>
                                                <ul uib-dropdown-menu busy-if= "vm.action_loading">
                                                    <li><a ng-click="vm.xemHoSoFull(this.dataItem)" ng-if="vm.arrExist(this.dataItem.arrChucNang, 9)"><em class="fa fa-eye"></em> Xem hồ sơ</a></li>
                                                    <li><a ng-click="vm.xemDoanThamDinh(this.dataItem)"><em class="fa fa-calendar"></em> Xem đoàn thẩm định</a></li>
                                                    <li><a ng-click="vm.tham_dinh_ho_so(this.dataItem)" ng-if="vm.arrExist(this.dataItem.arrChucNang, 34)"><em class="fa fa-bars"></em> Thẩm định hồ sơ</a></li>
                                                </ul>
                                            </div>
                                        </div>`;
                                            break;
                                        case 'tong_hop_tham_dinh':
                                            _colthaotac.template =
                                                `<div class="ui-grid-cell-contents">
                                            <div class="btn-group dropdown" uib-dropdown="" dropdown-append-to-body>
                                                <button class="btn btn-xs blue-soft" uib-dropdown-toggle="" aria-haspopup="true" aria-expanded="false"><em class="fa fa-cog"></em> Thao Tác<span class="caret"></span></button>
                                                <ul uib-dropdown-menu busy-if= "vm.action_loading">
                                                    <li><a ng-click="vm.xemHoSoFull(this.dataItem)" ng-if="vm.arrExist(this.dataItem.arrChucNang, 9)"><em class="fa fa-eye"></em> Xem hồ sơ</a></li>
                                                    <li><a ng-click="vm.xemDoanThamDinh(this.dataItem)"><em class="fa fa-calendar"></em> Xem đoàn thẩm định</a></li>
                                                    <li><a ng-click="vm.tong_hop_tham_dinh(this.dataItem)" ng-if="vm.arrExist(this.dataItem.arrChucNang, 34)"><em class="fa fa-bars"></em> Tổng hợp thẩm định</a></li>
                                                    <li><a ng-click="vm.cap_nhat_ket_qua(this.dataItem)" ng-if="vm.filter.formCase == 2"><em class="fa fa-paperclip"></em> Cập nhật kết quả</a></li>
                                                    <li><a ng-click="vm.tong_hop_tham_dinh(this.dataItem)" ng-if="vm.arrExist(this.dataItem.arrChucNang, 35)"><em class="fa fa-check-square-o"></em> Tổng hợp lại</a></li>
                                                </ul>
                                            </div>
                                        </div>`;
                                            break;
                                        case 'truong_phong_duyet_tham_dinh':
                                            _colthaotac.template =
                                                `<div class="ui-grid-cell-contents">
                                            <div class="btn-group dropdown" uib-dropdown="" dropdown-append-to-body>
                                                <button class="btn btn-xs blue-soft" uib-dropdown-toggle="" aria-haspopup="true" aria-expanded="false"><em class="fa fa-cog"></em> Thao Tác<span class="caret"></span></button>
                                                <ul uib-dropdown-menu busy-if= "vm.action_loading">
                                                    <li><a ng-click="vm.xemHoSoFull(this.dataItem)" ng-if="vm.arrExist(this.dataItem.arrChucNang, 9)"><em class="fa fa-eye"></em> Xem hồ sơ</a></li>
                                                    <li><a ng-click="vm.truong_phong_duyet_tham_dinh(this.dataItem)" ng-if="vm.arrExist(this.dataItem.arrChucNang, 42)"><em class="fa fa-star"></em> Duyệt thẩm định</a></li>
                                                    <li><a ng-click="vm.truong_phong_duyet_tham_dinh(this.dataItem)" ng-if="vm.arrExist(this.dataItem.arrChucNang, 44)"><em class="fa fa-star"></em> Duyệt thẩm định lại</a></li>
                                                </ul>
                                            </div>
                                        </div>`;
                                            break;

                                        case 'lanh_dao_cuc_duyet_tham_dinh':
                                            _colthaotac.template =
                                                `<div class="ui-grid-cell-contents">
                                            <div class="btn-group dropdown" uib-dropdown="" dropdown-append-to-body>
                                                <button class="btn btn-xs blue-soft" uib-dropdown-toggle="" aria-haspopup="true" aria-expanded="false"><em class="fa fa-cog"></em> Thao Tác<span class="caret"></span></button>
                                                <ul uib-dropdown-menu busy-if= "vm.action_loading">
                                                    <li><a ng-click="vm.xemHoSoFull(this.dataItem)" ng-if="vm.arrExist(this.dataItem.arrChucNang, 9)"><em class="fa fa-eye"></em> Xem hồ sơ</a></li>
                                                    <li><a ng-click="vm.lanh_dao_cuc_duyet_tham_dinh(this.dataItem)" ng-if="vm.arrExist(this.dataItem.arrChucNang, 52)"><em class="fa fa-check-circle-o"></em> Duyệt thẩm định</a></li>
                                                </ul>
                                            </div>
                                        </div>`;
                                            break;

                                        case 'van_thu_duyet_tham_dinh':
                                            _colthaotac.template =
                                                `<div class="ui-grid-cell-contents">
                                            <div class="btn-group dropdown" uib-dropdown="" dropdown-append-to-body>
                                                <button class="btn btn-xs blue-soft" uib-dropdown-toggle="" aria-haspopup="true" aria-expanded="false"><em class="fa fa-cog"></em> Thao Tác<span class="caret"></span></button>
                                                <ul uib-dropdown-menu busy-if= "vm.action_loading">
                                                    <li><a ng-click="vm.xemHoSoFull(this.dataItem)" ng-if="vm.arrExist(this.dataItem.arrChucNang, 9)"><em class="fa fa-eye"></em> Xem hồ sơ</a></li>
                                                    <li><a ng-click="vm.van_thu_duyet_tham_dinh(this.dataItem)" ng-if="vm.filter.formCase == 1"><em class="fa fa-hourglass"></em>Trả kết quả</a></li>
                                                </ul>
                                            </div>
                                        </div>`;
                                            break;
                                    }
                                    return _colthaotac;
                                };

                                //3. Checkbox chọn nhiều
                                var _checkBoxChonNhieu = function () {
                                    if (vm.form == 'dang_ky_ho_so') {
                                        colCheckBox = {
                                            field: "checkbox"
                                            , attributes: { class: "text-center" }
                                            , headerAttributes: { style: "text-align: center;" }
                                            , headerTemplate: `<label class="mt-checkbox mt-checkbox-outline" ng-show="vm.filter.formCase==1">
                                            <input name="chkH" type="checkbox" ng-change="vm.clickAllCheckbox()" ng-model="vm.checkboxAll"/><span>&nbsp;</span></label>`
                                            , template: `
                                            <div  ng-show="this.dataItem.isCA!=true" ng-if="vm.arrExist(this.dataItem.arrChucNang, 13)"><label class="mt-checkbox mt-checkbox-outline" >
                                            <input name="chk" type="checkbox"  ng-disabled="this.dataItem.trangThaiHoSo == null" ng-change="vm.clickCheckbox(this.dataItem)" ng-model="dataItem.checkbox"/><span>&nbsp;</span></label></div>
                                            <button class='btn btn-success btn-xs' ng-if="vm.arrExist(this.dataItem.arrChucNang, 15)" ng-click="vm.thanhToanHoSo(this.dataItem)"><em class="fa fa-sign-in"></em>Nộp</button>
                                            <button class='btn btn-success btn-xs' ng-if="vm.arrExist(this.dataItem.arrChucNang, 18)" ng-click="vm.nopHoSoBiTraLai(this.dataItem)"><em class="fa fa-sign-in"></em>Nộp</button>
                                            <button class='btn btn-success btn-xs' ng-if="vm.arrExist(this.dataItem.arrChucNang, 16)" ng-click="vm.nopHoSoBoSung(this.dataItem)"><em class="fa fa-sign-in"></em>Nộp</button>
                                            `
                                            , width: 75
                                            , hidden: vm.filter.formCase == 2 //locked: true
                                        };
                                        return colCheckBox;
                                    }
                                    else if (vm.form == 'mot_cua_phan_cong') {
                                        colCheckBox = {
                                            field: "checkbox"
                                            , attributes: { class: "text-center" }
                                            , headerAttributes: { style: "text-align: center;" }
                                            , headerTemplate: `<label class="mt-checkbox mt-checkbox-outline" ng-show="vm.filter.formCase==1">
                                            <input name="chkH" type="checkbox" ng-change="vm.clickAllCheckbox()" ng-model="vm.checkboxAll"/><span>&nbsp;</span></label>`
                                            , template: `<div class="mt-checkbox-inline" ng-show="vm.filter.formCase!=3"><label class="mt-checkbox mt-checkbox-outline" >
                                            <input name="chk" type="checkbox" ng-change="vm.clickCheckbox(this.dataItem)" ng-model="dataItem.checkbox"/><span>&nbsp;</span></label></div>`
                                            , width: 75 //locked: true
                                        };
                                        return colCheckBox;
                                    }
                                    else if (vm.form == 'phong_ban_phan_cong') {
                                        colCheckBox = {
                                            field: "checkbox"
                                            , attributes: { class: "text-center" }
                                            , headerAttributes: { style: "text-align: center;" }
                                            , headerTemplate: `<label class="mt-checkbox mt-checkbox-outline" ng-show="vm.filter.formCase!=3">
                                            <input name="chkH" type="checkbox" ng-change="vm.clickAllCheckbox()" ng-model="vm.checkboxAll"/><span>&nbsp;</span></label>`
                                            , template: `<div class="mt-checkbox-inline" ng-show="vm.filter.formCase!=3"><label class="mt-checkbox mt-checkbox-outline" >
                                            <input name="chk" type="checkbox" ng-change="vm.clickCheckbox(this.dataItem)" ng-model="dataItem.checkbox"/><span>&nbsp;</span></label></div>`
                                            , width: 75 //locked: true
                                        };
                                        return colCheckBox;
                                    }
                                    else if (vm.form == 'tham_xet_ho_so') {
                                        colCheckBox = {
                                            field: "checkbox"
                                            , attributes: { class: "text-center" }
                                            , headerAttributes: { style: "text-align: center;" }
                                            , headerTemplate: `<label class="mt-checkbox mt-checkbox-outline" ng-show="vm.filter.formCase!=3">
                                            <input name="chkH" type="checkbox" ng-change="vm.clickAllCheckbox()" ng-model="vm.checkboxAll"/><span>&nbsp;</span></label>`
                                            , template: `<div class="mt-checkbox-inline" ng-show="vm.filter.formCase!=3"><label class="mt-checkbox mt-checkbox-outline" >
                                            <input name="chk" type="checkbox" ng-change="vm.clickCheckbox(this.dataItem)" ng-model="dataItem.checkbox"/><span>&nbsp;</span></label></div>`
                                            , width: 75 //locked: true
                                        };
                                        return colCheckBox;
                                    }
                                    else if (vm.form == 'truong_phong_duyet') {
                                        colCheckBox = {
                                            field: "checkbox"
                                            , attributes: { class: "text-center" }
                                            , headerAttributes: { style: "text-align: center;" }
                                            , headerTemplate: `<label class="mt-checkbox mt-checkbox-outline" ng-show="vm.filter.formCase==1">
                                            <input name="chkH" type="checkbox" ng-change="vm.clickAllCheckbox()" ng-model="vm.checkboxAll"/><span>&nbsp;</span></label>`
                                            , template: `<div class="mt-checkbox-inline" ng-show="vm.filter.formCase==1 && this.dataItem.hoSoIsDat!=true"><label class="mt-checkbox mt-checkbox-outline">
                                            <input name="chk" type="checkbox" ng-change="vm.clickCheckbox(this.dataItem)" ng-model="dataItem.checkbox"/><span>&nbsp;</span></label></div>`
                                            , width: 75 //locked: true
                                        };
                                        return colCheckBox;
                                    }
                                    else if (vm.form == 'lanh_dao_cuc_duyet') {
                                        colCheckBox = {
                                            field: "checkbox"
                                            , attributes: { class: "text-center" }
                                            , headerAttributes: { style: "text-align: center;" }
                                            , headerTemplate: `<label class="mt-checkbox mt-checkbox-outline" ng-show="vm.filter.formCase==1 || vm.filter.formCase==2">
                                            <input name="chkH" type="checkbox" ng-change="vm.clickAllCheckbox()" ng-model="vm.checkboxAll"/><span>&nbsp;</span></label>`
                                            , template: `<div class="mt-checkbox-inline" ng-show="vm.filter.formCase==1 || vm.filter.formCase==2"><label class="mt-checkbox mt-checkbox-outline">
                                            <input name="chk" type="checkbox" ng-change="vm.clickCheckbox(dataItem)" ng-model="dataItem.checkbox"/><span>&nbsp;</span></label></div>`
                                            , width: 75 //locked: true
                                        };
                                        return colCheckBox;
                                    }
                                    else if (vm.form == 'van_thu_duyet') {
                                        colCheckBox = {
                                            field: "checkbox"
                                            , attributes: { class: "text-center" }
                                            , headerAttributes: { style: "text-align: center;" }
                                            , headerTemplate: `<label class="mt-checkbox mt-checkbox-outline" ng-show="vm.filter.formCase==1">
                                            <input name="chkH" type="checkbox" ng-change="vm.clickAllCheckbox()" ng-model="vm.checkboxAll"/><span>&nbsp;</span></label>`
                                            , template: `<div class="mt-checkbox-inline" ng-show="vm.filter.formCase==1"><label class="mt-checkbox mt-checkbox-outline" >
                                            <input name="chk" type="checkbox" ng-change="vm.clickCheckbox(this.dataItem)" ng-model="dataItem.checkbox"/><span>&nbsp;</span></label></div>`
                                            , width: 75
                                            , hidden: vm.filter.formCase != 1 //locked: true
                                        };
                                        return colCheckBox;
                                    }
                                    else if (vm.form == 'tham_dinh_ho_so') {
                                        colCheckBox = {
                                            field: "checkbox"
                                            , attributes: { class: "text-center" }
                                            , headerAttributes: { style: "text-align: center;" }
                                            , headerTemplate: `<label class="mt-checkbox mt-checkbox-outline" ng-show="vm.filter.formCase!=3">
                                            <input name="chkH" type="checkbox" ng-change="vm.clickAllCheckbox()" ng-model="vm.checkboxAll"/><span>&nbsp;</span></label>`
                                            , template: `<div class="mt-checkbox-inline" ng-show="vm.filter.formCase!=3"><label class="mt-checkbox mt-checkbox-outline" >
                                            <input name="chk" type="checkbox" ng-change="vm.clickCheckbox(this.dataItem)" ng-model="dataItem.checkbox"/><span>&nbsp;</span></label></div>`
                                            , width: 75 //locked: true
                                        };
                                        return colCheckBox;
                                    }
                                    else if (vm.form == 'tong_hop_tham_dinh') {
                                        colCheckBox = {
                                            field: "checkbox"
                                            , attributes: { class: "text-center" }
                                            , headerAttributes: { style: "text-align: center;" }
                                            , headerTemplate: `<label class="mt-checkbox mt-checkbox-outline" ng-show="vm.filter.formCase!=3">
                                            <input name="chkH" type="checkbox" ng-change="vm.clickAllCheckbox()" ng-model="vm.checkboxAll"/><span>&nbsp;</span></label>`
                                            , template: `<div class="mt-checkbox-inline" ng-show="vm.filter.formCase!=3"><label class="mt-checkbox mt-checkbox-outline" >
                                            <input name="chk" type="checkbox" ng-change="vm.clickCheckbox(this.dataItem)" ng-model="dataItem.checkbox"/><span>&nbsp;</span></label></div>`
                                            , width: 75 //locked: true
                                        };
                                        return colCheckBox;
                                    }
                                    else if (vm.form == 'truong_phong_duyet_tham_dinh') {
                                        colCheckBox = {
                                            field: "checkbox"
                                            , attributes: { class: "text-center" }
                                            , headerAttributes: { style: "text-align: center;" }
                                            , headerTemplate: `<label class="mt-checkbox mt-checkbox-outline" ng-show="vm.filter.formCase!=3">
                                            <input name="chkH" type="checkbox" ng-change="vm.clickAllCheckbox()" ng-model="vm.checkboxAll"/><span>&nbsp;</span></label>`
                                            , template: `<div class="mt-checkbox-inline" ng-show="vm.filter.formCase!=3"><label class="mt-checkbox mt-checkbox-outline" >
                                            <input name="chk" type="checkbox" ng-change="vm.clickCheckbox(this.dataItem)" ng-model="dataItem.checkbox"/><span>&nbsp;</span></label></div>`
                                            , width: 75 //locked: true
                                        };
                                        return colCheckBox;
                                    }
                                    else if (vm.form == 'lanh_dao_cuc_duyet_tham_dinh') {
                                        colCheckBox = {
                                            field: "checkbox"
                                            , attributes: { class: "text-center" }
                                            , headerAttributes: { style: "text-align: center;" }
                                            , headerTemplate: `<label class="mt-checkbox mt-checkbox-outline" ng-show="vm.filter.formCase!=3">
                                            <input name="chkH" type="checkbox" ng-change="vm.clickAllCheckbox()" ng-model="vm.checkboxAll"/><span>&nbsp;</span></label>`
                                            , template: `<div class="mt-checkbox-inline" ng-show="vm.filter.formCase!=3"><label class="mt-checkbox mt-checkbox-outline" >
                                            <input name="chk" type="checkbox" ng-change="vm.clickCheckbox(this.dataItem)" ng-model="dataItem.checkbox"/><span>&nbsp;</span></label></div>`
                                            , width: 75 //locked: true
                                        };
                                        return colCheckBox;
                                    }
                                    else if (vm.form == 'van_thu_duyet_tham_dinh') {
                                        colCheckBox = {
                                            field: "checkbox"
                                            , attributes: { class: "text-center" }
                                            , headerAttributes: { style: "text-align: center;" }
                                            , headerTemplate: `<label class="mt-checkbox mt-checkbox-outline" ng-show="vm.filter.formCase!=3">
                                            <input name="chkH" type="checkbox" ng-change="vm.clickAllCheckbox()" ng-model="vm.checkboxAll"/><span>&nbsp;</span></label>`
                                            , template: `<div class="mt-checkbox-inline" ng-show="vm.filter.formCase!=3"><label class="mt-checkbox mt-checkbox-outline" >
                                            <input name="chk" type="checkbox" ng-change="vm.clickCheckbox(this.dataItem)" ng-model="dataItem.checkbox"/><span>&nbsp;</span></label></div>`
                                            , width: 75 //locked: true
                                        };
                                        return colCheckBox;
                                    }
                                };

                                //4. Ký số
                                var _colKyDienTu = {
                                    field: ""
                                    , headerTemplate: `<div align='center'>Ký</div>`
                                    , template: `
                                  <div align='center' ng-if='vm.arrExist(this.dataItem.arrChucNang, 13)' title='Ký điện tử' >
                                    <a class='see-hoso' ng-click='vm.kySoHoSo(this.dataItem)'><em class='attp-ky'></em></a>
                                  </div>                              
                                  <div align='center' ng-if='this.dataItem.isCA==true' title='Click vào đây để xem bản ký' >
                                    <a ng-click="vm.xemHoSo(this.dataItem)" class="label bg-blue">Đã ký</a>
                                  </div>
                                  `
                                    , width: 70//, locked: true
                                    , hidden: vm.filter.formCase == 2
                                };

                                //5. Thông tin hồ sơ
                                var _thongtinHoSo = {
                                    field: "",
                                    title: "Thông tin hồ sơ",
                                    template:
                                        `<div>
                                        <strong>Họ và tên: <span style="font-size:16px;">{{ this.dataItem.hoTenNguoiDeNghi }}</span></strong><br/>
                                        <strong>Ngày sinh:</strong> {{ this.dataItem.ngaySinh | date:'dd/MM/yyyy'}}<br/>
                                        <strong>Địa chỉ:</strong> {{ this.dataItem.diaChiCuTru }}<br/>
                                        <strong>Mã hồ sơ:</strong> {{ this.dataItem.maHoSo }}
                                    </div>`,
                                    width:400
                                };

                                //6. Ngày nộp
                                var _colNgayNop = {
                                    field: "ngayNop",
                                    title: app.localize('Ngày nộp'),
                                    template: "{{this.dataItem.ngayTiepNhan | date:'dd/MM/yyyy'}}",
                                    attributes: { class: "text-center" },
                                    headerAttributes: { style: "text-align: center;" },
                                    width: 120
                                };

                                //7. Ngày hẹn trả
                                var _colNgayHenTra = {
                                    field: "ngayHenTra",
                                    title: app.localize('Ngày hẹn trả'),
                                    attributes: { class: "text-center" },
                                    headerAttributes: { style: "text-align: center;" },
                                    template: `{{this.dataItem.ngayHenTra | date:'dd/MM/yyyy'}}<br/>
                                    <div ng-if="this.dataItem.ngayHenTra && this.dataItem.trangThaiHoSo != 4 && this.dataItem.trangThaiHoSo != 6 && this.dataItem.trangThaiHoSo != 7 ">
                                        <div ng-show='this.dataItem.soNgayQuaHan<=0'><span class='label label-success'><em class='fa fa-clock-o'></em> {{this.dataItem.strQuaHan}}</span></div>
                                        <div ng-show='this.dataItem.soNgayQuaHan<=5 && this.dataItem.soNgayQuaHan>0'><span class='label bg-yellow-crusta'><em class='fa fa-clock-o'></em> {{this.dataItem.strQuaHan}}</span></div>
                                        <div ng-show='this.dataItem.soNgayQuaHan>5'><span class='label bg-red'><em class='fa fa-clock-o'></em> {{this.dataItem.strQuaHan}}</span></div>
                                    </div>
                                    <div ng-if="this.dataItem.trangThaiHoSo == 4 && this.dataItem.vanThuIsCA == true">
                                         <span class='label label-warning'><em class='fa fa-mail-reply'></em> {{this.dataItem.strQuaHan}}</span>
                                    </div>
                                    <div ng-if="this.dataItem.trangThaiHoSo == 6">
                                         <span class='label bg-green-jungle font-bg-green-jungle'><em class='fa fa-check'></em> {{this.dataItem.strQuaHan}}</span>
                                    </div>
                                    <div ng-if="this.dataItem.trangThaiHoSo == 7">
                                         <span class='label red'><em class='fa fa-ban'></em> {{this.dataItem.strQuaHan}}</span>
                                    </div>`,
                                    width: 160
                                };

                                //9. Đơn vị gửi
                                var _colDonViGui = {
                                    field: ""
                                    , headerTemplate: 'Bộ phận gửi'
                                    , template: `<div>
                                      <p style='margin:0'><strong>{{this.dataItem.strDonViGui}}</strong></p>
                                      <p style='margin:0; font-size:12px;'><em>{{dataItem.tenNguoiGui}}</em></p>
                                      <p style='margin:0; font-size:12px;'><em>{{dataItem.ngayGui | date:'dd/MM/yyyy HH:mm:ss'}}</em></p>
                                    </div>`
                                    , width: 150
                                };

                                //10. Đơn vị xử lý
                                var _colDonViXuLy = {
                                    field: ""
                                    , headerTemplate: 'Bộ phận xử lý'
                                    , template: `<div>
                                      <p style='margin:0'><strong>{{this.dataItem.strDonViXuLy}}</strong></p>
                                      <p style='margin:0; font-size:12px;'><em>{{dataItem.tenNguoiXuLy}}</em></p>
                                    </div>`
                                    , width: 160
                                };

                                var _colChuyenVienXuLy = {
                                    field: ""
                                    , headerTemplate: 'Chuyên viên xử lý'
                                    , template: `<div ng-show='this.dataItem.chuyenVienThuLyId!=null'>
                                                <b>Thẩm xét 1:</b> {{this.dataItem.chuyenVienThuLyName}}
                                                <em class='fa fa-check font-green-meadow' ng-show='this.dataItem.chuyenVienThuLyDaDuyet==true'></em>
                                                <br ng-show='this.dataItem.chuyenVienPhoiHopId!=null'/>
                                                <span ng-show='this.dataItem.chuyenVienPhoiHopId!=null'><b>Thẩm xét 2:</b> {{this.dataItem.chuyenVienPhoiHopName}}
                                                <em class='fa fa-check font-green-meadow' ng-show='this.dataItem.chuyenVienPhoiHopDaDuyet==true'></em></span></div>`
                                    , width: 250
                                };

                                //11. Kết quả hồ sơ
                                var _colKetQua = {
                                    field: ""
                                    , attributes: { class: "text-center" },
                                    headerAttributes: { style: "text-align: center;" }
                                    , headerTemplate: "Kết quả"
                                    , template: "<div class='label bg-green-jungle bg-font-green-jungle' ng-show='this.dataItem.hoSoIsDat == true'>Hồ sơ đạt</div>"
                                        + "<div class='label label-warning' ng-show='this.dataItem.trangThaiXuLy == 2'>Hồ sơ bổ sung</div>"
                                        + "<div class='label label-danger' ng-show='this.dataItem.trangThaiXuLy == 3'>Từ chối hồ sơ</div>"
                                    , width: 120
                                };

                                //12. Trạng thái hồ sơ
                                var _colTrangThai = {
                                    field: ""
                                    , headerTemplate: app.localize('Trạng thái hồ sơ')
                                    , template: "<div><text>{{this.dataItem.strTrangThai}}</text></div>"
                                    , attributes: { class: "text-center" }
                                    , headerAttributes: { style: "text-align: center;" }
                                    , width: vm.form == 'dang_ky_ho_so' ? 350 : 200
                                };

                                //14. Cột thông tin hồ sơ
                                var _colShortThongTinHoSo = {
                                    field: ""
                                    , headerTemplate: "Thông tin hồ sơ"
                                    , template: `<div> <strong>Mã HS: </strong><a ng-click="vm.mot_cua_phan_cong(this.dataItem)"><strong>{{this.dataItem.maHoSo}}</strong></a></div>
                                            <div> <strong>Doanh nghiệp: </strong>{{this.dataItem.tenNguoiDaiDien}} </div>`

                                };

                                var _colShortThongTinHoSoVanThuDuyet = {
                                    field: ""
                                    , headerTemplate: "Thông tin hồ sơ"
                                    , template: `<div> <strong>Mã HS: </strong><a ng-click="vm.arrExist(this.dataItem.arrChucNang, 7)?vm.van_thu_duyet(this.dataItem): vm.xemHoSoFull(this.dataItem)"><strong>{{this.dataItem.maHoSo}}</strong></a></div>
                                             <div> <strong>Doanh nghiệp: </strong>{{this.dataItem.tenNguoiDaiDien}} </div>`
                                };

                                // Set column cho Grid
                                var setColForForm = function () {
                                    switch (vm.form) {
                                        case 'dang_ky_ho_so':
                                            vm.columns = [_stt, _thaotac(),
                                                _thongtinHoSo, _colNgayNop, _colNgayHenTra, _colTrangThai, _colKetQua];
                                            break;
                                        case 'mot_cua_ra_soat':
                                            vm.columns = [_stt, _thaotac(), _thongtinHoSo, _colTrangThai, _colNgayNop, _colNgayHenTra, _colDonViGui, _colDonViXuLy];
                                            break;
                                        case 'mot_cua_phan_cong':
                                            vm.columns = [_stt, _thaotac(), _thongtinHoSo, _colNgayNop, _colNgayHenTra, _colDonViGui, _colDonViXuLy, _colTrangThai];
                                            break;
                                        case 'phong_ban_phan_cong':
                                            vm.columns = [_stt, _thaotac(), _checkBoxChonNhieu(),
                                                _thongtinHoSo, _colNgayNop, _colDonViGui, _colDonViXuLy, _colTrangThai];
                                            break;
                                        case 'tham_xet_ho_so':
                                            vm.columns = [_stt, _thaotac(),
                                                _thongtinHoSo, _colNgayNop, _colNgayHenTra, _colChuyenVienXuLy, _colDonViGui, _colDonViXuLy, _colTrangThai];
                                            break;
                                        case 'truong_phong_duyet':
                                            vm.columns = [_stt, _thaotac(),
                                                _thongtinHoSo, _colNgayNop, _colNgayHenTra, _colDonViGui, _colDonViXuLy, _colTrangThai, _colKetQua];
                                            break;
                                        case 'lanh_dao_cuc_duyet':
                                            vm.columns = [_stt, _thaotac(),
                                                _thongtinHoSo, _colNgayNop, _colNgayHenTra, _colDonViGui, _colDonViXuLy, _colTrangThai, _colKetQua];
                                            break;
                                        case 'van_thu_duyet':
                                            vm.columns = [_stt, _thaotac(),
                                                _thongtinHoSo, _colNgayNop, _colNgayHenTra, _colDonViGui, _colDonViXuLy, _colKetQua];
                                            break;
                                        case 'tham_dinh_ho_so':
                                            vm.columns = [_stt, _thaotac(),
                                                _thongtinHoSo, _colTrangThai,_colNgayNop, _colNgayHenTra, _colDonViGui];
                                            break;
                                        case 'tong_hop_tham_dinh':
                                            vm.columns = [_stt, _thaotac(),
                                                _thongtinHoSo, _colTrangThai,_colNgayNop, _colNgayHenTra, _colDonViGui, _colDonViXuLy, _colKetQua];
                                            break;
                                        case 'truong_phong_duyet_tham_dinh':
                                            vm.columns = [_stt, _thaotac(),
                                                _thongtinHoSo, _colNgayNop, _colNgayHenTra, _colDonViGui, _colDonViXuLy, _colKetQua];
                                            break;
                                        case 'lanh_dao_cuc_duyet_tham_dinh':
                                            vm.columns = [_stt, _thaotac(),
                                                _thongtinHoSo, _colNgayNop, _colNgayHenTra, _colDonViGui, _colDonViXuLy, _colKetQua];
                                            break;
                                        case 'van_thu_duyet_tham_dinh':
                                            vm.columns = [_stt, _thaotac(),
                                                _thongtinHoSo, _colNgayNop, _colNgayHenTra, _colDonViGui, _colDonViXuLy, _colKetQua];
                                            break;
                                    }
                                };
                                setColForForm();

                                //Thao tác
                                vm.action_loading = false;

                                vm.columnGridHoSo = vm.columns;
                            };
                            vm.refreshColumns();

                            vm.gridHoSoDataSource = new kendo.data.DataSource({
                                transport: {
                                    read: function (options) {
                                        vm.requestParams.skipCount = (options.data.page - 1) * options.data.pageSize;
                                        vm.requestParams.maxResultCount = options.data.pageSize;
                                        xuLyHoSoGridViewService.getListHoSoPaging($.extend(vm.filter, vm.requestParams))
                                            .then(function (result) {
                                                console.log(result.data, 'result');
                                                if (result.data && result.data.items) {
                                                    vm.gvHoSoCallBack = options;
                                                    var i = 1;
                                                    result.data.items.forEach(function (item) {
                                                        item.STT = i;
                                                        i++;
                                                    });
                                                    options.success(result.data);
                                                }

                                                //CallBack checkBoxAll
                                                vm.arrCheckbox = [];
                                                try {
                                                    if ($scope.loadcheckall()) {
                                                        $scope.loadcheckall()(vm.arrCheckbox);
                                                    }
                                                } catch (ex) {
                                                    console.log(ex);
                                                }

                                                vm.reloadTotal();

                                            }).finally(function () {
                                                abp.ui.clearBusy();
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

                            vm.refreshHoSo = function () {
                                abp.ui.setBusy();
                                vm.arrCheckbox = [];
                                vm.gridHoSoDataSource.transport.read(vm.gvHoSoCallBack);
                                vm.refreshColumns();
                                vm.checkboxAll = false;
                            };
                            vm.hoSoGridOptions = {
                                pageable:
                                {
                                    "refresh": true,
                                    messages: {
                                        empty: "Không có dữ liệu",
                                        display: "Tổng {2} hồ sơ",
                                        itemsPerPage: "Hồ sơ mỗi trang"
                                    },
                                    pageSizes: [5, 10, 20, 50, 100]
                                },
                                resizable: true,
                                scrollable: true,
                                dataBound: function (e) {
                                    if (vm.form == 'dang_ky_ho_so') {
                                        
                                    }
                                    else if (vm.form == 'van_thu_duyet') {
                                        
                                    }
                                    else if (vm.form == 'truong_phong_duyet') {
                                        this.hideColumn(7);
                                    }
                                },
                                columns: vm.columnGridHoSo
                            };

                            $scope.$on('refreshGridHoSo', function (event, filter) {
                                vm.refreshHoSo();
                            });
                        }
                    }

                    //--- FormCase ---//
                    {
                        // không dùng nữa
                        vm.listFormCase = [];
                        vm.listFormCase2 = [];
                        vm.initFormCase = function () {
                            if (vm.filter.formId) {
                                xuLyHoSoGridViewService.getListFormCase(vm.filter.formId)
                                    .then(function (result) {
                                        $rootScope.listFormCase = result.data;
                                        if (result.data) {
                                            vm.listFormCase = result.data.formCase;
                                            vm.listFormCase2 = result.data.formCase2;
                                        }
                                    }).finally(function () {
                                        vm.loading = false;
                                    });
                            }
                        };

                        vm.formCaseOnChange = function (formCase) {
                            abp.ui.setBusy();
                            vm.filter.formCase = angular.copy(formCase);
                            vm.refreshHoSo();
                        };

                        // get form case end Tính số hồ sơ theo FormCase
                        {
                            //Cách mới
                            vm.reloadTotal = function () {
                                _req = angular.copy(vm.filter);
                                _req.skipCount = 0;
                                _req.maxResultCount = 1;
                                xuLyHoSoGridViewService.getListFormCaseCountNumber(_req)
                                    .then(function (result) {
                                        if (result) {
                                            console.log(result.data, 'listFormCase');
                                            if (vm.listFormCase.length < 1) {
                                                vm.listFormCase = result.data;
                                                if (vm.filter.formId == app.FORM_ID.FORM_DANG_KY_HO_SO) {
                                                    vm.listFormCase = vm.listFormCase.filter(x => x.id != 9 && x.id != 3 && x.id != 4);
                                                    vm.listFormCase[4].name = "Hồ sơ đang xử lý";
                                                }
                                                else if (vm.filter.formId == app.FORM_ID.FORM_MOT_CUA_RA_SOAT) {
                                                    vm.listFormCase = vm.listFormCase.filter(x => x.id != 2 && x.id != 4);
                                                    vm.listFormCase[3].name = 'Hồ sơ đã gửi phân công';
                                                }
                                                else if (vm.filter.formId == app.FORM_ID.FORM_THAM_XET_HO_SO) {
                                                    vm.listFormCase = vm.listFormCase.filter(x => x.id == 0 || x.id == 1 || x.id == 4 || x.id == 5);
                                                }
                                                else if (vm.filter.formId == app.FORM_ID.FORM_TRUONG_PHONG_DUYET) {
                                                    vm.listFormCase = vm.listFormCase.filter(x => x.id != 3);
                                                }
                                            }
                                            else {
                                                result.data.forEach(function (item) {
                                                    vm.listFormCase.forEach(function (fc) {
                                                        if (fc.id == item.id) {
                                                            fc.totalCount = item.totalCount;
                                                        }
                                                    });
                                                });
                                            }
                                        }
                                    });
                            };
                        }
                    }

                    //--- CheckBox All ---//
                    {
                        vm.checkboxAll = false;
                        vm.arrCheckbox = [];
                        vm.clickCheckbox = function (ite) {
                            if (ite.checkbox) {
                                vm.arrCheckbox.push(ite);
                            }
                            else {
                                var idx = vm.arrCheckbox.indexOf(ite);
                                if (idx >= 0)
                                    vm.arrCheckbox.splice(idx, 1);
                            }

                            try {
                                $scope.loadcheckall()(vm.arrCheckbox);
                            } catch (ex) {
                                console.log(ex, 'ex');
                            }
                        };
                        vm.clickAllCheckbox = function () {
                            vm.arrCheckbox = [];
                            if (vm.checkboxAll) {
                                if (vm.form == 'dang_ky_ho_so') {
                                    angular.forEach(vm.gridHoSoDataSource.data(), function (dat) {
                                        if (dat.trangThaiHoSo != null && !dat.isCA) {
                                            vm.arrCheckbox.push(dat);
                                            dat.checkbox = true;
                                        }
                                    });
                                }
                                else {
                                    angular.forEach(vm.gridHoSoDataSource.data(), function (dat) {
                                        vm.arrCheckbox.push(dat);
                                        dat.checkbox = true;
                                    });
                                }
                            }
                            else {
                                angular.forEach(vm.gridHoSoDataSource.data(), function (dat) {
                                    dat.checkbox = false;
                                });
                            }
                            try {
                                $scope.loadcheckall()(vm.arrCheckbox);
                            } catch (ex) {
                                console.log(ex, 'ex');
                            }
                        };
                    }

                    //--- Functions Common ---//
                    {

                        vm.xemHoSoFull = function (dataItem) {
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
                        };

                        vm.nopHoSoDeRaSoat = function (dataItem) {
                            abp.message.confirm(app.localize('Thông tin hồ sơ "' + dataItem.maHoSo + '" sẽ được gửi !!!'),
                                app.localize('Bạn chắc chắn muốn gửi hồ sơ này?'),
                                function (isConfirmed) {
                                    if (isConfirmed) {
                                        appChuKySo.insertPdfHoSo(dataItem, function (param) {
                                            abp.ui.setBusy();
                                            let input = {
                                                HoSoId: param.hoSoId,
                                                DuongDanTep: param.duongDanTep
                                            }
                                            xuLyHoSoDoanhNghiepService.nopHoSoDeRaSoat(input)
                                                .then(function (result) {
                                                    if (result) {
                                                        abp.notify.success('Gửi hồ sơ thành công');
                                                    }
                                                }).finally(function () {
                                                    abp.ui.clearBusy();
                                                    vm.refreshHoSo();
                                                });
                                        })
                                    }
                                }
                            );
                        };

                        vm.xemCongVan = function (dataItem) { // công văn yêu cầu bổ sung
                            appChuKySo.xemFilePDF(dataItem.hsxlDuongDanTepCA, 'Xem Công Văn');
                        };
                        
                        vm.xemLyDoTraLai = function (dataItem) {
                            if (dataItem && dataItem.id > 0) {
                                var modalData = {
                                    lyDo: dataItem.lyDoTraLai
                                };
                                var modalInstance = $uibModal.open({
                                    templateUrl: '~/App/quanlyhoso/thutuc37/directives/modal/viewLyDoTraLaiModal.cshtml',
                                    controller: 'quanlyhoso.thutuc37.directives.modal.viewLyDoTraLaiModel as vm',
                                    backdrop: 'static',
                                    size: 'md',
                                    resolve: {
                                        modalData: modalData
                                    }
                                });
                            }
                        }

                        vm.xemDoanThamDinh = function (dataItem) {
                            var modalInstance = $uibModal.open({
                                templateUrl: '~/App/quanlyhoso/thutuc37/directives/modal/viewDoanThamDinhModel.cshtml',
                                controller: 'quanlyhoso.thutuc37.directives.modal.viewdoanthamdinh as vm',
                                backdrop: 'static',
                                size: 'lg',
                                resolve: {
                                    modalData: dataItem
                                }
                            });
                        }
                    }

                    //--- XuLyHoSoDoanhNghiep ---//
                    {
                        vm.kySoHoSo = function (dataItem) {

                            if (dataItem && dataItem.id > 0) {
                                appChuKySo.kySoHoSo(dataItem, function (paramKySo) {
                                    vm.saving = true;
                                    xuLyHoSoDoanhNghiepService.updateKySoHoSo(paramKySo).then(function (result) {
                                        abp.notify.info(app.localize('SavedSuccessfully'));
                                        vm.refreshHoSo();

                                        appChuKySo.xemFilePDF(paramKySo.duongDanTep, 'Hồ sơ đã ký số');

                                    }).finally(function () {
                                        vm.saving = false;
                                    });
                                });
                            }
                        };

                        vm.kySoNhieuHoSo = function () {
                            if (vm.arrCheckbox && vm.arrCheckbox.length > 0) {

                                abp.message.confirm(app.localize(""),
                                    app.localize('Bạn chắc chắn muốn ký nhiều hồ sơ?'),
                                    function (isConfirmed) {
                                        if (isConfirmed) {
                                            var _count = 0;
                                            vm.arrCheckbox.forEach(function (item) {
                                                _count++;
                                                appChuKySo.kySoHoSo(item, function (paramKySo) {

                                                    vm.saving = true;
                                                    xuLyHoSoDoanhNghiepService.updateKySoHoSo(paramKySo).then(function (result) {
                                                        abp.notify.info(app.localize('SavedSuccessfully'));

                                                        if (_count == vm.arrCheckbox.length) {
                                                            vm.checkboxAll = false;
                                                            vm.refreshHoSo();
                                                        }

                                                    }).finally(function () {
                                                        vm.saving = false;
                                                    });
                                                });
                                            });
                                        }
                                    });
                            }
                        };

                        vm.huyHoSo = function (dataItem) {
                            abp.message.confirm(app.localize('Thông tin hồ sơ "' + dataItem.maHoSo + '" sẽ bị xóa!!!'),
                                app.localize('Bạn chắc chắn muốn hủy hồ sơ này?'),
                                function (isConfirmed) {
                                    if (isConfirmed) {
                                        abp.ui.setBusy();
                                        xuLyHoSoDoanhNghiepService.openHuyHoSo(dataItem.id).then(function (result) {
                                            if (result) {
                                                abp.notify.success('Lưu thành công');
                                                vm.refreshHoSo();
                                            }
                                        }).finally(function () {
                                            abp.ui.clearBusy();
                                        });
                                    }
                                }
                            );
                        };

                        vm.nopHoSoBoSung = function (dataItem) {
                            abp.message.confirm(app.localize("Bạn có muốn nộp hồ sơ bổ sung ngay không?"),
                                app.localize('Nộp hồ sơ bổ sung'),
                                function (isConfirmed) {
                                    if (isConfirmed) {
                                        abp.ui.setBusy();
                                        xuLyHoSoDoanhNghiepService.nopHoSoBoSung(dataItem.id).then(function (result) {
                                            if (result) {
                                                abp.notify.success('Nộp thành công');
                                                vm.refreshHoSo();
                                            }
                                        }).finally(function () {
                                            abp.ui.clearBusy();
                                        });
                                    }
                                }
                            );
                        };

                        vm.taoBanSaoHoSo = function (dataItem) {
                            abp.message.confirm(app.localize('Thông tin hồ sơ "' + dataItem.maHoSo + '" sẽ được nhân bản!!!'),
                                app.localize('Bạn chắc chắn muốn tạo bản sao của hồ sơ?'),
                                function (isConfirmed) {
                                    if (isConfirmed) {
                                        xuLyHoSoDoanhNghiepService.taoBanSaoHoSo(dataItem.id).then(function (result) {
                                            abp.notify.success('Tạo bản sao thành công');
                                            vm.refreshHoSo();
                                        }).finally(function () {
                                        });
                                    }
                                }
                            );
                        };

                    }

                    //--- Functions CallBack ---//
                    {
                        //=== Common
                        {
                            //Form doanh nghiệp
                            vm.sua_ho_so = function (dataItem) {
                                $scope.suahoso()(dataItem);
                            };
                            vm.nopHoSoBiTraLai = function (dataItem) {
                                $scope.nophosobitralai()(dataItem);
                            }

                            //Form một cửa rà soát hồ sơ
                            vm.mot_cua_ra_soat = function (dataItem) {
                                $scope.motcuarasoat()(dataItem);
                            };

                            vm.mot_cua_gui_phan_cong = function (dataItem) {
                                $scope.motcuaguiphancong()(dataItem);
                            }

                            //Form lanh dao cuc phân công hồ sơ
                            vm.mot_cua_phan_cong = function (dataItem) {
                                $scope.motcuaphancong()(dataItem);
                            };

                            // Form phòng ban phân công
                            vm.phong_ban_phan_cong = function (dataItem) {
                                $scope.phongbanphancong()(dataItem);
                            }
                            vm.phong_ban_tra_lai = function (dataItem) {
                                $scope.phongbantralai()(dataItem);
                            }

                            //Form thẩm xet hồ sơ
                            vm.tham_xet_ho_so = function (dataItem) {
                                $scope.thamxethoso()(dataItem);
                            };
                            vm.chuyen_vien_tra_lai_ho_so = function (dataItem) {
                                $scope.chuyenvientralaihoso()(dataItem);
                            }

                            //Form trưởng phòng duyệt
                            vm.truong_phong_duyet = function (dataItem) {
                                $scope.truongphongduyet()(dataItem);
                            };

                            //Form lãnh đạo cục duyệt
                            vm.lanh_dao_cuc_duyet = function (dataItem) {
                                $scope.lanhdaocucduyet()(dataItem);
                            };

                            //Form Văn thư đóng dấu          
                            vm.van_thu_duyet = function (dataItem) {
                                $scope.vanthuduyet()(dataItem);
                            };

                            // tham dinh ho so
                            vm.tham_dinh_ho_so = function (dataItem) {
                                $scope.thamdinhhoso()(dataItem);
                            }
                            // tong hop tham dinh
                            vm.tong_hop_tham_dinh = function (dataItem) {
                                $scope.tonghopthamdinh()(dataItem);
                            }
                            vm.cap_nhat_ket_qua = function (dataItem) {
                                $scope.capnhatketqua()(dataItem);
                            }
                            // tong hop tham dinh
                            vm.truong_phong_duyet_tham_dinh = function (dataItem) {
                                $scope.truongphongduyetthamdinh()(dataItem);
                            }
                            // tong hop tham dinh
                            vm.lanh_dao_cuc_duyet_tham_dinh = function (dataItem) {
                                $scope.lanhdaocucduyetthamdinh()(dataItem);
                            }
                            // tong hop tham dinh
                            vm.van_thu_duyet_tham_dinh = function (dataItem) {
                                $scope.vanthuduyetthamdinh()(dataItem);
                            }
                        }
                    }

                    //--- Util ---//
                    vm.arrExist = function (arrId, id) {
                        if (arrId && arrId.length) {
                            for (var i = 0; i < arrId.length; i++) {
                                if (arrId[i] == id) {
                                    return true;
                                }
                            }
                        }
                        return false;
                    };

                    //-- Control ---//
                    vm.initControl = function () {
                        $scope.internalControl = $scope.control || {};
                        vm.changeTabThamDinhHoSo = function (d) {
                            abp.ui.setBusy();
                            vm.filter.formCase = '1';
                            vm.filter.formCase2 = d;
                            vm.refreshHoSo();
                        };
                    };

                    //-- Start Init() ---//
                    function init() {
                        vm.initControl();
                    }
                    init();
                }];

            return {
                restrict: 'EA',
                scope: {
                    form: '=',
                    filterobj: '=',
                    loadcheckall: '&',

                    //'dang_ky_ho_so'          
                    suahoso: '&',
                    nophosobitralai:'&',

                    //mot_cua_ra_soat
                    motcuarasoat: '&',
                    motcuaguiphancong:'&',

                    // phan cong ho so
                    motcuaphancong: '&',
                    phongbanphancong: '&',
                    phongbantralai:'&',

                    // thẩm xét hồ sơ
                    thamxethoso: '&',
                    chuyenvientralaihoso: '&',

                    //'truong_phong_duyet'
                    truongphongduyet: '&',

                    //'lanh_dao_cuc_duyet'
                    lanhdaocucduyet: '&',

                    //'van_thu_duyet'
                    vanthuduyet: '&',

                    // tham dinh ho so
                    thamdinhhoso: '&',
                    // tong hop tham dinh
                    tonghopthamdinh: '&',
                    capnhatketqua: '&',

                    // truong phong duyett tham dinh
                    truongphongduyetthamdinh: '&',

                    // lanh dao cuc duyet tham dinh
                    lanhdaocucduyetthamdinh: '&',

                    // van thu duyet tham dinh
                    vanthuduyetthamdinh: '&',


                    // control goi grid view
                    control: '='
                },
                controller: controller,
                controllerAs: 'vm',
                bindToController: false,
                link: function (scope, elem, attr, ctrl) {
                    $templateRequest("/App/quanlyhoso/thutuc37/directives/gridHoSo.cshtml").then(function (html) {
                        var template = angular.element(html);
                        elem.append(template);
                        $compile(template)(scope);
                    });
                }
            };
        }
    ]);
})();