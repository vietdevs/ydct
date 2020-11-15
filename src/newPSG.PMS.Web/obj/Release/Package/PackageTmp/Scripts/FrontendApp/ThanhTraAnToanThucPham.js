MyApp.controller('ctrl.tracuu.viphamattp', [
    '$scope', '$location', '$http', '$window','$timeout',
    function ($scope, $location, $http, $window, $timeout) {
        var vm = this;
        vm.saving = false;
        vm.requestParams = {
            skipCount: 0,
            maxResultCount: 10,
            sorting: null
        };
        vm.initFilter = function () {
            vm.filter = {
                tinhId: null,
                tenDoanhNghiep: "",
                tenSanPham: "",
                diaChi: "",
            };
        }
        vm.initFilter();
        vm.auto_complete_doanh_nghiep_options = {
            dataTextField: "tenDoanhNghiep",
            headerTemplate: "",
            template: `<div>
                        <h5 class ='bold font-blue'>#:data.tenDoanhNghiep #</h5>
                        <span>Địa chỉ: #: data.diaChi #</span>`,
            minLength: 3,
            dataSource: {
                serverFiltering: true,
                serverPaging: true,
                transport: {
                    read: function (options) {
                        _request = {
                            skipCount: 0,
                            maxResultCount: 20,
                            filter: vm.filter.tenDoanhNghiep,
                        }
                        $http.post('/api/services/app/doanhNgiepPublish/getAllDoanhNghiepServerPaging', _request).then(function (ret) {
                            options.success(ret.data.result);
                        });
                    }
                },
                serverPaging: true,
                sortable: true,
                selectable: false,
                schema: {
                    data: "items",
                    total: "totalCount"
                },
            }
        }
        vm.auto_complete_ten_san_pham_options = {
            dataTextField: "tenSanPham",
            headerTemplate: "",
            template: `<div>
                        <h5 class ='bold font-blue'>#:data.tenSanPham #</h5>
                        <span>#: data.doanhNghiep #</span>`,
            minLength: 3,
            dataSource: {
                serverFiltering: true,
                serverPaging: true,
                transport: {
                    read: function (options) {
                        _request = {
                            Filter: vm.filter.tenSanPham
                        }
                        $http.post('/api/services/app/keHoachThanhTra/sanPhamAutoCompleteDataSource', _request).then(function (ret) {
                            options.success(ret.data.result);
                        });
                    }
                },
                serverPaging: true,
                sortable: true,
                selectable: false,
            }
        }
        vm.tinhOptions = {
            dataSource: new kendo.data.DataSource({
                transport: {
                    read: function (options) {
                        $http.post('/api/services/app/tinh/getAllTinh').then(function (response) {
                            $timeout(function () {
                                options.success(response.data.result);
                            });

                        });
                    }
                },
            }),
            dataValueField: "id",
            dataTextField: "ten",
            optionLabel: app.localize('Chọn ...'),
            filter: "contains",
        }
        
        vm.gv_vi_pham_attp_options = {
            autoBind: false,
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
            dataSource: new kendo.data.DataSource({
                transport: {
                    read: function (options) {
                        _request = vm.filter;
                        _request.skipCount = (options.data.page - 1) * options.data.pageSize;
                        _request.maxResultCount = options.data.pageSize;
                        $http.post('/api/services/app/keHoachThanhTra/traCuuViPhamAnToanThucPham', _request).then(function (response) {
                            options.success(response.data.result);
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
                    buttonCount: [5, 10, 20, 50, 100]
                },
                schema: {
                    data: "items",
                    total: "totalCount"
                }
            }),
            columns: [
                {
                    title: "STT",
                    template: "<div style='text-align: center;' class='row-number'></div>",
                    width: 50,
                },
                {
                    title: 'Tên Doanh Nghiệp',
                    template: `<div class='bold font-blue'>{{dataItem.tenDoanhNghiep}}</div>
                               <div>{{dataItem.diaChi}}</div>`
                },
                {
                    title: 'Tên Sản Phẩm',
                    template: `<div class='bold'>{{dataItem.tenSanPham}}</div>`
                },
                {
                    title: 'Ngày thanh tra',
                    template: `<div>{{dataItem.chiTietViPham.ngayThanhTra | date :'dd/MM/yyyy'}}</div>`
                },
                {
                    template: "{{dataItem.chiTietViPham.ketQua}}",
                    title: 'Nội dung vi phạm',
                },
                {
                    template: "{{dataItem.chiTietViPham.mucDoViPham}}",
                    title: 'Mức độ vi phạm',
                },
                {
                    template: "{{dataItem.chiTietViPham.cachKhacPhuc}}",
                    title: "Biện pháp khắc phục",
                }
            ],
            dataBound: function (e) {
                var grid = this;
                record = (e.sender.dataSource.page() - 1) * e.sender.dataSource.pageSize() + 1;
                grid.tbody.find(".row-number").each(function () {
                    var index = record++;
                    $(this).html(index);
                });
            },
        }
        vm.search = function () {
            vm.showResultSearch = true;
            vm.gv_vi_pham_attp_options.dataSource.page(1);
        }

        function init() {
            vm.search();
        }
        init();        
    }
]);