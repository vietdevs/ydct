
(function () {
    appModule.directive('doanhnghiepcombo', ['$compile', '$timeout', '$templateRequest',
        function ($compile, $timeout, $templateRequest) {
            var controller = ['$scope', '$timeout',
                function ($scope, $timeout) {
                    $scope.showInput = true;
                    $scope.searchOptions = {
                        dataSource: new kendo.data.DataSource({
                            serverFiltering: true,
                            transport: {
                                read: function (options) {
                                    var _req = {
                                        maSoThue: $scope.selectedItem,
                                        filter: $scope.selectedItem,
                                        skipCount: 0,
                                        maxResultCount: 50
                                    };
                                    abp.services.app.doanhNghiep.getAllDoanhNghiepSearch(_req)
                                        .then(function (result) {
                                            $timeout(function () {
                                                options.success(result);
                                            });
                                        });
                                }
                            },
                            serverPaging: true,
                            sortable: true,
                            selectable: false
                        }),
                        height: 300,
                        headerTemplate: "",
                        template: `<div>
                                    <div class="attp-filter-media">
                                        <div class="media-left">
                                           <img src="../Common/Images/nhathuoc-default.jpg" class="media-object" />
                                       </div>
                                       <div class="media-body">
                                            <p><b>Tên DN :</b> {{dataItem.tenDoanhNghiep}}</p>
                                          <p><b>Mã số thuế :</b>{{dataItem.maSoThue}}</p>
                                          <p><b>Địa chỉ:</b> {{dataItem.diaChi}}</p>
                                          <p><b>SĐT:</b> {{dataItem.soDienThoai}}</p>
                                       </div>
                                  </div>
                             </div>
                                   `,
                        noDataTemplate: `<div >
                                <span class="k-icon k-i-warning"></span><br /><br />
                                Không tìm thấy dữ liệu!
                                </div>                               
                                `,
                        select: function (e) {
                            e.preventDefault();
                            var item = e.item;
                            var dataItem = this.dataItem(e.item.index());
                            $scope.selectItem(dataItem);
                            this.close();
                        },
                        filtering: function (e) {
                        },
                        close: function (e) {

                        }
                    };
                    $scope.selectItem = function (dataItem) {
                        $timeout(function () {
                            $scope.showInput = false;
                            $scope.doanhNghiepId = dataItem.id;
                            $scope.tenDoanhNghiep = dataItem.tenDoanhNghiep;
                            $scope.tinhId = angular.copy(dataItem.tinhId);
                        });
                    };
                    $scope.cancelSelected = function () {
                        $timeout(function () {
                            $scope.showInput = true;
                            $scope.selectedItem = "";
                            $scope.doanhNghiepId = null;
                            $scope.tinhId = null;
                        });
                    };
                    function init() {
                        $scope.internalControl = $scope.control || {};
                        $scope.internalControl.refreshDoanhNghiepId = function () {
                            $scope.cancelSelected();
                        };
                    }
                    init();
                }];

            return {
                restrict: 'EA',
                scope: {
                    doanhNghiepId: '=',
                    tinhId: '=?',
                    control: '='
                },
                controller: controller,
                controllerAs: 'vm',
                bindToController: false,
                link: function (scope, elem, attr, ctrl) {
                    $templateRequest("/App/common/filters/doanh-nghiep/filter-doanh-nghiep.cshtml").then(function (html) {
                        var template = angular.element(html);
                        elem.append(template);
                        $compile(template)(scope);
                    });
                }
            };
        }
    ]);
})();