(function () {
    appModule.directive('quanlyhoso.thutuc99.directives.gridhoso', ['$compile', '$templateRequest',
        'abp.services.app.xuLyHoSoGridView99', 'abp.services.app.xuLyHoSoDoanhNghiep99', 'abp.services.app.xuLyHoSoTruongPhong99', 'abp.services.app.xuLyHoSoLanhDaoCuc99', 'abp.services.app.xuLyHoSoVanThu99',
        'appSession', 'quanlyhoso.thutuc99.services.appChuKySo',
        function ($compile, $templateRequest, xuLyHoSoGridViewService, xuLyHoSoDoanhNghiepService, xuLyHoSoTruongPhongService, xuLyHoSoLanhDaoCucService, xuLyHoSoVanThuService, appSession, appChuKySo) {
            var controller = ['$rootScope', '$scope', '$uibModal',
                function ($rootScope, $scope, $uibModal) {
                    var vm = this;
                    vm.loading = false;
                    vm.roleLevel = appSession.user.roleLevel;
                    vm.user = appSession.user;
                    vm.DON_VI_XU_LY = app.DON_VI_XU_LY;

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
                                                    <button class="btn btn-xs green-soft" uib-dropdown-toggle="" aria-haspopup="true" aria-expanded="false"><em class="fa fa-cog"></em>Thao Tác<span class="caret"></span></button>
                                                    <ul uib-dropdown-menu busy-if= "vm.action_loading">
                                                        <li><a ng-click="vm.xemHoSoFull(this.dataItem)" ng-if="vm.arrExist(this.dataItem.arrChucNang, 9)"><em class="fa fa-eye"></em> Xem hồ sơ</a></li>
                                                        <li><a ng-click="vm.xemHoSo(this.dataItem)"  ng-if="vm.arrExist(this.dataItem.arrChucNang, 91)"><em class="fa fa-eye"></em> Xem bản đăng ký</a></li>
                                                        <li><a ng-click="vm.sua_ho_so(this.dataItem)" ng-if="vm.arrExist(this.dataItem.arrChucNang, 11)"><em class="fa fa-edit"></em> Sửa hồ sơ</a></li>
                                                        <li><a ng-click="vm.huyHoSo(this.dataItem)" ng-if="vm.arrExist(this.dataItem.arrChucNang, 12)"><em class="fa fa-ban"></em> Hủy hồ sơ</a></li>
                                                        <li>
                                                            <a ng-show="vm.form==\'dang_ky_ho_so\'" ng-click="vm.kySoHoSo(this.dataItem)" ng-if="vm.arrExist(this.dataItem.arrChucNang, 13)">
                                                                <text ng-if="this.dataItem.onIsCA!=true"><em class="fa fa-pencil"></em> Ký điện tử</text>
                                                                <text ng-if="this.dataItem.onIsCA==true"><em class="fa fa-pencil"></em> Ký điện tử lại</text>
                                                            </a>
                                                        </li>
                                                        <li><a ng-click="vm.nopHoSoMoi(this.dataItem)"  ng-if="vm.arrExist(this.dataItem.arrChucNang, 14)"><em class="fa fa-support"></em> Nộp để rà soát</a></li>
                                                        <li><a ng-click="vm.thanhToanHoSo(this.dataItem)"  ng-if="vm.arrExist(this.dataItem.arrChucNang, 15)"><em class="fa fa-dollar"></em> Thanh toán hồ sơ</a></li>
                                                        <li><a ng-click="vm.xemThanhToanHoSo(this.dataItem)" ng-if="vm.arrExist(this.dataItem.arrChucNang, 94)"><em class="fa fa-reorder"></em> Xem Thanh toán</a></li>
                                                        <li><a ng-click="vm.nopHoSoBiTraLai(this.dataItem)" ng-if="vm.arrExist(this.dataItem.arrChucNang, 18)"><em class="fa fa-support"></em> Nộp hồ sơ bị trả lại</a></li>
                                                        <li><a ng-click="vm.nopHoSoBoSung(this.dataItem)" ng-if="vm.arrExist(this.dataItem.arrChucNang, 16)"><em class="fa fa-send"></em> Nộp hồ sơ bổ sung</a></li>
                                                        <li><a ng-click="vm.taoBanSaoHoSo(this.dataItem)" ng-if="vm.arrExist(this.dataItem.arrChucNang, 17)"><em class="fa fa-copy"></em> Tạo bản sao hồ sơ</a></li>
                                                    </ul>
                                                </div>
                                            </div>`;
                                            break;
                                        case 'mot_cua_ra_soat':
                                            _colthaotac.template =
                                                `<div class="ui-grid-cell-contents">
                                                <div class="btn-group dropdown" uib-dropdown="" dropdown-append-to-body>
                                                    <button class="btn btn-xs green-meadow" uib-dropdown-toggle="" aria-haspopup="true" aria-expanded="false"><em class="fa fa-cog"></em> Thao Tác <span class="caret"></span></button>
                                                    <ul uib-dropdown-menu busy-if= "vm.action_loading">
                                                        <li><a ng-click="vm.xemHoSoFull(this.dataItem)" ng-if="vm.arrExist(this.dataItem.arrChucNang, 9)"><em class="fa fa-eye"></em> Xem hồ sơ</a></li>
                                                        <li><a ng-click="vm.mot_cua_ra_soat(this.dataItem)" ng-if="vm.arrExist(this.dataItem.arrChucNang, 21)"><em class="fa fa-search-plus"></em> Rà soát hồ sơ</a></li>
                                                    </ul>
                                                </div>
                                            </div>`;
                                            break;
                                        case 'mot_cua_phan_cong':
                                            _colthaotac.template =
                                                `<div class="ui-grid-cell-contents  text-center">
                                                <div class="btn-group dropdown" uib-dropdown="" dropdown-append-to-body>
                                                    <button class="btn btn-xs green-soft" uib-dropdown-toggle="" aria-haspopup="true" aria-expanded="false"><em class="fa fa-cog"></em>Thao Tác<span class="caret"></span></button>
                                                    <ul uib-dropdown-menu busy-if= "vm.action_loading">
                                                        <li><a ng-click="vm.xemHoSoFull(this.dataItem)" ng-if="vm.arrExist(this.dataItem.arrChucNang, 9)"><em class="fa fa-eye"></em> Xem hồ sơ</a></li>
                                                        <li><a ng-click="vm.xemHoSo(this.dataItem)"  ng-if="vm.arrExist(this.dataItem.arrChucNang, 91)"><em class="fa fa-eye"></em> Xem đơn hàng</a></li>
                                                        <li><a ng-click="vm.mot_cua_phan_cong(this.dataItem)" ng-if="vm.arrExist(this.dataItem.arrChucNang, 21)"><text ng-if="!this.dataItem.phongBanId"><em class="fa fa-share"></em> Chuyển hồ sơ</text> <text ng-if="this.dataItem.phongBanId">Chuyển lại hồ sơ</text></a></li>
                                                    </ul>
                                                </div>
                                            </div>`;
                                            break;
                                        case 'phong_ban_phan_cong':
                                            _colthaotac.template =
                                                `<div class="ui-grid-cell-contents  text-center">
                                                <div class="btn-group dropdown" uib-dropdown="" dropdown-append-to-body>
                                                    <button class="btn btn-xs green-soft" uib-dropdown-toggle="" aria-haspopup="true" aria-expanded="false"><em class="fa fa-cog"></em>Thao Tác<span class="caret"></span></button>
                                                    <ul uib-dropdown-menu busy-if= "vm.action_loading">
                                                        <li><a ng-click="vm.xemHoSoFull(this.dataItem)" ng-if="vm.arrExist(this.dataItem.arrChucNang, 9)"><em class="fa fa-eye"></em> Xem hồ sơ</a></li>
                                                        <li><a ng-click="vm.xemHoSo(this.dataItem)"  ng-if="vm.arrExist(this.dataItem.arrChucNang, 91)"><em class="fa fa-eye"></em> Xem đơn hàng</a></li>
                                                        <li><a  ng-click="vm.phong_ban_phan_cong(this.dataItem)"         ng-if="vm.arrExist(this.dataItem.arrChucNang, 25)"><em class="fa fa-users"></em> Phân công cán bộ</a></li>
                                                        <li><a  ng-click="vm.phong_ban_phan_cong(this.dataItem)"  ng-if="vm.arrExist(this.dataItem.arrChucNang, 26)"><em class="fa fa-users"></em> Phân công lại</a></li>
                                                        <li><a  ng-click="vm.phong_ban_phan_cong(this.dataItem)" ng-if="vm.arrExist(this.dataItem.arrChucNang, 27)"><em class="fa fa-users"></em> Phân công lại</a></li> 
                                                    </ul>
                                                </div>
                                            </div>`;
                                            break;
                                        case 'tham_dinh_ho_so':
                                            _colthaotac.template =
                                                `<div class="ui-grid-cell-contents  text-center">
                                                <div class="btn-group dropdown" uib-dropdown="" dropdown-append-to-body>
                                                    <button class="btn btn-xs green-soft" uib-dropdown-toggle="" aria-haspopup="true" aria-expanded="false"><em class="fa fa-cog"></em>Thao Tác<span class="caret"></span></button>
                                                    <ul uib-dropdown-menu busy-if= "vm.action_loading">
                                                        <li><a ng-click="vm.xemHoSoFull(this.dataItem)" ng-if="vm.arrExist(this.dataItem.arrChucNang, 9)"><em class="fa fa-eye"></em> Xem hồ sơ</a></li>
                                                        <li><a ng-click="vm.xemHoSo(this.dataItem)"  ng-if="vm.arrExist(this.dataItem.arrChucNang, 91)"><em class="fa fa-eye"></em> Xem đơn hàng</a></li>
                                                        <li><a ng-click="vm.tham_dinh_ho_so(this.dataItem)" ng-if="vm.arrExist(this.dataItem.arrChucNang, 3)"><em class="fa fa-check-square-o"></em> Thẩm định hồ sơ</a></li>
                                                        <li><a ng-click="vm.tham_dinh_ho_so_bo_sung(this.dataItem)"      ng-if="vm.arrExist(this.dataItem.arrChucNang, 31)"><em class="fa fa-check-square-o"></em> Thẩm định bổ sung</a></li>
                                                        <li><a ng-click="vm.tham_dinh_lai(this.dataItem)"  ng-if="vm.arrExist(this.dataItem.arrChucNang, 32)"><em class="fa fa-check-square-o"></em> Thẩm định lại</a></li>
                                                        <li><a ng-click="vm.tong_hop_tham_dinh(this.dataItem)" ng-if="vm.arrExist(this.dataItem.arrChucNang, 34)"><em class="fa fa-check-square-o"></em> Tổng hợp thẩm định</a></li>
                                                        <li><a ng-click="vm.xemBienBanThamDinh(this.dataItem)"  ng-if="vm.arrExist(this.dataItem.arrChucNang, 95)"><em class="fa fa-folder-open"></em> Xem biên bản thẩm định</a></li>
                                                    </ul>
                                                </div>
                                             </div>`;
                                            break;
                                        case 'pho_phong_duyet':
                                            _colthaotac.template =
                                                `<div class="ui-grid-cell-contents  text-center">
                                                <div class="btn-group dropdown" uib-dropdown="" dropdown-append-to-body>
                                                    <button class="btn btn-xs green-soft" uib-dropdown-toggle="" aria-haspopup="true" aria-expanded="false"><em class="fa fa-cog"></em>Thao Tác<span class="caret"></span></button>
                                                    <ul uib-dropdown-menu busy-if= "vm.action_loading">
                                                        <li><a ng-click="vm.xemHoSoFull(this.dataItem)" ng-if="vm.arrExist(this.dataItem.arrChucNang, 9)"><em class="fa fa-eye"></em> Xem hồ sơ</a></li>
                                                        <li><a ng-click="vm.xemHoSo(this.dataItem)"  ng-if="vm.arrExist(this.dataItem.arrChucNang, 91)"><em class="fa fa-eye"></em> Xem đơn hàng</a></li>
                                                        <li><a ng-click="vm.pho_phong_duyet(this.dataItem)" ng-if="vm.arrExist(this.dataItem.arrChucNang, 8)"><em class="fa fa-star"></em> Duyệt hồ sơ</a></li>
                                                        <li><a ng-click="vm.xemBienBanThamDinh(this.dataItem)"  ng-if="vm.arrExist(this.dataItem.arrChucNang, 95)"><em class="fa fa-folder-open"></em> Xem biên bản thẩm định</a></li>
                                                    </ul>
                                                </div>
                                            </div>`;
                                            break;
                                        case 'truong_phong_duyet':
                                            _colthaotac.template =
                                                `<div class="ui-grid-cell-contents  text-center">
                                                <div class="btn-group dropdown" uib-dropdown="" dropdown-append-to-body>
                                                    <button class="btn btn-xs green-soft" uib-dropdown-toggle="" aria-haspopup="true" aria-expanded="false"><em class="fa fa-cog"></em>Thao Tác<span class="caret"></span></button>
                                                    <ul uib-dropdown-menu busy-if= "vm.action_loading">
                                                        <li><a ng-click="vm.xemHoSoFull(this.dataItem)" ng-if="vm.arrExist(this.dataItem.arrChucNang, 9)"><em class="fa fa-eye"></em> Xem hồ sơ</a></li>
                                                        <li><a ng-click="vm.xemHoSo(this.dataItem)"  ng-if="vm.arrExist(this.dataItem.arrChucNang, 91)"><em class="fa fa-eye"></em> Xem đơn hàng</a></li>
                                                        <li><a ng-click="vm.truong_phong_duyet(this.dataItem)"      ng-if="vm.arrExist(this.dataItem.arrChucNang, 4)"><em class="fa fa-star"></em> Duyệt hồ sơ</a></li>
                                                        <li><a ng-click="vm.xemBienBanThamDinh(this.dataItem)"  ng-if="vm.arrExist(this.dataItem.arrChucNang, 95)"><em class="fa fa-folder-open"></em> Xem biên bản thẩm định</a></li>
                                                    </ul>
                                                </div>
                                            </div>`;
                                            break;
                                        case 'lanh_dao_cuc_duyet':
                                            _colthaotac.template =
                                                `<div class="ui-grid-cell-contents  text-center">
                                                <div class="btn-group dropdown" uib-dropdown="" dropdown-append-to-body>
                                                <button class="btn btn-xs green-soft" uib-dropdown-toggle="" aria-haspopup="true" aria-expanded="false"><em class="fa fa-cog"></em>Thao Tác<span class="caret"></span></button>
                                                <ul uib-dropdown-menu busy-if= "vm.action_loading">
                                                    <li><a ng-click="vm.xemHoSoFull(this.dataItem)" ng-if="vm.arrExist(this.dataItem.arrChucNang, 9)"><em class="fa fa-eye"></em> Xem hồ sơ</a></li>
                                                    <li><a ng-click="vm.xemHoSo(this.dataItem)"  ng-if="vm.arrExist(this.dataItem.arrChucNang, 91)"><em class="fa fa-eye"></em> Xem đơn hàng</a></li>
                                                    <li><a ng-click="vm.lanh_dao_cuc_duyet(this.dataItem)"   ng-if="vm.arrExist(this.dataItem.arrChucNang, 5)"><em class="fa fa-check-circle-o"></em> Duyệt hồ sơ</a></li>
                                                    <li><a ng-click="vm.lanhDaoCucKySoNhanh(this.dataItem)"   ng-if="vm.arrExist(this.dataItem.arrChucNang, 51)"><em class="fa fa-flash"></em> Ký số & Chuyển nhanh</a></li>
                                                    <li><a ng-click="vm.xemBienBanThamDinh(this.dataItem)"  ng-if="vm.arrExist(this.dataItem.arrChucNang, 95)"><em class="fa fa-folder-open"></em> Xem biên bản thẩm định</a></li>
                                                </ul>
                                                </div>
                                            </div>`;
                                            break;
                                        case 'lanh_dao_bo_duyet':
                                            _colthaotac.template =
                                                `<div class="ui-grid-cell-contents  text-center">
                                                <div class="btn-group dropdown" uib-dropdown="" dropdown-append-to-body>
                                                <button class="btn btn-xs green-soft" uib-dropdown-toggle="" aria-haspopup="true" aria-expanded="false"><em class="fa fa-cog"></em>Thao Tác<span class="caret"></span></button>
                                                <ul uib-dropdown-menu busy-if= "vm.action_loading">
                                                    <li><a ng-click="vm.xemHoSoFull(this.dataItem)" ng-if="vm.arrExist(this.dataItem.arrChucNang, 9)"><em class="fa fa-eye"></em> Xem hồ sơ</a></li>
                                                    <li><a ng-click="vm.xemHoSo(this.dataItem)"  ng-if="vm.arrExist(this.dataItem.arrChucNang, 91)"><em class="fa fa-eye"></em> Xem đơn hàng</a></li>
                                                    <li><a  ng-click="vm.lanh_dao_bo_duyet(this.dataItem)"   ng-if="vm.arrExist(this.dataItem.arrChucNang, 6)"><em class="fa fa-check-circle-o"></em> Duyệt hồ sơ</a></li>
                                                    <li><a  ng-click="vm.lanhDaoBoKySoNhanh(this.dataItem)"   ng-if="vm.arrExist(this.dataItem.arrChucNang, 61)"><em class="fa fa-flash"></em> Ký số & chuyển nhanh</a></li>
                                                    <li><a ng-click="vm.xemBienBanThamDinh(this.dataItem)"  ng-if="vm.arrExist(this.dataItem.arrChucNang, 95)"><em class="fa fa-folder-open"></em> Xem biên bản thẩm định</a></li>
                                                </ul>
                                                </div>
                                            </div>`;
                                            break;
                                        case 'van_thu_duyet':
                                            _colthaotac.template =
                                                `<div class="ui-grid-cell-contents  text-center">
                                                <div class="btn-group dropdown" uib-dropdown="" dropdown-append-to-body>
                                                    <button class="btn btn-xs green-soft" uib-dropdown-toggle="" aria-haspopup="true" aria-expanded="false"><em class="fa fa-cog"></em>Thao Tác<span class="caret"></span></button>
                                                    <ul uib-dropdown-menu busy-if= "vm.action_loading">
                                                        <li><a ng-click="vm.xemHoSoFull(this.dataItem)" ng-if="vm.arrExist(this.dataItem.arrChucNang, 9)"><em class="fa fa-eye"></em> Xem hồ sơ</a></li>
                                                        <li><a ng-click="vm.xemHoSo(this.dataItem)"  ng-if="vm.arrExist(this.dataItem.arrChucNang, 91)"><em class="fa fa-eye"></em> Xem đơn hàng</a></li>
                                                        <li><a ng-click="vm.van_thu_duyet(this.dataItem)"  ng-if="vm.arrExist(this.dataItem.arrChucNang, 7)"><em class="fa fa-hourglass"></em> Xem & đóng dấu</a></li>
                                                        <li><a ng-click="vm.vanThuDongDauNhanh(this.dataItem)"  ng-if="vm.arrExist(this.dataItem.arrChucNang, 71)"><em class="fa fa-flash"></em> Đóng dấu nhanh</a></li>
                                                    </ul>
                                                </div>
                                            </div>`;
                                            break;
                                        case 'tra_cuu_ho_so':
                                            _colthaotac.template =
                                                `<div class="ui-grid-cell-contents">
                                                <div class="btn-group dropdown" uib-dropdown="" dropdown-append-to-body>
                                                    <button class="btn btn-xs green-soft" uib-dropdown-toggle="" aria-haspopup="true" aria-expanded="false"><em class="fa fa-cog"></em>Thao Tác<span class="caret"></span></button>
                                                    <ul uib-dropdown-menu busy-if= "vm.action_loading">
                                                        <li><a ng-click="vm.xemHoSoFull(this.dataItem)"><em class="fa fa-eye"></em> Xem hồ sơ</a></li>
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
                                    else if (vm.form == 'lanh_dao_bo_duyet') {
                                        colCheckBox = {
                                            field: "checkbox"
                                            , attributes: { class: "text-center" }
                                            , headerAttributes: { style: "text-align: center;" }
                                            , headerTemplate: `<label class="mt-checkbox mt-checkbox-outline" ng-show="vm.filter.formCase==1">
                                            <input name="chkH" type="checkbox" ng-change="vm.clickAllCheckbox()" ng-model="vm.checkboxAll"/><span>&nbsp;</span></label>`
                                            , template: `<div class="mt-checkbox-inline" ng-show="vm.filter.formCase==1"><label class="mt-checkbox mt-checkbox-outline">
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
                                        <strong>Số hồ sơ: <span style="font-size:16px;">{{ this.dataItem.soDangKy }}</span></strong><br/>
                                        <strong>Mã hồ sơ:</strong> {{ this.dataItem.maHoSo }}<br/>
                                        <strong>Nơi gửi:</strong> {{ this.dataItem.tenDoanhNghiep }}<br/>
                                        <strong>Loại hồ sơ:</strong> {{ this.dataItem.strLoaiHoSo }}
                                    </div>`
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
                                    <div ng-if="this.dataItem.ngayHenTra">
                                        <div ng-show='this.dataItem.soNgayQuaHan<=0'><span class='label label-success'><em class='fa fa-clock-o'></em> {{this.dataItem.strQuaHan}}</span></div>
                                        <div ng-show='this.dataItem.soNgayQuaHan<=5 && this.dataItem.soNgayQuaHan>0'><span class='label bg-yellow-crusta'><em class='fa fa-clock-o'></em> {{this.dataItem.strQuaHan}}</span></div>
                                        <div ng-show='this.dataItem.soNgayQuaHan>5'><span class='label bg-red'><em class='fa fa-clock-o'></em> {{this.dataItem.strQuaHan}}</span></div>
                                    </div>`,
                                    width: 160
                                };

                                //8. Chuyên viên xử lý
                                var _colChuyenVienXuLy = {
                                    field: ""
                                    , headerTemplate: 'Chuyên viên xử lý'
                                    , template: `<div ng-show='this.dataItem.chuyenVienThuLyId!=null'>
                                                    <strong>CV thụ lý:</strong> {{this.dataItem.chuyenVienThuLyName}} 
                                                    <em class='fa fa-check font-green-soft' ng-show='this.dataItem.chuyenVienThuLyDaDuyet==true'></em>
                                                    <br ng-show='this.dataItem.chuyenVienPhoiHopId!=null'/>
                                                    <span ng-show='this.dataItem.chuyenVienPhoiHopId!=null'><strong>CV phối hợp:</strong> {{this.dataItem.chuyenVienPhoiHopName}} 
                                                    <em class='fa fa-check font-green-soft' ng-show='this.dataItem.chuyenVienPhoiHopDaDuyet==true'></em></span></div>`
                                    , width: 250
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

                                //11. Kết quả hồ sơ
                                var _colKetQua = {
                                    field: ""
                                    , attributes: { class: "text-center" },
                                    headerAttributes: { style: "text-align: center;" }
                                    , headerTemplate: "Kết quả"
                                    , template: "<span class='label bg-green-jungle bg-font-green-jungle' ng-show='this.dataItem.hoSoIsDat == true'>Hồ sơ đạt</span><br ng-show='this.dataItem.hoSoIsDat == false || ((this.dataItem.donViXuLy == 13) && (this.dataItem.phoPhongDaDuyet == true) && (this.dataItem.truongPhongId == vm.user.id))'/>"
                                        + "<span class='label label-danger' ng-show='this.dataItem.hoSoIsDat == false'>Hồ sơ cần SĐBS</span><br ng-show='this.dataItem.hoSoIsDat == false' />"
                                        + "<span class='label bg-purple-medium' ng-show='(this.dataItem.donViXuLy == 13) && (this.dataItem.phoPhongDaDuyet == true) && (this.dataItem.truongPhongId == vm.user.id)'>Chỉ theo dõi</span>"
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

                                //13. Danh sách phòng ban xử lý
                                var _colListPhongBan = {
                                    field: ""
                                    , headerTemplate: "<span style='margin: 0;text-align:center;'>Phòng ban xử lý</span>"
                                    , template: `<div ng-if="this.dataItem.arrPhongBanXuLy.length>0">
                                                                        <p ng-repeat="phongban in this.dataItem.arrPhongBanXuLy">- <em class="fa fa-check font-green-soft" ng-if="phongban.checked==true"></em> {{phongban.name}}</p>
                                                                        </div>`
                                    , width: 200
                                };

                                //14. Cột thông tin hồ sơ
                                var _colShortThongTinHoSo = {
                                    field: ""
                                    , headerTemplate: "Thông tin hồ sơ"
                                    , template: `<div> <strong>Mã HS: </strong><a ng-click="vm.mot_cua_phan_cong(this.dataItem)"><strong>{{this.dataItem.maHoSo}}</strong></a></div>
                                            <div> <strong>Doanh nghiệp: </strong>{{this.dataItem.tenDoanhNghiep}} </div>`

                                };
                                var _colShortThongTinHoSoVanThuDuyet = {
                                    field: ""
                                    , headerTemplate: "Thông tin hồ sơ"
                                    , template: `<div> <strong>Mã HS: </strong><a ng-click="vm.arrExist(this.dataItem.arrChucNang, 7)?vm.van_thu_duyet(this.dataItem): vm.xemHoSoFull(this.dataItem)"><strong>{{this.dataItem.maHoSo}}</strong></a></div>
                                             <div> <strong>Doanh nghiệp: </strong>{{this.dataItem.tenDoanhNghiep}} </div>`
                                };

                                // Set column cho Grid
                                var setColForForm = function () {
                                    switch (vm.form) {
                                        case 'dang_ky_ho_so':
                                            vm.columns = [_stt, _thaotac(), _checkBoxChonNhieu(), _colKyDienTu,
                                                _thongtinHoSo, _colNgayNop, _colNgayHenTra, _colTrangThai];
                                            break;
                                        case 'mot_cua_ra_soat':
                                            vm.columns = [_stt, _thaotac(), _thongtinHoSo, _colNgayNop, _colNgayHenTra, _colChuyenVienXuLy, _colDonViGui, _colDonViXuLy, _colTrangThai];
                                            break;
                                        case 'mot_cua_phan_cong':
                                            vm.columns = [_stt, _colShortThongTinHoSo, _colNgayNop, _colNgayHenTra, _colChuyenVienXuLy, _colDonViGui, _colDonViXuLy, _colTrangThai];
                                            break;
                                        case 'phong_ban_phan_cong':
                                            vm.columns = [_stt, _thaotac(), _checkBoxChonNhieu(),
                                                _thongtinHoSo, _colNgayNop, /*_colNgayHenTra,*/ _colChuyenVienXuLy, _colDonViGui, _colDonViXuLy, _colTrangThai];
                                            break;
                                        case 'tham_dinh_ho_so':
                                            vm.columns = [_stt, _thaotac(),
                                                _thongtinHoSo, _colNgayNop, _colNgayHenTra, _colChuyenVienXuLy, _colDonViGui, _colDonViXuLy, _colTrangThai, _colKetQua];
                                            break;
                                        case 'pho_phong_duyet':
                                            vm.columns = [_stt, _thaotac(),
                                                _thongtinHoSo, _colNgayNop, _colNgayHenTra, _colChuyenVienXuLy, _colDonViGui, _colDonViXuLy, _colTrangThai, _colKetQua];
                                            break;
                                        case 'truong_phong_duyet':
                                            vm.columns = [_stt, _thaotac(),
                                                _thongtinHoSo, _colNgayNop, _colNgayHenTra, _colChuyenVienXuLy, _colDonViGui, _colDonViXuLy, _colTrangThai, _colKetQua];
                                            break;
                                        case 'lanh_dao_cuc_duyet':
                                            vm.columns = [_stt, _thaotac(), _checkBoxChonNhieu(),
                                                _thongtinHoSo, _colNgayNop, _colNgayHenTra, _colChuyenVienXuLy, _colDonViGui, _colDonViXuLy, _colTrangThai, _colKetQua];
                                            break;
                                        case 'lanh_dao_bo_duyet':
                                            vm.columns = [_stt, _thaotac(), _checkBoxChonNhieu(),
                                                _thongtinHoSo, _colNgayNop, _colNgayHenTra, _colChuyenVienXuLy, _colDonViGui, _colDonViXuLy, _colTrangThai, _colKetQua];
                                            break;
                                        case 'van_thu_duyet':
                                            vm.columns = [_stt, _checkBoxChonNhieu(),
                                                _colShortThongTinHoSoVanThuDuyet, _colNgayNop, _colNgayHenTra, _colChuyenVienXuLy, _colDonViGui, _colDonViXuLy, _colKetQua];
                                            break;
                                        case 'tra_cuu_ho_so':
                                            vm.columns = [_stt, _thaotac(),
                                                _thongtinHoSo, _colNgayNop, _colNgayHenTra, _colChuyenVienXuLy, _colDonViGui, _colDonViXuLy, _colTrangThai, _colKetQua];
                                            break;
                                    }
                                };
                                setColForForm();

                                //Thao tác
                                vm.action_loading = false;

                                vm.columnGridHoSo = vm.columns;
                            };
                            vm.refreshColumns();

                            vm.disableCallTotal = false;
                            vm.gridHoSoDataSource = new kendo.data.DataSource({
                                transport: {
                                    read: function (options) {
                                        // get ngay gui 
                                        //if (vm.form == 'dang_ky_ho_so' && vm.filter.formCase == 1) {
                                        //    vm.filter.ngayNopTu = null;
                                        //    vm.filter.ngayNopToi = null;
                                        //}
                                        //else {
                                        //    vm.filter.ngayNopTu = angular.copy(vm.ngayGuiModel.startDate);
                                        //    vm.filter.ngayNopToi = angular.copy(vm.ngayGuiModel.endDate);
                                        //}
                                        //end get ngay gui
                                        vm.requestParams.skipCount = (options.data.page - 1) * options.data.pageSize;
                                        vm.requestParams.maxResultCount = options.data.pageSize;
                                        xuLyHoSoGridViewService.getListHoSoPaging($.extend(vm.filter, vm.requestParams))
                                            .then(function (result) {
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

                                                // check if total grid khac so voi label total --> reload
                                                if (vm.disableCallTotal == true) {
                                                    vm.disableCallTotal = false;
                                                }
                                                else {
                                                    vm.reloadTotal();
                                                }

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
                                    if (vm.form == 'mot_cua_phan_cong') {
                                        if (vm.filter.formCase == 1) {
                                            this.hideColumn(5);
                                            this.hideColumn(6);
                                            this.hideColumn(7);
                                            this.hideColumn(8);
                                        } else if (vm.filter.formCase == 2) {
                                            this.hideColumn(5);
                                            this.showColumn(6);
                                            this.showColumn(7);
                                            this.showColumn(8);
                                        }
                                        else {
                                            this.showColumn(5);
                                            this.showColumn(6);
                                            this.showColumn(7);
                                            this.showColumn(8);
                                        }
                                    } else if (vm.form == 'dang_ky_ho_so') {
                                        if (vm.filter.formCase == 1 || vm.filter.formCase == 3 || vm.filter.formCase == 4) {
                                            this.hideColumn(6);
                                            this.hideColumn(7);
                                        }
                                        else {
                                            this.showColumn(6);
                                            this.showColumn(7);
                                        }
                                    }
                                    else if (vm.form == 'phong_ban_phan_cong') {
                                        if (vm.filter.formCase == 3) {
                                            this.hideColumn(2);
                                            this.showColumn(7);
                                        }
                                        else {
                                            if (vm.filter.formCase == 1) {
                                                this.hideColumn(7);
                                            } else {
                                                this.showColumn(7);
                                            }
                                            this.showColumn(2);
                                        }
                                    }
                                    else if (vm.form == 'van_thu_duyet') {
                                        if (vm.filter.formCase == 1) {
                                            this.showColumn(1);
                                            this.hideColumn(6);
                                            this.hideColumn(7);
                                            this.hideColumn(8);
                                        }
                                        else {
                                            this.hideColumn(1);
                                            this.showColumn(6);
                                            this.showColumn(7);
                                            this.showColumn(8);
                                        }
                                    }
                                },
                                columns: vm.columnGridHoSo
                            };

                            //SUBSCRIBE function 'refreshGridHoSo'
                            //$rootScope.$broadcast('refreshGridHoSo', 'ok');
                            $scope.$on('refreshGridHoSo', function (event, filter) {
                                //if (filter && filter.formCase) {
                                //    vm.filter.formCase = filter.formCase;
                                //}
                                vm.refreshHoSo();
                            });
                        }
                    }

                    //--- FormCase ---//
                    {
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
                            vm.disableCallTotal = true;
                            vm.refreshHoSo();
                        };

                        //Tính số hồ sơ theo FormCase
                        {
                            //Cách mới
                            vm.reloadTotal = function () {
                                _req = angular.copy(vm.filter);
                                _req.skipCount = 0;
                                _req.maxResultCount = 1;
                                xuLyHoSoGridViewService.getListFormCaseCountNumber(_req)
                                    .then(function (result) {
                                        if (result) {
                                            result.data.forEach(function (item) {
                                                vm.listFormCase.forEach(function (fc) {
                                                    if (fc.id == item.id) {
                                                        fc.totalItems = item.totalCount;
                                                    }
                                                });
                                            });
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
                        vm.xemHoSo = function (dataItem) {
                            if (dataItem && dataItem.id > 0) {
                                var modalData = {
                                    title: 'Xem đơn hàng',
                                    id: dataItem.id
                                };
                                var modalInstance = $uibModal.open({
                                    templateUrl: '~/App/quanlyhoso/thutuc99/directives/modal/viewHoSoModal.cshtml',
                                    controller: 'quanlyhoso.thutuc99.directives.modal.viewHoSoModal as vm',
                                    backdrop: 'static',
                                    size: 'lg',
                                    resolve: {
                                        modalData: modalData
                                    }
                                });
                            }
                        };

                        vm.xemHoSoFull = function (dataItem) {
                            if (dataItem && dataItem.id > 0) {
                                var modalData = {
                                    title: 'Xem hồ sơ',
                                    id: dataItem.id
                                };
                                var modalInstance = $uibModal.open({
                                    templateUrl: '~/App/quanlyhoso/thutuc99/directives/modal/viewHoSoFullModal.cshtml',
                                    controller: 'quanlyhoso.thutuc99.directives.modal.viewHoSoFullModal as vm',
                                    backdrop: 'static',
                                    size: 'lg',
                                    resolve: {
                                        modalData: modalData
                                    }
                                });
                            }
                        };

                        vm.nopHoSoMoi = function (dataItem) {
                            abp.message.confirm(app.localize('Thông tin hồ sơ "' + dataItem.maHoSo + '" sẽ được gửi để rà soát!!!'),
                                app.localize('Bạn chắc chắn muốn gửi hồ sơ này?'),
                                function (isConfirmed) {
                                    if (isConfirmed) {
                                        xuLyHoSoDoanhNghiepService.nopHoSoDeRaSoat(dataItem.id)
                                            .then(function (result) {
                                                abp.notify.success(app.localize('SavedSuccessfully'));
                                            }).finally(function () {
                                                vm.refreshHoSo();
                                            });
                                    }
                                }
                            );
                        };

                        vm.xemThanhToanHoSo = function (dataItem) {
                            if (dataItem && dataItem.id) {
                                var modalInstance = $uibModal.open({
                                    templateUrl: '~/App/quanlythanhtoan/common/modal/xemThanhToanHoSoModal.cshtml',
                                    controller: 'quanlythanhtoan.common.modal.xemThanhToanHoSoModal as vm',
                                    backdrop: 'static',
                                    size: 'lg',
                                    resolve: {
                                        hoSo: dataItem
                                    }
                                });
                                modalInstance.result.then(function (result) {

                                });
                            }
                        };

                        vm.xemBienBanThamDinh = function (dataItem) {
                            if (dataItem && dataItem.id > 0) {
                                var item = {
                                    id: dataItem.id
                                };
                                appChuKySo.xemBienBanThamDinh(item, function () {

                                });
                            }
                        };
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

                        vm.thanhToanHoSo = function (dataItem) {
                            var modalInstance = $uibModal.open({
                                templateUrl: '~/App/quanlyhoso/thutuc99/views/1_dangkyhoso/modal/thanhToanModal.cshtml',
                                controller: 'quanlyhoso.thutuc99.views.dangkyhoso.thanhToanModal as vm',
                                backdrop: 'static',
                                size: 'lg',
                                resolve: {
                                    dataItem: dataItem
                                }
                            });
                            modalInstance.result.then(function (result) {
                                vm.refreshHoSo();
                            });
                        };

                        vm.huyHoSo = function (dataItem) {
                            abp.message.confirm(app.localize('Thông tin hồ sơ "' + dataItem.maHoSo + '" sẽ bị xóa!!!'),
                                app.localize('Bạn chắc chắn muốn hủy hồ sơ này?'),
                                function (isConfirmed) {
                                    if (isConfirmed) {
                                        xuLyHoSoDoanhNghiepService.openHuyHoSo(dataItem.id).then(function (result) {
                                            abp.notify.success('Lưu thành công');
                                            vm.refreshHoSo();
                                        }).finally(function () {
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
                                        xuLyHoSoDoanhNghiepService.nopHoSoBoSung(dataItem.id).then(function (result) {
                                            abp.notify.success('Nộp thành công');
                                            vm.refreshHoSo();
                                        }).finally(function () {
                                        });
                                    }
                                }
                            );
                        };
                        vm.nopHoSoBiTraLai = function (dataItem) {
                            abp.message.confirm(app.localize("Bạn có muốn nộp hồ bị trả lại này không?"),
                                app.localize('Nộp hồ sơ trả lại'),
                                function (isConfirmed) {
                                    if (isConfirmed) {
                                        xuLyHoSoDoanhNghiepService.nopHoSoTraLai(dataItem.id).then(function (result) {
                                            abp.notify.success('Nộp thành công');
                                            vm.refreshHoSo();
                                        }).finally(function () {
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

                    //--- KySoNhanh ---//
                    {
                        //truong-phong
                        vm.truongPhongKySoNhanh = function (dataItem) {
                            if (dataItem && dataItem.id > 0) {
                                abp.message.confirm(app.localize(""),
                                    app.localize('Bạn chắc chắn muốn ký công văn?'),
                                    function (isConfirmed) {
                                        if (isConfirmed) {
                                            appChuKySo.kySoCongVanBoSung(dataItem, function (paramKySo) {

                                                var duyetHoSo = {};
                                                duyetHoSo.hoSoXuLyId = dataItem.hoSoXuLyId_Active;
                                                duyetHoSo.hoSoId = dataItem.id;
                                                duyetHoSo.hoSoIsDat = false;
                                                duyetHoSo.donViKeTiep = vm.DON_VI_XU_LY.VAN_THU;
                                                duyetHoSo.duongDanTepCA = paramKySo.duongDanTep;

                                                vm.saving = true;
                                                //Chuyển đơn vị xử lý
                                                xuLyHoSoTruongPhongService.truongPhongDuyet_Chuyen(duyetHoSo)
                                                    .then(function (result) {
                                                        abp.notify.info(app.localize('SavedSuccessfully'));
                                                        vm.refreshHoSo();

                                                        appChuKySo.xemFilePDF(paramKySo.duongDanTep, 'Công văn đã ký số');

                                                    }).finally(function () {
                                                        vm.saving = false;
                                                    });
                                            });
                                        }
                                    }
                                );
                            }
                        };
                        vm.truongPhongKySoNhieu = function () {

                            if (vm.arrCheckbox && vm.arrCheckbox.length > 0) {

                                abp.message.confirm(app.localize(""),
                                    app.localize('Bạn chắc chắn muốn ký nhiều công văn?'),
                                    function (isConfirmed) {
                                        if (isConfirmed) {
                                            var _count = 0;
                                            vm.arrCheckbox.forEach(function (item) {
                                                _count++;
                                                if (item.hoSoIsDat != true) {
                                                    //Ký số
                                                    appChuKySo.kySoCongVanBoSung(item, function (paramKySo) {

                                                        var duyetHoSo = {};
                                                        duyetHoSo.hoSoXuLyId = item.hoSoXuLyId_Active;
                                                        duyetHoSo.hoSoId = item.id;
                                                        duyetHoSo.hoSoIsDat = false;
                                                        duyetHoSo.donViKeTiep = vm.DON_VI_XU_LY.VAN_THU;
                                                        duyetHoSo.duongDanTepCA = paramKySo.duongDanTep;

                                                        vm.saving = true;
                                                        //Chuyển đơn vị xử lý
                                                        xuLyHoSoTruongPhongService.truongPhongDuyet_Chuyen(duyetHoSo)
                                                            .then(function (result) {
                                                                abp.notify.info(app.localize('SavedSuccessfully'));

                                                                if (_count == vm.arrCheckbox.length) {
                                                                    vm.refreshHoSo();
                                                                }

                                                            }).finally(function () {
                                                                vm.saving = false;
                                                            });
                                                    });
                                                }
                                            });
                                        }
                                    });
                            }
                        };

                        //lanh-dao-cuc
                        vm.lanhDaoCucKySoNhanh = function (dataItem) {
                            if (dataItem && dataItem.id > 0) {
                                abp.message.confirm(app.localize(""),
                                    app.localize('Bạn chắc chắn muốn ký công văn?'),
                                    function (isConfirmed) {
                                        if (isConfirmed) {

                                            if (dataItem.hoSoIsDat == true) {
                                                //Ký số Giấy Chứng Nhận
                                                appChuKySo.kySoGiayTiepNhan(dataItem, function (paramKySo) {
                                                    var duyetHoSo = {};
                                                    duyetHoSo.hoSoXuLyId = dataItem.hoSoXuLyId_Active;
                                                    duyetHoSo.hoSoId = dataItem.id;
                                                    duyetHoSo.soTiepNhan = paramKySo.soTiepNhan;
                                                    duyetHoSo.duongDanTepCA = paramKySo.duongDanTep;
                                                    duyetHoSo.giayTiepNhanCA = paramKySo.giayTiepNhanCA;

                                                    vm.saving = true;
                                                    //Chuyển đơn vị xử lý
                                                    xuLyHoSoLanhDaoCucService.kyVaChuyenVanThu(duyetHoSo)
                                                        .then(function (result) {
                                                            if (result.data) {
                                                                abp.notify.success(app.localize('SavedSuccessfully'));
                                                                vm.refreshHoSo();

                                                                appChuKySo.xemFilePDF(paramKySo.giayTiepNhanCA, 'Giấy chứng nhận đã ký số');
                                                            }
                                                        }).finally(function () {
                                                            vm.saving = false;
                                                        });
                                                });
                                            }
                                            else {
                                                //Ký số Công Văn Bổ Sung
                                                appChuKySo.kySoCongVanBoSung(dataItem, function (paramKySo) {
                                                    var duyetHoSo = {};
                                                    duyetHoSo.hoSoXuLyId = dataItem.hoSoXuLyId_Active;
                                                    duyetHoSo.hoSoId = dataItem.id;
                                                    duyetHoSo.hoSoIsDat = false;
                                                    duyetHoSo.duongDanTepCA = paramKySo.duongDanTep;

                                                    vm.saving = true;
                                                    //Chuyển đơn vị xử lý
                                                    xuLyHoSoLanhDaoCucService.kyVaChuyenVanThu(duyetHoSo)
                                                        .then(function (result) {
                                                            abp.notify.info(app.localize('SavedSuccessfully'));
                                                            vm.refreshHoSo();

                                                            appChuKySo.xemFilePDF(paramKySo.duongDanTep, 'Công văn đã ký số');

                                                        }).finally(function () {
                                                            vm.saving = false;
                                                        });
                                                });
                                            }
                                        }

                                    });
                            }
                        };

                        vm.lanhDaoCucKySoNhieu = function () {
                            if (vm.arrCheckbox && vm.arrCheckbox.length > 0) {

                                abp.message.confirm(app.localize(""),
                                    app.localize('Bạn chắc chắn muốn ký nhiều hồ sơ?'),
                                    function (isConfirmed) {
                                        if (isConfirmed) {
                                            var _count = 0;
                                            vm.arrCheckbox.forEach(function (item) {
                                                _count++;
                                                if (item.hoSoIsDat == true) {
                                                    //Ký số
                                                    appChuKySo.kySoGiayTiepNhan(item, function (paramKySo) {
                                                        var duyetHoSo = {};
                                                        duyetHoSo.hoSoXuLyId = item.hoSoXuLyId_Active;
                                                        duyetHoSo.hoSoId = item.id;
                                                        duyetHoSo.duongDanTepCA = paramKySo.duongDanTep;
                                                        duyetHoSo.giayTiepNhanCA = paramKySo.giayTiepNhanCA;
                                                        duyetHoSo.soTiepNhan = paramKySo.soTiepNhan;
                                                        vm.saving = true;
                                                        //Chuyển đơn vị xử lý
                                                        xuLyHoSoLanhDaoCucService.kyVaChuyenVanThu(duyetHoSo)
                                                            .then(function (result) {
                                                                abp.notify.info(app.localize('SavedSuccessfully'));

                                                                if (_count == vm.arrCheckbox.length) {
                                                                    vm.refreshHoSo();
                                                                }

                                                            }).finally(function () {
                                                                vm.saving = false;
                                                            });
                                                    });
                                                }
                                                else {
                                                    //Ký số Công Văn Bổ Sung
                                                    appChuKySo.kySoCongVanBoSung(item, function (paramKySo) {
                                                        var duyetHoSo = {};
                                                        duyetHoSo.hoSoXuLyId = item.hoSoXuLyId_Active;
                                                        duyetHoSo.hoSoId = item.id;
                                                        duyetHoSo.hoSoIsDat = false;
                                                        //duyetHoSo.trangThaiCV = vm.TRANG_THAI_CONG_VAN.DAT;
                                                        duyetHoSo.duongDanTepCA = paramKySo.duongDanTep;

                                                        vm.saving = true;
                                                        //Chuyển đơn vị xử lý
                                                        xuLyHoSoLanhDaoCucService.kyVaChuyenVanThu(duyetHoSo)
                                                            .then(function (result) {
                                                                abp.notify.info(app.localize('SavedSuccessfully'));

                                                                if (_count == vm.arrCheckbox.length) {
                                                                    vm.refreshHoSo();
                                                                }

                                                            }).finally(function () {
                                                                vm.saving = false;
                                                            });
                                                    });
                                                }
                                            });
                                        }
                                    });
                            }
                        };

                        //lanh-dao-bo
                        vm.lanhDaoBoKySoNhanh = function (dataItem) {

                        };
                        vm.lanhDaoBoKySoNhieu = function () {

                        };

                        //van-thu
                        vm.vanThuDongDauNhanh = function (dataItem) {
                            if (dataItem && dataItem.id > 0) {
                                abp.message.confirm(app.localize(""),
                                    app.localize('Bạn chắc chắn muốn đóng dấu văn bản?'),
                                    function (isConfirmed) {
                                        if (isConfirmed) {
                                            //Ký số
                                            appChuKySo.vanThuDongDau(dataItem, function (paramKySo) {
                                                var duyetHoSo = {
                                                    hoSoXuLyId: dataItem.hoSoXuLyId_Active,
                                                    hoSoId: dataItem.id,
                                                    duongDanTepCA: paramKySo.duongDanTep,
                                                    giayTiepNhanCA: paramKySo.giayTiepNhanCA
                                                };

                                                vm.saving = true;
                                                xuLyHoSoVanThuService.dongDau(duyetHoSo)
                                                    .then(function (result) {
                                                        abp.notify.info(app.localize('SavedSuccessfully'));
                                                        vm.refreshHoSo();

                                                        appChuKySo.xemFilePDF(paramKySo.duongDanTep, 'Văn bản đã ký số');
                                                    }).finally(function () {
                                                        vm.saving = false;
                                                    });
                                            });
                                        }
                                    }
                                );
                            }
                        };

                        vm.vanThuDongDauNhieu = function () {
                            if (vm.arrCheckbox && vm.arrCheckbox.length > 0) {

                                abp.message.confirm(app.localize(""),
                                    app.localize('Bạn chắc chắn muốn đóng dấu nhiều văn bản?'),
                                    function (isConfirmed) {
                                        if (isConfirmed) {
                                            var _count = 0;
                                            vm.arrCheckbox.forEach(function (item) {
                                                _count++;
                                                //Ký số
                                                appChuKySo.vanThuDongDau(item, function (paramKySo) {
                                                    var duyetHoSo = {
                                                        hoSoXuLyId: item.hoSoXuLyId_Active,
                                                        hoSoId: item.id,
                                                        duongDanTepCA: paramKySo.duongDanTep,
                                                        giayTiepNhanCA: paramKySo.giayTiepNhanCA
                                                    };

                                                    vm.saving = true;
                                                    xuLyHoSoVanThuService.dongDau(duyetHoSo)
                                                        .then(function (result) {
                                                            abp.notify.info(app.localize('SavedSuccessfully'));

                                                            if (_count == vm.arrCheckbox.length) {
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
                    }

                    //--- Functions CallBack ---//
                    {
                        //=== Common
                        {
                            //Form doanh nghiệp
                            vm.sua_ho_so = function (dataItem) {
                                $scope.suahoso()(dataItem);
                            };

                            //Form kế toán xác nhận thanh toán
                            vm.xac_nhan_thanh_toan = function (dataItem) {
                                $scope.xacnhanthanhtoan()(dataItem);
                            };

                            //Form một cửa rà soát hồ sơ
                            vm.mot_cua_ra_soat = function (dataItem) {
                                $scope.motcuarasoat()(dataItem);
                            };
                            //Form một cửa phân công hồ sơ
                            vm.mot_cua_phan_cong = function (dataItem) {
                                $scope.motcuaphancong()(dataItem);
                            };

                            //Form phòng ban phân công cán bộ
                            vm.phong_ban_phan_cong = function (dataItem) {
                                $scope.phongbanphancong()(dataItem);
                            };

                            //Form trưởng phòng duyệt
                            vm.truong_phong_duyet = function (dataItem) {
                                $scope.truongphongduyet()(dataItem);
                            };

                            //Form lãnh đạo cục duyệt
                            vm.lanh_dao_cuc_duyet = function (dataItem) {
                                $scope.lanhdaocucduyet()(dataItem);
                            };

                            //Form lãnh đạo bộ duyệt
                            vm.lanh_dao_bo_duyet = function (dataItem) {
                                $scope.lanhdaoboduyet()(dataItem);
                            };

                            //Form Văn thư đóng dấu          
                            vm.van_thu_duyet = function (dataItem) {
                                $scope.vanthuduyet()(dataItem);
                            };
                        }

                        //=== Hop Den
                        {
                            //Form thẩm định hồ sơ
                            vm.tham_dinh_ho_so = function (dataItem) {
                                $scope.thamdinhhoso()(dataItem);
                            };
                            vm.tham_dinh_ho_so_bo_sung = function (dataItem) {
                                $scope.thamdinhhosobosung()(dataItem);
                            };
                            vm.tham_dinh_lai = function (dataItem) {
                                $scope.thamdinhlai()(dataItem);
                            };
                            vm.tong_hop_tham_dinh = function (dataItem) {
                                $scope.tonghopthamdinh()(dataItem);
                            };

                            //Form phó phòng duyệt
                            vm.pho_phong_duyet = function (dataItem) {
                                $scope.phophongduyet()(dataItem);
                            };
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
                        vm.initFormCase();
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

                    //comon ky_nhieu
                    kynhieuhoso: '&',
                    kynhieugiaytiepnhan: '&',
                    kynhieucongvan: '&',
                    dongdaunhieu: '&',

                    //'dang_ky_ho_so'          
                    suahoso: '&',

                    //'xac_nhan_thanh_toan'
                    xacnhanthanhtoan: '&',

                    //mot_cua_ra_soat
                    motcuarasoat: '&',
                    //'mot_cua_phan_cong'
                    motcuaphancong: '&',

                    //'phong_ban_phan_cong'
                    phongbanphancong: '&',
                    phanconglaihosochuaxuly: '&',
                    phanconglaihosodaxuly: '&',

                    //'truong_phong_duyet'
                    truongphongduyet: '&',

                    //'lanh_dao_cuc_duyet'
                    lanhdaocucduyet: '&',

                    //'van_thu_duyet'
                    vanthuduyet: '&',

                    //'tham_dinh_ho_so'
                    thamdinhhoso: '&',
                    thamdinhhosobosung: '&',
                    thamdinhlai: '&',
                    tonghopthamdinh: '&',

                    //'pho_phong_duyet'
                    phophongduyet: '&',

                    // control goi grid view
                    control: '='
                },
                controller: controller,
                controllerAs: 'vm',
                bindToController: false,
                link: function (scope, elem, attr, ctrl) {
                    $templateRequest("/App/quanlyhoso/thutuc99/directives/gridHoSo.cshtml").then(function (html) {
                        var template = angular.element(html);
                        elem.append(template);
                        $compile(template)(scope);
                    });
                }
            };
        }
    ]);
})();