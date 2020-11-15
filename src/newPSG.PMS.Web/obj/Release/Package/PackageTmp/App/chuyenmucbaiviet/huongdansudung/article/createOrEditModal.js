(function () {
    appModule.controller('chuyenmucbaiviet.huongdansudung.article.createOrEditModal', [
        '$scope', '$uibModalInstance', 'abp.services.app.article', 'article', 'baseService',
        function ($scope, $uibModalInstance, articleService, article, baseService) {
            //variable
            var vm = this;
            vm.saving = false;
            vm.article = {
                isActive: true,
                isHome: false,
                level: 0
            };
            $scope.ckDetails = {
                languague: 'vi',
                height: '320px'
            };
            vm.GetSeo = function () {
                vm.article.description = app.locdau(vm.article.title);
            };
            vm.ChooseImage = function () {
                var finder = new CKFinder();
                finder.selectActionFunction = function (fileUrl) {
                    $scope.$apply(function () {
                        vm.article.file = fileUrl;
                    });
                };
                finder.popup();
            };
            vm.categoryOptions = {
                dataSource: new kendo.data.DataSource({
                    transport: {
                        read: function (options) {
                            abp.services.app.category.allCategoryToDDL().done(function (result) {
                                options.success(result);
                            });
                        }
                    }
                }),
                dataValueField: "id",
                dataTextField: "title",
                optionLabel: app.localize('Chọn ...'),
                filter: "contains",
                select: function (e) {
                    var dataItem = this.dataItem(e.item);
                    vm.article.roleLevel = dataItem.roleLevel;
                }
            };
            //function
            vm.save = function () {
                baseService.ValidatorForm("#articleCreateOrEditForm");
                var frmValidatorForm = angular.element(document.querySelector('#articleCreateOrEditForm'));
                var formValidation = frmValidatorForm.data('formValidation').validate();
                if (formValidation.isValid()) {
                    vm.saving = true;
                    articleService.createOrUpdate(vm.article).then(function (result) {
                        abp.notify.success(app.localize('SavedSuccessfully'));
                        $uibModalInstance.close();
                    }).finally(function () {
                        vm.saving = false;
                    });
                }
            };

            vm.cancel = function () {
                $uibModalInstance.dismiss();
            };

            var init = function () {
                if (article != null) {
                    vm.article = article;
                }
            };
            init();
        }
    ]);
})();